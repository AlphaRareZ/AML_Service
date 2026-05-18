using System.Security.Claims;
using System.Text;
using Application.CQRS.Queries;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Application.Services.Core;
using Application.Settings;
using Application.UseCases.Models;
using Infrastructure.Interfaces;
using Infrastructure.Models.MessageBus;
using Infrastructure.Persistence;
using Infrastructure.Repositories;
using Infrastructure.Services.Background_Services;
using Infrastructure.Services.MessageBus;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddOpenApi(); // Minimal APIs (modern)
builder.Services.AddEndpointsApiExplorer(); // Endpoint discovery
builder.Services.AddSwaggerGen();

// Add services to the container.

// Repos
builder.Services.AddScoped<IAnalysisRepository, AnalysisRepository>();
builder.Services.AddScoped<ILigandRepository, LigandRepository>();
// Core Services
builder.Services.AddScoped<IAnalysisService, AnalysisService>();
builder.Services.AddScoped<ILigandService, LigandService>();
// Message Bus Services
builder.Services.AddScoped<IMessageProducer<AnalysisMessage>, AnalysisMessageProducer>();
builder.Services.AddScoped<IMessageProducer<LigandMessage>, LigandMessageProducer>();
builder.Services.AddScoped<IResponseMessageService<AnalysisResponseMessage>, AnalysisResponseMessageHandler>();
builder.Services.AddScoped<IResponseMessageService<LigandResponseMessage>, LigandResponseMessageHandler>();
// Email Services
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddTransient<IEmailService, SmtpEmailService>();
// Background Services
builder.Services.AddHostedService<AnalysisMessageConsumer>();
builder.Services.AddHostedService<LigandMessageConsumer>();

// Add MediatR
builder.Services.AddMediatR(
    con => { con.RegisterServicesFromAssembly(typeof(GetAnalysisStatusQuery).Assembly); });
// Add DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("MyDB")));
// Add Controllers
builder.Services.AddControllers();

// Authentication 
builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        var secretKey = builder.Configuration.GetSection("JwtSettings")["SecretKey"];
        var issuer = builder.Configuration.GetSection("JwtSettings")["Issuer"];
        var audience = builder.Configuration.GetSection("JwtSettings")["Audience"];

        if (string.IsNullOrEmpty(secretKey) || secretKey.Length < 32)
        {
            throw new InvalidOperationException("JWT SecretKey must be at least 32 characters long");
        }

        options.SaveToken = true;
        options.RequireHttpsMetadata = false;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
            ValidateIssuer = true,
            ValidIssuer = issuer,
            ValidateAudience = true,
            ValidAudience = audience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero,

            // Critical: These settings handle claim mapping correctly
            NameClaimType = ClaimTypes.Name,
            RoleClaimType = ClaimTypes.Role,

            // For .NET 7 compatibility
            RequireExpirationTime = true,
            RequireSignedTokens = true
        };
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                if (context.Request.Cookies.TryGetValue("accessToken", out var token))
                {
                    context.Token = token;
                }
                return Task.CompletedTask;
            }
        };
    });
// Authorization
builder.Services.AddAuthorizationBuilder()
    .AddPolicy("User", policy => policy.RequireRole("User"))
    .AddPolicy("Admin", policy => policy.RequireRole("Admin"));
// CRITICAL FIX: CORS policies must explicitly define origins when dealing with Cookies/Credentials
builder.Services.AddCors(options =>
{
    options.AddPolicy("NextJsCorsPolicy", policy =>
    {
        policy.WithOrigins("https://www.aml2ligand.online", "https://aml2ligand.online","http://localhost:3000") // Must be explicit for credentials!
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials(); // This line allows the browser to send the HttpOnly cookie
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi(); // exposes /openapi/v1.json

    app.MapScalarApiReference(options =>
    {
        options
            .WithTitle("My API")
            .WithTheme(ScalarTheme.BluePlanet);
    });
}
app.UseCors("NextJsCorsPolicy");
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<AppDbContext>();
        await context.Database.MigrateAsync(); 
        Console.WriteLine("Database migration completed successfully!");
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating the database.");
    }
}
app.Run();
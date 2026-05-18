// Infrastructure/Services/SmtpEmailService.cs
using System.Net;
using System.Net.Mail;
using Application.Interfaces;
using Application.Interfaces.Services;
using Application.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;


public class SmtpEmailService : IEmailService
{
    private readonly EmailSettings _emailSettings;
    private readonly ILogger<SmtpEmailService> _logger;

    // We use IOptions<T> to cleanly inject strongly-typed settings from appsettings.json
    public SmtpEmailService(IOptions<EmailSettings> emailSettings, ILogger<SmtpEmailService> logger)
    {
        _emailSettings = emailSettings.Value;
        _logger = logger;
    }

    public async Task SendAnalysisCompleteEmailAsync(string toEmail, string clientName, string analysisId, string resultLink, CancellationToken ct = default)
    {
        try
        {
            var mailMessage = new MailMessage
            {
                From = new MailAddress(_emailSettings.SenderEmail, _emailSettings.SenderName),
                Subject = $"Your AML Analysis [{analysisId}] is Complete",
                IsBodyHtml = true,
                Body = GetHtmlBody(clientName, analysisId, resultLink)
            };

            mailMessage.To.Add(toEmail);

            using var smtpClient = new SmtpClient(_emailSettings.SmtpServer, _emailSettings.SmtpPort)
            {
                Credentials = new NetworkCredential(_emailSettings.SenderEmail, _emailSettings.Password),
                EnableSsl = _emailSettings.EnableSsl
            };

            _logger.LogInformation("Sending completion email for Analysis {AnalysisId} to {Email}", analysisId, toEmail);
            
            await smtpClient.SendMailAsync(mailMessage, ct);
            
            _logger.LogInformation("Email sent successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email for Analysis {AnalysisId} to {Email}", analysisId, toEmail);
            throw; // Rethrow if you want the caller to handle the failure, or swallow it if emails aren't mission-critical
        }
    }

    // A private helper to generate a clean, professional HTML email
    private string GetHtmlBody(string clientName, string analysisId, string resultLink)
    {
        return $@"
        <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; padding: 20px; border: 1px solid #e0e0e0; border-radius: 8px;'>
            <h2 style='color: #2c3e50;'>Analysis Complete</h2>
            <p>Hello <strong>{clientName}</strong>,</p>
            <p>Good news! Your Acute Myeloid Leukemia (AML) pipeline analysis has successfully finished processing.</p>
            
            <div style='background-color: #f8f9fa; padding: 15px; border-left: 4px solid #007bff; margin: 20px 0;'>
                <strong>Analysis ID:</strong> {analysisId}
            </div>

            <p>You can view your 3D protein structures, biomarker graphs, and download your CSV reports by clicking the button below:</p>
            
            <div style='text-align: center; margin: 30px 0;'>
                <a href='{resultLink}' style='background-color: #007bff; color: white; padding: 12px 24px; text-decoration: none; border-radius: 5px; font-weight: bold;'>View Results</a>
            </div>

            <p style='font-size: 12px; color: #7f8c8d;'>If the button does not work, copy and paste this link into your browser:<br>{resultLink}</p>
            
            <hr style='border: none; border-top: 1px solid #eee; margin: 20px 0;' />
            <p style='font-size: 12px; color: #95a5a6; text-align: center;'>Automated message from the AML Analysis System. Please do not reply.</p>
        </div>";
    }
}
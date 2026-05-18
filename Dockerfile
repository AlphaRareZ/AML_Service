FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy csproj files to restore dependencies
COPY ["Presentation/Presentation.csproj", "Presentation/"]
COPY ["Application/Application.csproj", "Application/"]
COPY ["Domain/Domain.csproj", "Domain/"]
COPY ["Infrastructure/Infrastructure.csproj", "Infrastructure/"]

RUN dotnet restore "Presentation/Presentation.csproj"

# Copy the remaining source code
COPY . .
WORKDIR "/src/Presentation"

# Build the application
RUN dotnet build "Presentation.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "Presentation.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final stage runtime image
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

# ASP.NET Core 8.0+ images use port 8080 by default
EXPOSE 5193

ENTRYPOINT ["dotnet", "Presentation.dll"]

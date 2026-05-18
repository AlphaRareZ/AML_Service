namespace Application.Interfaces.Services;

public interface IEmailService
{
    Task SendAnalysisCompleteEmailAsync(string toEmail, string clientName, string analysisId, string resultLink, CancellationToken ct = default);
}
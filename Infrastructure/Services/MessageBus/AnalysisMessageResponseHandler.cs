using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Domain.Entities;
using Domain.Enums;
using Domain.Extensions;
using Infrastructure.Interfaces;
using Infrastructure.Models.MessageBus;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services.MessageBus;

public class AnalysisResponseMessageHandler(
    ILogger<AnalysisResponseMessage> _logger,
    IAnalysisService service,
    IAnalysisRepository repo,
    IEmailService emailService)
    : IResponseMessageService<AnalysisResponseMessage>
{
    public async Task DoWork(AnalysisResponseMessage? result, CancellationToken ct)
    {
        if (result == null)
        {
            _logger.LogInformation("Analysis message received an empty response");
        }
        else if (result.Success == false)
        {
            _logger.LogInformation("Analysis message received an error {msg}", result.Message);
        }

        foreach (var file in result.DownloadLinks)
        {
            string fileName = file.Key;
            string fileUrl = file.Value;
            FileType fileType = fileName.ToFileType();
            // 3. Classify based on the file extension or name
            if (fileType == FileType.Pdb)
            {
                // It's a Protein 3D Structure
                _logger.LogInformation("PROTEIN FILE FOUND: {Name} -> {Url}", fileName, fileUrl);
                var x = new Protein()
                {
                    Name = fileName,
                    AnalysisId = new Guid(result.AnalysisId),
                    StructureUrl = fileUrl
                };
                var created = await service.CreateProteinAsync(x, ct);
                _logger.LogInformation("PROTEIN Created:{Created}", created);
            }
            // Analysis File
            else if (fileType == FileType.Json || fileType == FileType.Csv || fileType == FileType.Png)
            {
                // It's a Graph / Image
                _logger.LogInformation("Analysis File FOUND: {Name} -> {Url}", fileName, fileUrl);

                var analysisFile = new AnalysisFile()
                {
                    AnalysisId = new Guid(result.AnalysisId),
                    Type = fileType,
                    FileName = fileName,
                    FileUrl = fileUrl
                };
                var created = await service.CreateAnalysisFileAsync(analysisFile, ct);
                _logger.LogInformation("Analysis File Created:{Created}", created);
            }
            else
            {
                // Unknown type fallback
                _logger.LogWarning("Unknown file type: {Name}", fileName);
            }
        }
        
        var analysis = await repo.GetAnalysisByIdAsync(new Guid(result.AnalysisId), ct);
        
        if (analysis == null) return;
        
        analysis.Success = result.Success;
        analysis.FilesFailed = result.FilesFailed;
        analysis.FilesUploaded = result.FilesUploaded;
        analysis.Status = result.Success? AnalysisStatus.Completed : AnalysisStatus.Failed;
        
        // Update the analysis with the new data
        await repo.UpdateAnalysisAsync(analysis, ct);
        // notify the user that his analysis has completed
        await emailService.SendAnalysisCompleteEmailAsync(result.Email, result.Username, result.AnalysisId,
            "Analysis Completed", ct);
    }
}
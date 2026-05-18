using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Application.UseCases.DTOs;
using Application.UseCases.DTOs;
using Application.UseCases.Models;
using Domain.Entities;
using Domain.Enums;

namespace Application.Services.Core;

public class AnalysisService(IAnalysisRepository repo, IMessageProducer<AnalysisMessage> producer)
    : IAnalysisService
{
    public async Task<CreateAnalysisDTO?> CreateAnalysisAsync(AnalysisMetaData data, CancellationToken token)
    {
        var analysis = new Analysis()
        {
            UserID = data.UserId,
            Name = data.Name,
            ExonDataUrl = data.ExonDataUrl,
            MappingDataUrl = data.MappingDataUrl,
        };
        var analysisGuid = await repo.CreateAnalysisAsync(analysis, token);

        var positionInQueue = await producer.PublishAsync(new AnalysisMessage()
        {
            AnalysisID = analysisGuid.ToString(),
            expression_file_url = data.ExonDataUrl,
            mapping_file_url = data.MappingDataUrl,
            Email = data.Email,
            Username = data.Username
        }, token);
        await repo.UpdateAnalysisStatusAsync(analysisGuid,AnalysisStatus.Pending, token);
        return new CreateAnalysisDTO()
        {
            AnalysisId = analysisGuid,
            EstimatedTime = 3.04 * positionInQueue,
            PositionInQueue = positionInQueue,
        };
    }

    public async Task<DeleteAnalysisDTO?> DeleteAnalysisAsync(Guid analysisId, CancellationToken token)
    {
        var numberOfRowsAffected = await repo.DeleteAnalysisByIdAsync(analysisId, token);
        /*
         * Zero Refers to 0 Number of rows affected which means there is no analysis id matching this id
         * One Refers to one deleted analysis matching the id
         */
        return numberOfRowsAffected switch
        {
            0 => null,
            1 => new DeleteAnalysisDTO()
            {
                AnalysisId = analysisId.ToString(),
            },
            _ => null
        };
    }

    public async Task<UpdateAnalysisDTO?> UpdateAnalysisAsync(Analysis analysis, CancellationToken token)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> CreateAnalysisFileAsync(AnalysisFile file, CancellationToken token)
    {
        return await repo.CreateAnalysisFileAsync(file, token);
    }

    public async Task<bool> CreateProteinAsync(Protein protein, CancellationToken token)
    {
        return await repo.CreateProteinAsync(protein, token);
    }

    public async Task<AnalysisDTO?> GetAnalysisByIdAsync(Guid id, CancellationToken ct)
    {
        var x = await repo.GetAnalysisByIdAsync(id, ct);
        var dto = x?.ToDTO();
        return dto;
    }

    public async Task<AnalysisListDTO?> GetUserAnalysesAsync(string userId, CancellationToken ct)
    {
        var analyses = await repo.GetUserAnalysesAsync(userId, ct); // returns IReadOnlyList<Analysis>
        if (analyses.Count == 0) return null;
        var dto = new AnalysisListDTO()
        {
            Analyses = analyses.Select(a => a.ToDTO()).ToList(),
        };

        return dto;
    }

    public async Task<AnalysisListDTO?> GetUserAnalysesByStatus(string userId, AnalysisStatus status,
        CancellationToken ct)
    {
        var analyses = await repo.GetUserAnalysesByStatusAsync(userId, status, ct);
        if (analyses.Count == 0) return null;
        var dto = new AnalysisListDTO()
        {
            Analyses = analyses.Select(a => a.ToDTO()).ToList(),
        };
        return dto;
    }

    public async Task<AnalysisStatusDTO?> GetAnalysisStatusByIdAsync(Guid id, CancellationToken ct)
    {
        var status = await repo.GetAnalysisStatusByIdAsync(id, ct);

        return status switch
        {
            null =>null,
            _ => new AnalysisStatusDTO()
            {
                Status = status.GetValueOrDefault().ToString()
            }
        };
    }
}
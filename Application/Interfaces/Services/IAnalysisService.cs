using Application.UseCases.DTOs;
using Application.UseCases.DTOs;
using Application.UseCases.Models;
using Domain.Entities;
using Domain.Enums;

namespace Application.Interfaces.Services;

public interface IAnalysisService
{
    // Done
    Task<CreateAnalysisDTO?> CreateAnalysisAsync(AnalysisMetaData data, CancellationToken token);
    
    Task<DeleteAnalysisDTO?> DeleteAnalysisAsync(Guid analysisId, CancellationToken token);
    Task<UpdateAnalysisDTO?> UpdateAnalysisAsync(Analysis analysis, CancellationToken token);
    Task<bool> CreateAnalysisFileAsync(AnalysisFile file, CancellationToken token);
    Task<bool> CreateProteinAsync(Protein protein, CancellationToken token);
    
    Task<AnalysisDTO?> GetAnalysisByIdAsync(Guid id, CancellationToken ct);
    Task<AnalysisListDTO?> GetUserAnalysesAsync(string userId, CancellationToken ct);
    Task<AnalysisListDTO?> GetUserAnalysesByStatus(string userId,AnalysisStatus status,CancellationToken ct);
    Task<AnalysisStatusDTO?> GetAnalysisStatusByIdAsync(Guid id, CancellationToken ct);
}
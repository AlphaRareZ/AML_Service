using Domain.Entities;
using Domain.Enums;

namespace Application.Interfaces.Repositories;

public interface IAnalysisRepository
{
    // Query
    Task<IReadOnlyList<Analysis>> GetUserAnalysesAsync(string userId, CancellationToken ct);
    Task<IReadOnlyList<Analysis>> GetUserAnalysesByStatusAsync(string userId,AnalysisStatus status,CancellationToken ct);
    Task<AnalysisStatus?> GetAnalysisStatusByIdAsync(Guid id,CancellationToken ct);
    Task<Analysis?> GetAnalysisByIdAsync(Guid id, CancellationToken ct);
    Task<Protein?> GetProteinByIdAsync(int id, CancellationToken ct);
    // Command
    Task<Guid> CreateAnalysisAsync(Analysis analysis, CancellationToken ct);
    Task<bool> CreateAnalysisFileAsync(AnalysisFile file, CancellationToken ct);
    Task<bool> CreateProteinAsync(Protein protein, CancellationToken ct);
    Task<bool> UpdateAnalysisAsync(Analysis analysis, CancellationToken ct);
    Task<bool> UpdateProteinAsync(Protein protein, CancellationToken ct);

    Task<bool> UpdateAnalysisStatusAsync(Guid id,AnalysisStatus status, CancellationToken ct);
    Task<int> DeleteAnalysisByIdAsync(Guid analysisId, CancellationToken ct);
}
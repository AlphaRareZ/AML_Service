using Application.Interfaces.Repositories;
using Domain;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class AnalysisRepository(AppDbContext context) : IAnalysisRepository
{
    public async Task<IReadOnlyList<Analysis>> GetUserAnalysesAsync(string userId, CancellationToken ct)
    {
        return await context.Analyses.Where(a => a.UserID == userId).ToListAsync(cancellationToken: ct);
    }

    public async Task<IReadOnlyList<Analysis>> GetUserAnalysesByStatusAsync(string userId, AnalysisStatus status,
        CancellationToken ct)
    {
        return await context.Analyses.Where(a => a.UserID == userId)
            .Where(a => a.Status.ToString() == status.ToString()).ToListAsync(cancellationToken: ct);
    }

    public async Task<AnalysisStatus?> GetAnalysisStatusByIdAsync(Guid id, CancellationToken ct)
    {
        var x = await context.Analyses.Where(a => a.Id == id).FirstOrDefaultAsync(cancellationToken: ct);
        return x?.Status;
    }

    public async Task<Analysis?> GetAnalysisByIdAsync(Guid id, CancellationToken ct)
    {
        return await context.Analyses
            .Where(a => a.Id == id)
            .Include(a => a.Proteins)
            .Include(a => a.Files)
            .FirstOrDefaultAsync(cancellationToken: ct);
    }

    public async Task<Protein?> GetProteinByIdAsync(int id, CancellationToken ct)
    {
        return await context.Proteins.Where(a => id == a.Id).FirstOrDefaultAsync(cancellationToken: ct);
    }

    public async Task<Guid> CreateAnalysisAsync(Analysis analysis, CancellationToken ct)
    {
        var entity = await context.Analyses.AddAsync(analysis, ct);
        var x = await context.SaveChangesAsync(cancellationToken: ct);
        return entity.Entity.Id;
    }

    public async Task<bool> CreateAnalysisFileAsync(AnalysisFile file, CancellationToken ct)
    {
        await context.AddAsync(file, ct);
        var rowsAffected = await context.SaveChangesAsync(ct);
        return rowsAffected > 0;
    }

    public async Task<bool> CreateProteinAsync(Protein protein, CancellationToken ct)
    {
        await context.AddAsync(protein, ct);
        var rowsAffected = await context.SaveChangesAsync(ct);
        return rowsAffected > 0;
    }


    public async Task<bool> UpdateAnalysisAsync(Analysis analysis, CancellationToken ct)
    {
        context.Analyses.Update(analysis);
        var n = await context.SaveChangesAsync(ct);
        return n > 0;
    }

    public async Task<bool> UpdateProteinAsync(Protein protein, CancellationToken ct)
    {
        context.Proteins.Update(protein);
        var n = await context.SaveChangesAsync(ct);
        return n > 0;
    }

    public async Task<bool> UpdateAnalysisStatusAsync(Guid id, AnalysisStatus status, CancellationToken ct)
    {
        var analysis = await context.Analyses.FindAsync(id, ct);
        if (analysis is null) return false;
        analysis.Status = status;
        await context.SaveChangesAsync(ct);
        return true;
    }

    public async Task<int> DeleteAnalysisByIdAsync(Guid analysisId, CancellationToken ct)
    {
        var entity = await context.Analyses.FirstOrDefaultAsync(a => a.Id == analysisId, ct);
        if (entity == null) return 0;
        context.Analyses.Remove(entity);
        return await context.SaveChangesAsync(ct);
    }
}
using System.Xml;
using Application.Interfaces.Repositories;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class LigandRepository(AppDbContext context) : ILigandRepository
{
    public async Task<IReadOnlyList<Ligand>> GetLigandsAsync(int proteinId, CancellationToken ct)
    {
        return await context.Ligands.Where(l => l.ProteinId == proteinId).ToListAsync(ct);
    }

    public async Task<(IReadOnlyList<Protein>proteins, int count)> GetPagedProteinsWithLigandsByUserId(string userId,
        int pageNumber,
        int pageSize, CancellationToken ct)
    {
        var query = await context.Proteins
            .Where(a => a.Analysis.UserID == userId && a.Ligands.Any())
            .OrderByDescending(a => a.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Include(a => a.Ligands)
            .ToListAsync(ct);
        var count = await context.Proteins.Where(a => a.Analysis.UserID == userId).CountAsync(ct);
        return (query, count);
    }

    public async Task<(IReadOnlyList<Protein>proteins, int count)> GetPagedProteinsWithNoLigandsByUserId(string userId,
        int pageNumber,
        int pageSize, CancellationToken ct)
    {
        var query = await context.Proteins
            .Where(a => a.Analysis.UserID == userId && !a.Ligands.Any())
            .OrderByDescending(a => a.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Include(a => a.Ligands)
            .ToListAsync(ct);
        var count = await context.Proteins.Where(a => a.Analysis.UserID == userId).CountAsync(ct);
        return (query, count);
    }

    public async Task<int> CreateLigandAsync(Ligand ligand, CancellationToken ct)
    {
        await context.AddAsync(ligand, ct);
        return await context.SaveChangesAsync(ct);
    }

    public async Task<int> AddLigandsAsync(List<Ligand> newLigands, CancellationToken ct)
    {
        await context.AddRangeAsync(newLigands, ct);
        return await context.SaveChangesAsync(ct);
    }

    public async Task<int> GetTotalRowsCountAsync(string userId, CancellationToken ct)
    {
        return await context.Proteins.Where(a => a.Analysis.UserID == userId).CountAsync(cancellationToken: ct);
    }
}
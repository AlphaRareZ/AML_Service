using Domain.Entities;

namespace Application.Interfaces.Repositories;

public interface ILigandRepository
{
    Task<IReadOnlyList<Ligand>> GetLigandsAsync(int proteinId, CancellationToken ct);
    Task<(IReadOnlyList<Protein> proteins,int count)> GetPagedProteinsWithLigandsByUserId(string userId,int pageNumber,int pageSize, CancellationToken ct);
    Task<(IReadOnlyList<Protein>proteins,int count)> GetPagedProteinsWithNoLigandsByUserId(string userId,int pageNumber,int pageSize, CancellationToken ct);
    Task<int> CreateLigandAsync(Ligand ligand, CancellationToken ct);

    Task<int> AddLigandsAsync(List<Ligand> newLigands, CancellationToken ct);
    Task<int> GetTotalRowsCountAsync(string userId,CancellationToken ct);
}
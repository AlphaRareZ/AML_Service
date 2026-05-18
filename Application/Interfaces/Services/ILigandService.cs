using Application.UseCases.DTOs.Ligands;
using Domain.Entities;

namespace Application.Interfaces.Services;

public interface ILigandService
{
    Task<CreateLigandDTO?> CreateLigandsAsync(Dictionary<int, string> accessions, CancellationToken token);
    Task<DeleteLigandDTO?> DeleteLigandAsync(int id, CancellationToken token);
    Task<LigandsListDTO?> GetLigandsByProteinId(int id, CancellationToken ct);

    Task<ProteinsListDTO?> GetPagedProteinsWithLigandsByUserId(string userId, int pageNumber, int pageSize,
        CancellationToken ct);

    Task<ProteinsListDTO?> GetPagedProteinsWithNoLigandsByUserId(string userId, int pageNumber, int pageSize,
        CancellationToken ct);
}
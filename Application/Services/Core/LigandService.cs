using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Application.UseCases.DTOs;
using Application.UseCases.DTOs.Ligands;
using Application.UseCases.Models;

namespace Application.Services.Core;

public class LigandService(ILigandRepository repo, IMessageProducer<LigandMessage> producer) : ILigandService
{
    public async Task<CreateLigandDTO?> CreateLigandsAsync(Dictionary<int, string> accessions, CancellationToken token)
    {
        int queueCount = 0;
        foreach (var kvp in accessions)
        {
            var proteinId = kvp.Key;
            var proteinAccession = kvp.Value;
            var qCount = await producer.PublishAsync(new LigandMessage()
            {
                ProteinId = proteinId,
                PDBAccession = proteinAccession
            }, token);

            queueCount = int.Max(queueCount, qCount);
        }

        return new CreateLigandDTO()
        {
            EstimatedTime = queueCount * 1.0,
            PositionInQueue = queueCount
        };
    }

    public async Task<DeleteLigandDTO?> DeleteLigandAsync(int id, CancellationToken token)
    {
        throw new NotImplementedException();
    }

    public async Task<LigandsListDTO?> GetLigandsByProteinId(int id, CancellationToken ct)
    {
        var result = await repo.GetLigandsAsync(id, ct);
        return result.Count == 0
            ? null
            : new LigandsListDTO()
            {
                Ligands = result.Select(a => a.ToDTO()).ToList(),
            };
    }

    public async Task<ProteinsListDTO?> GetPagedProteinsWithLigandsByUserId(string userId, int pageNumber, int pageSize,
        CancellationToken ct)
    {
        var result = await repo.GetPagedProteinsWithLigandsByUserId(userId, pageNumber, pageSize, ct);
        return result.proteins.Count == 0
            ? null
            : new ProteinsListDTO()
            {
                Proteins = result.proteins.Select(a => a.ToDTO()).ToList(),
                TotalCount = result.count,
            };
    }public async Task<ProteinsListDTO?> GetPagedProteinsWithNoLigandsByUserId(string userId, int pageNumber, int pageSize,
        CancellationToken ct)
    {
        var result = await repo.GetPagedProteinsWithNoLigandsByUserId(userId, pageNumber, pageSize, ct);
        return result.proteins.Count == 0
            ? null
            : new ProteinsListDTO()
            {
                Proteins = result.proteins.Select(a => a.ToDTO()).ToList(),
                TotalCount = result.count,
            };
    }
}
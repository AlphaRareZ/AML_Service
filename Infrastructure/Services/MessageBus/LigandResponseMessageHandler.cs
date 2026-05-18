using System.IO;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Interfaces;
using Infrastructure.Models.MessageBus;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services.MessageBus;

public class LigandResponseMessageHandler(
    ILogger<LigandResponseMessageHandler> _logger,
    ILigandRepository ligandRepo,
    IAnalysisRepository analysisRepo)
    : IResponseMessageService<LigandResponseMessage>
{
    public async Task DoWork(LigandResponseMessage? result, CancellationToken ct)
    {
        if (result == null)
        {
            _logger.LogWarning("Analysis message received an empty response");
            return;
        }

        if (!result.Success)
        {
            _logger.LogError("Analysis message received an error: {Message}", result.Message);
        }

        string? proteinPngUrl = null;
        string? proteinCsvUrl = null;
        
        // Dictionary to group ligand files by their base name (without extension)
        // Key: Ligand Name, Value: Tuple of (PdbUrl, SdfUrl)
        var ligandFiles = new Dictionary<string, (string? PdbUrl, string? SdfUrl)>();

        // 1. Parse and group all incoming files in one pass
        foreach (var file in result.DownloadLinks)
        {
            string fileName = file.Key;
            string fileUrl = file.Value;
            
            if (fileName.EndsWith(".png", StringComparison.OrdinalIgnoreCase))
            {
                proteinPngUrl = fileUrl;
                _logger.LogInformation("PROTEIN IMAGE FOUND: {Name} -> {Url}", fileName, fileUrl);
            }
            else if (fileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
            {
                proteinCsvUrl = fileUrl;
                _logger.LogInformation("PROTEIN CSV FOUND: {Name} -> {Url}", fileName, fileUrl);
            }
            else if (fileName.EndsWith(".pdb", StringComparison.OrdinalIgnoreCase))
            {
                string baseName = Path.GetFileNameWithoutExtension(fileName);
                if (!ligandFiles.ContainsKey(baseName)) ligandFiles[baseName] = (null, null);
                
                ligandFiles[baseName] = (fileUrl, ligandFiles[baseName].SdfUrl);
            }
            else if (fileName.EndsWith(".sdf", StringComparison.OrdinalIgnoreCase))
            {
                string baseName = Path.GetFileNameWithoutExtension(fileName);
                if (!ligandFiles.ContainsKey(baseName)) ligandFiles[baseName] = (null, null);
                
                ligandFiles[baseName] = (ligandFiles[baseName].PdbUrl, fileUrl);
            }
            else
            {
                _logger.LogWarning("Unknown file type ignored: {Name}", fileName);
            }
        }

        // 2. Fetch the Protein (Ideally, your repo method should .Include(p => p.Analysis))
        var protein = await analysisRepo.GetProteinByIdAsync(result.ProteinId, ct);
        if (protein == null)
        {
            _logger.LogWarning("Protein with ID {ProteinId} not found. Cannot attach ligands.", result.ProteinId);
            return;
        }

        // 3. Assign Protein Files
        if (proteinPngUrl != null) protein.Top10AdvancedSaveLigandsImgUrl = proteinPngUrl;
        if (proteinCsvUrl != null) protein.Top10AdvancedSaveLigandsCsvUrl = proteinCsvUrl;

        // 4. Assemble the grouped Ligands into entity objects
        var newLigands = new List<Ligand>();
        foreach (var kvp in ligandFiles)
        {
            newLigands.Add(new Ligand
            {
                Name = kvp.Key,
                PdbUrl = kvp.Value.PdbUrl ?? string.Empty,
                SdfUrl = kvp.Value.SdfUrl ?? string.Empty,
                ProteinId = result.ProteinId
            });
            
            _logger.LogInformation("LIGAND ASSEMBLED: {Name} | PDB: {PdbUrl} | SDF: {SdfUrl}", 
                kvp.Key, kvp.Value.PdbUrl, kvp.Value.SdfUrl);
        }

        // 6. Bulk Save to Database (Adjust method names to match your actual ILigandRepository)
        await analysisRepo.UpdateProteinAsync(protein, ct); // Saves PNG/CSV updates
        
        if (newLigands.Count != 0)
        {
            await ligandRepo.AddLigandsAsync(newLigands, ct); // Saves all ligands in ONE round-trip (e.g. AddRangeAsync)
        }

     
    }
}
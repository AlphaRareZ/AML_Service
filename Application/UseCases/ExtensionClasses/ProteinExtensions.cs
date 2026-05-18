using Domain.Entities;

namespace Application.UseCases.DTOs;

public static class ProteinExtensions
{
    public static ProtienDTO ToDTO(this Protein protein)
    {
        return new ProtienDTO()
        {
            Id = protein.Id,
            Name = protein.Name,
            StructureUrl = protein.StructureUrl,
            CreatedAt = protein.CreatedAt,
            Ligands = protein.Ligands,
            Top10AdvancedSaveLigandsCsvUrl = protein.Top10AdvancedSaveLigandsCsvUrl,
            Top10AdvancedSaveLigandsImgUrl = protein.Top10AdvancedSaveLigandsImgUrl,
        };
    }
}
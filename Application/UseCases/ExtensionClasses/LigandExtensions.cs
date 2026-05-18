using Application.UseCases.DTOs.Ligands;
using Domain.Entities;

namespace Application.UseCases.DTOs;

public static class LigandExtensions
{
    public static LigandDTO ToDTO(this Ligand ligand)
    {
        return new LigandDTO()
        {
            Id = ligand.Id,
            Name = ligand.Name,
            PdbUrl = ligand.PdbUrl,
            SdfUrl = ligand.SdfUrl
        };
    }
}
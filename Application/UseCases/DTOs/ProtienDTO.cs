using Application.Services.Core;
using Application.UseCases.DTOs;
using Domain.Entities;

namespace Application.UseCases.DTOs;

public class ProtienDTO
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string StructureUrl { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Top10AdvancedSaveLigandsImgUrl { get; set; }
    public string Top10AdvancedSaveLigandsCsvUrl { get; set; }
    public ICollection<Ligand> Ligands { get; set; }
}
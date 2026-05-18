using System.Text.Json.Serialization;

namespace Domain.Entities;

public class Protein
{
    public int Id { get; set; }

    public string Name { get; set; }
    public string StructureUrl { get; set; }
    public string Top10AdvancedSaveLigandsImgUrl { get; set; }
    public string Top10AdvancedSaveLigandsCsvUrl { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid AnalysisId { get; set; }

    [JsonIgnore]
    public Analysis Analysis { get; set; }
    
    public ICollection<Ligand> Ligands { get; set; }
    
}
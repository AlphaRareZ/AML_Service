using System.Text.Json.Serialization;

namespace Domain.Entities;

public class Ligand
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string PdbUrl { get; set; }
    public string SdfUrl { get; set; }

    public int ProteinId { get; set; }
    [JsonIgnore]
    public Protein Protein { get; set; }
}
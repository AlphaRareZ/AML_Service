namespace Application.UseCases.DTOs.Ligands;
public class ProteinsListDTO
{
    public ICollection<ProtienDTO>? Proteins { get; set; } = new List<ProtienDTO>();
    public int TotalCount { get; set; }

}
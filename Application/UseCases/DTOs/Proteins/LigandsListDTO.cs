namespace Application.UseCases.DTOs.Ligands;

public class LigandsListDTO
{
    public ICollection<LigandDTO>? Ligands { get; set; } = new List<LigandDTO>();

}
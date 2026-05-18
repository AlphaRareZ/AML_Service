using Application.UseCases.DTOs;

namespace Application.UseCases.DTOs;

public class AnalysisListDTO
{
    public ICollection<AnalysisDTO>? Analyses { get; set; } = new List<AnalysisDTO>();
}
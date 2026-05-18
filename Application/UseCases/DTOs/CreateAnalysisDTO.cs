using Application.UseCases.DTOs;

namespace Application.UseCases.DTOs;

public class CreateAnalysisDTO
{
    public Guid AnalysisId { get; set; }
    public double EstimatedTime { get; set; }
    public int PositionInQueue { get; set; }
}
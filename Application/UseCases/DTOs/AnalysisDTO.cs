using Application.UseCases.DTOs;
using Domain.Enums;

namespace Application.UseCases.DTOs;

public class AnalysisDTO
{
    public Guid Id { get; set; }
    public string UserID { get; set; }
    public string Name { get; set; }
    public string Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public ICollection<ProtienDTO>? Proteins { get; set; }
    public ICollection<AnalysisFileDTO>? Files { get; set; }
}
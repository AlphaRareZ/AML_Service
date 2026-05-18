using Application.UseCases.DTOs;

namespace Application.UseCases.DTOs;

public class AnalysisFileDTO
{
    public int Id { get; set; }

    public string FileName { get; set; }

    public string FileUrl { get; set; }

    public string Type { get; set; }
}
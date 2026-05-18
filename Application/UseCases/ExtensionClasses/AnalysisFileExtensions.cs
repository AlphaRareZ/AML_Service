using Domain.Entities;

namespace Application.UseCases.DTOs;

public static class AnalysisFileExtensions
{
    public static AnalysisFileDTO ToDTO(this AnalysisFile file)
    {
        return new AnalysisFileDTO()
        {
            FileName = file.FileName,
            FileUrl = file.FileUrl,
            Id = file.Id,
            Type = file.Type.ToString(),
        };
    }
}
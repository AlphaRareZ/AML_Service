using Domain.Entities;

namespace Application.UseCases.DTOs;

public static class AnalysisExtensions
{
    public static AnalysisDTO ToDTO(this Analysis analysis)
    {
        return new AnalysisDTO
        {
            Id = analysis.Id,
            Name = analysis.Name,
            UserID = analysis.UserID,
            Status = analysis.Status.ToString(),
            CreatedAt = analysis.CreatedAt,

            Proteins = (analysis.Proteins ?? Enumerable.Empty<Protein>())
                .Select(p => p.ToDTO())
                .ToList(),
            Files = (analysis.Files ?? Enumerable.Empty<AnalysisFile>())
                .Select(f => f.ToDTO())
                .ToList()
        };
    }
}
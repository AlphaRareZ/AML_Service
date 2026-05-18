using System.Text.Json.Serialization;
using Domain.Enums;

namespace Domain.Entities;

public class AnalysisFile
{
    public int Id { get; set; }

    public string FileName { get; set; }

    public string FileUrl { get; set; }

    public FileType Type { get; set; }

    public Guid AnalysisId { get; set; }
    [JsonIgnore]
    public Analysis Analysis { get; set; }
}
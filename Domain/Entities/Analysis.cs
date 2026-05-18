using System.Runtime.InteropServices.JavaScript;
using Domain.Enums;

namespace Domain.Entities;

public class Analysis
{
    public Guid Id { get; set; }
    public string ExonDataUrl { get; set; }
    public string MappingDataUrl { get; set; }
    public string UserID { get; set; }
    public string Name { get; set; }
    public AnalysisStatus Status { get; set; } = AnalysisStatus.ToDo;
    public bool Success { get; set; }
    public int FilesUploaded { get; set; }
    public int FilesFailed { get; set; }
    public DateTime CreatedAt { get; set; }

    public ICollection<Protein> Proteins { get; set; }
    public ICollection<AnalysisFile> Files { get; set; }
}
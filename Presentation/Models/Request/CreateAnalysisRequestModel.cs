using Application.UseCases.Models;

namespace Presentation.Models.Request;

public record CreateAnalysisRequestModel(string Name,string ExonDataUrl,string MappingDataUrl);

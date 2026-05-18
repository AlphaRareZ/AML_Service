using Application.CQRS.Queries;
using Application.Interfaces.Services;
using Application.UseCases.DTOs;
using MediatR;

namespace Application.CQRS.Handlers.QueryHandlers;

public class GetAnalysisByIdQueryHandler(IAnalysisService service) : IRequestHandler<GetAnalysisByIdQuery, AnalysisDTO?>
{
    public async Task<AnalysisDTO?> Handle(GetAnalysisByIdQuery request, CancellationToken cancellationToken)
    {
        return await service.GetAnalysisByIdAsync(request.Id, cancellationToken);
    }
}
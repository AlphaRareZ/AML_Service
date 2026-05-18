using Application.CQRS.Queries;
using Application.Interfaces.Services;
using Application.UseCases.DTOs;
using Application.UseCases.DTOs;
using MediatR;

namespace Application.CQRS.Handlers.QueryHandlers;

public class GetUserAnalysesByStatusQueryHandler(IAnalysisService service) : IRequestHandler<GetUserAnalysesByStatusQuery, AnalysisListDTO?>
{
    public async Task<AnalysisListDTO?> Handle(GetUserAnalysesByStatusQuery request, CancellationToken cancellationToken)
    {
        return await service.GetUserAnalysesByStatus(request.UserId, request.Status, cancellationToken);
    }
}
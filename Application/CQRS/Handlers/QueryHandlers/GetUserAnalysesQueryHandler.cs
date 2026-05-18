using Application.CQRS.Queries;
using Application.Interfaces.Services;
using Application.UseCases.DTOs;
using Application.UseCases.DTOs;
using MediatR;

namespace Application.CQRS.Handlers.QueryHandlers;

public class GetUserAnalysesQueryHandler(IAnalysisService service)
    : IRequestHandler<GetUserAnalysesQuery, AnalysisListDTO?>
{
    public async Task<AnalysisListDTO?> Handle(GetUserAnalysesQuery request, CancellationToken cancellationToken)
    {
        return  await service.GetUserAnalysesAsync(request.userID, cancellationToken);
    }
}
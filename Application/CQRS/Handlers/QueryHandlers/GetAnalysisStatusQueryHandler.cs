using Application.CQRS.Queries;
using Application.Interfaces.Services;
using Application.UseCases.DTOs;
using MediatR;

namespace Application.CQRS.Handlers.QueryHandlers;

public class 
    GetAnalysisStatusQueryHandler(IAnalysisService service):IRequestHandler<GetAnalysisStatusQuery,AnalysisStatusDTO?>
{
    public async Task<AnalysisStatusDTO?> Handle(GetAnalysisStatusQuery request, CancellationToken cancellationToken)
    {
        return await service.GetAnalysisStatusByIdAsync(request.Id, cancellationToken);
    }
}
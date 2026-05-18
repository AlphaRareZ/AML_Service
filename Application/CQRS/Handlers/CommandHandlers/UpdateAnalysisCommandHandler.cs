using Application.CQRS.Commands;
using Application.Interfaces.Services;
using MediatR;

namespace Application.CQRS.Handlers.CommandHandlers;

public class UpdateAnalysisCommandHandler(IAnalysisService service) : IRequestHandler<UpdateAnalysisCommand>
{
    public async Task Handle(UpdateAnalysisCommand request, CancellationToken cancellationToken)
    {
        await service.UpdateAnalysisAsync(request.analysis, cancellationToken);
    }
}
using Application.CQRS.Commands;
using Application.Interfaces.Services;
using Application.UseCases.DTOs;
using MediatR;

namespace Application.CQRS.Handlers.CommandHandlers;

public class DeleteAnalysisCommandHandler(IAnalysisService service) : IRequestHandler<DeleteAnalysisCommand,DeleteAnalysisDTO?>
{
    public async Task<DeleteAnalysisDTO?> Handle(DeleteAnalysisCommand request, CancellationToken cancellationToken)
    {
        return await service.DeleteAnalysisAsync(request.Id, cancellationToken);
    }
}
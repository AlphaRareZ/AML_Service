using Application.CQRS.Commands;
using Application.Interfaces.Services;
using Application.UseCases.DTOs;
using MediatR;

namespace Application.CQRS.Handlers.CommandHandlers;

public class CreateAnalysisCommandHandler(IAnalysisService service)
    : IRequestHandler<CreateAnalysisCommand, CreateAnalysisDTO?>
{
    public async Task<CreateAnalysisDTO?> Handle(CreateAnalysisCommand request, CancellationToken cancellationToken)
    {
        return await service.CreateAnalysisAsync(request.data, cancellationToken);
    }
}
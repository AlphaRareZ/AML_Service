using Application.Interfaces.Services;
using Application.UseCases.DTOs.Ligands;
using MediatR;

namespace Application.CQRS.Commands;

public class CreateLigandsCommandHandler(ILigandService service):IRequestHandler<CreateLigandsCommand, CreateLigandDTO?>
{
    public async Task<CreateLigandDTO?> Handle(CreateLigandsCommand request, CancellationToken ct)
    {
        return await service.CreateLigandsAsync(request.ProteinAccessions,ct);
    }
}
using System.Diagnostics.CodeAnalysis;
using Application.CQRS.Queries;
using Application.Interfaces.Services;
using Application.UseCases.DTOs.Ligands;
using MediatR;

namespace Application.CQRS.Handlers.QueryHandlers;

public class GetUserProteinsWithLigandsHandler(ILigandService service)
    : IRequestHandler<GetUserProteinsWithLigands, ProteinsListDTO?>
{
    public async Task<ProteinsListDTO?> Handle(GetUserProteinsWithLigands request, CancellationToken cancellationToken)
    {
        return await service.GetPagedProteinsWithLigandsByUserId(request.userID, request.pageNumber, request.pageSize,
            cancellationToken);
    }
}
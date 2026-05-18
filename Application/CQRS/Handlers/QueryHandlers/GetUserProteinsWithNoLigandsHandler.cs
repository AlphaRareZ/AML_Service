using Application.CQRS.Queries;
using Application.Interfaces.Services;
using Application.UseCases.DTOs.Ligands;
using MediatR;

namespace Application.CQRS.Handlers.QueryHandlers;

public class GetUserProteinsWithNoLigandsHandler(ILigandService service)
    : IRequestHandler<GetUserProteinsWithNoLigands, ProteinsListDTO?>
{
    public async Task<ProteinsListDTO?> Handle(GetUserProteinsWithNoLigands request, CancellationToken cancellationToken)
    {
        return await service.GetPagedProteinsWithNoLigandsByUserId(request.userID, request.pageNumber, request.pageSize,
            cancellationToken);
    }
}
using Application.UseCases.DTOs.Ligands;
using MediatR;

namespace Application.CQRS.Queries;

public record GetUserProteinsWithNoLigands(string userID,int pageNumber,int pageSize):IRequest<ProteinsListDTO?>;
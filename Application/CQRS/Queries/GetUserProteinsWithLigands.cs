using Application.UseCases.DTOs;
using Application.UseCases.DTOs.Ligands;
using MediatR;

namespace Application.CQRS.Queries;

public record GetUserProteinsWithLigands(string userID,int pageNumber,int pageSize):IRequest<ProteinsListDTO?>;
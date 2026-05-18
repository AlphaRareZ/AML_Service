using Application.UseCases.DTOs;
using Application.UseCases.DTOs;
using MediatR;

namespace Application.CQRS.Queries;

public record GetUserAnalysesQuery(string userID):IRequest<AnalysisListDTO?>;
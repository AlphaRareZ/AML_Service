using Application.UseCases.DTOs;
using MediatR;

namespace Application.CQRS.Queries;

public record GetAnalysisByIdQuery(Guid Id):IRequest<AnalysisDTO?>;
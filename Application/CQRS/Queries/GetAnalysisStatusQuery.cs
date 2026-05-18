using Application.UseCases.DTOs;
using Domain;
using MediatR;

namespace Application.CQRS.Queries;

public record GetAnalysisStatusQuery(Guid Id) : IRequest<AnalysisStatusDTO?>;
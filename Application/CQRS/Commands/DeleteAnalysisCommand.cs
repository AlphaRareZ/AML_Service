using Application.UseCases.DTOs;
using MediatR;

namespace Application.CQRS.Commands;

public record DeleteAnalysisCommand(Guid Id): IRequest<DeleteAnalysisDTO?>;
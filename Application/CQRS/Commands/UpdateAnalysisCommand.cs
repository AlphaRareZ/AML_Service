using Domain.Entities;
using MediatR;

namespace Application.CQRS.Commands;

public record UpdateAnalysisCommand(Analysis analysis):IRequest;
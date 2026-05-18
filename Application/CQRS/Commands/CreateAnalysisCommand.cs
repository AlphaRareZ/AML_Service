using Application.UseCases.DTOs;
using Application.UseCases.Models;
using Domain.Entities;
using MediatR;

namespace Application.CQRS.Commands;

public record CreateAnalysisCommand(AnalysisMetaData data):IRequest<CreateAnalysisDTO?>;
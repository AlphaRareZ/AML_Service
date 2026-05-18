using Application.UseCases.DTOs;
using Application.UseCases.DTOs;
using Domain;
using Domain.Enums;
using MediatR;

namespace Application.CQRS.Queries;

public record GetUserAnalysesByStatusQuery(string UserId,AnalysisStatus Status):IRequest<AnalysisListDTO?>;
using Application.UseCases.DTOs.Ligands;
using Application.UseCases.Models;
using MediatR;

namespace Application.CQRS.Commands;

public record CreateLigandsCommand(Dictionary<int,string> ProteinAccessions) : IRequest<CreateLigandDTO?>;
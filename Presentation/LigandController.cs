using System.Security.Claims;
using Application.CQRS.Commands;
using Application.CQRS.Queries;
using Application.UseCases.DTOs;
using Application.UseCases.DTOs.Ligands;
using Application.UseCases.Models;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Presentation.Models.Request;

namespace Presentation;

[Authorize("User")]
[ApiController]
[Route("api/[controller]")]
public class LigandController(IMediator mediator) : Controller
{
    [HttpPost("[action]")]
    public async Task<IActionResult> Create([FromBody] CreateLigandsRequestModel model)
    {
        var firstName = User.FindFirst("firstName")?.Value;
        var lastName = User.FindFirst("lastName")?.Value;
        var username = firstName + ' ' + lastName;
        var command = new CreateLigandsCommand(model.ProteinAccessions);
        var x = await mediator.Send(command);
        return x switch
        {
            null => BadRequest(new ApiResponse<string>
            {
                Success = false,
                Message = "Could not create Ligands for Protein-ID",
            }),
            _ => Ok(new ApiResponse<CreateLigandDTO>
            {
                Success = true,
                Message = "Created analysis Successfully",
                Data = x
            })
        };
    }

    [HttpGet("[action]")]
    public async Task<IActionResult> Get([FromQuery] Guid guid)
    {
        //throw new NotImplementedException();
        var userId = User.Claims.First(a => a.Type == ClaimTypes.NameIdentifier).Value;
        var command = new GetAnalysisByIdQuery(guid);
        var x = await mediator.Send(command);
        return x switch
        {
            null => NotFound(new ApiResponse<string>
            {
                Success = false,
                Message = $"Analysis not found with id {guid}",
            }),
            _ => x.UserID == userId
                ? Ok(new ApiResponse<AnalysisDTO>
                {
                    Success = true,
                    Message = "Retrieved analysis Successfully",
                    Data = x
                })
                : NotFound(new ApiResponse<string>
                {
                    Success = false,
                    Message = "The user_id doesn't match the analysis user_id",
                })
        };
    }

    [HttpGet("[action]")]
    [Authorize]
    public async Task<IActionResult> GetProteinsWithLigands([FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var userId = User.Claims.First(a => a.Type == ClaimTypes.NameIdentifier).Value;
        var command = new GetUserProteinsWithLigands(userId, pageNumber, pageSize);
        var x = await mediator.Send(command);
        int totalPages = 0;
        if (x is not null)
            totalPages = x.TotalCount / pageSize + (x.TotalCount % pageSize != 0 ? 1 : 0);
        return x switch
        {
            null => NotFound(new ApiResponse<string>
            {
                Success = false,
                Message = $"No Proteins found for user {userId}",
            }),
            _ => Ok(new PagedResponse<ICollection<ProtienDTO>>
            {
                Success = true,
                Message = $"Retrieved Proteins Successfully for user {userId}",
                Data = x.Proteins,
                TotalCount = x.TotalCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = totalPages,
                HasNextPage = pageNumber < totalPages,
                HasPreviousPage = pageNumber > 1
            })
        };
    }

    [HttpGet("[action]")]
    [Authorize]
    public async Task<IActionResult> GetProteinsWithNoLigands([FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var userId = User.Claims.First(a => a.Type == ClaimTypes.NameIdentifier).Value;
        var command = new GetUserProteinsWithNoLigands(userId, pageNumber, pageSize);
        var x = await mediator.Send(command);
        int totalPages = 0;
        if (x is not null)
            totalPages = x.TotalCount / pageSize + (x.TotalCount % pageSize != 0 ? 1 : 0);
        return x switch
        {
            null => NotFound(new ApiResponse<string>
            {
                Success = false,
                Message = $"No Proteins found for user {userId}",
            }),
            _ => Ok(new PagedResponse<ICollection<ProtienDTO>>
            {
                Success = true,
                Message = $"Retrieved Proteins Successfully for user {userId}",
                Data = x.Proteins,
                TotalCount = x.TotalCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = totalPages,
                HasNextPage = pageNumber < totalPages,
                HasPreviousPage = pageNumber > 1
            })
        };
    }
}
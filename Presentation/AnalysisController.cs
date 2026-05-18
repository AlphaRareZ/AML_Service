using System.Collections.Immutable;
using System.Security.Claims;
using Application.CQRS.Commands;
using Application.CQRS.Queries;
using Application.Interfaces.Services;
using Application.UseCases.DTOs;
using Application.UseCases.Models;
using Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Presentation.Models.Request;

namespace Presentation;

[ApiController]
[Route("api/[controller]")]
[Authorize("User")]
public class AnalysisController(IMediator mediator) : Controller
{
    [HttpPost("[action]")]
    public async Task<IActionResult> Create([FromBody] CreateAnalysisRequestModel model)
    {
        var userId = User.Claims.First(a => a.Type == ClaimTypes.NameIdentifier).Value;
        var email = User.Claims.First(a => a.Type == ClaimTypes.Email).Value;
        var firstName = User.FindFirst("firstName")?.Value;
        var lastName = User.FindFirst("lastName")?.Value;
        
        var command = new CreateAnalysisCommand(new AnalysisMetaData()
        {
            ExonDataUrl = model.ExonDataUrl,
            MappingDataUrl = model.MappingDataUrl,
            Name = model.Name,
            UserId = userId,
            Email = email,
            Username = firstName + " " + lastName,
        });

        var x = await mediator.Send(command);
        return x switch
        {
            null => BadRequest(new ApiResponse<string>
            {
                Success = false,
                Message = "Could not create analysis",
            }),
            _ => Ok(new ApiResponse<CreateAnalysisDTO>
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
    public async Task<IActionResult> GetAll()
    {
        var userId = User.Claims.First(a => a.Type == ClaimTypes.NameIdentifier).Value;
        var command = new GetUserAnalysesQuery(userId);
        var x = await mediator.Send(command);
        return x switch
        {
            null => NotFound(new ApiResponse<string>
            {
                Success = false,
                Message = $"No analyses found for user {userId}",
            }),
            _ => Ok(new ApiResponse<AnalysisListDTO>
            {
                Success = true,
                Message = $"Retrieved analyses Successfully for user {userId}",
                Data = x
            })
        };
    }

    [HttpPost("[action]")]
    public async Task<IActionResult> Delete([FromBody] DeleteAnalysisRequestModel request)
    {
        var analysis = await mediator.Send(new GetAnalysisByIdQuery(request.Guid));
        var userId = User.Claims.First(a => a.Type == ClaimTypes.NameIdentifier).Value;
        if (analysis is null)
            return NotFound(new ApiResponse<string>
            {
                Success = false,
                Message = $"Analysis with ID {request.Guid} does not exist",
            });
        if (analysis.UserID != userId)
            return BadRequest(new ApiResponse<string>
            {
                Success = false,
                Message = "The user_id doesn't match the analysis user_id"
            });
        
        var command = new DeleteAnalysisCommand(request.Guid);
        var x = await mediator.Send(command);
        return x switch
        {
            null => NotFound(new ApiResponse<string>
            {
                Success = false,
                Message = $"Analysis with ID {request.Guid} does not exist",
            }),
            _ => Ok(new ApiResponse<DeleteAnalysisDTO>
            {
                Success = true,
                Message = $"Successfully deleted analysis with id {request.Guid}",
                Data = x
            })
        };
    }

    [HttpGet("[action]")]
    public async Task<IActionResult> GetStatus([FromQuery] Guid id)
    {
        var command = new GetAnalysisStatusQuery(id);
        var x = await mediator.Send(command);
        return x switch
        {
            null => NotFound(new ApiResponse<string>
            {
                Success = false,
                Message = $"Analysis with id {id} does not exist",
            }),
            _ => Ok(new ApiResponse<AnalysisStatusDTO>
            {
                Success = true,
                Message = $"Successfully retrieved analysis status with id {id}",
                Data = x
            })
        };
    }

    [HttpGet("[action]")]
    public async Task<IActionResult> GetAllByStatus([FromQuery] AnalysisStatus status)
    {
        var userId = User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
        var command = new GetUserAnalysesByStatusQuery(userId, status);
        var x = await mediator.Send(command);
        return x switch
        {
            null => NotFound(new ApiResponse<string>
            {
                Success = false,
                Message = $"No Analyses are found for user {userId} by status {status}",
            }),
            _ => Ok(new ApiResponse<AnalysisListDTO>
            {
                Success = true,
                Message = $"Successfully retrieved Analyses for user {userId} by status {status}",
                Data = x
            })
        };
    }
}
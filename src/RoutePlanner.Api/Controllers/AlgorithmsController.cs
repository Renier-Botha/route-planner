namespace RoutePlanner.Api.Controllers;

using Microsoft.AspNetCore.Mvc;
using RoutePlanner.Api.Mappers;
using RoutePlanner.Api.Models.Requests;
using RoutePlanner.Api.Models.Responses;
using RoutePlanner.Core.Interfaces;

[ApiController]
[Route("api/algorithms")]
public class AlgorithmsController : ControllerBase
{
    private readonly IPathfindingAlgorithm _algorithm;
    
    public AlgorithmsController(IPathfindingAlgorithm algorithm)
    {
        _algorithm = algorithm;
    }

    /// <summary>Computes shortest path using the configured algorithm.</summary>
    /// <param name="requestDto">Grid configuration with start/end points.</param>
    /// <returns>Path with performance metrics.</returns>
    [HttpPost("pathfind")]
    [ProducesResponseType(typeof(PathfindingResponseDto), StatusCodes.Status200OK)]
    public ActionResult<PathfindingResponseDto> FindPath(
        [FromBody] PathfindingRequestDto requestDto)
    {
        // 1. Map DTO → Domain
        var domainRequest = PathfindingMapper.ToDomain(requestDto);
        
        // 2. Execute domain logic
        var domainResult = _algorithm.FindPath(domainRequest);
        
        // 3. Map Domain → DTO
        var responseDto = PathfindingMapper.ToDto(domainResult);
        
        return Ok(responseDto);
    }
}
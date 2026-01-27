namespace RoutePlanner.Api.Controllers;

using Microsoft.AspNetCore.Mvc;
using RoutePlanner.Api.Mappers;
using RoutePlanner.Api.Models.Requests;
using RoutePlanner.Api.Models.Responses;
using RoutePlanner.Core.Interfaces;

[ApiController]
[Route("api/algorithms")]
public class AlgorithmsController(IServiceProvider serviceProvider) : ControllerBase
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    /// <summary>Computes shortest path using the specified algorithm.</summary>
    /// <param name="algorithm">Algorithm to use: "astar" or "dijkstra".</param>
    /// <param name="requestDto">Grid configuration with start/end points.</param>
    /// <returns>Path with performance metrics.</returns>
    [HttpPost("pathfind/{algorithm}")]
    [ProducesResponseType(typeof(PathfindingResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<PathfindingResponseDto> FindPath(
        [FromRoute] string algorithm,
        [FromBody] PathfindingRequestDto requestDto)
    {
        // Get the algorithm by key
        var pathfinder = _serviceProvider.GetKeyedService<IPathfindingAlgorithm>(algorithm);
        
        if (pathfinder == null)
            return BadRequest($"Unknown algorithm: {algorithm}");
        
        // 1. Map DTO → Domain
        var domainRequest = PathfindingMapper.ToDomain(requestDto);
        
        // 2. Execute domain logic
        var domainResult = pathfinder.FindPath(domainRequest);
        
        // 3. Map Domain → DTO
        var responseDto = PathfindingMapper.ToDto(domainResult);
        
        return Ok(responseDto);
    }
}
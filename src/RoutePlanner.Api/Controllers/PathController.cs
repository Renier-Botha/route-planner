using Microsoft.AspNetCore.Mvc;
using RoutePlanner.Api.Models;
using RoutePlanner.Api.Services;

namespace RoutePlanner.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PathController : ControllerBase
{
    private readonly IPathfindingService _pathfinding;

    public PathController(IPathfindingService pathfinding) => _pathfinding = pathfinding;

    /// <summary>Computes the path using grid parameters.</summary>
    /// <param name="req">Grid size, start/end points, and walls.</param>
    /// <returns>Path from start to end; empty if unreachable.</returns>
    [HttpPost("path")]
    [ProducesResponseType(typeof(PathResponse), StatusCodes.Status200OK)]
    public ActionResult<PathResponse> Post([FromBody] GridRequest req)
    {
        var path = _pathfinding.FindPath(req);
        return Ok(new PathResponse(path));
    }
}
namespace RoutePlanner.Api.Models.Requests;

using RoutePlanner.Api.Models;

/// <summary>API contract for pathfinding requests.</summary>
public record PathfindingRequestDto
{
    // Option 1: Use walls (legacy)
    public int? Width { get; init; }
    public int? Height { get; init; }
    public List<PointDto>? Walls { get; init; }
    
    // Option 2: Use terrain
    public int[][]? Terrain { get; init; }
    
    // Required fields
    public required PointDto Start { get; init; }
    public required PointDto End { get; init; }
}
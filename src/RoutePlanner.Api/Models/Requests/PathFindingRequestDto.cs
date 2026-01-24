namespace RoutePlanner.Api.Models.Requests;

using RoutePlanner.Api.Models;

/// <summary>API contract for pathfinding requests.</summary>
public record PathfindingRequestDto(
    int Width,
    int Height,
    PointDto Start,
    PointDto End,
    List<PointDto> Walls
);
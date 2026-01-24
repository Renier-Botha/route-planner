namespace RoutePlanner.Api.Models.Responses;

using RoutePlanner.Api.Models;

public record PathfindingResponseDto(
    List<PointDto> Path,
    int NodesExplored,
    double DurationMs,
    string AlgorithmUsed
);
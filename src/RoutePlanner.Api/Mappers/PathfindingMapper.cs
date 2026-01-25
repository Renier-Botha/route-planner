namespace RoutePlanner.Api.Mappers;

using RoutePlanner.Api.Models;
using RoutePlanner.Api.Models.Requests;
using RoutePlanner.Api.Models.Responses;
using RoutePlanner.Core.Entities;
using RoutePlanner.Core.Models;
using RoutePlanner.Core.ValueObjects;

/// <summary>Maps between API DTOs and domain models.</summary>
public static class PathfindingMapper
{
    public static PathRequest ToDomain(PathfindingRequestDto dto)
    {
        // Convert DTO points to domain points
        var start = new Point(dto.Start.X, dto.Start.Y);
        var end = new Point(dto.End.X, dto.End.Y);
        var walls = dto.Walls.Select(w => new Point(w.X, w.Y)).ToList();
        
        // Create domain grid
        var grid = new Grid(dto.Width, dto.Height, walls);
        
        return new PathRequest(grid, start, end);
    }
    
    public static PathfindingResponseDto ToDto(PathResult result)
    {
        var pathDto = result.Path
            .Select(p => new PointDto(p.X, p.Y))
            .ToList();

        var visitedPointsDto = result.VisitedPoints
            .Select(p => new PointDto(p.X, p.Y))
            .ToHashSet();
            
        return new PathfindingResponseDto(
            pathDto,
            result.NodesExplored,
            result.Duration.TotalMilliseconds,
            result.AlgorithmName,
            visitedPointsDto
        );
    }
}
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
        var start = new Point(dto.Start.X, dto.Start.Y);
        var end = new Point(dto.End.X, dto.End.Y);
        
        Grid grid;
        
        // Check if terrain data is provided
        if (dto.Terrain != null && dto.Terrain.Length > 0)
        {
            // Convert jagged array to 2D array
            int width = dto.Terrain.Length;
            int height = dto.Terrain[0].Length;
            var terrain = new int[width, height];
            
            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                    terrain[x, y] = dto.Terrain[x][y];
            
            grid = new Grid(terrain);
        }
        else if (dto.Width.HasValue && dto.Height.HasValue && dto.Walls != null)
        {
            // Legacy: use walls
            var walls = dto.Walls.Select(w => new Point(w.X, w.Y)).ToList();
            grid = new Grid(dto.Width.Value, dto.Height.Value, walls);
        }
        else
        {
            throw new ArgumentException("Must provide either Terrain or (Width, Height, Walls)");
        }
        
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
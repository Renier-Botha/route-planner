namespace RoutePlanner.Core.Entities;

using RoutePlanner.Core.ValueObjects;

/// <summary>Represents a 2D grid with obstacles.</summary>
public class Grid
{
    private readonly bool[,] _blocked;
    
    public int Width { get; }
    public int Height { get; }
    
    public Grid(int width, int height, IEnumerable<Point> walls)
    {
        if (width <= 0 || height <= 0)
            throw new ArgumentException("Grid dimensions must be positive");
            
        Width = width;
        Height = height;
        _blocked = new bool[width, height];
        
        foreach (var wall in walls)
        {
            if (IsInBounds(wall))
                _blocked[wall.X, wall.Y] = true;
        }
    }
    
    public bool IsBlocked(Point point) => 
        IsInBounds(point) && _blocked[point.X, point.Y];
    
    public bool IsInBounds(Point point) =>
        point.X >= 0 && point.X < Width && 
        point.Y >= 0 && point.Y < Height;
}
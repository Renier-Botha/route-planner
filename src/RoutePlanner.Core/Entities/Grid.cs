namespace RoutePlanner.Core.Entities;

using RoutePlanner.Core.ValueObjects;

/// <summary>
/// Represents a 2D grid with terrain costs.
/// Cost of 0 means blocked/impassable.
/// Cost >= 1 means traversable with that movement cost.
/// </summary>
public class Grid
{
    private readonly int[,] _terrain;
    
    public int Width { get; }
    public int Height { get; }
    
    /// <summary>
    /// Create a grid with terrain costs.
    /// </summary>
    /// <param name="terrain">2D array where terrain[x,y] = movement cost (0 = blocked)</param>
    public Grid(int[,] terrain)
    {
        if (terrain.GetLength(0) <= 0 || terrain.GetLength(1) <= 0)
            throw new ArgumentException("Grid dimensions must be positive");
            
        Width = terrain.GetLength(0);
        Height = terrain.GetLength(1);
        _terrain = terrain;
    }
    
    /// <summary>
    /// Legacy constructor for backward compatibility with boolean walls.
    /// </summary>
    public Grid(int width, int height, IEnumerable<Point> walls)
    {
        if (width <= 0 || height <= 0)
            throw new ArgumentException("Grid dimensions must be positive");
            
        Width = width;
        Height = height;
        _terrain = new int[width, height];
        
        // Initialize all as passable (cost = 1)
        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                _terrain[x, y] = 1;
        
        // Mark walls as blocked (cost = 0)
        foreach (var wall in walls)
        {
            if (IsInBounds(wall))
                _terrain[wall.X, wall.Y] = 0;
        }
    }
    
    /// <summary>Get the movement cost at a point (0 = blocked).</summary>
    public int GetCost(Point point) =>
        IsInBounds(point) ? _terrain[point.X, point.Y] : 0;
    
    /// <summary>Check if a point is blocked (cost = 0).</summary>
    public bool IsBlocked(Point point) => 
        !IsInBounds(point) || _terrain[point.X, point.Y] == 0;
    
    public bool IsInBounds(Point point) =>
        point.X >= 0 && point.X < Width && 
        point.Y >= 0 && point.Y < Height;
}
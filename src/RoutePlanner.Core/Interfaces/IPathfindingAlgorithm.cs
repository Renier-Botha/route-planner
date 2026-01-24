namespace RoutePlanner.Core.Interfaces;

using RoutePlanner.Core.Models;

/// <summary>Strategy interface for pathfinding algorithms.</summary>
public interface IPathfindingAlgorithm
{
    /// <summary>Name of the algorithm.</summary>
    string Name { get; }
    
    /// <summary>Computes a shortest path on a grid.</summary>
    /// <param name="request">Grid, start, and end points.</param>
    /// <returns>Result containing path and metadata.</returns>
    PathResult FindPath(PathRequest request);
}
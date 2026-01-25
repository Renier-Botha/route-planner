namespace RoutePlanner.Core.Models;

using RoutePlanner.Core.ValueObjects;

/// <summary>Result of a pathfinding operation.</summary>
public record PathResult(
    List<Point> Path,
    int NodesExplored,
    TimeSpan Duration,
    string AlgorithmName,
    HashSet<Point> VisitedPoints
);
namespace RoutePlanner.Infrastructure.Algorithms;

using System.Diagnostics;
using RoutePlanner.Core.Interfaces;
using RoutePlanner.Core.Models;
using RoutePlanner.Core.ValueObjects;

// TODO: Implement Dijkstra's algorithm
// https://en.wikipedia.org/wiki/Dijkstra%27s_algorithm
public class DijkstrasAlgorithm : IPathfindingAlgorithm
{
    public string Name => "Dijkstra's Algorithm";

    public PathResult FindPath(PathRequest request)
    {
        var stopwatch = Stopwatch.StartNew();
        var grid = request.Grid;
        var start = request.Start;
        var end = request.End;
        
        if (!grid.IsInBounds(start) || !grid.IsInBounds(end))
            return EmptyResult(stopwatch);
        
        if (grid.IsBlocked(start) || grid.IsBlocked(end))
            return EmptyResult(stopwatch);

        var directions = new[] { (1, 0), (-1, 0), (0, 1), (0, -1) };
        var open = new PriorityQueue<Point, int>();
        var cameFrom = new Dictionary<Point, Point>();
        var gScore = new Dictionary<Point, int> { [start] = 0 };
        var visited = new HashSet<Point>();
        
        open.Enqueue(start, Heuristic(start, end));
        var nodesExplored = 0;

        while (open.TryDequeue(out var current, out _))
        {
            nodesExplored++;
            
            if (current.Equals(end))
            {
                var path = ReconstructPath(cameFrom, current);
                stopwatch.Stop();
                return new PathResult(path, nodesExplored, stopwatch.Elapsed, Name);
            }

            if (!visited.Add(current)) continue;

            foreach (var (dx, dy) in directions)
            {
                var neighbor = new Point(current.X + dx, current.Y + dy);
                
                if (!grid.IsInBounds(neighbor) || grid.IsBlocked(neighbor))
                    continue;

                var tentativeG = gScore[current] + 1;

                if (!gScore.TryGetValue(neighbor, out var g) || tentativeG < g)
                {
                    cameFrom[neighbor] = current;
                    gScore[neighbor] = tentativeG;
                    var f = tentativeG + Heuristic(neighbor, end);
                    open.Enqueue(neighbor, f);
                }
            }
        }

        stopwatch.Stop();
        return new PathResult([], nodesExplored, stopwatch.Elapsed, Name);
    }

    private static int Heuristic(Point a, Point b) => 
        Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y);

    private static List<Point> ReconstructPath(Dictionary<Point, Point> cameFrom, Point current)
    {
        var path = new List<Point> { current };
        while (cameFrom.TryGetValue(current, out var prev))
        {
            current = prev;
            path.Add(current);
        }
        path.Reverse();
        return path;
    }
    
    private PathResult EmptyResult(Stopwatch stopwatch)
    {
        stopwatch.Stop();
        return new PathResult([], 0, stopwatch.Elapsed, Name);
    }
}
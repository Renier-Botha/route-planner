using RoutePlanner.Api.Models;

namespace RoutePlanner.Api.Services;

public interface IPathfindingService
{
    /// <summary>Computes a shortest path on a grid with 4-way movement.</summary>
    /// <param name="req">Grid dimensions, start/end, and wall cells.</param>
    /// <returns>List of points from start to end, or empty if no path.</returns>
    List<PointDto> FindPath(GridRequest req);
}

public class PathfindingService : IPathfindingService
{
    public List<PointDto> FindPath(GridRequest req)
    {
        var w = req.Width;
        var h = req.Height;
        var blocked = new bool[w, h];
        foreach (var p in req.Walls)
        {
            if (p.X >= 0 && p.X < w && p.Y >= 0 && p.Y < h)
                blocked[p.X, p.Y] = true;
        }

        if (!InBounds(req.Start, w, h) || !InBounds(req.End, w, h)) return [];
        if (blocked[req.Start.X, req.Start.Y] || blocked[req.End.X, req.End.Y]) return [];

        var dirs = new (int dx, int dy)[] { (1, 0), (-1, 0), (0, 1), (0, -1) };
        var open = new PriorityQueue<PointDto, int>();
        var cameFrom = new Dictionary<PointDto, PointDto>();
        var gScore = new Dictionary<PointDto, int> { [req.Start] = 0 };

        open.Enqueue(req.Start, Heuristic(req.Start, req.End));
        var visited = new HashSet<PointDto>();

        while (open.TryDequeue(out var current, out _))
        {
            if (current.Equals(req.End))
                return ReconstructPath(cameFrom, current);

            if (!visited.Add(current)) continue;

            foreach (var (dx, dy) in dirs)
            {
                var nx = current.X + dx;
                var ny = current.Y + dy;
                if (nx < 0 || nx >= w || ny < 0 || ny >= h) continue;
                if (blocked[nx, ny]) continue;

                var neighbor = new PointDto(nx, ny);
                var tentativeG = gScore[current] + 1;

                if (!gScore.TryGetValue(neighbor, out var g) || tentativeG < g)
                {
                    cameFrom[neighbor] = current;
                    gScore[neighbor] = tentativeG;
                    var f = tentativeG + Heuristic(neighbor, req.End);
                    open.Enqueue(neighbor, f);
                }
            }
        }

        return [];
    }

    static int Heuristic(PointDto a, PointDto b) => Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y);
    static bool InBounds(PointDto p, int w, int h) => p.X >= 0 && p.X < w && p.Y >= 0 && p.Y < h;

    static List<PointDto> ReconstructPath(Dictionary<PointDto, PointDto> cameFrom, PointDto current)
    {
        var path = new List<PointDto> { current };
        while (cameFrom.TryGetValue(current, out var prev))
        {
            current = prev;
            path.Add(current);
        }
        path.Reverse();
        return path;
    }
}
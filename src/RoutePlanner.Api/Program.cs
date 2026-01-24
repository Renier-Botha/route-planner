
namespace RoutePlanner.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddAuthorization();

        builder.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                policy.WithOrigins("http://localhost:4200")
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
        });

        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.UseHttpsRedirection();
        app.UseCors();

        app.UseAuthorization();

        var summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        app.MapPost("/api/path", (GridRequest req) =>
        {
            var path = Pathfinding.FindPath(req);
            return Results.Ok(new PathResponse(path));
        })
        .WithName("ComputePath");

        app.Run();
    }
}

record PointDTO(int X, int Y);
record GridRequest(int Width, int Height, PointDTO Start, PointDTO End, List<PointDTO> Walls);
record PathResponse(List<PointDTO> Path);

static class Pathfinding
{
    public static List<PointDTO> FindPath(GridRequest req)
    {
        var w = req.Width;
        var h = req.Height;
        var blocked = new bool[w, h];
        foreach (var p in req.Walls)
        {
            if (p.X >= 0 && p.X < w && p.Y >= 0 && p.Y < h)
                blocked[p.X, p.Y] = true;
        }

        // Reject invalid start/end
        if (!InBounds(req.Start, w, h) || !InBounds(req.End, w, h)) return [];
        if (blocked[req.Start.X, req.Start.Y] || blocked[req.End.X, req.End.Y]) return [];

        var dirs = new (int dx, int dy)[] { (1, 0), (-1, 0), (0, 1), (0, -1) };
        var open = new PriorityQueue<PointDTO, int>();
        var cameFrom = new Dictionary<PointDTO, PointDTO>();
        var gScore = new Dictionary<PointDTO, int> { [req.Start] = 0 };

        open.Enqueue(req.Start, Heuristic(req.Start, req.End));

        var visited = new HashSet<PointDTO>();

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

                var neighbor = new PointDTO(nx, ny);
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

        return []; // no path
    }

    static int Heuristic(PointDTO a, PointDTO b) => Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y);
    static bool InBounds(PointDTO p, int w, int h) => p.X >= 0 && p.X < w && p.Y >= 0 && p.Y < h;

    static List<PointDTO> ReconstructPath(Dictionary<PointDTO, PointDTO> cameFrom, PointDTO current)
    {
        var path = new List<PointDTO> { current };
        while (cameFrom.TryGetValue(current, out var prev))
        {
            current = prev;
            path.Add(current);
        }
        path.Reverse();
        return path;
    }
}
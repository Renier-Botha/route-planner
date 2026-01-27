using System.Reflection;
using RoutePlanner.Core.Interfaces;
using RoutePlanner.Infrastructure.Algorithms;

namespace RoutePlanner.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Services
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
        builder.Services.AddControllers();

        // Swashbuckle: generator + UI
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new()
            {
                Title = "Route Planner API",
                Version = "v1",
                Description = "Grid pathfinding endpoints with multiple algorithms"
            });

            // XML doc comments
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            if (File.Exists(xmlPath))
                options.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
        });

        // DI for pathfinding algorithms
        // Default algorithm (can be changed via DI in future)
        // builder.Services.AddScoped<IPathfindingAlgorithm, AStarAlgorithm>();
        
        // Alternative: Register multiple algorithms with keys
        builder.Services.AddKeyedScoped<IPathfindingAlgorithm, AStarAlgorithm>("astar");
        builder.Services.AddKeyedScoped<IPathfindingAlgorithm, DijkstrasAlgorithm>("dijkstra");
        builder.Services.AddKeyedScoped<IPathfindingAlgorithm, GreedyAlgorithm>("greedy");

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Route Planner API v1");
                c.RoutePrefix = "swagger";
            });
        }

        app.UseHttpsRedirection();
        app.UseCors();
        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}
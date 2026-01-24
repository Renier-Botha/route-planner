using Microsoft.OpenApi;
using System.Reflection;

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
        builder.Services.AddEndpointsApiExplorer(); // required for Minimal APIs; fine with controllers too
        builder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Route Planner API",
                Version = "v1",
                Description = "Grid pathfinding endpoints"
            });

            // XML doc comments (already enabled in csproj)
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            if (File.Exists(xmlPath))
                options.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);

            // Optional: annotations
            // options.EnableAnnotations();
        });

        // DI for pathfinding
        builder.Services.AddScoped<Services.IPathfindingService, Services.PathfindingService>();

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();    // serves /swagger/v1/swagger.json
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Route Planner API v1");
                c.RoutePrefix = "swagger"; // UI at /swagger
            });
        }

        app.UseHttpsRedirection();
        app.UseCors();
        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}
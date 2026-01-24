namespace RoutePlanner.Core.Models;

using RoutePlanner.Core.Entities;
using RoutePlanner.Core.ValueObjects;

/// <summary>Request for pathfinding operation.</summary>
public record PathRequest(
    Grid Grid,
    Point Start,
    Point End
);
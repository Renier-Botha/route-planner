namespace RoutePlanner.Api.Models;

public record PointDto(int X, int Y);
public record GridRequest(int Width, int Height, PointDto Start, PointDto End, List<PointDto> Walls);
public record PathResponse(List<PointDto> Path);
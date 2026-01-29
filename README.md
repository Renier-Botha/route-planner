# Route Planner

A pathfinding visualization and traffic simulation system built with C# .NET and Angular.

## Features

- **Pathfinding Algorithms**: A*, Dijkstra, and Greedy Best-First Search
- **Grid-based Terrain**: Support for weighted terrain with movement costs
- **REST API**: Clean architecture with ASP.NET Core
- **Interactive Visualizer**: Angular-based grid visualization with real-time pathfinding
- **Performance Metrics**: Track algorithm execution time and nodes explored

## Tech Stack

**Backend:**
- .NET 9.0
- ASP.NET Core Web API

**Frontend:**
- Angular
- TypeScript

## Project Structure

```
src/
├── RoutePlanner.Api/          # Web API controllers and endpoints
├── RoutePlanner.Application/  # Application services and use cases
├── RoutePlanner.Core/         # Domain entities, interfaces, value objects
└── RoutePlanner.Infrastructure/ # Algorithm implementations

client/                        # Angular frontend application
tests/                         # Unit test projects
```

## Getting Started

### Prerequisites
- .NET 9.0 SDK
- Node.js and npm
- Angular CLI

### Running the Application

**Quick Start:**
```bash
./start-dev.sh
```

**Manual Start:**

Backend:
```bash
cd src/RoutePlanner.Api
dotnet run
```

Frontend:
```bash
cd client
npm install
npm start
```

The API will be available at `https://localhost:5001` and the client at `http://localhost:4200`.

## API Endpoints

- `POST /api/algorithms/pathfind/{algorithm}` - Find path using specified algorithm (astar, dijkstra, greedy)
- `GET /api/algorithms` - List available algorithms

## Roadmap

See [TODO.md](TODO.md) for planned features including:
- Perlin noise terrain generation
- Multi-agent traffic simulation
- Collision avoidance
- Dynamic routing

## License

See [LICENSE](LICENSE) for details.

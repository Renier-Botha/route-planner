# Route Planner - TODO List

## Current Features
- ✅ Core pathfinding algorithms (A*, Dijkstra, Greedy)
- ✅ Grid-based terrain with movement costs
- ✅ REST API for pathfinding operations
- ✅ Angular client with visualizer
- ✅ Performance metrics tracking

---

## Outstanding Features & Improvements

### 1. Grid Weight Generation & Visualization
**Goal:** Enhance the visualizer with dynamic terrain generation capabilities

- [ ] **Perlin Noise Implementation**
  - [ ] Create terrain generator service in C# (Infrastructure layer)
  - [ ] Add API endpoint: `POST /api/grid/generate` with parameters:
    - `width`, `height`
    - `seed` (for reproducibility)
    - `scale`, `octaves`, `persistence`, `lacunarity`
    - `minWeight`, `maxWeight` (cost range)
  - [ ] Consider alternative noise algorithms:
    - Simplex noise (improved performance)
    - Worley/Cellular noise (for more structured patterns)
    - Diamond-Square algorithm (classic terrain generation)

- [ ] **Client-Side Terrain Editor**
  - [ ] Add UI controls for noise parameters
  - [ ] Real-time preview of generated terrain
  - [ ] Manual cell weight editing (click/drag to paint weights)
  - [ ] Save/load terrain presets
  - [ ] Color gradient visualization based on weights

- [ ] **Terrain Presets**
  - [ ] Mountains (high cost peaks)
  - [ ] Rivers/Water (impassable or high cost)
  - [ ] Roads (low cost paths)
  - [ ] Urban areas (mixed costs)

### 2. Traffic Simulation System
**Goal:** Create a multi-agent simulation that uses pathfinding for realistic traffic behavior

#### 2.1 Foundation
- [ ] **Agent/Vehicle Entity** (`src/RoutePlanner.Core/Entities/Vehicle.cs`)
  ```
  Properties:
  - Id, Position, Destination
  - Speed (cells/second), MaxSpeed
  - CurrentPath (from pathfinding)
  - State (Idle, Moving, Waiting, Arrived)
  ```

- [ ] **Simulation Manager** (`src/RoutePlanner.Application/Services/SimulationService.cs`)
  - [ ] Manage collection of vehicles
  - [ ] Time-step based updates (delta time)
  - [ ] Spawn/despawn vehicles
  - [ ] Global simulation state (running, paused, reset)

#### 2.2 Core Simulation Features
- [ ] **Multi-Destination Routing**
  - [ ] Define waypoints/points of interest on grid
  - [ ] Random destination selection for vehicles
  - [ ] Sequential waypoint navigation (delivery routes)
  - [ ] Re-routing when destination changes

- [ ] **Speed & Movement**
  - [ ] Velocity-based movement (not instant cell jumps)
  - [ ] Speed influenced by terrain cost
  - [ ] Acceleration/deceleration for realistic behavior
  - [ ] Turn speed limitations (can't instantly reverse)

- [ ] **Dynamic Pathfinding**
  - [ ] Path recalculation triggers:
    - New obstacles detected
    - Traffic congestion ahead
    - Better route becomes available
  - [ ] Caching paths for performance
  - [ ] Path smoothing for natural movement

- [ ] **Collision Avoidance**
  - [ ] Cell occupancy tracking (which vehicle is where)
  - [ ] Predictive collision detection (next N steps)
  - [ ] Avoidance strategies:
    - Slow down/stop before collision
    - Route around blocked cells
    - Wait for path to clear
  - [ ] Personal space/following distance
  - [ ] Deadlock detection and resolution

#### 2.3 Advanced Traffic Features
- [ ] **Traffic Density Analysis**
  - [ ] Heatmap of cell usage frequency
  - [ ] Congestion detection (bottlenecks)
  - [ ] Dynamic cost adjustment based on traffic

- [ ] **Vehicle Priorities**
  - [ ] Emergency vehicles (higher priority routing)
  - [ ] Different vehicle types (car, truck, bike)
  - [ ] Lane preferences

- [ ] **Traffic Lights/Signals**
  - [ ] Timed signals at intersections
  - [ ] Vehicle queuing behavior
  - [ ] Green wave optimization

#### 2.4 API Endpoints
- [ ] `POST /api/simulation/create` - Initialize new simulation
- [ ] `POST /api/simulation/{id}/start` - Start/resume simulation
- [ ] `POST /api/simulation/{id}/pause` - Pause simulation
- [ ] `POST /api/simulation/{id}/step` - Single time-step update
- [ ] `GET /api/simulation/{id}/state` - Current state of all vehicles
- [ ] `POST /api/simulation/{id}/vehicles` - Spawn new vehicle
- [ ] `DELETE /api/simulation/{id}/vehicles/{vehicleId}` - Remove vehicle
- [ ] `PUT /api/simulation/{id}/config` - Update simulation parameters

#### 2.5 Client Visualization
- [ ] Real-time vehicle rendering (animated sprites/dots)
- [ ] Path visualization per vehicle (optional toggle)
- [ ] Speed indicators (color-coded by velocity)
- [ ] Collision warning indicators
- [ ] Simulation controls (play, pause, speed up/slow down)
- [ ] Statistics panel:
  - Active vehicles count
  - Average travel time
  - Collision count
  - Congestion level

---

## Nice-to-Have Enhancements
- [ ] WebSocket support for real-time simulation updates (instead of polling)
- [ ] Save/replay simulation recordings
- [ ] Performance benchmarks (vehicles/second)
- [ ] 3D visualization option
- [ ] Machine learning for traffic flow optimization
- [ ] Parking simulation (destination full, find alternative)
- [ ] Weather effects on movement costs

---

## Architecture Notes

### Suggested Project Structure
```
RoutePlanner.Core/
  Entities/
    Vehicle.cs         (new)
    Simulation.cs      (new)
  Interfaces/
    ITerrainGenerator.cs     (new)
    ISimulationService.cs    (new)
    ICollisionDetector.cs    (new)

RoutePlanner.Application/
  Services/
    SimulationService.cs     (new)
    CollisionDetectionService.cs (new)

RoutePlanner.Infrastructure/
  Terrain/
    PerlinNoiseGenerator.cs  (new)
    SimplexNoiseGenerator.cs (new)
  Simulation/
    BasicCollisionDetector.cs (new)
```

### Key Design Considerations
1. **Performance**: With many vehicles, consider spatial partitioning (quadtree/grid cells)
2. **Threading**: Simulation updates could be async/parallel per vehicle
3. **State Management**: Immutable simulation snapshots for replay/debugging
4. **Event System**: Publish events for collisions, arrivals, congestion

---

## Resources & Documentation
- [Perlin Noise Tutorial](https://adrianb.io/2014/08/09/perlinnoise.html)
- [Traffic Flow Simulation Basics](https://en.wikipedia.org/wiki/Traffic_flow)
- [Collision Avoidance Algorithms](https://en.wikipedia.org/wiki/Collision_avoidance)
- [Reciprocal Velocity Obstacles](https://gamma.cs.unc.edu/RVO/) - Advanced collision avoidance

---

**Last Updated:** January 29, 2026

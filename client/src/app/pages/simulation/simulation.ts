import { Component, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { PathfindingService } from '../../services/pathfinding.service';

@Component({
  selector: 'app-simulation',
  imports: [
    CommonModule,
    FormsModule,
    RouterLink
  ],
  templateUrl: './simulation.html',
  styleUrl: './simulation.css'
})
export class Simulation {
  protected readonly title = signal('Traffic Simulation');
  private readonly pathfindingService = inject(PathfindingService);

  algorithms: { key: string; name: string }[] = [];
  selectedAlgorithm = 'astar';
  isRunning = false;
  
  // Simulation parameters
  agentCount = 10;
  simulationSpeed = 1;
  trafficDensity = 'medium';
  
  ngOnInit(): void {
    this.pathfindingService.getAlgorithms().subscribe(algorithms => {
      this.algorithms = algorithms;
    });
  }

  startSimulation(): void {
    this.isRunning = true;
    // TODO: Implement simulation logic
    console.log('Starting simulation with:', {
      algorithm: this.selectedAlgorithm,
      agents: this.agentCount,
      speed: this.simulationSpeed,
      density: this.trafficDensity
    });
  }

  pauseSimulation(): void {
    this.isRunning = false;
    // TODO: Implement pause logic
  }

  resetSimulation(): void {
    this.isRunning = false;
    // TODO: Implement reset logic
  }
}
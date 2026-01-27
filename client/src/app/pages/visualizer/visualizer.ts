import { Component, ViewChild, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { GridComponent } from '../../components/grid/grid.component';
import { CommonModule } from '@angular/common';
import { PathfindingService } from '../../services/pathfinding.service';

type Mode = 'wall' | 'erase' | 'start' | 'end';

@Component({
  selector: 'app-visualizer',
  imports: [
    CommonModule,
    GridComponent,
    FormsModule
  ],
  templateUrl: './visualizer.html',
  styleUrl: './visualizer.css'
})
export class Visualizer {
  protected readonly title = signal('Route Planner');
  private readonly pathfindingService = inject(PathfindingService);

  @ViewChild(GridComponent) grid!: GridComponent;

  currentMode: Mode = 'wall';
  selectedAlgorithm = 'astar';
  algorithms: { key: string; name: string }[] = [];
  lastSolveStats = signal<{ nodes: number; timeMs: number; algorithm: string } | null>(null);
  isCollapsed = false;
  gridCols = 60;
  gridRows = 40;

  readonly modes: readonly { value: Mode; label: string }[] = [
    { value: 'wall', label: 'Walls' },
    { value: 'erase', label: 'Erase' },
    { value: 'start', label: 'Start' },
    { value: 'end', label: 'End' }
  ];

  ngOnInit(): void {
    this.pathfindingService.getAlgorithms().subscribe(algorithms => {
      this.algorithms = algorithms;
    });
  }

  onModeChange(mode: Mode): void {
    this.currentMode = mode;
    this.grid?.setMode(mode);
  }

  onGridSizeChange(): void {
    // Clear the grid when dimensions change to avoid state issues
    this.grid?.clear();
    this.lastSolveStats.set(null);
  }

  onClear(): void {
    this.grid?.clear();
    this.lastSolveStats.set(null);
  }

  onSolve(): void {
    this.grid?.solve();
  }

  onStatsChanged(stats: { nodes: number; timeMs: number; algorithm: string } | null): void {
    this.lastSolveStats.set(stats);
  }
}
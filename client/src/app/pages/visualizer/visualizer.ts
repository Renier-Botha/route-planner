import { Component, ViewChild, signal } from '@angular/core';
import { GridComponent } from '../../components/grid/grid.component';

type Mode = 'wall' | 'erase' | 'start' | 'end';

@Component({
  selector: 'app-visualizer',
  imports: [GridComponent],
  templateUrl: './visualizer.html',
  styleUrl: './visualizer.css'
})
export class Visualizer {
  protected readonly title = signal('Route Planner');
  
  @ViewChild(GridComponent) grid!: GridComponent;
  
  currentMode: Mode = 'wall';
  lastSolveStats = signal<{ nodes: number; timeMs: number; algorithm: string } | null>(null);

  setMode(mode: Mode): void {
    this.currentMode = mode;
    this.grid?.setMode(mode);
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
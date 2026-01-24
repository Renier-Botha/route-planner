import { Component, ViewChild, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { GridComponent } from './components/grid/grid.component';

type Mode = 'wall' | 'erase' | 'start' | 'end';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, GridComponent],
  templateUrl: './app.html',
  styleUrl: './app.css',
  standalone: true
})
export class App {
  protected readonly title = signal('route-planner-frontend');
  
  @ViewChild(GridComponent) grid!: GridComponent;
  
  currentMode: Mode = 'wall';
  lastSolveStats = signal<{ nodes: number; timeMs: number; algorithm: string } | null>(null);

  setMode(mode: Mode): void {
    this.currentMode = mode;
    this.grid?.setMode(mode);
  }

  onClear(): void {
    this.grid?.clear();
  }

  onSolve(): void {
    this.grid?.solve();
  }
  
  onStatsChanged(stats: { nodes: number; timeMs: number; algorithm: string } | null): void {
    this.lastSolveStats.set(stats);
  }
}
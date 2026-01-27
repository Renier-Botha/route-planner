import { Component, ViewChild, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { GridComponent } from '../../components/grid/grid.component';
import { NzSpaceModule } from 'ng-zorro-antd/space';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzRadioModule } from 'ng-zorro-antd/radio';
import { NzSelectModule } from 'ng-zorro-antd/select';
import { CommonModule } from '@angular/common';
import { NzFlexModule } from 'ng-zorro-antd/flex';

type Mode = 'wall' | 'erase' | 'start' | 'end';

@Component({
  selector: 'app-visualizer',
  imports: [CommonModule, GridComponent, NzSpaceModule, NzButtonModule, NzRadioModule, NzSelectModule, FormsModule, NzFlexModule],
  templateUrl: './visualizer.html',
  styleUrl: './visualizer.css'
})
export class Visualizer {
  protected readonly title = signal('Route Planner');

  @ViewChild(GridComponent) grid!: GridComponent;

  currentMode: Mode = 'wall';
  selectedAlgorithm = 'astar';
  algorithms = ['astar', 'dijkstra', 'BFS', 'DFS'];
  lastSolveStats = signal<{ nodes: number; timeMs: number; algorithm: string } | null>(null);

  onModeChange(mode: Mode): void {
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
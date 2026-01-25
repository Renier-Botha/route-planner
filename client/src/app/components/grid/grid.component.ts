import {
  Component,
  ViewChild,
  ElementRef,
  AfterViewInit,
  HostListener,
  inject,
  PLATFORM_ID,
  input,
  output
} from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
import { PathfindingService, Point } from '../../services/pathfinding.service';

type Mode = 'wall' | 'erase' | 'start' | 'end';

interface SolveStats {
  nodes: number;
  timeMs: number;
  algorithm: string;
}

@Component({
  selector: 'app-grid',
  templateUrl: './grid.component.html',
  styleUrl: './grid.component.css',
  standalone: true
})
export class GridComponent implements AfterViewInit {
  private readonly isBrowser = isPlatformBrowser(inject(PLATFORM_ID));
  private readonly pathfindingService = inject(PathfindingService);

  // Inputs (configurable from parent)
  cols = input<number>(60);
  rows = input<number>(40);

  // Outputs (emit events to parent)
  statsChanged = output<SolveStats | null>();

  @ViewChild('canvas', { static: true }) private canvasRef!: ElementRef<HTMLCanvasElement>;
  private ctx!: CanvasRenderingContext2D;
  private dpr = 1;
  private drawing = false;

  private cellW = 0;
  private cellH = 0;

  // State
  mode: Mode = 'wall';
  blocked: boolean[][] = [];
  start: Point | null = null;
  end: Point | null = null;
  path: Point[] = [];
  visited: Point[] = [];

  ngAfterViewInit(): void {
    if (!this.isBrowser) return;
    this.initializeGrid();
    this.resizeCanvas();
    this.render();
  }

  private initializeGrid(): void {
    this.blocked = Array.from(
      { length: this.cols() },
      () => Array(this.rows()).fill(false)
    );
  }

  @HostListener('window:resize')
  onWindowResize(): void {
    if (!this.isBrowser) return;
    this.resizeCanvas();
    this.render();
  }

  setMode(m: Mode): void {
    this.mode = m;
  }

  private resizeCanvas(): void {
    const canvas = this.canvasRef.nativeElement;
    this.dpr = (globalThis as any)?.devicePixelRatio || 1;

    const rect = canvas.getBoundingClientRect();
    canvas.width = Math.max(1, Math.floor(rect.width * this.dpr));
    canvas.height = Math.max(1, Math.floor(rect.height * this.dpr));

    this.ctx = canvas.getContext('2d')!;
    this.ctx.setTransform(1, 0, 0, 1, 0, 0);
    this.ctx.scale(this.dpr, this.dpr);

    this.cellW = rect.width / this.cols();
    this.cellH = rect.height / this.rows();

    this.ctx.lineWidth = 2;
    this.ctx.lineCap = 'round';
    this.ctx.lineJoin = 'round';
    this.ctx.strokeStyle = '#1f51ff';
  }

  private getCellFromEvent(ev: PointerEvent): Point {
    const rect = this.canvasRef.nativeElement.getBoundingClientRect();
    const x = Math.floor((ev.clientX - rect.left) / this.cellW);
    const y = Math.floor((ev.clientY - rect.top) / this.cellH);
    return {
      x: Math.min(this.cols() - 1, Math.max(0, x)),
      y: Math.min(this.rows() - 1, Math.max(0, y)),
    };
  }

  onPointerDown(ev: PointerEvent): void {
    if (!this.isBrowser) return;
    this.drawing = true;
    this.applyAtEvent(ev);
  }

  onPointerMove(ev: PointerEvent): void {
    if (!this.isBrowser || !this.drawing) return;
    this.applyAtEvent(ev);
  }

  onPointerUpCancel(): void {
    if (!this.isBrowser) return;
    this.drawing = false;
  }

  private applyAtEvent(ev: PointerEvent): void {
    const cell = this.getCellFromEvent(ev);
    if (this.mode === 'wall') {
      this.blocked[cell.x][cell.y] = true;
      this.path = [];
    } else if (this.mode === 'erase') {
      this.blocked[cell.x][cell.y] = false;
      this.path = [];
    } else if (this.mode === 'start') {
      this.start = cell;
      this.path = [];
    } else if (this.mode === 'end') {
      this.end = cell;
      this.path = [];
    }
    this.render();
  }

  clear(): void {
    for (let x = 0; x < this.cols(); x++) {
      this.blocked[x].fill(false);
    }
    this.start = null;
    this.end = null;
    this.path = [];
    this.statsChanged.emit(null);
    this.render();
  }

  solve(): void {
    if (!this.isBrowser || !this.start || !this.end) {
      console.warn('Cannot solve: missing start or end point');
      return;
    }

    const walls: Point[] = [];
    for (let x = 0; x < this.cols(); x++) {
      for (let y = 0; y < this.rows(); y++) {
        if (this.blocked[x][y]) walls.push({ x, y });
      }
    }

    const request = {
      width: this.cols(),
      height: this.rows(),
      start: this.start,
      end: this.end,
      walls,
    };

    this.pathfindingService.findPath(request).subscribe({
      next: (response) => {
        this.path = response.path ?? [];
        this.visited = response.visitedPoints ?? [];

        const stats: SolveStats = {
          nodes: response.nodesExplored,
          timeMs: response.durationMs,
          algorithm: response.algorithmUsed
        };

        this.statsChanged.emit(stats);

        console.log(`✅ Path found using ${response.algorithmUsed}`);
        console.log(`   Nodes explored: ${response.nodesExplored}`);
        console.log(`   Time: ${response.durationMs.toFixed(2)}ms`);

        this.render();
      },
      error: (err) => {
        console.error('❌ Pathfinding failed:', err);
        this.path = [];
        this.render();
      }
    });
  }

  private render(): void {
    const canvas = this.canvasRef.nativeElement;
    const ctx = this.ctx;

    // Clear
    ctx.save();
    ctx.setTransform(1, 0, 0, 1, 0, 0);
    ctx.clearRect(0, 0, canvas.width, canvas.height);
    ctx.restore();

    // Background
    ctx.fillStyle = '#fafafa';
    ctx.fillRect(0, 0, canvas.width / this.dpr, canvas.height / this.dpr);

    // Grid lines
    ctx.strokeStyle = '#e5e7eb';
    ctx.lineWidth = 1;
    for (let c = 1; c < this.cols(); c++) {
      const x = c * this.cellW;
      ctx.beginPath();
      ctx.moveTo(x, 0);
      ctx.lineTo(x, this.rows() * this.cellH);
      ctx.stroke();
    }
    for (let r = 1; r < this.rows(); r++) {
      const y = r * this.cellH;
      ctx.beginPath();
      ctx.moveTo(0, y);
      ctx.lineTo(this.cols() * this.cellW, y);
      ctx.stroke();
    }

    // Visited Points (draw FIRST, as background layer)
    if (this.visited.length > 0) {
      ctx.fillStyle = 'rgba(223, 210, 99, 0.3)'; // Semi-transparent gray
      for (let i = 0; i < this.visited.length; i++) {
        ctx.fillRect(
          this.visited[i].x * this.cellW,
          this.visited[i].y * this.cellH,
          this.cellW,
          this.cellH
        );
      }
    }

    // Walls
    ctx.fillStyle = '#374151';
    for (let x = 0; x < this.cols(); x++) {
      for (let y = 0; y < this.rows(); y++) {
        if (this.blocked[x][y]) {
          ctx.fillRect(x * this.cellW, y * this.cellH, this.cellW, this.cellH);
        }
      }
    }

    // Start/End
    if (this.start) {
      this.drawMarker(this.start, '#10b981');
    }
    if (this.end) {
      this.drawMarker(this.end, '#ef4444');
    }

    // Path (on top of visited and walls)
    if (this.path.length > 0) {
      ctx.strokeStyle = '#1f51ff';
      ctx.lineWidth = 3;
      ctx.beginPath();
      for (let i = 0; i < this.path.length; i++) {
        const p = this.cellCenter(this.path[i]);
        if (i === 0) ctx.moveTo(p.x, p.y);
        else ctx.lineTo(p.x, p.y);
      }
      ctx.stroke();
    }
  }

  private drawMarker(p: Point, color: string): void {
    const c = this.cellCenter(p);
    this.ctx.fillStyle = color;
    this.ctx.beginPath();
    this.ctx.arc(c.x, c.y, Math.min(this.cellW, this.cellH) * 0.35, 0, Math.PI * 2);
    this.ctx.fill();
  }

  private cellCenter(p: Point): Point {
    return {
      x: p.x * this.cellW + this.cellW / 2,
      y: p.y * this.cellH + this.cellH / 2,
    };
  }
}
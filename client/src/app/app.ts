import {
  Component,
  signal,
  ViewChild,
  ElementRef,
  AfterViewInit,
  HostListener,
  inject,
  PLATFORM_ID
} from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
import { RouterOutlet } from '@angular/router';

type Point = { x: number; y: number };
type Mode = 'wall' | 'erase' | 'start' | 'end';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App implements AfterViewInit {
  protected readonly title = signal('route-planner-frontend');
  private readonly isBrowser = isPlatformBrowser(inject(PLATFORM_ID));

  @ViewChild('canvas', { static: true }) private canvasRef!: ElementRef<HTMLCanvasElement>;
  private ctx!: CanvasRenderingContext2D;
  private dpr = 1;
  private drawing = false;

  // Grid config
  readonly cols = 60;
  readonly rows = 40;
  private cellW = 0;
  private cellH = 0;

  // State
  mode: Mode = 'wall';
  blocked: boolean[][] = Array.from({ length: this.cols }, () => Array(this.rows).fill(false));
  start: Point | null = null;
  end: Point | null = null;
  path: Point[] = [];

  ngAfterViewInit(): void {
    if (!this.isBrowser) return;
    this.resizeCanvas();
    this.render();
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

    // cell sizes in CSS pixels
    this.cellW = rect.width / this.cols;
    this.cellH = rect.height / this.rows;

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
      x: Math.min(this.cols - 1, Math.max(0, x)),
      y: Math.min(this.rows - 1, Math.max(0, y)),
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

  onClear(): void {
    for (let x = 0; x < this.cols; x++) {
      this.blocked[x].fill(false);
    }
    this.start = null;
    this.end = null;
    this.path = [];
    this.render();
  }

  async onSolve(): Promise<void> {
    if (!this.isBrowser || !this.start || !this.end) return;

    const walls: { x: number; y: number }[] = [];
    for (let x = 0; x < this.cols; x++) {
      for (let y = 0; y < this.rows; y++) {
        if (this.blocked[x][y]) walls.push({ x, y });
      }
    }

    const req = {
      width: this.cols,
      height: this.rows,
      start: { x: this.start.x, y: this.start.y },
      end: { x: this.end.x, y: this.end.y },
      walls,
    };

    try {
      const res = await fetch('http://localhost:5088/api/path', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(req),
      });
      const data: { path: { x: number; y: number }[] } = await res.json();
      this.path = data.path ?? [];
      this.render();
    } catch (e) {
      console.error('Path solve failed', e);
    }
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

    // Optional grid lines
    ctx.strokeStyle = '#e5e7eb';
    ctx.lineWidth = 1;
    for (let c = 1; c < this.cols; c++) {
      const x = c * this.cellW;
      ctx.beginPath(); ctx.moveTo(x, 0); ctx.lineTo(x, this.rows * this.cellH); ctx.stroke();
    }
    for (let r = 1; r < this.rows; r++) {
      const y = r * this.cellH;
      ctx.beginPath(); ctx.moveTo(0, y); ctx.lineTo(this.cols * this.cellW, y); ctx.stroke();
    }

    // Walls
    ctx.fillStyle = '#374151';
    for (let x = 0; x < this.cols; x++) {
      for (let y = 0; y < this.rows; y++) {
        if (this.blocked[x][y]) {
          ctx.fillRect(x * this.cellW, y * this.cellH, this.cellW, this.cellH);
        }
      }
    }

    // Start/End
    if (this.start) {
      this.drawMarker(this.start, '#10b981'); // green
    }
    if (this.end) {
      this.drawMarker(this.end, '#ef4444'); // red
    }

    // Path
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
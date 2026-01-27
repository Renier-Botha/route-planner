// src/app/services/pathfinding.service.ts
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface Point {
  x: number;
  y: number;
}

export interface PathfindingRequest {
  width: number;
  height: number;
  start: Point;
  end: Point;
  walls: Point[];
}

export interface PathfindingResponse {
  path: Point[];
  nodesExplored: number;
  durationMs: number;
  algorithmUsed: string;
  visitedPoints: Point[];
}

@Injectable({
  providedIn: 'root'
})
export class PathfindingService {
  private apiUrl = 'http://localhost:5088/api/algorithms';

  constructor(private http: HttpClient) { }

  findPath(request: PathfindingRequest, algorithm: string = 'astar'): Observable<PathfindingResponse> {
    return this.http.post<PathfindingResponse>(
      `${this.apiUrl}/pathfind/${algorithm}`,
      request
    );
  }
}
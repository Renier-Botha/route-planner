import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-home',
  imports: [CommonModule, RouterLink],
  templateUrl: './home.html',
  styleUrl: './home.css'
})
export class Home {
  features = [
    {
      title: 'Pathfinding Visualizer',
      description: 'Visualize and compare different pathfinding algorithms on a grid',
      route: '/visualizer',
      icon: '🎯'
    },
    {
      title: 'Traffic Simulation',
      description: 'Simulate traffic patterns with multiple agents using pathfinding algorithms',
      route: '/simulation',
      icon: '🚗'
    }
  ];
}
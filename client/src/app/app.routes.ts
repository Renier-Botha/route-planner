import { Routes } from '@angular/router';

export const routes: Routes = [
  { path: '', pathMatch: 'full', redirectTo: '/home' },
  { path: 'home', loadChildren: () => import('./pages/home/home.routes').then(m => m.HOME_ROUTES) },
  { path: 'visualizer', loadChildren: () => import('./pages/visualizer/visualizer.routes').then(m => m.VISUALIZER_ROUTES) },
  { path: 'simulation', loadChildren: () => import('./pages/simulation/simulation.routes').then(m => m.SIMULATION_ROUTES) }
];
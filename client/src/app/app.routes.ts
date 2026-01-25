import { Routes } from '@angular/router';

export const routes: Routes = [
  { path: '', pathMatch: 'full', redirectTo: '/visualizer' },
  { path: 'visualizer', loadChildren: () => import('./pages/visualizer/visualizer.routes').then(m => m.VISUALIZER_ROUTES) }
];

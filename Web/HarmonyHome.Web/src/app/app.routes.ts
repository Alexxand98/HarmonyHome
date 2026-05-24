import { Routes } from '@angular/router';

import { Login } from './pages/login/login';
import { Layout } from './pages/layout/layout';
import { Productos } from './pages/productos/productos';
import { authGuard } from './guards/auth-guard';

export const routes: Routes = [
  {
    path: 'login',
    component: Login
  },
  {
    path: '',
    component: Layout,
    canActivate: [authGuard],
    children: [
      {
        path: 'productos',
        component: Productos
      },
      {
        path: '',
        redirectTo: 'productos',
        pathMatch: 'full'
      }
    ]
  },
  {
    path: '**',
    redirectTo: 'login'
  }
];
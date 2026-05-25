import { Routes } from '@angular/router';

import { Login } from './pages/login/login';
import { Layout } from './pages/layout/layout';
import { Productos } from './pages/productos/productos';
import { Clientes } from './pages/clientes/clientes';
import { Venta } from './pages/venta/venta';
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
        path: 'clientes',
        component: Clientes
      },
      {
        path: 'venta',
        component: Venta
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
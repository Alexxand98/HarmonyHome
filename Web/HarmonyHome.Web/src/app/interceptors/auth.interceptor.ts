import { HttpInterceptorFn } from '@angular/common/http';
import { catchError, throwError } from 'rxjs';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const token = localStorage.getItem('hh_token');
  const esLogin = req.url.includes('/Auth/login');

  const authRequest =
    token && !esLogin
      ? req.clone({
          setHeaders: {
            Authorization: `Bearer ${token}`
          }
        })
      : req;

  return next(authRequest).pipe(
    catchError(error => {
      if (error.status === 401 && !esLogin && window.location.pathname !== '/login') {
        localStorage.removeItem('hh_token');
        localStorage.removeItem('hh_user');
        window.location.href = '/login';
      }

      return throwError(() => error);
    })
  );
};
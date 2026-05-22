import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { AuthService } from './auth.service';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  // 1. Récupérer le service d'auth
  const auth = inject(AuthService);
  const token = auth.token();

  // 2. Si on a un token, cloner la requête en ajoutant le header
  //    (HttpRequest est immuable, donc on clone)
  if (token) {
    req = req.clone({
      setHeaders: { Authorization: `Bearer ${token}` }
    });
  }

  // 3. Passer la requête au handler suivant
  return next(req);
};

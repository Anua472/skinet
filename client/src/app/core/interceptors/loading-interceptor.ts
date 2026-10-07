import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { delay, finalize, identity } from 'rxjs';
import { Busy } from '../services/busy';
import { environment } from '../../../environments/environment';

export const loadingInterceptor: HttpInterceptorFn = (req, next) => {
  const busyServive = inject(Busy);

  busyServive.busy();

  return next(req).pipe(
    (environment.production ? identity : delay(500)),
    finalize(() => busyServive.idle())
  )
};

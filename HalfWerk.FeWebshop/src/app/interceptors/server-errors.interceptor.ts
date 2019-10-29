// server-errors.interceptor.ts
import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor,
  HttpErrorResponse
} from '@angular/common/http';

import { Observable } from 'rxjs/Observable';
import 'rxjs/add/operator/retry';
import { catchError } from 'rxjs/operators';
import { throwError } from 'rxjs';
import { AuthenticationService } from '@services/authentication/authentication.service';

@Injectable()
export class ServerErrorsInterceptor implements HttpInterceptor {

  constructor(private authenticationService: AuthenticationService) { }

  intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    return next.handle(request)
      .pipe(
        catchError((err) => {
          // if (err.status === 401) {
          //   // auto logout if 401 response returned from api
          //   this.authenticationService.logout();
          // }

          return throwError(err);
        }));
  }
}

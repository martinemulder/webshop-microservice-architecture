import { Injectable } from '@angular/core';
import { HttpRequest, HttpHandler, HttpEvent, HttpInterceptor } from '@angular/common/http';
import { Observable } from 'rxjs';

import { AuthenticationService } from '@services/authentication/authentication.service';

@Injectable()
export class JwtInterceptor implements HttpInterceptor {
  constructor(private authenticationService: AuthenticationService) { }

  intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        // add authorization header with jwt token if available
    const currentGebruiker = this.authenticationService.currentGebruikerValue;
    if (currentGebruiker && currentGebruiker.token) {
      // tslint:disable-next-line:no-parameter-reassignment
      request = request.clone({
        setHeaders: {
          Authorization: `Bearer ${currentGebruiker.token}`
        }
      });
    }

    return next.handle(request);
  }
}

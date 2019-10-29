import { Injectable } from '@angular/core';
import { JwtHelperService } from '@auth0/angular-jwt';

@Injectable({
  providedIn: 'root'
})
export class JwtService {

  public readonly ACCESS_TOKEN_KEY = 'access_token';

  set token(value: string) {
    window.localStorage.setItem(this.ACCESS_TOKEN_KEY, value);
  }

  get token() {
    return window.localStorage.getItem(this.ACCESS_TOKEN_KEY);
  }

  decodedToken() {
    return new JwtHelperService().decodeToken(this.token);
  }
}

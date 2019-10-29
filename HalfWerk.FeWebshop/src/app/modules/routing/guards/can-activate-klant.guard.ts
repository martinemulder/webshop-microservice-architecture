import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, CanActivateChild, Router, UrlTree } from '@angular/router';
import { Observable, of } from 'rxjs';
import { AuthenticationService } from '@services/authentication/authentication.service';

@Injectable({
  providedIn: 'root'
})
export class CanActivateKlant implements CanActivate, CanActivateChild {

  constructor(private authenticationService: AuthenticationService,
              private router: Router) { }

  canActivate(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): Observable<boolean | UrlTree> | Promise<boolean | UrlTree> | boolean | UrlTree {
    const canActivate = this.authenticationService.currentGebruikerRoles.includes('Klant');

    if (!canActivate) {
      this.router.navigate(['/login'], { queryParams: { returnUrl: state.url } });
    }

    return of(canActivate);
  }

  canActivateChild(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): Observable<boolean | UrlTree> | Promise<boolean | UrlTree> | boolean | UrlTree {
    const canActivate = this.authenticationService.currentGebruikerRoles.includes('Klant');

    if (!canActivate) {
      this.router.navigate(['/login'], { queryParams: { returnUrl: state.url } });
    }

    return of(canActivate);
  }
}

import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, RouterStateSnapshot, UrlTree, Router, CanActivateChild, CanActivate } from '@angular/router';
import { Observable, of } from 'rxjs';
import { AuthenticationService } from '@services/authentication/authentication.service';

@Injectable()
export class CanActivateWarehouse implements CanActivate, CanActivateChild {
  path: ActivatedRouteSnapshot[];
  route: ActivatedRouteSnapshot;

  constructor(private router: Router, private authenticationService: AuthenticationService) {}

  canActivate(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): Observable<boolean|UrlTree>|Promise<boolean|UrlTree>|boolean|UrlTree {
    const canActivate = this.authenticationService.currentGebruikerRoles.includes('Magazijn');

    if (!canActivate) {
      this.router.navigate(['/login'], { queryParams: { returnUrl: state.url } });
    }

    return of(canActivate);
  }

  canActivateChild(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): Observable<boolean|UrlTree>|Promise<boolean|UrlTree>|boolean|UrlTree {
    const canActivate = this.authenticationService.currentGebruikerRoles.includes('Magazijn');

    if (!canActivate) {
      this.router.navigate(['/login'], { queryParams: { returnUrl: state.url } });
    }

    return of(canActivate);
  }
}

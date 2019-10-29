import { Injectable } from '@angular/core';
import { CanActivate } from '@angular/router/src/utils/preactivation';
import { ActivatedRouteSnapshot, RouterStateSnapshot, UrlTree, Router, CanActivateChild } from '@angular/router';
import { Observable, of } from 'rxjs';
import { AuthenticationService } from '@services/authentication/authentication.service';

@Injectable()
export class CanActivateSales implements CanActivate, CanActivateChild {
  path: ActivatedRouteSnapshot[];
  route: ActivatedRouteSnapshot;

  constructor(private router: Router, private authenticationService: AuthenticationService) {}

  canActivate(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): Observable<boolean|UrlTree>|Promise<boolean|UrlTree>|boolean|UrlTree {
    const canActivate = this.authenticationService.currentGebruikerRoles.includes('Sales');

    if (!canActivate) {
      this.router.navigate(['/login'], { queryParams: { returnUrl: state.url } });
    }

    return of(canActivate);
  }

  canActivateChild(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): Observable<boolean|UrlTree>|Promise<boolean|UrlTree>|boolean|UrlTree {
    const canActivate = this.authenticationService.currentGebruikerRoles.includes('Sales');

    if (!canActivate) {
      this.router.navigate(['/login'], { queryParams: { returnUrl: state.url } });
    }

    return of(canActivate);
  }
}

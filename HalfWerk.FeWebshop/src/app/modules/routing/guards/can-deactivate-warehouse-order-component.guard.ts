import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, CanDeactivate } from '@angular/router';
import { Observable } from 'rxjs';
import { OrderComponent } from '@components/warehouse/order/order.component';

@Injectable({
  providedIn: 'root'
})
export class CanDeactivateWarehouseOrderComponentGuard implements CanDeactivate<OrderComponent> {

  canDeactivate(component: OrderComponent) {
    if (component.canClosePage()) {
      return true;
    }
    return window.confirm('Weet je zeker dat je de pagina wilt verlaten? De bestelling is nog niet volledig ingepakt!');
  }
}

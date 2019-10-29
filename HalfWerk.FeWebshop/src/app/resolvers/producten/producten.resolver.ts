import { ProductenService } from '@services/producten/producten.service';
import { Injectable } from '@angular/core';
import { Resolve, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { Product } from '@models/product';
import { Observable, of } from 'rxjs';

@Injectable()
export class ProductenResolver implements Resolve<Product[]> {

  constructor(private productenService: ProductenService) { }

  resolve(route: ActivatedRouteSnapshot,
          state: RouterStateSnapshot): Observable<Product[]> {

    return this.productenService.getProducten();
  }
}

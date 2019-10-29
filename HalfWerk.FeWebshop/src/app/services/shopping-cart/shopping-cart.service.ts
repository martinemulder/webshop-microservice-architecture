import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { Product } from '@models/product';
import { ToastrService } from 'ngx-toastr';
import { Location } from '@angular/common';

@Injectable({
  providedIn: 'root'
})
export class ShoppingCartService {

  private subject: BehaviorSubject<ICartProduct[]> = new BehaviorSubject([]);

  private shoppingCartContentMap: Map<number, ICartProduct> = new Map<number, ICartProduct>();

  public readonly cart$: Observable<ICartProduct[]> = this.subject.asObservable();

  constructor(private toasterService: ToastrService, private location: Location) {
    this.checkLocalStorage();
  }

  addToCart(product: Product): void {
    if (this.shoppingCartContentMap.has(product.artikelnummer)) {
      this.shoppingCartContentMap.get(product.artikelnummer).quantity += 1;
    } else {
      const newCartProduct = { product, quantity: 1 };
      this.shoppingCartContentMap.set(product.artikelnummer, newCartProduct);
    }
    this.showInfoToaster(`Artikel ${product.naam} is toegevoegd aan je winkelmandje.`);
    this.subject.next(Array.from(this.shoppingCartContentMap.values()));
    this.saveToLocalStorage();
  }

  removeFromCart(product: Product): void {
    if (this.shoppingCartContentMap.has(product.artikelnummer)) {
      if (this.shoppingCartContentMap.get(product.artikelnummer).quantity > 0) {
        this.shoppingCartContentMap.get(product.artikelnummer).quantity -= 1;
      } else {
        if (!this.location.path().includes('shop/cart')) {
          this.shoppingCartContentMap.delete(product.artikelnummer);
        }
      }
    }
    this.showInfoToaster(`Artikel ${product.naam} is verwijderd uit je winkelmandje.`, 1500);
    this.subject.next(Array.from(this.shoppingCartContentMap.values()));
    this.saveToLocalStorage();
  }

  clearFromCart(product: Product): void {
    if (this.shoppingCartContentMap.has(product.artikelnummer)) {
      this.shoppingCartContentMap.delete(product.artikelnummer);
    }
    this.subject.next(Array.from(this.shoppingCartContentMap.values()));
    this.saveToLocalStorage();
  }

  saveToLocalStorage(): void {
    localStorage.setItem('shoppingCart', JSON.stringify(Array.from(this.shoppingCartContentMap)));
  }

  clearLocalStorage(): void {
    if (localStorage.getItem('shoppingCart')) {
      this.saveToLocalStorage();
    }
  }

  /**
   * Convert the content of the shopping cart to ICartProduct[].
   *
   * @returns {ICartProduct[]}
   */
  getCartContentAsCartProducten(): ICartProduct[] {
    return Array.from(this.shoppingCartContentMap.values());
  }

  /**
   * Convert the content of the shopping cart to Product[].
   *
   * @returns {Product[]}
   */
  getCartContentAsProducten(): Product[] {
    return this.getCartContentAsCartProducten()
      .map((cartProduct) => {
        return cartProduct.product;
      });
  }

  getTotalItemCount(): number {
    let totalCount = 0;
    this.shoppingCartContentMap.forEach((item) => {
      totalCount += item.quantity;
    });
    return totalCount;
  }

  hasItemsWithQuantityZero(): boolean {
    return Array.from(this.shoppingCartContentMap.values()).filter((product) => {
      return product.quantity === 0;
    }).length > 0;
  }

  removeCartItemsWithQuantityZero(): void {
    this.shoppingCartContentMap.forEach((item) => {
      if (item.quantity <= 0) {
        this.shoppingCartContentMap.delete(item.product.artikelnummer);
      }
    });
    this.subject.next(Array.from(this.shoppingCartContentMap.values()));
  }

  removeAll(): void {
    this.shoppingCartContentMap.forEach((item) => {
      this.shoppingCartContentMap.delete(item.product.artikelnummer);
    });
    this.clearLocalStorage();
    this.subject.next(Array.from(this.shoppingCartContentMap.values()));
  }

  private showInfoToaster(message: string, timeOut: number = 3000): void {
    if (this.location.path().includes('/cart')) {
      return;
    }

    this.toasterService.info(message, '', {
      timeOut,
      closeButton: true,
      progressBar: true
    });
  }

  private checkLocalStorage(): void {
    if (localStorage.getItem('shoppingCart') !== null) {
      const shoppingCartContentMapFromLocalStorage = toJSON(JSON.parse(localStorage.getItem('shoppingCart')));

      if (shoppingCartContentMapFromLocalStorage) {
        for (const contentItem of shoppingCartContentMapFromLocalStorage) {
          const currentMapEntry = contentItem[1];
          this.shoppingCartContentMap.set(currentMapEntry[0], currentMapEntry[1]);
        }
      }
      this.subject.next(Array.from(this.shoppingCartContentMap.values()));
    }
  }
}

export interface ICartProduct {
  quantity: number;
  product: Product;
}

export const toJSON = (map: Map<any, any>) => {
  if (map === undefined) {
    return;
  }
  return Array.from(map.entries());
};

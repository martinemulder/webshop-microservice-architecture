import { Injectable } from '@angular/core';
import { ApiService } from '../api/api.service';
import { ShoppingCartService } from '../shopping-cart/shopping-cart.service';

@Injectable({
  providedIn: 'root'
})
export class CheckoutService {

  private _bestelling: IBestelling;

  get bestelling(): IBestelling {
    if (this._bestelling === null) {
      throw new Error('Bestelling heeft de waarde NULL');
    }
    return this._bestelling;
  }

  constructor(private shoppingCartService: ShoppingCartService, private apiService: ApiService<IBestelling>) { }

  canCheckout(): boolean {
    return this.getBestelregels().length > 0;
  }

  checkoutCart(klant: IContactInfo, adres: IAfleveradres) {
    this._bestelling = {
      contactInfo: klant,
      afleverAdres: adres,
      bestelregels: this.getBestelregels()
    };
    return this.apiService.post('bestelling', this.bestelling);
  }

  clearBestelling(): void {
    this._bestelling = null;
    this.shoppingCartService.getCartContentAsProducten()
                            .forEach((product) => {
                              this.shoppingCartService.clearFromCart(product);
                            });
  }

  /**
   * Convert the content of the shopping cart to IBestelRegel[].
   *
   * @private
   * @returns {IBestelregel[]}
   */
  private getBestelregels(): IBestelregel[] {
    return this.shoppingCartService.getCartContentAsCartProducten()
      .map((cartProduct) => {
        const bestelregel: IBestelregel = {
          aantal: cartProduct.quantity,
          artikelnummer: cartProduct.product.artikelnummer,
          leveranciercode: cartProduct.product.leverancier,
          naam: cartProduct.product.naam,
          prijs: cartProduct.product.prijs
        };
        return bestelregel;
      });
  }
}

export interface IBestelling {
  contactInfo: IContactInfo;
  afleverAdres: IAfleveradres;
  bestelregels: IBestelregel[];
}

export interface IAfleveradres {
  adres: string;
  postcode: string;
  plaats: string;
  land: string;
}

export interface IContactInfo {
  naam: string;
  telefoonnummer: string;
  email: string;
}

export interface IBestelregel {
  artikelnummer: number;
  leveranciercode: string;
  naam: string;
  prijs: number;
  aantal: number;
}

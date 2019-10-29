import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { ShoppingCartService } from '@services/shopping-cart/shopping-cart.service';
import { map } from 'rxjs/operators';
import { Observable } from 'rxjs';
import { Product } from '@models/product';
import { CheckoutService } from '@root/services/checkout/checkout.service';
import { PriceHelper } from '@root/helpers/price-helper';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';

@Component({
  selector: 'app-shopping-cart',
  templateUrl: './shopping-cart.component.html',
  styleUrls: ['./shopping-cart.component.scss']
})
export class ShoppingCartComponent implements OnInit {

  isLinear = false;
  firstFormGroup: FormGroup;
  secondFormGroup: FormGroup;

  displayedColumns: string[] = ['afbeelding', 'naam', 'prijsperstuk', 'aantal', 'prijs', 'actie'];
  private readonly BUTTON_TIMEOUT_IN_MS = 500;

  constructor(public shoppingCartService: ShoppingCartService,
              private checkoutService: CheckoutService,
              private _formBuilder: FormBuilder) { }

  ngOnInit() {
    this.firstFormGroup = this._formBuilder.group({
      firstCtrl: ['', Validators.required]
    });
    this.secondFormGroup = this._formBuilder.group({
      secondCtrl: ['', Validators.required]
    });
  }

  ngOnDestroy() {
    this.shoppingCartService.removeCartItemsWithQuantityZero();
  }

  getTotalCost(): Observable<number> {
    return this.shoppingCartService.cart$
      .pipe(
        map(
          cart => cart.reduce((acc, item) => acc + (this.getPriceInclBtw(item.product.prijs) * item.quantity), 0)
        )
      );
  }

  getTotalCostExclBtw(): Observable<number> {
    return this.shoppingCartService.cart$
      .pipe(
        map(
          cart => cart.reduce((acc, item) => acc + (item.product.prijs * item.quantity), 0)
        )
      );
  }

  getPriceInclBtw(prijs: number): number {
    return PriceHelper.addBtwToPrice(prijs);
  }

  increaseQuanityAmount(product: Product, event) : void {
    this.timeoutButton(event);
    this.shoppingCartService.addToCart(product);
  }

  decreaseQuanityAmount(product: Product, event): void {
    this.timeoutButton(event);
    this.shoppingCartService.removeFromCart(product);
  }

  clearQuantityAmount(product: Product) {
    this.shoppingCartService.clearFromCart(product);
  }

  /**
   * Add a timeout to the target element of a given event, this adds a natural delay to certain actions.
   *
   * @private
   * @param {*} event
   */
  private timeoutButton(event): void {
    event.target.disabled = true;
    setTimeout(() => event.target.disabled = false, this.BUTTON_TIMEOUT_IN_MS);
  }
}

import { Component, OnInit, Input } from '@angular/core';
import { Product } from '@models/product';
import { ShoppingCartService } from '@services/shopping-cart/shopping-cart.service';
import { PriceHelper } from '@helpers/price-helper';

@Component({
  selector: 'app-product',
  templateUrl: './product.component.html',
  styleUrls: ['./product.component.scss']
})
export class ProductComponent implements OnInit {

  @Input() product: Product;

  get imageUrl(): string {
    return `/assets/images/producten/${this.product.afbeeldingUrl}`;
  }

  get priceInclBtw(): number {
    return PriceHelper.addBtwToPrice(this.product.prijs);
  }

  constructor(private shoppingCartService: ShoppingCartService) { }

  ngOnInit() {
  }

  addProductToShoppingCart() {
    this.shoppingCartService.addToCart(this.product);
  }
}

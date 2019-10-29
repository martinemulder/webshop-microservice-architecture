import { TestBed } from '@angular/core/testing';

import { ShoppingCartService } from './shopping-cart.service';
import { Product } from '@models/product';
import { ToastrService, ToastrModule } from 'ngx-toastr';
import { AppModule } from '@root/app.module';

describe('ShoppingCartService', () => {
  let service: ShoppingCartService;
  let product: Product;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [
        AppModule
      ]
    });
    service = TestBed.get(ShoppingCartService);
    product = new Product();
    product.prijs = 10.00;
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should add a product to cart', () => {
    service.addToCart(product);
    expect(service.getTotalItemCount()).toBe(1);
  });

  it('should have a quantity of 2 when the same product is added twice to cart', () => {
    service.addToCart(product);
    service.addToCart(product);
    expect(service.getTotalItemCount()).toBe(2);
  });

  it('should set the quantity of a product in cart from 1 to 0 when product quantity is decreased', () => {
    service.addToCart(product);
    service.removeFromCart(product);
    expect(service.getTotalItemCount()).toBe(0);
  });

  it('should keep a product with a quantity of 0 in cart', () => {
    service.addToCart(product);
    service.removeFromCart(product);
    expect(service.getTotalItemCount()).toBe(0);
    expect(service.hasItemsWithQuantityZero()).toBe(true);
  });

  it('should delete a product in cart when quantity is 0 and product quantity is decreased', () => {
    service.addToCart(product);
    service.removeFromCart(product);
    service.removeFromCart(product);
    expect(service.getTotalItemCount()).toBe(0);
    expect(service.hasItemsWithQuantityZero()).toBe(false);
  });

  it('should delete all products with a quantity of 0 from cart', () => {
    service.addToCart(product);
    service.removeFromCart(product);
    service.removeCartItemsWithQuantityZero();
    expect(service.getTotalItemCount()).toBe(0);
    expect(service.hasItemsWithQuantityZero()).toBe(false);
  });

  afterEach(() => {
    service.clearFromCart(product);
  });
});

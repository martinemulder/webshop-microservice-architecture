import { TotalCartItemPricePipe } from './total-cart-item-price.pipe';
import { Product } from '@models/product';
import { ICartProduct } from '@services/shopping-cart/shopping-cart.service';

describe('TotalCartItemPricePipe', () => {
  let totalCartItemPricePipe: TotalCartItemPricePipe;
  let cartProduct: ICartProduct;

  beforeEach(() => {
    totalCartItemPricePipe = new TotalCartItemPricePipe();
    const product = new Product();
    product.prijs = 11.50;
    cartProduct = { product, quantity: 1 };
  });

  it('should return 13.92 (incl btw)', () => {
    expect(totalCartItemPricePipe.transform(cartProduct)).toBe(13.92);
  });

  it('should return 27.84 (incl btw)', () => {
    cartProduct.quantity = 2;
    expect(totalCartItemPricePipe.transform(cartProduct)).toBe(27.84);
  });

  it('should return 60.5 (incl btw)', () => {
    const product = new Product();
    product.prijs = 10.00;
    cartProduct.product = product;

    cartProduct.quantity = 5;
    expect(totalCartItemPricePipe.transform(cartProduct)).toBe(60.5);
  });
});

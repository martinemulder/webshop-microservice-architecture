import { Pipe, PipeTransform } from '@angular/core';
import { ICartProduct } from '@services/shopping-cart/shopping-cart.service';
import { PriceHelper } from '@helpers/price-helper';

@Pipe({
  name: 'totalCartItemPrice',
  pure: false,
})
export class TotalCartItemPricePipe implements PipeTransform {

  transform(value: ICartProduct): number {
    const price = PriceHelper.addBtwToPrice(value.product.prijs);
    return (value.quantity * price);
  }

}

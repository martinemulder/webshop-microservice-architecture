import { Pipe, PipeTransform } from '@angular/core';
import { PriceHelper } from '@helpers/price-helper';
import { Bestelling } from '@models/bestelling';

@Pipe({
  name: 'totalOrderPrice',
  pure: false,
})
export class TotalOrderPricePipe implements PipeTransform {

  transform(value: Bestelling): number {
    return value.bestelRegels.reduce((acc, item) => acc + (PriceHelper.addBtwToPrice(item.prijsExclBtw) * item.aantal), 0);
  }

}

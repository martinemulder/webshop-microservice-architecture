import { Pipe, PipeTransform } from '@angular/core';
import { Bestelling } from '@models/bestelling';

@Pipe({
  name: 'totalNumberProductsOrder',
  pure: false,
})
export class TotalNumberProductsOrderPipe implements PipeTransform {

  transform(value: Bestelling): number {
    return value.bestelRegels.reduce((acc, item) => acc + item.aantal, 0);
  }

}

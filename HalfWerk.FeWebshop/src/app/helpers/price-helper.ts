import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class PriceHelper {

  static addBtwToPrice(price: number) {
    const returnVal = price * 1.21;
    return Math.round(returnVal * 100) / 100;
  }

}

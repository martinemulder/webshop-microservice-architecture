import { Component, OnInit } from '@angular/core';
import { Product } from '@models/product';
import { ActivatedRoute } from '@angular/router';
import { IProductSearchQueryResult } from '../product-search/product-search.component';
import { throttleFunction } from '@extensions/throttle/throttle';

@Component({
  selector: 'app-shop',
  templateUrl: './shop.component.html',
  styleUrls: ['./shop.component.scss']
})
export class ShopComponent implements OnInit {

  _producten: Product[] = [];

  length = 100;
  pageSize = 10;
  pageIndex = 0;
  pageSizeOptions: number[] = [5, 10, 25, 100];

  queryResult?: IProductSearchQueryResult;

  get allProducten(): Product[] {
    return this._producten;
  }

  get resultaatText(): string {
    if (this.queryResult) {
      if (this.queryResult.producten.length === 1) {
        return 'resultaat';
      }
      return 'resultaten';
    }
  }

  get producten(): Product[] {
    if (this.queryResult) {
      throttleFunction(
        setTimeout(() => {
          if (this.queryResult) {
            this.length = this.queryResult.producten.length;
          } else {
            this.length = this._producten.length;
          }
        },         0),
        200);
      const startIndex = this.pageIndex * this.pageSize;
      const endIndex = startIndex + this.pageSize;
      return this.queryResult.producten.slice(startIndex, endIndex);
    }

    const startIndex = this.pageIndex * this.pageSize;
    const endIndex = startIndex + this.pageSize;
    return this._producten.slice(startIndex, endIndex);
  }

  constructor(private activatedRoute: ActivatedRoute) { }

  ngOnInit() {
    this._producten = this.activatedRoute.snapshot.data['producten'];
    this.length = this._producten.length;
  }

  handlePageEvent(event) {
    this.pageSize = event.pageSize;
    this.pageIndex = event.pageIndex;
  }

  handleQueryResult(queryResult: IProductSearchQueryResult) {
    if (queryResult.query.length > 0) {
      return this.queryResult = queryResult;
    }
    this.queryResult = null;
  }
}

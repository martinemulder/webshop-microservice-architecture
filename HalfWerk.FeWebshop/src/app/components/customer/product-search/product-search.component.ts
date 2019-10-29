import { Component, Input, ViewChild, OnInit, Output, EventEmitter, AfterViewInit } from '@angular/core';
import { FormControl } from '@angular/forms';
import { Observable, Subscription } from 'rxjs';
import { map, startWith } from 'rxjs/operators';
import { Product } from '@models/product';
import { MatInput, MatAutocompleteTrigger, MatAutocomplete } from '@angular/material';

@Component({
  selector: 'app-product-search',
  templateUrl: './product-search.component.html',
  styleUrls: ['./product-search.component.scss'],
})
export class ProductSearchComponent {

  @Input('products') producten: Product[];

  @Output('productQueryResult') queryResultEmitter: EventEmitter<IProductSearchQueryResult> = new EventEmitter<IProductSearchQueryResult>();

  @ViewChild(MatAutocompleteTrigger) autocompleteTrigger: MatAutocompleteTrigger;

  @ViewChild(MatAutocomplete) autocomplete: MatAutocomplete;

  @ViewChild('matInput') matInput: MatInput;

  productControl = new FormControl();
  filteredProducts: Observable<Product[]>;

  constructor() {
    this.filteredProducts = this.productControl.valueChanges
      .pipe(
        startWith(''),
        map(product => product ? this.filterProducts(product) : this.producten.slice())
      );
  }

  handleEnter() {
    this.autocompleteTrigger.closePanel();
    this.matInput.focused = false;
  }

  handleKeyup() {
    if (this.matInput.value === '') {
      return this.emitQueryResult('', []);
    }
    return;
  }

  private passesFilter(value: string, filterText: string): boolean {
    return (
      value !== null &&
      value !== undefined &&
      value.toLowerCase().indexOf(filterText) !== -1
    );
  }

  private passesCategoryFilter(categories: string[], filterText: string): boolean {
    return categories.some(
      category =>
        this.passesFilter(category, filterText)
    );
  }

  private filterProducts(query: string): Product[] {
    const filterText = query.toLowerCase();
    const filteredProducts = this.producten.filter(
      product =>
        this.passesFilter(product.naam, filterText) || this.passesCategoryFilter(product.categorieen, filterText)
    );

    this.emitQueryResult(query, filteredProducts);

    return filteredProducts;
  }

  private emitQueryResult(query: string, producten: Product[]) {
    const queryResult: IProductSearchQueryResult = { query, producten };
    this.queryResultEmitter.emit(queryResult);
  }
}

export interface IProductSearchQueryResult {
  query: string;
  producten: Product[];
}

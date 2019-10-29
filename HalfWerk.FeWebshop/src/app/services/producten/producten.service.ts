import { Injectable } from '@angular/core';
import { ApiService } from '@services/api/api.service';
import { Product } from '@models/product';
import { Subject, Observable, BehaviorSubject, of } from 'rxjs';
import { Cacheable, CacheBuster } from 'ngx-cacheable';

export const productenCacheBuster$ = new Subject<any>();

@Injectable({
  providedIn: 'root'
})
export class ProductenService {

  private subject: BehaviorSubject<Product[]> = new BehaviorSubject([]);

  public readonly producten$: Observable<Product[]> = this.subject.asObservable();

  constructor(private apiService: ApiService<Product>) {
    this.loadAll();
  }

  @Cacheable({
    maxAge: 300000,
    cacheBusterObserver: productenCacheBuster$
  })
  getProducten() {
    return this.apiService.get<Product[]>('artikel');
  }

  @CacheBuster({
    cacheBusterNotifier: productenCacheBuster$
  })
  bustCache() {
    return of(true);
  }

  private loadAll(): void {
    this.getProducten()
      .subscribe((res: any) => {
        this.subject.next(res);
        this.subject.complete();
      });
  }
}

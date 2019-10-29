import { TestBed } from '@angular/core/testing';

import { ProductenService } from './producten.service';
import { AppModule } from '@root/app.module';
import { HttpClientTestingModule } from '@angular/common/http/testing';

describe('ProductenService', () => {
  beforeEach(() => TestBed.configureTestingModule({
    imports: [
      AppModule,
      HttpClientTestingModule
    ]
  }));

  it('should be created', () => {
    const service: ProductenService = TestBed.get(ProductenService);
    expect(service).toBeTruthy();
  });
});

import { TestBed } from '@angular/core/testing';

import { CheckoutService } from './checkout.service';
import { AppModule } from '@root/app.module';

describe('CheckoutService', () => {
  beforeEach(() => TestBed.configureTestingModule({
    imports: [
      AppModule,
    ]
  }));

  it('should be created', () => {
    const service: CheckoutService = TestBed.get(CheckoutService);
    expect(service).toBeTruthy();
  });
});

import { TestBed } from '@angular/core/testing';

import { AuthenticationService } from './authentication.service';
import { RouterTestingModule } from '@angular/router/testing';
import { ShopComponent } from '@components/customer/shop/shop.component';
import { AppModule } from '@root/app.module';

describe('AuthenticationService', () => {
  beforeEach(() => TestBed.configureTestingModule({
    imports: [
      RouterTestingModule.withRoutes([
        { path: 'shop', component: ShopComponent }
      ]),
      AppModule
    ]
  }));

  it('should be created', () => {
    const service: AuthenticationService = TestBed.get(AuthenticationService);
    expect(service).toBeTruthy();
  });
});

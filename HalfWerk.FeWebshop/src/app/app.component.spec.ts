import { TestBed, async, ComponentFixture, tick } from '@angular/core/testing';
import { RouterTestingModule } from '@angular/router/testing';
import { AppComponent } from './app.component';
import { AppModule } from '@root/app.module';
import { NavigationStart, RouterEvent, NavigationEnd, Router } from '@angular/router';
import { ShopComponent } from '@components/customer/shop/shop.component';

let router: Router;

describe('AppComponent', () => {
  beforeEach(async(() => {
    TestBed.configureTestingModule({
      imports: [
        RouterTestingModule.withRoutes([
          { path: 'shop', component: ShopComponent }
        ]),
        AppModule
      ],
    }).compileComponents();

    router = TestBed.get(Router);
    router.initialNavigation();
  }));

  it('should create the app', () => {
    const fixture = TestBed.createComponent(AppComponent);
    const app = fixture.debugElement.componentInstance;
    expect(app).toBeTruthy();
  });

  // it('should check router events', async () => {
  //   router.navigate(['shop']);
  //   tick();
  //   expect(true).toBe(true);
  //   // expect(component.checkEventFromRouter).toHaveBeenCalledTimes(1);
  // });

  // it('should set loading to false when router navigation ended', () => {
  //   const fixture = TestBed.createComponent(AppComponent);
  //   const app: AppComponent = fixture.debugElement.componentInstance;
  //   const mockEvent: NavigationEnd = new NavigationEnd(1, '', '');
  //   app.loading = true;
  //   app.checkEventFromRouter(mockEvent);
  //   expect(app.loading).toBe(false);
  // });
});

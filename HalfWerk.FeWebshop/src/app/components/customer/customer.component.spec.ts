import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CustomerComponent } from './customer.component';
import { NavigationStart } from '@angular/router';
import { AppModule } from '@root/app.module';
import { RouterTestingModule } from '@angular/router/testing';
import { ShopComponent } from './shop/shop.component';

describe('CustomerComponent', () => {
  let component: CustomerComponent;
  let fixture: ComponentFixture<CustomerComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      imports: [
        RouterTestingModule.withRoutes([
          { path: 'shop', component: ShopComponent }
        ]),
        AppModule
      ],
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CustomerComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();

    spyOn(component, 'checkEventFromRouter');
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should have an instance of mat-spinner when loading is true', () => {
    component.loading = true;
    const compiled = fixture.debugElement.nativeElement;
    expect(compiled.querySelector('mat-spinner')).not.toBe(null);
  });

  it('should have an instance of router outlet when loading is false', () => {
    component.loading = false;
    const compiled = fixture.debugElement.nativeElement;
    expect(compiled.querySelector('router-outlet')).toBe(null);
  });

  it('should set expanded to true when the sidenav has been opened', () => {
    const fixture = TestBed.createComponent(CustomerComponent);
    const app: CustomerComponent = fixture.debugElement.componentInstance;
    app.expanded = false;
    app.onSideNavStateChanged(true);
    expect(app.expanded).toBe(true);
  });

  it('should set expanded to false when the sidenav has been closed', () => {
    const fixture = TestBed.createComponent(CustomerComponent);
    const app: CustomerComponent = fixture.debugElement.componentInstance;
    app.expanded = true;
    app.onSideNavStateChanged(false);
    expect(app.expanded).toBe(false);
  });

  it('should set loading to true when router navigation started', () => {
    const fixture = TestBed.createComponent(CustomerComponent);
    const app: CustomerComponent = fixture.debugElement.componentInstance;
    const mockEvent: NavigationStart = new NavigationStart(1, '');
    app.loading = false;
    app.checkEventFromRouter(mockEvent);
    expect(app.loading).toBe(true);
  });
});

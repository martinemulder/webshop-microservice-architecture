import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { AppModule } from '@root/app.module';
import { SidebarComponent, IRouterLink } from './sidebar.component';
import { ShoppingCartService } from '@services/shopping-cart/shopping-cart.service';

describe('SidebarComponent', () => {
  let component: SidebarComponent;
  let fixture: ComponentFixture<SidebarComponent>;
  let service: ShoppingCartService;
  let spy: any;

  beforeEach(async(() => {

    TestBed.configureTestingModule({
      imports: [
        AppModule
      ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(SidebarComponent);
    service = TestBed.get(ShoppingCartService);
    component = fixture.componentInstance;
    fixture.detectChanges();
  }));

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('Winkelmandje routerlink should display correct easter egg when cart count is smaller than 10', () => {
    spy = spyOn(service, 'getTotalItemCount').and.returnValue(5);
    component.ngOnInit();

    const element: IRouterLink = component.routerLinks.find(link => link.name === 'Winkelmandje');
    expect(element).not.toBeUndefined();
    expect(element.icon).toBe('shopping_basket');
  });

  it('Winkelmandje routerlink should display correct easter egg when cart count is smaller than 99 but bigger than 10', () => {
    spy = spyOn(service, 'getTotalItemCount').and.returnValue(11);
    component.ngOnInit();

    const element: IRouterLink = component.routerLinks.find(link => link.name === 'Winkelmandje');
    expect(element).not.toBeUndefined();
    expect(element.icon).toBe('shopping_cart');
  });

  it('Winkelmandje routerlink should display correct easter egg when cart count is bigger than 99', () => {
    spy = spyOn(service, 'getTotalItemCount').and.returnValue(100);
    component.ngOnInit();

    const element: IRouterLink = component.routerLinks.find(link => link.name === 'Winkelmandje');
    expect(element).not.toBeUndefined();
    expect(element.icon).toBe('local_shipping');
  });

});

import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { AppModule } from '@root/app.module';
import { Product } from '@models/product';
import { ProductComponent } from './product.component';

describe('ProductComponent', () => {
  let component: ProductComponent;
  let fixture: ComponentFixture<ProductComponent>;
  let productStub: Product;

  beforeEach((() => {
    productStub = new Product();
    productStub.afbeeldingUrl = '';
    TestBed.configureTestingModule({
      imports: [
        AppModule
      ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ProductComponent);
    component = fixture.componentInstance;
    component.product = productStub;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

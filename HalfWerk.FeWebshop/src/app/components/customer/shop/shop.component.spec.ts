import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AppModule } from '@root/app.module';
import { ActivatedRoute } from '@angular/router';
import { Product } from '@models/product';
import { ShopComponent } from './shop.component';

describe('ShopComponent', () => {
  let component: ShopComponent;
  let fixture: ComponentFixture<ShopComponent>;
  let routeStub: any;

  beforeEach((async () => {
    routeStub = {
      snapshot: {
        data: {
          producten: []
        }
      }
    },
      TestBed.configureTestingModule({
        imports: [
          AppModule
        ],
        providers: [
          { provide: ActivatedRoute, useValue: routeStub }
        ]
      })
        .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ShopComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

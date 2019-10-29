import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { NextOrderComponent } from './next-order.component';
import { AppModule } from '@root/app.module';

describe('NextOrderComponent', () => {
  let component: NextOrderComponent;
  let fixture: ComponentFixture<NextOrderComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      imports: [
        AppModule
      ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(NextOrderComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

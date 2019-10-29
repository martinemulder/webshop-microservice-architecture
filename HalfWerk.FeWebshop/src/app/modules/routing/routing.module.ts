import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ProductenResolver } from '@root/resolvers/producten/producten.resolver';
import { ShopComponent } from '@components/customer/shop/shop.component';
import { ShoppingCartComponent } from '@components/customer/shopping-cart/shopping-cart.component';
import { CheckoutComponent } from '@components/customer/checkout/checkout.component';
import { CustomerComponent } from '@components/customer/customer.component';
import { PageNotFoundComponent } from '@components/shared/page-not-found/page-not-found.component';
import { SalesComponent } from '@components/sales/sales.component';
import { OrdersComponent } from '@components/sales/orders/orders.component';
import { ShoppingCartContainerComponent } from '@components/customer/shopping-cart-container/shopping-cart-container.component';
import { LoginComponent } from '@components/shared/login/login.component';
import { CanActivateSales } from './guards/can-activate-sales.guard';
import { AuthGuard } from './guards/auth.guard';
import { WarehouseComponent } from '@components/warehouse/warehouse.component';
import { NextOrderComponent } from '@components/warehouse/next-order/next-order.component';
import { CanActivateWarehouse } from '@modules/routing/guards/can-activate-warehouse.guard';
import { RegisterComponent } from '@components/shared/register/register.component';
import { OrderComponent } from '@components/warehouse/order/order.component';
import { PaymentComponent } from '@components/sales/payment/payment.component';
import { CanActivateKlant } from './guards/can-activate-klant.guard';
import { CanDeactivateWarehouseOrderComponentGuard } from './guards/can-deactivate-warehouse-order-component.guard';

const appRoutes: Routes = [
  {
    path: '',
    redirectTo: 'shop',
    pathMatch: 'full'
  },
  {
    path: 'login',
    component: LoginComponent
  },
  {
    path: 'register',
    component: RegisterComponent
  },
  {
    path: 'sales',
    component: SalesComponent,
    canActivate: [
      AuthGuard
    ],
    canActivateChild: [
      CanActivateSales
    ],
    children: [
      {
        path: '',
        pathMatch: 'full',
        redirectTo: 'orders'
      },
      {
        path: 'orders',
        component: OrdersComponent
      },
      {
        path: 'payment',
        component: PaymentComponent
      }
    ]
  },
  {
    path: 'warehouse',
    canActivate: [
      AuthGuard
    ],
    canActivateChild: [
      CanActivateWarehouse
    ],
    component: WarehouseComponent,
    children: [
      {
        path: '',
        component: NextOrderComponent
      },
      {
        path: 'nextorder',
        component: OrderComponent,
        canDeactivate: [
          CanDeactivateWarehouseOrderComponentGuard
        ]
      },
    ]
  },
  {
    path: 'shop',
    component: CustomerComponent,
    children: [
      {
        path: '',
        resolve: { producten: ProductenResolver },
        component: ShopComponent,
      },
      {
        path: 'cart',
        component: ShoppingCartContainerComponent,
        children: [
          {
            path: '',
            component: ShoppingCartComponent
          },
          {
            path: 'checkout',
            component: CheckoutComponent,
            canActivate: [
              CanActivateKlant
            ]
          }
        ]
      }
    ]
  },
  {
    path: '**',
    component: PageNotFoundComponent
  }
];

@NgModule({
  imports: [
    RouterModule.forRoot(
      appRoutes,
      { enableTracing: true } // <-- debugging purposes only
    )
  ],
  exports: [RouterModule]
})
export class RoutingModule {
}

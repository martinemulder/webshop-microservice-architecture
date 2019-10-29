import { NgModule, LOCALE_ID, ErrorHandler } from '@angular/core';
import { AppComponent } from './app.component';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { BrowserModule } from '@angular/platform-browser';
import { MaterialModule } from '@modules/material/material.module';
import { RoutingModule } from '@modules/routing/routing.module';
import { FormBuilder, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { ServicesModule } from '@modules/services/services.module';
import nl from '@angular/common/locales/nl';
import { registerLocaleData } from '@angular/common';
import { TotalCartItemPricePipe } from '@pipes/total-cart-item-price/total-cart-item-price.pipe';
import { ProductListComponent } from '@components/customer/shop/product-list/product-list.component';
import { ShopComponent } from '@components/customer/shop/shop.component';
import { ShoppingCartComponent } from '@components/customer/shopping-cart/shopping-cart.component';
import { ProductComponent } from '@components/customer/shop/product-list/product/product.component';
import { CheckoutComponent } from '@components/customer/checkout/checkout.component';
import { CustomerComponent } from '@components/customer/customer.component';
import { SidebarComponent } from '@components/customer/sidebar/sidebar.component';
import { PageNotFoundComponent } from '@components/shared/page-not-found/page-not-found.component';
import { SalesComponent } from '@components/sales/sales.component';
import { OrdersComponent } from '@components/sales/orders/orders.component';
import { OrdersListComponent } from '@components/sales/orders/orders-list/orders-list.component';
import { TotalNumberProductsOrderPipe } from '@pipes/total-number-products-order/total-number-products-order.pipe';
import { TotalOrderPricePipe } from '@pipes/total-order-price/total-order-price.pipe';
import { ToastrModule } from 'ngx-toastr';
import { ErrorsHandler } from '@root/extensions/errors-handler/errors-handler';
import { ServerErrorsInterceptor } from '@root/interceptors/server-errors.interceptor';
import { ShoppingCartContainerComponent } from '@components/customer/shopping-cart-container/shopping-cart-container.component';
import { LoginComponent } from '@components/shared/login/login.component';
import { CanActivateSales } from '@modules/routing/guards/can-activate-sales.guard';
import { JwtInterceptor } from '@root/interceptors/jwt.interceptor';
import { AuthGuard } from '@modules/routing/guards/auth.guard';
import { WarehouseComponent } from '@components/warehouse/warehouse.component';
import { NextOrderComponent } from '@components/warehouse/next-order/next-order.component';
import { CanActivateWarehouse } from '@modules/routing/guards/can-activate-warehouse.guard';
import { VoorraadPipe } from '@pipes/voorraad.pipe.ts/voorraad.pipe';
import { LoginButtonComponent } from '@components/shared/login/login-button/login-button.component';
import { JwtModule } from '@auth0/angular-jwt';
import { tokenGetter } from '@extensions/token-getter/token-getter';
import { RegisterComponent } from './components/shared/register/register.component';
import { OrderComponent } from './components/warehouse/order/order.component';
import { ProductSearchComponent } from './components/customer/product-search/product-search.component';
import { PaymentComponent } from './components/sales/payment/payment.component';
import { CanActivateKlant } from '@modules/routing/guards/can-activate-klant.guard';
import { CanDeactivateWarehouseOrderComponentGuard } from '@modules/routing/guards/can-deactivate-warehouse-order-component.guard';
import { RegisterButtonComponent } from './components/shared/login/register-button/register-button.component';

registerLocaleData(nl);

@NgModule({
  declarations: [
    AppComponent,
    ShopComponent,
    PageNotFoundComponent,
    ShoppingCartComponent,
    SidebarComponent,
    ProductListComponent,
    ProductComponent,
    TotalCartItemPricePipe,
    TotalNumberProductsOrderPipe,
    TotalOrderPricePipe,
    VoorraadPipe,
    CheckoutComponent,
    CustomerComponent,
    SalesComponent,
    OrdersComponent,
    OrdersListComponent,
    ShoppingCartContainerComponent,
    WarehouseComponent,
    NextOrderComponent,
    LoginComponent,
    LoginButtonComponent,
    RegisterComponent,
    OrderComponent,
    ProductSearchComponent,
    PaymentComponent,
    RegisterButtonComponent,
  ],
  imports: [
    HttpClientModule,
    BrowserModule,
    RoutingModule,
    MaterialModule,
    ServicesModule,
    FormsModule,
    ReactiveFormsModule,
    ToastrModule.forRoot({
      timeOut: 5000,
      positionClass: 'toast-top-center',
      preventDuplicates: true,
    }),
    JwtModule.forRoot({
      config: {
        tokenGetter,
        whitelistedDomains: [
          'localhost:4200',
          'infosupport.net'
        ]
      }
    })
  ],
  providers: [
    FormBuilder,
    {
      provide: LOCALE_ID,
      useValue: 'nl-NL'
    },
    {
      provide: ErrorHandler,
      useClass: ErrorsHandler
    },
    {
      provide: HTTP_INTERCEPTORS,
      useClass: ServerErrorsInterceptor,
      multi: true,
    },
    {
      provide: HTTP_INTERCEPTORS,
      useClass: JwtInterceptor,
      multi: true
    },
    AuthGuard,
    CanActivateSales,
    CanActivateWarehouse,
    CanActivateKlant,
    CanDeactivateWarehouseOrderComponentGuard
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }

import { NgModule } from '@angular/core';
import { HttpService, ApiService } from '@services/api/api.service';
import { ShoppingCartService } from '@services/shopping-cart/shopping-cart.service';
import { ProductenResolver } from '@root/resolvers/producten/producten.resolver';
import { AuthenticationService } from '@services/authentication/authentication.service';
import { BestellingenService } from '@services/bestellingen/bestellingen.service';
import { BetalingenService } from '@services/betalingen/betalingen.service';

@NgModule({
  imports: [
  ],
  exports: [
  ],
  providers: [
    HttpService,
    ApiService,
    ShoppingCartService,
    BestellingenService,
    AuthenticationService,
    BetalingenService,
    ProductenResolver,
  ]
})
export class ServicesModule {}

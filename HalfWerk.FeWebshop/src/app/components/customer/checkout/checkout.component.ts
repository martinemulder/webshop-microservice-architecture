import { Component, OnInit, ViewChild } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { CheckoutService, IBestelregel, IContactInfo, IAfleveradres } from '@services/checkout/checkout.service';
import { Router } from '@angular/router';
import { MatStepper } from '@angular/material';
import { ToastrService } from 'ngx-toastr';
import { ShoppingCartService } from '@root/services/shopping-cart/shopping-cart.service';
import { ProductenService } from '@services/producten/producten.service';
import { Gebruiker } from '@models/gebruiker';
import { AuthenticationService } from '@services/authentication/authentication.service';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-checkout',
  templateUrl: './checkout.component.html',
  styleUrls: ['./checkout.component.scss']
})
export class CheckoutComponent implements OnInit {
  @ViewChild('stepper') stepper: MatStepper;

  private _bestellingDto: IBestellingDto;
  private _currentUser: Gebruiker;

  firstFormGroup: FormGroup;
  secondFormGroup: FormGroup;

  orderPlaced: boolean = false;
  showSpinner: boolean = true;

  editable: boolean = true;

  get bestellingDto(): IBestellingDto {
    if (this._bestellingDto === null) {
      throw new Error('BestellingDto is NULL');
    }
    return this._bestellingDto;
  }

  get ContactInfoFromForm(): IContactInfo {
    return {
      naam: `${this.firstFormGroup.value.voornaam} ${this.firstFormGroup.value.achternaam}`,
      email: this.firstFormGroup.value.email,
      telefoonnummer: this.firstFormGroup.value.telefoonnummer,
    };
  }

  get AfleveradresFromForm(): IAfleveradres {
    return {
      adres: `${this.secondFormGroup.value.straat} ${this.secondFormGroup.value.huisnummer}`,
      postcode: this.secondFormGroup.value.postcode,
      plaats: this.secondFormGroup.value.woonplaats,
      land: this.secondFormGroup.value.land,
    };
  }

  constructor(public checkoutService: CheckoutService,
              private _formBuilder: FormBuilder,
              private router: Router,
              private toasterService: ToastrService,
              private cartService: ShoppingCartService,
              private productenService: ProductenService,
              private authenticationService: AuthenticationService) { }

  ngOnInit() {
    this._currentUser = this.authenticationService.getCurrentUser();

    this.authenticationService.getCurrentUser();

    if (!this.checkoutService.canCheckout()) {
      this.router.navigate(['shop']);
    }
    this.firstFormGroup = this._formBuilder.group({
      voornaam: ['', Validators.required],
      achternaam: ['', Validators.required],
      email: ['', Validators.required],
      telefoonnummer: ['', Validators.required],
    });
    this.secondFormGroup = this._formBuilder.group({
      straat: ['', Validators.required],
      huisnummer: ['', Validators.required],
      postcode: ['', Validators.required],
      woonplaats: ['', Validators.required],
      land: ['', Validators.required],
    });

    if (this._currentUser) {
      this.firstFormGroup.get('voornaam').setValue(this._currentUser.klant.voornaam);
      this.firstFormGroup.get('achternaam').setValue(this._currentUser.klant.achternaam);
      this.firstFormGroup.get('email').setValue(this._currentUser.email);
      this.firstFormGroup.get('telefoonnummer').setValue(this._currentUser.klant.telefoonnummer);

      this.secondFormGroup.get('straat').setValue(this._currentUser.klant.adres.straatnaam);
      this.secondFormGroup.get('huisnummer').setValue(this._currentUser.klant.adres.huisnummer);
      this.secondFormGroup.get('postcode').setValue(this._currentUser.klant.adres.postcode);
      this.secondFormGroup.get('woonplaats').setValue(this._currentUser.klant.adres.plaats);
    }

    this.secondFormGroup.get('land').setValue('Nederland');
  }

  ngOnDestroy(): void {
    if (this.orderPlaced) {
      this.checkoutService.clearBestelling();
    }
  }

  checkout(): void {
    this.orderPlaced = true;
    this.checkoutService.checkoutCart(this.ContactInfoFromForm, this.AfleveradresFromForm)
      .subscribe((response) => {
        if (Number.isInteger(response as unknown as number)) {
          this._bestellingDto = { bestelnummer: response as unknown as number, ... this.checkoutService.bestelling };
          this.handleSuccesfulCheckout();
        } else {
          this.toasterService.error('Er ging iets mis tijdens het plaatsen van de bestelling, probeer later opnieuw');
          this.orderPlaced = false;
        }
      },
                 (err) => {
                   this.orderPlaced = false;
                   throw err;
                 }
      );
  }

  private handleSuccesfulCheckout(): void {
    this.showSpinner = false;
    this.cartService.removeAll();
    this.stepper.next();
    this.editable = false;
    const subscription = this.productenService
      .bustCache()
      .subscribe(() => {
        if (subscription) {
          subscription.unsubscribe();
        }
      });
  }
}

export interface IBestellingDto {
  bestelnummer: number;
  contactInfo: IContactInfo;
  afleverAdres: IAfleveradres;
  bestelregels: IBestelregel[];
}

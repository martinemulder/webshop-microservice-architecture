import { Component, OnInit, Input } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthenticationService } from '@services/authentication/authentication.service';
import { ToastrService } from 'ngx-toastr';
import { CustomValidators } from '@extensions/custom-validators/custom-validators';
import { Klant } from '@models/klant';
import { Gebruiker } from '@models/gebruiker';
import { Adres } from '@models/adres';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.scss']
})
export class RegisterComponent implements OnInit {

  registerForm: FormGroup;

  showSpinner: boolean = false;

  constructor(private authenticationService: AuthenticationService,
              private router: Router,
              private formBuilder: FormBuilder,
              private toasterService: ToastrService) { }

  ngOnInit() {
    this.registerForm = this.formBuilder.group(
      {
        voornaam: ['', Validators.required],
        achternaam: ['', Validators.required],
        straatnaam: ['', Validators.required],
        huisnummer: ['', Validators.required],
        postcode: ['', Validators.required],
        plaats: ['', Validators.required],
        land: ['', Validators.required],
        telefoonnummer: ['', Validators.required],
        email: ['', Validators.email],
        wachtwoord: ['', Validators.required],
        bevestigWachtwoord: ['', Validators.required],
      },
      {
        validator: CustomValidators.passwordMatchValidator
      });

    this.registerForm.get('land').setValue('Nederland');
  }

  onSubmit() {
    if (!this.registerForm.invalid) {
      this.showSpinner = true;

      const voornaam = this.registerForm.get('voornaam').value;
      const achternaam = this.registerForm.get('achternaam').value;
      const straatnaam = this.registerForm.get('straatnaam').value;
      const huisnummer = this.registerForm.get('huisnummer').value;
      const postcode = this.registerForm.get('postcode').value;
      const plaats = this.registerForm.get('plaats').value;
      const land = this.registerForm.get('land').value;
      const telefoonnummer = this.registerForm.get('telefoonnummer').value;
      const email = this.registerForm.get('email').value;
      const password = this.registerForm.get('wachtwoord').value;

      const adres: Adres = { straatnaam, huisnummer, postcode, plaats, land };
      const klant: Klant = { voornaam, achternaam, telefoonnummer, email, adres };
      const gebruiker: Gebruiker = { email, password, klant };

      const subscription = this.authenticationService.register(gebruiker)
                                                    .subscribe(() => {
                                                      // tslint:disable-next-line:max-line-length
                                                      this.toasterService.info('Registratie succesvol! U wordt nu ingelogd.');
                                                      setTimeout(() => {
                                                        this.authenticationService.login(email, password).subscribe(() => {
                                                          this.router.navigate(['/login']);
                                                        });
                                                        this.showSpinner = false;
                                                      },         1000);
                                                      subscription.unsubscribe();
                                                    },         (err) => {
                                                      this.handleFailedRegistration();
                                                      throw err;
                                                    }
                                                    );
    }
  }

  private handleFailedRegistration() {
    this.showSpinner = false;
    this.registerForm.get('wachtwoord').setValue('');
    this.registerForm.get('wachtwoord').markAsUntouched();
    this.registerForm.get('bevestigWachtwoord').setValue('');
    this.registerForm.get('bevestigWachtwoord').markAsUntouched();
  }
}

import { Component, OnInit } from '@angular/core';
import { AuthenticationService } from '@services/authentication/authentication.service';
import { Router, ActivatedRoute } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent implements OnInit {

  loginForm: FormGroup;

  private returnUrl: string;

  showSpinner: boolean = false;

  constructor(private route: ActivatedRoute,
              private router: Router,
              private authenticationService: AuthenticationService,
              private formBuilder: FormBuilder,
              private toasterService: ToastrService) { }

  ngOnInit() {
    this.returnUrl = this.route.snapshot.queryParams['returnUrl'] || '/';
    this.loginForm = this.formBuilder.group({
      email: ['', Validators.required],
      wachtwoord: ['', Validators.required]
    });
    if (this.authenticationService.currentGebruikerValue) {
      if (this.authenticationService.currentGebruikerValue.token) {
        this.router.navigate(['']);
      }
    }
  }

  login() {
    if (this.loginForm.invalid) {
      return;
    }
    this.showSpinner = true;
    const subscription = this.authenticationService
      .login(this.loginForm.get('email').value, this.loginForm.get('wachtwoord').value)
      .subscribe((result) => {
        if (result) {
          if (this.returnUrl === '/') {
            this.returnUrl = this.getBaseUrlForRole();
          }
          this.showSpinner = false;
          this.router.navigateByUrl(this.returnUrl);

          if (subscription) {
            subscription.unsubscribe();
          }
        } else {
          this.toasterService.error('Foutieve gegevens, probeer opnieuw in te loggen.');
          this.showSpinner = false;
        }
      },         (err) => {
        this.handleFailedLogin();
        throw err;
      }
      );
  }

  /**
   * Get the base URL for a given role.
   *
   * @private
   * @returns {string}
   */
  private getBaseUrlForRole(): string {
    if (this.authenticationService.currentGebruikerValue) {
      if (this.authenticationService.currentGebruikerRoles.includes('Sales')) {
        return '/sales';
      }
      if (this.authenticationService.currentGebruikerRoles.includes('Magazijn')) {
        return '/warehouse';
      }
      if (this.authenticationService.currentGebruikerRoles.includes('Klant')) {
        return '/shop';
      }
    }
    return '/';
  }

  private handleFailedLogin() {
    this.showSpinner = false;
    this.loginForm.get('wachtwoord').setValue('');
    this.loginForm.get('wachtwoord').markAsUntouched();
  }

}

import { Component, OnInit, OnDestroy } from '@angular/core';
import { AuthenticationService } from '@services/authentication/authentication.service';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-register-button',
  templateUrl: './register-button.component.html',
  styleUrls: ['./register-button.component.scss']
})
export class RegisterButtonComponent implements OnInit, OnDestroy {

  private loginSubscription: Subscription;

  _loggedIn: boolean;
  showButtonSpinner: boolean = false;

  get loggedIn(): boolean {
    if (this.authenticationService.currentGebruikerValue) {
      this._loggedIn = this.authenticationService.currentGebruikerValue.token !== undefined;
    } else {
      this._loggedIn = false;
    }
    return this._loggedIn;
  }

  constructor(public authenticationService: AuthenticationService) { }

  ngOnInit() {
    this.loginSubscription = this.authenticationService
      .gebruiker$
      .subscribe((gebruiker) => {
        if (gebruiker.token) {
          this._loggedIn = true;
        } else {
          this._loggedIn = false;
        }
      });
  }

  ngOnDestroy(): void {
    if (this.loginSubscription) {
      this.loginSubscription.unsubscribe();
    }
  }

}

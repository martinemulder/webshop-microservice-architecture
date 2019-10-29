import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { of, BehaviorSubject, Observable, throwError, interval } from 'rxjs';
import { Gebruiker } from '@models/gebruiker';
import { ApiService } from '@services/api/api.service';
import { map, retryWhen, flatMap } from 'rxjs/operators';
import { ToastrService } from 'ngx-toastr';
import { JwtHelperService } from '@auth0/angular-jwt';
import { Adres } from '@models/adres';
import { Klant } from '@models/klant';

@Injectable({
  providedIn: 'root'
})
export class AuthenticationService {
  private gebruikerSubject: BehaviorSubject<Gebruiker> = new BehaviorSubject<Gebruiker>(new Gebruiker());

  public readonly gebruiker$: Observable<Gebruiker> = this.gebruikerSubject.asObservable();

  constructor(private router: Router,
              private apiService: ApiService<any>,
              private toasterService: ToastrService,
              private jwtHelper: JwtHelperService) {
    this.gebruikerSubject = new BehaviorSubject<Gebruiker>(JSON.parse(localStorage.getItem('currentUser')));
    this.checkTokenValidity();
  }

  public get currentGebruikerValue(): Gebruiker {
    return this.gebruikerSubject.value;
  }

  public get currentGebruikerRoles(): string[] {
    if (!this.gebruikerSubject.value) {
      return [];
    }

    try {
      const jwtMap = new Map(Object.entries(this.jwtHelper.decodeToken(this.gebruikerSubject.value.token)));
      if (jwtMap) {
        return jwtMap.get('http://schemas.microsoft.com/ws/2008/06/identity/claims/role') as string[];
      }
    } catch {
      return [];
    }
  }

  register(gebruiker: Gebruiker): Observable<any> {
    const email = gebruiker.email;
    const password = gebruiker.password;
    const klant = gebruiker.klant;
    return this.apiService.post<any>('account/register', { email, password, klant });
  }

  getCurrentUser(): Gebruiker {
    const retrievedObject = localStorage.getItem('currentUser');

    if (retrievedObject) {
      const gebruiker = JSON.parse(retrievedObject);
      return gebruiker;
    }
  }

  login(email: string, password: string): Observable<any> {
    return this.apiService.post<any>('account/login', { email, password })
      .pipe(
        map((response) => {
          if (response && response.token) {
            const responseToken = response.token;
            const gebruiker : Gebruiker = { email: '', password: '', token: response.token };
            localStorage.setItem('currentUser', JSON.stringify(gebruiker));
            this.gebruikerSubject.next(gebruiker);

            this.apiService.get<any>('account')
              .pipe(
                map((response) => {
                  if (response) {
                    const adres: Adres = { straatnaam: response.adres.straatnaam, huisnummer: response.adres.huisnummer,
                      plaats: response.adres.plaats, postcode: response.adres.postcode, land: response.adres.land };
                    const klant: Klant = { voornaam: response.voornaam, achternaam: response.achternaam,
                      telefoonnummer: response.telefoonnummer, email: response.email, adres };
                    const gebruiker: Gebruiker = { password: '', email: response.email, token: responseToken, klant };

                    localStorage.setItem('currentUser', JSON.stringify(gebruiker));
                    this.gebruikerSubject.next(gebruiker);
                    setTimeout(() => this.toasterService.info('U bent succesvol ingelogd!'), 500);
                  }
                })
              ).subscribe((response) => console.log(response));
          } else {
            return 'error';
          }
          return response;
        })
      );
  }

  logout(): void {
    if (!this.currentGebruikerValue) {
      return;
    }
    localStorage.removeItem('currentUser');
    this.router.navigate(['/']);
    setTimeout(() => {
      this.gebruikerSubject.next(null);
      this.toasterService.info('U bent succesvol uitgelogd!');
    },         500);
  }

  private checkTokenValidity(): void {
    if (!this.currentGebruikerValue) {
      return;
    }

    const token = this.currentGebruikerValue.token || '';
    try {
      const isInvalid = this.jwtHelper.isTokenExpired(token);
      if (isInvalid) {
        this.logout();
      }
    } catch (error) {
      this.logout();
    }
  }
}

export interface ITokenDto {
  token: string;
}

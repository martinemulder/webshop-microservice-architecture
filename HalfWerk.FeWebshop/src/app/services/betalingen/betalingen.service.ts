import { Injectable } from '@angular/core';
import { ApiService } from '@services/api/api.service';

@Injectable({
  providedIn: 'root'
})
export class BetalingenService {

  constructor(private apiService: ApiService<IBetaling>) { }

  registreerBetaling(factuurnummer: number, bedrag: number) {
    return this.apiService.post<any>('betaling', { factuurnummer, bedrag });
  }

}

export interface IBetaling {
  factuurnummer: number;
  bedrag: number;
}

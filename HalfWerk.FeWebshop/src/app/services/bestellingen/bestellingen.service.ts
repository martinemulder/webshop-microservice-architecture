import { Injectable } from '@angular/core';
import { BehaviorSubject, forkJoin, of, zip, merge, Subscription } from 'rxjs';
import { Bestelling } from '@models/bestelling';
import { ApiService } from '@services/api/api.service';
import { BestelStatus } from '@models/bestelStatus';
import { HttpErrorResponse } from '@angular/common/http';
import { UpdateBestelStatus } from '@models/UpdateBestelStatus';
import { ToastrService } from 'ngx-toastr';
import { map } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class BestellingenService {

  private subject: BehaviorSubject<Bestelling[]> = new BehaviorSubject<Bestelling[]>([]);

  public readonly bestellingen$ = this.subject.asObservable();

  constructor(private apiService: ApiService<Bestelling>, private toasterService: ToastrService) {
  }

  getBestelling(query: string = 'Geplaatst') {
    return this.apiService.get<Bestelling[]>(`bestelling?status=${query}`);
  }

  getNext() {
    return this.apiService.get<any>('bestelling/next');
  }

  updateBestelStatus(factuurnummer: number, newStatus: BestelStatus) {
    const req = new UpdateBestelStatus(newStatus);

    return this.apiService.patch<UpdateBestelStatus>(`bestelling/${factuurnummer}/status`, req)
      .pipe(
        map(
          () => {
            this.toasterService.success(`Bestelstatus van factuurnummer ${factuurnummer} gewijzigd naar ${newStatus}.`);
            const bestellingen = this.subject.value.filter(b => b.factuurnummer !== factuurnummer);
            this.subject.next(bestellingen);
          },
          (error: HttpErrorResponse) =>
            this.toasterService.error(`Er ging iets mis tijdens het updaten van de status (errorcode: ${error.status}).`)
        )
      );
  }

  loadAll() {
    const bestellingen = this.getBestelling();
    const bestellingenInAanbetaling = this.getBestelling('WachtenOpAanbetaling');
    const allBestellingen = forkJoin(bestellingen, bestellingenInAanbetaling);

    return allBestellingen
      .pipe(
        map((bestellingen: any[]) => {
          let mergedBestellingen: Bestelling[] = [].concat.apply([], bestellingen);
          mergedBestellingen = mergedBestellingen.sort(this.compareBestelling);

          if (!this.subject.value) {
            this.subject.next(mergedBestellingen);
          } else if (bestellingen && bestellingen.length > 0) {
            this.subject.next(mergedBestellingen);
          }
        })
      );
  }

  private compareBestelling(bestelling1: Bestelling, bestelling2: Bestelling): number {
    if (bestelling1.id > bestelling2.id) {
      return 1;
    }
    if (bestelling1.id < bestelling2.id) {
      return -1;
    }
    return 0;
  }
}

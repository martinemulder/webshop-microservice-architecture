import { Component, Input, ViewChild } from '@angular/core';
import { Bestelling } from '@models/bestelling';
import { MatTableDataSource } from '@angular/material';
import { BestellingenService } from '@services/bestellingen/bestellingen.service';
import { BestelStatus } from '@models/bestelStatus';
import { HttpErrorResponse } from '@angular/common/http';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-orders-list',
  templateUrl: './orders-list.component.html',
  styleUrls: ['./orders-list.component.scss']
})
export class OrdersListComponent {

  @Input() bestellingen: Bestelling[];
  dataSource: MatTableDataSource<Bestelling>;

  pageSizeOptions: number[] = [5, 10, 25, 100];
  displayedColumns: string[] = ['factuurnummer', 'klant', 'telefoon', 'besteldatum', 'totaalprijs', 'actie'];

  constructor(private bestellingenService: BestellingenService,
              private toasterService: ToastrService) { }

  ngOnInit() {
  }

  approveBestelling(factuurnummer: number) {
    const subscription = this.bestellingenService.updateBestelStatus(factuurnummer, BestelStatus.Goedgekeurd)
      .subscribe(
        () => {
          if (subscription) {
            subscription.unsubscribe();
          }
        },
        (error: HttpErrorResponse) => {
          // tslint:disable-next-line:max-line-length
          this.toasterService.error(`Er ging iets mis tijdens het updaten van de bestelstatus bij factuurnummer ${factuurnummer} (errorcode: ${error.status}).`);
        }
      );
  }

  pendingPayment(factuurnummer: number) {
    const subscription = this.bestellingenService.updateBestelStatus(factuurnummer, BestelStatus.WachtenOpAanbetaling)
      .subscribe(
        () => {
          if (subscription) {
            subscription.unsubscribe();
          }
        },
        (error: HttpErrorResponse) => {
          // tslint:disable-next-line:max-line-length
          this.toasterService.error(`Er ging iets mis tijdens het updaten van de bestelstatus bij factuurnummer ${factuurnummer} (errorcode: ${error.status}).`);
        }
      );
  }

  denyBestelling(factuurnummer: number) {
    const subscription = this.bestellingenService.updateBestelStatus(factuurnummer, BestelStatus.Afgekeurd)
      .subscribe(
        () => {
          if (subscription) {
            subscription.unsubscribe();
          }
        },
        (error: HttpErrorResponse) => {
          // tslint:disable-next-line:max-line-length
          this.toasterService.error(`Er ging iets mis tijdens het updaten van de bestelstatus bij factuurnummer ${factuurnummer} (errorcode: ${error.status}).`);
        }
      );
  }
}

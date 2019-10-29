import { Component } from '@angular/core';
import { Bestelling } from '@models/bestelling';
import { BestellingenService } from '@services/bestellingen/bestellingen.service';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-orders',
  templateUrl: './orders.component.html',
  styleUrls: ['./orders.component.scss']
})
export class OrdersComponent {

  loading: boolean = true;

  bestellingen: Bestelling[] = [];

  constructor(public bestellingenService: BestellingenService) { }

  ngOnInit() {
    const subscription = this.bestellingenService.loadAll().subscribe(() => {
      this.loading = false;
      subscription.unsubscribe();
    });
  }
}

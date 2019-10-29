import { Component, OnInit, HostListener } from '@angular/core';
import { Bestelling } from '@models/bestelling';
import { BestellingenService } from '@services/bestellingen/bestellingen.service';
import { Subscription } from 'rxjs';
import { BestelStatus } from '@models/bestelStatus';
import { Router } from '@angular/router';

@Component({
  selector: 'app-order',
  templateUrl: './order.component.html',
  styleUrls: ['./order.component.scss']
})
export class OrderComponent implements OnInit {

  printedFactuur = false;
  printedAdreslabel = false;
  isPrintingFactuur = false;
  isPrintingAdreslabel = false;

  displayedColumns: string[] = ['artikelafbeelding', 'artikelnaam', 'leverancierCode', 'aantal', 'prijsInclBtw'];

  @HostListener('window:beforeunload', ['$event'])
  beforeunloadHandler(event) {
    return false;
  }

  bestelling: Bestelling;
  subscription: Subscription;

  constructor(private bestellingService: BestellingenService,
              private router: Router) { }

  ngOnInit() {
    this.subscription = this.bestellingService.getNext()
      .subscribe((bestelling: Bestelling) => {
        this.bestelling = bestelling;
      });
  }

  ngOnDestroy() {
    if (this.subscription) {
      this.subscription.unsubscribe();
    }
  }

  getTotalCost(): number {
    return this.bestelling.factuurTotaalInclBtw;
  }

  getTotalCostExclBtw(): number {
    return this.bestelling.factuurTotaalExclBtw;
  }

  printFactuur() {
    this.isPrintingFactuur = true;
    setTimeout(() => {
      window.print();
      this.isPrintingFactuur = false;
    },         500);
    this.printedFactuur = true;
  }

  printAdreslabel() {
    this.isPrintingAdreslabel = true;
    setTimeout(() => {
      window.print();
      this.isPrintingAdreslabel = false;
    },         500);
    this.printedAdreslabel = true;
  }

  /**
   * Predicate function which checks if a user is allowed to leave the current page.
   *
   * @returns {boolean}
   */
  canClosePage(): boolean {
    return this.hasPrintedBothDocuments() || !this.bestelling;
  }

  hasPrintedBothDocuments() {
    return (this.printedFactuur === true && this.printedAdreslabel === true);
  }

  bestellingAfronden() {
    const subscription = this.bestellingService.updateBestelStatus(this.bestelling.factuurnummer, BestelStatus.Verzonden)
                          .subscribe(() => {
                            this.router.navigate(['/warehouse']);
                            if (subscription) {
                              subscription.unsubscribe();
                            }
                          });
  }

}

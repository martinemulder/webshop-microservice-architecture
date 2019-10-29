import { Component, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { BetalingenService, IBetaling } from '@services/betalingen/betalingen.service';
import { HttpErrorResponse } from '@angular/common/http';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-payment',
  templateUrl: './payment.component.html',
  styleUrls: ['./payment.component.scss']
})
export class PaymentComponent implements OnInit {

  @ViewChild('inputRef') inputRef;

  paymentForm: FormGroup;

  get betalingFromForm(): IBetaling {
    const factuurnummer = parseInt(this.paymentForm.get('factuurnummer').value, 10);
    const bedrag = this.convertToFloat(this.paymentForm.get('bedrag').value);

    return { factuurnummer, bedrag };
  }

  constructor(private formBuilder: FormBuilder,
              private betalingenService: BetalingenService,
              private toasterService: ToastrService) { }

  ngOnInit() {
    this.paymentForm = this.formBuilder.group({
      factuurnummer: ['', [Validators.required, Validators.pattern(/^\d+$/)]],
      bedrag: ['', [Validators.required, Validators.pattern(/^\d*(,\d{2})?$/)]],
    });
  }

  handleBetaling() {
    if (this.paymentForm.invalid) {
      return;
    }

    const betaling = this.betalingFromForm;
    this.betalingenService.registreerBetaling(betaling.factuurnummer, betaling.bedrag)
      .subscribe(
        () => {
          this.toasterService.success(`Betaling van factuurnummer ${betaling.factuurnummer} is verwerkt.`);
          this.clearForm();
        },
        (error: HttpErrorResponse) => {
          // tslint:disable-next-line:max-line-length
          this.toasterService.error(`Er ging iets mis tijdens het verwerken van de bestelling bij factuurnummer ${betaling.factuurnummer} (errorcode: ${error.status}).`);
        }
      );
  }

  private convertToFloat(input: string): number {
    return parseFloat(input.replace(/[,.]/g, (m) => {
      return m === ',' ? '.' : ',';
    }));
  }

  private clearForm() {
    this.paymentForm.reset();
  }
}

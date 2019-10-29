import { TestBed } from '@angular/core/testing';

import { BestellingenService } from './bestellingen.service';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { AppModule } from '@root/app.module';
import { BestelStatus } from '@models/bestelStatus';
import { UpdateBestelStatus } from '@models/UpdateBestelStatus';

describe('BestellingenService', () => {
  let target: BestellingenService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [
        AppModule,
        HttpClientTestingModule
      ]
    });

    target = TestBed.get(BestellingenService);
    httpMock = TestBed.get(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should be created', async () => {
    expect(target).toBeTruthy();
  });

  it('should update bestel status', () => {
    const factuurnummer = 42;
    const newStatus = BestelStatus.Goedgekeurd;

    target.updateBestelStatus(factuurnummer, newStatus).subscribe();

    const req = httpMock.expectOne(`/api/bestelling/${factuurnummer}/status`);
    expect(req.request.method).toEqual('PATCH');
    expect(req.request.body).toEqual(new UpdateBestelStatus(newStatus));
  });
});

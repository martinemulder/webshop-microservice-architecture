import { TestBed } from '@angular/core/testing';

import { BetalingenService } from './betalingen.service';
import { AppModule } from '@root/app.module';
import { HttpClientTestingModule } from '@angular/common/http/testing';

describe('BetalingenService', () => {
  beforeEach(() => TestBed.configureTestingModule({
    imports: [
      AppModule,
      HttpClientTestingModule
    ]
  }));

  it('should be created', () => {
    const service: BetalingenService = TestBed.get(BetalingenService);
    expect(service).toBeTruthy();
  });
});

import { TestBed } from '@angular/core/testing';
import { AppModule } from '@root/app.module';
import { HttpService, ApiService } from './api.service';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';

describe('HttpService', () => {
  let service: HttpService<any>;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [
        AppModule,
        HttpClientTestingModule
      ]
    });

    httpMock = TestBed.get(HttpTestingController);
    service = TestBed.get(HttpService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should have an initialized httpClient', () => {
    expect(service.http).not.toBe(null);
  });

  it('should have initialized httpOptions', () => {
    expect(service.httpOptions).not.toBe(null);
  });

  it('should make a GET request', () => {
    service.get<any>('').subscribe();
    const req = httpMock.expectOne('');
    expect(req.request.method).toBe('GET');
  });

  it('should make a POST request', () => {
    const body = {
      header: 'post'
    };

    service.post<any>('', body).subscribe();
    const req = httpMock.expectOne('');
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toBe(body);
  });

  it('should make a PUT request', () => {
    const body = {
      header: 'put'
    };

    service.put<any>('', body).subscribe();
    const req = httpMock.expectOne('');
    expect(req.request.method).toBe('PUT');
    expect(req.request.body).toBe(body);
  });

  it('should make a DELETE request', () => {
    service.delete<any>('', null).subscribe();
    const req = httpMock.expectOne('');
    expect(req.request.method).toBe('DELETE');
  });

  afterEach(() => {
    httpMock.verify();
  });
});

describe('ApiService', () => {
  let service: ApiService<any>;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [
        AppModule,
        HttpClientTestingModule
      ]
    });

    httpMock = TestBed.get(HttpTestingController);
    service = TestBed.get(ApiService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should make a GET request with a correct baseUrl', () => {
    service.get<any>('').subscribe();
    const req = httpMock.expectOne('/api/');
    expect(req.request.method).toBe('GET');
  });

  it('should make a POST request with a correct baseUrl', () => {
    const body = {
      header: 'post'
    };

    service.post<any>('', body).subscribe();
    const req = httpMock.expectOne('/api/');
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toBe(body);
  });

  it('should make a PATCH request with a correct baseUrl', () => {
    const body = {
      header: 'patch'
    };

    service.patch<any>('', body).subscribe();
    const req = httpMock.expectOne('/api/');
    expect(req.request.method).toBe('PATCH');
    expect(req.request.body).toBe(body);
  });

  it('should make a PUT request with a correct baseUrl', () => {
    const body = {
      header: 'put'
    };

    service.put<any>('', body).subscribe();
    const req = httpMock.expectOne('/api/');
    expect(req.request.method).toBe('PUT');
    expect(req.request.body).toBe(body);
  });

  it('should make a DELETE request with a correct baseUrl', () => {
    service.delete<any>('', null).subscribe();
    const req = httpMock.expectOne('/api/');
    expect(req.request.method).toBe('DELETE');
  });

  afterEach(() => {
    httpMock.verify();
  });
});

import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse, HttpHeaders } from '@angular/common/http';
import { Observable, throwError, interval, of } from 'rxjs';
import { retryWhen, flatMap } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class HttpService<T> {
  protected baseUrl = '';

  get httpOptions() {
    // let role = this.authorisationService.userRole;
    return {
      headers: new HttpHeaders({
        'Content-Type': 'application/json',
        Authorization: 'my-auth-token',
        // 'User-role': role ? role : 'none'
      }),
      withCredentials: true
    };
  }

  constructor(public http: HttpClient) { }

  /**
   * Make a a generic GET request.
   *
   * @template T
   * @param {string} path
   * @returns {Observable<T>}
   */
  get<T>(path: string): Observable<T> {
    return this.http.get<T>(`${this.baseUrl + path}`)
                    .pipe(http_retry());
  }

  /**
   * Make a generic POST request.
   * NOTE: Call 'subscribe()' on this method to initiate the request, without 'subscribe()', nothing happens.
   *
   * @template T
    * @param {T} body
   * @param {T} body
   * @returns {Observable<T>}
   */
  post<T>(path: string, body: T): Observable<T> {
    return this.http.post<T>(`${this.baseUrl + path}`, body, this.httpOptions);
  }

  /**
   * Make a generic PUT request.
   * NOTE: Call 'subscribe()' on this method to initiate the request, without 'subscribe()', nothing happens.
   *
   * @template T
   * @param {string} path
   * @param {T} body
   * @returns {Observable<T>}
   */
  put<T>(path: string, body: T): Observable<T> {
    return this.http.put<T>(`${this.baseUrl + path}`, body, this.httpOptions);
  }

    /**
   * Make a generic PATCH request.
   * NOTE: Call 'subscribe()' on this method to initiate the request, without 'subscribe()', nothing happens.
   *
   * @template T
   * @param {string} path
   * @param {T} body
   * @returns {Observable<T>}
   */
  patch<T>(path: string, body: T): Observable<T> {
    return this.http.patch<T>(`${this.baseUrl + path}`, body, this.httpOptions);
  }

  /**
   * Make a generic DELETE request.
   * NOTE: Call 'subscribe()' on this method to initiate the request, without 'subscribe()', nothing happens.
   *
   * @template T
   * @param {string} path
   * @param {T} body
   * @returns {Observable<T>}
   */
  delete<T>(path: string, body: T): Observable<T> {
    return this.http.delete<T>(`${this.baseUrl + path}`, this.httpOptions);
  }

  // A client-side or network error occurred. Handle it accordingly.
  protected handleError(error: HttpErrorResponse) {
    if (error.error instanceof ErrorEvent) {
      // A client-side or network error occurred. Handle it accordingly.
      return throwError(`Er ging iets mis tijdens de communicatie met de server: ${error.error}.
                          Check je internetinstellingen en probeer opnieuw.`);
    }
    // The backend returned an unsuccessful response code.
    // The response body may contain clues as to what went wrong,
    // console.error(
    //   `Backend returned code ${error.status}, ` +
    //   `body was: ${error.error}`);
    return throwError(`Er ging iets mis op de server: ${error.error}. Probeer later opnieuw.`);

  }
}

@Injectable()
export class ApiService<T> extends HttpService<T> {

  constructor(public http: HttpClient) {
    super(http);
    this.baseUrl = '/api/';
  }
}

/**
 * Generic function which handles retries on an Observable, can be used within a pipe.
 *
 * @export
 * @template T
 * @param {number} [maxRetry=5]
 * @param {number} [delayMs=2000]
 * @returns Observable<T>
 */
export function http_retry<T>(maxRetry: number = 2, delayMs: number = 2000) {
  return (src: Observable<T>) => {
    return src.pipe(retryWhen((_) => {
      // tslint:disable-next-line:max-line-length
      return interval(delayMs).pipe(flatMap(count => count === maxRetry ? throwError(`Max amount of retries reached: (${maxRetry}), perhaps the server is down?`) : of(count)));
    }));
  };
}

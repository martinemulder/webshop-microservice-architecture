import { ToastrService } from 'ngx-toastr';
import { ErrorHandler, Injectable, Injector, Inject } from '@angular/core';
import { HttpErrorResponse } from '@angular/common/http';
import { Router } from '@angular/router';

@Injectable()
export class ErrorsHandler extends ErrorHandler {

  constructor(@Inject(Injector) private readonly injector: Injector) {
    super();
  }

  handleError(error: any) {

    const router = this.injector.get(Router);

    if (error instanceof HttpErrorResponse) {
      // Server or connection error happened
      if (!navigator.onLine) {
        // Handle offline error
        this.toastrService.error('Let op! U heeft geen internetverbinding. Check uw instellingen en probeer daarna opnieuw.');
        // tslint:disable-next-line:no-else-after-return
      } else {
        // Handle Http Error (error.status === 403, 404...)
        if (error.status === 401) {
          return this.toastrService.warning('Geen toegang, probeer opnieuw in te loggen.',  null, {
            onActivateTick: true,
            timeOut: 5000,
            progressBar: true
          });
        }
        if (error.error) {
          return this.toastrService.error(`Error ${error.status}: ${error.error}`,  null, {
            onActivateTick: true,
            timeOut: 10000,
            progressBar: true
          });
        }
        return this.toastrService.error(`${error.message}`,  null, {
          onActivateTick: true,
          timeOut: 10000,
          progressBar: true
        });
      }
      // tslint:disable-next-line:no-else-after-return
    } else {
      // Handle Client Error (Angular Error, ReferenceError...)
      if (error.error) {
        return this.toastrService.error(`Error ${error.status}: ${error.error.message}`,  null, {
          onActivateTick: true,
          timeOut: 10000,
          progressBar: true
        });
      }
    }

    console.error(error);
    console.error('Error: ', JSON.stringify(error));
  }

  private get toastrService(): ToastrService {
    return this.injector.get(ToastrService);
  }
}

import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor
} from '@angular/common/http';
import { delay, finalize, Observable } from 'rxjs';
import { BussyService } from '../_services/bussy.service';

@Injectable()
export class LoadingInterceptor implements HttpInterceptor {

  constructor(private bussyService: BussyService) {}

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    this.bussyService.busy();
    return next.handle(request).pipe(
      finalize(() => {
        this.bussyService.idle();
      })
    );
  }
}

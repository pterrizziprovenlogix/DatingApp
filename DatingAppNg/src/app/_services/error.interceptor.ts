import { Injectable } from '@angular/core';
import { HttpInterceptor, HttpEvent, HttpHandler, HttpRequest, HttpErrorResponse, HTTP_INTERCEPTORS } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';

@Injectable()
export class ErrorInterceptor implements HttpInterceptor {
    intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        return next
            .handle(req)
            .pipe(
                catchError(error => {
                    if (error instanceof HttpErrorResponse) {
                        const serverError = error.error;
                        if (error.status === 401) {
                            return throwError(error.statusText);
                        }
                        if (error.status === 400 && serverError.errors) {
                            let modelStateErrors = '';
                            for (const key in serverError.errors) {
                                modelStateErrors += serverError.errors[key]["0"] + '\n';
                            }
                            return throwError(modelStateErrors);
                        }
                        return throwError(serverError || 'Server error');
                    }
                }
                //})
            )
        );
    }
}

export const ErrorInterceptorProvider = {
    provide: HTTP_INTERCEPTORS,
    useClass: ErrorInterceptor,
    multi: true
};

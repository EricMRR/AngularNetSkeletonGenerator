using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Generation.Soporte
{
    public class ServicioAngular
    {
        public Tabla tabla { get; set; }

        public string rutaModelos { get; set; }
        public string rutaApp { get; set; }
        public string rutaServicios { get; set; }
        public string apiURL { get; set; }

        public ServicioAngular(Tabla tabla, string rutaModelos, string rutaApp, string rutaServicios, string apiURL) {
            this.tabla = tabla;
            this.rutaModelos = rutaModelos;
            this.rutaApp = rutaApp;
            this.rutaServicios = rutaServicios;
            this.apiURL = apiURL;
        }

        private string rutaRelativa(string ruta) {
            return ruta.Replace(rutaApp, @"..\").Replace('\\','/');
        }

        public override string ToString()
        {
            return @"
import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpResponse, HttpErrorResponse } from '@angular/common/http';
import { Observable, Subject, forkJoin, throwError, map } from 'rxjs';
import { mergeMap, tap, catchError, retry } from 'rxjs/operators';

import { AuthGuardService } from './authguard.service';

import { GlobalConfiguration } from '../app.config';
import { PeticionAPI } from '../models/PeticionAPI';
import { " + tabla.name + @" } from '" + rutaRelativa(rutaModelos) + tabla.name + @"';

@Injectable({ providedIn: 'root' })
export class " + tabla.name + @"Service {
  constructor(protected http: HttpClient, public authGuardService: AuthGuardService) {
  }
  ENDPOINT_URL: string = GlobalConfiguration.API_LOCATION + ""/" + tabla.name + @""";

  get" + tabla.name + @"<" + tabla.name + @">(): Observable<" + tabla.name + @" | null> {
    return this.http.get<" + tabla.name + @">(this.ENDPOINT_URL, {""headers"":this.authGuardService.getHeaders(), ""observe"":""response""}).pipe(map(Response => { this.authGuardService.readHeaders(Response.headers); return Response.body;})).pipe(retry(3), catchError(this.handleError));
  }
  get" + tabla.name + @"Id<" + tabla.name + @">(Id: number): Observable<" + tabla.name + @" | null> {
    return this.http.get<" + tabla.name + @">(this.ENDPOINT_URL + ""/?Id="" + Id, {""headers"":this.authGuardService.getHeaders(), ""observe"":""response""}).pipe(map(Response => { this.authGuardService.readHeaders(Response.headers); return Response.body;})).pipe(retry(3), catchError(this.handleError));
  }
  filter" + tabla.name + @"Field<" + tabla.name + @">(filtro: " + tabla.name + @"): Observable<" + tabla.name + @" | null> {
    var peticion : PeticionAPI<" + tabla.name + @"> = { Data: filtro };
    return this.http.post<" + tabla.name + @">(this.ENDPOINT_URL + ""/FiltrarCampo/"", peticion, {""headers"":this.authGuardService.getHeaders(), ""observe"":""response""}).pipe(map(Response => { this.authGuardService.readHeaders(Response.headers); return Response.body;})).pipe(retry(3), catchError(this.handleError));
  }
  filter" + tabla.name + @"All<" + tabla.name + @">(filtros: string[]): Observable<" + tabla.name + @" | null> {
    var peticion : PeticionAPI<string[]> = { Data: filtros };
    return this.http.post<" + tabla.name + @">(this.ENDPOINT_URL + ""/FiltrarTodo/"", peticion, {""headers"":this.authGuardService.getHeaders(), ""observe"":""response""}).pipe(map(Response => { this.authGuardService.readHeaders(Response.headers); return Response.body;})).pipe(retry(3), catchError(this.handleError));
  }
  post" + tabla.name + @"<" + tabla.name + @">(filtro: " + tabla.name + @"): Observable<" + tabla.name + @" | null> {
    var peticion : PeticionAPI<" + tabla.name + @"> = { Data: filtro };
    return this.http.post<" + tabla.name + @">(this.ENDPOINT_URL, peticion, {""headers"":this.authGuardService.getHeaders(), ""observe"":""response""}).pipe(map(Response => { this.authGuardService.readHeaders(Response.headers); return Response.body;})).pipe(retry(3), catchError(this.handleError));
  }
  put" + tabla.name + @"<" + tabla.name + @">(filtro: " + tabla.name + @"): Observable<" + tabla.name + @" | null> {
    var peticion : PeticionAPI<" + tabla.name + @"> = { Data: filtro };
    return this.http.put<" + tabla.name + @">(this.ENDPOINT_URL, peticion, {""headers"":this.authGuardService.getHeaders(), ""observe"":""response""}).pipe(map(Response => { this.authGuardService.readHeaders(Response.headers); return Response.body;})).pipe(retry(3), catchError(this.handleError));
  }
  delete" + tabla.name + @"<" + tabla.name + @">(Id: number): Observable<" + tabla.name + @" | null> {
    return this.http.delete<" + tabla.name + @">(this.ENDPOINT_URL + ""/?Id="" + Id, {""headers"":this.authGuardService.getHeaders(), ""observe"":""response""}).pipe(map(Response => { this.authGuardService.readHeaders(Response.headers); return Response.body;})).pipe(retry(3), catchError(this.handleError));
  }

  private handleError(error: HttpErrorResponse) {
    let errorMessage = '';
    if (error.error instanceof ErrorEvent) {
      errorMessage = 'Error: ${error.error.message}';
    } else {
      errorMessage = 'Código de error: ${error.status}\nMensaje: ${error.message}';
    }
    return throwError(errorMessage);
  }
}
";
        }

    }
}

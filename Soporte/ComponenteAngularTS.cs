using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Generation.Soporte
{
    public class ComponenteAngularTS
    {
        private Tabla tabla;
        private List<CampoSistema> exclusiones;
        public ComponenteAngularTS(Tabla tabla, List<CampoSistema> exclusiones)
        {
            this.tabla = tabla;
            this.exclusiones = exclusiones;
        }

        private string name
        {
            get
            {
                return tabla.name.Substring(0, 1).ToUpper() + ((tabla.name != null && tabla.name.Length > 1) ? tabla.name.Substring(1, tabla.name.Length - 1) : "");
            }
        }

        private string _columnas() {
            StringBuilder res = new StringBuilder();

            foreach (Columna c in tabla.Columnas)
            {
                //exclusion de columnas
                if (ComponenteAngularHTML.excluir(c, exclusiones)) continue;

                if (res.Length > 0) res.Append(", ");
                res.Append("'" + c.COLUMN_NAME + "'");
            }

            return res.ToString();
        }

        private string serviciosForaneos() {
            StringBuilder res = new StringBuilder();
            foreach (LLaveForanea llf in tabla.Foraneas)
            {
                if (llf.PKTABLE_NAME.ToLower() == tabla.name.ToLower()) continue;
                res.Append(@"import { " + llf.PKTABLE_NAME + @"Service } from '../../services/" + llf.PKTABLE_NAME.ToLower() + @".service';
");
            }
            foreach (LLaveForanea llf in tabla.Foraneas)
            {
                if (llf.PKTABLE_NAME.ToLower() == tabla.name.ToLower()) continue;
                res.Append(@"import { " + llf.PKTABLE_NAME + @" } from '../../models/" + llf.PKTABLE_NAME + @"';
");
            }
            return res.ToString();
        }

        private string cargaCatalogos() { 
            StringBuilder res = new StringBuilder();

            foreach (LLaveForanea llf in tabla.Foraneas) res.Append(@"
  public catalogo" + llf.PKTABLE_NAME + @": " + llf.PKTABLE_NAME + @"[] = [];");

            res.Append(@"

  _refreshCatalogs(){");

            foreach (LLaveForanea llf in tabla.Foraneas) res.Append(@"
    this." + llf.PKTABLE_NAME.ToLower() + @"Service.get" + llf.PKTABLE_NAME + @"().subscribe(
      (response) => {
        this.catalogo" + llf.PKTABLE_NAME + @" = ((response as RespuestaAPI<any>).Data) as " + llf.PKTABLE_NAME + @"[];
      }, (error) => console.error('Error:', error)
    );");

            res.Append(@"
  }");

            return res.ToString();
        }

        private string serviciosInyectados() { 
            StringBuilder res = new StringBuilder();
            res.Append(@"private " + tabla.name.ToLower() + @"Service: " + tabla.name + @"Service, ");
            foreach (LLaveForanea llf in tabla.Foraneas)
            {
                if (llf.PKTABLE_NAME.ToLower() == tabla.name.ToLower()) continue;
                res.Append(@"private " + llf.PKTABLE_NAME.ToLower() + @"Service: " + llf.PKTABLE_NAME + @"Service, ");
            }
            string _res = res.ToString();
            return _res.Substring(0, _res.Length - 2);
        }

        public override string ToString()
        {
            return @"
import { Component, OnInit, OnDestroy, Input, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { Subscription, Observable } from 'rxjs';
import { MatTableModule } from '@angular/material/table';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatGridListModule } from '@angular/material/grid-list';
import { MatSelectModule } from '@angular/material/select';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatDividerModule } from '@angular/material/divider';
import { MatChipInputEvent, MatChipsModule } from '@angular/material/chips';
import { MatIconModule } from '@angular/material/icon';
import { COMMA, ENTER } from '@angular/cdk/keycodes';

import { RespuestaAPI } from '../../models/RespuestaAPI';
import { " + tabla.name + @" } from '../../models/" + tabla.name + @"';
import { " + tabla.name + @"Service } from '../../services/" + tabla.name.ToLower() + @".service';

" + serviciosForaneos() + @"

type __funcionAccion = () => void; //<TABLE>(filtro: TABLE) => Observable<TABLE>;

@Component({
  selector: '" + tabla.name.ToLower() + @"-component',
  templateUrl: './" + tabla.name.ToLower() + @".component.html',
  styleUrl: './" + tabla.name.ToLower() + @".component.scss',
  imports: [CommonModule, FormsModule, MatTableModule, MatPaginatorModule, MatCardModule, MatButtonModule, MatGridListModule, MatSelectModule, MatInputModule, MatFormFieldModule, MatCheckboxModule, MatDividerModule, MatChipsModule, MatIconModule],
})

export class " + name + @"Component implements OnInit, OnDestroy {
  constructor ("+ serviciosInyectados() + @") {
  }

  @Input() public editable: boolean = true;

  public _funcionAccion: __funcionAccion | null = null;
  public _seleccion: " + tabla.name + @" | null = null;
  public resultadosCompletos: " + tabla.name + @"[] = [];
  public resultadosCompletosFiltrados: " + tabla.name + @"[] = [];
  public dataSource: " + tabla.name + @"[] = [];
  public displayedColumns: string[] = [" + _columnas() + @"];
  public loadingData: boolean = false;
  public elementosPorPagina: number = 20;
  public paginaActual: number = 1;
  public busquedaEnServidor: boolean = true;

  ngOnInit() {
    this._refreshGrid();
    this._refreshCatalogs();
  }

  ngOnDestroy() {
    console.log(""ngOnDestroy " + tabla.name.ToLower() + @""");
  }

" + cargaCatalogos() + @"

  _refreshGrid() {
    if (this.busquedaEnServidor) {
      this." + tabla.name.ToLower() + @"Service.filter" + tabla.name + @"All(this.filtros).subscribe(
        (response) => {
          let r: " + tabla.name + @"[] = ((response as RespuestaAPI<any>).Data) as " + tabla.name + @"[];
          this.resultadosCompletos = r;
          this.resultadosCompletosFiltrados = r;
          this._cambiarPagina(0);
        }
        , (error) => console.error('Error:', error)
      );
    } else {
      this." + tabla.name.ToLower() + @"Service.get" + tabla.name + @"().subscribe( //este trae todo
        (response) => {
          let r: " + tabla.name + @"[] = ((response as RespuestaAPI<any>).Data) as " + tabla.name + @"[];
          this.resultadosCompletos = r;
          this.resultadosCompletosFiltrados = this._aplicarFiltrosLocal();
          this._cambiarPagina(0);
        }
        , (error) => console.error('Error:', error)
      );
    }
  }

  _cambiarPagina(paginaNueva: number = -1) {
    if (paginaNueva > -1) this.paginaActual = paginaNueva;
    this.dataSource = this.resultadosCompletosFiltrados.filter((item, index) => (index >= this.paginaActual * this.elementosPorPagina) && (index < (this.paginaActual + 1) * this.elementosPorPagina));
  }

  _agregar() {
    this._seleccion = {} as " + tabla.name + @";
    this._funcionAccion = this.__agregar;
  }

  __agregar() {
    this." + tabla.name.ToLower() + @"Service.post" + tabla.name + @"(this._seleccion).subscribe(
      (response) => {
        this._funcionAccion = null;
        this._refreshGrid();
      }
      , (error) => console.error('Error:', error)
    );
  }

  _seleccionar(elemento: " + tabla.name + @") {
    this._seleccion = elemento;
    this._funcionAccion = this.__actualizar;
  }

  __actualizar() {
    this." + tabla.name.ToLower() + @"Service.put" + tabla.name + @"(this._seleccion).subscribe(
      (response) => {
        this._funcionAccion = null;
        this._refreshGrid();
      }
      , (error) => console.error('Error:', error)
    );
  }

  _aceptar() {
    if (this._funcionAccion != null) this._funcionAccion();
    this._seleccion = null;
  }

  _cancelar() {
    this._seleccion = null;
    this._funcionAccion = null;
  }

  readonly separatorKeysCodes: number[] = [ENTER, COMMA];
  filtros: string[] = [];
  filtroActual: string = """";
  quitarFiltro(filtro: string) {
    let t: string[] = Array();
    for (var i = 0; i < this.filtros.length; i++) {
      if (this.filtros[i] != filtro) t[t.length] = this.filtros[i];
    }
    this.filtros = t;

    if (this.busquedaEnServidor) this._refreshGrid();
    else {
      this.resultadosCompletosFiltrados = this._aplicarFiltrosLocal();
      this._cambiarPagina(0);
    }
  }
  agregarFiltro(event: MatChipInputEvent) {
    let t: string[] = Array();
    for (var i = 0; i < this.filtros.length; i++) t[t.length] = this.filtros[i];
    t[t.length] = this.filtroActual.trim();
    this.filtroActual = """";
    this.filtros = t;

    if (this.busquedaEnServidor) this._refreshGrid();
    else {
      this.resultadosCompletosFiltrados = this._aplicarFiltrosLocal();
      this._cambiarPagina(0);
    }
  }

  _aplicarFiltrosLocal(): " + tabla.name + @"[] {
    let r: " + tabla.name + @"[] = [];
    for (var i = 0; i < this.resultadosCompletos.length; i++) {
      let _todos: boolean = true;
      for (var j = 0; j < this.filtros.length; j++) {
        let _alguno = false;
        for (var _key in this.resultadosCompletos[i]) {
          if (_key != null && _key != undefined) {
            if (("""" + (this.resultadosCompletos[i] as any)[_key]).toLowerCase().indexOf(this.filtros[j].toLocaleLowerCase()) >= 0) {
              _alguno = true;
              break;
            }
          }
        }
        if (_alguno == false) {
          _todos = false;
          break;
        }
      }
      if (_todos == true) r[r.length] = this.resultadosCompletos[i];
    }
    return r;
  }

}

";
        }
    }
}

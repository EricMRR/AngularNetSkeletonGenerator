using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Generation.Soporte
{
    public class ComponenteAngularHTML
    {
        private Tabla tabla;
        private TablaCatalogo[] catalogos;
        private List<CampoSistema> exclusiones;
        public ComponenteAngularHTML(Tabla tabla, TablaCatalogo[] catalogos, List<CampoSistema> exlusiones)
        {
            this.tabla = tabla;
            this.catalogos = catalogos;
            this.exclusiones = exlusiones;
        }

        public static bool excluir(Columna c, List<CampoSistema> exclusiones) {
            foreach (CampoSistema ex in exclusiones) {
                if (ex.Nombre.ToLower().Equals(c.COLUMN_NAME.ToLower())) return true;
            }
            return false;
        }

        public string listadoPrincipal()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(@"

        <mat-form-field *ngIf=""this._seleccion == null"" class=""example-chip-list"">
            <mat-label>Filtros</mat-label>
            <mat-chip-grid #chipGrid aria-label=""Selección de filtros"">
                @for (filtro of filtros; track $index) {
                <mat-chip-row (removed)=""quitarFiltro(filtro)"">
                    {{filtro}}
                    <button matChipRemove [attr.aria-label]=""'quitar ' + filtro"">
                        <mat-icon>cancel</mat-icon>
                    </button>
                </mat-chip-row>
                }
            </mat-chip-grid>
            <input name=""filtroActual"" placeholder=""Nuevo filtro..."" #filterInput [(ngModel)]=""filtroActual""
                [matChipInputFor]=""chipGrid"" [matChipInputSeparatorKeyCodes]=""separatorKeysCodes""
                (matChipInputTokenEnd)=""agregarFiltro($event)"" />
        </mat-form-field>

        <table mat-table *ngIf=""this._seleccion == null"" [dataSource]=""dataSource"" class=""mat-elevation-z8"">
");

            foreach (Columna c in tabla.Columnas) {
                //exclusion de columnas
                if (excluir(c, exclusiones)) continue;

                sb.Append(@"
            <ng-container matColumnDef=""" + c.COLUMN_NAME + @""">
                <th mat-header-cell *matHeaderCellDef>" + c.COLUMN_NAME + @"</th>
                <td mat-cell *matCellDef=""let " + tabla.name.ToLower() + @""">{{" + tabla.name.ToLower() + @"." + c.COLUMN_NAME + @"}}</td>
            </ng-container>
");
            }

            sb.Append(@"
            <tr mat-header-row *matHeaderRowDef=""displayedColumns""></tr>
            <tr mat-row *matRowDef=""let row; columns: displayedColumns;"" (click)=""this._seleccionar(row)""></tr>

        </table>
        <mat-paginator *ngIf=""this._seleccion == null""
                      [pageSize]=""this.elementosPorPagina""
                      [length]=""this.resultadosCompletosFiltrados.length""
                      (page)=""_cambiarPagina($event.pageIndex)""
                      aria-label=""Cambiar página"">
        </mat-paginator>
");
            return sb.ToString();
        }

        public string agregarPrincipal()
        {
            StringBuilder sb = new StringBuilder();
            int dtPicker = 0;
            sb.Append(@"

        <div *ngIf=""this._seleccion != null"" class=""example-form"">
");

            foreach (Columna columna in tabla.Columnas) {
                //exclusion de columnas
                if (excluir(columna, exclusiones)) continue;

                //necesito saber si es un catalogo... es decir si la columna tiene alguna regla de referencia
                LLaveForanea llf = null;
                foreach (LLaveForanea _llf in tabla.Foraneas) {
                    if (_llf.FKCOLUMN_NAME == columna.COLUMN_NAME)
                    {
                        llf = _llf;
                        break;
                    }
                }
                if (llf != null) {
                    string nombreDisplay = null;
                    foreach (TablaCatalogo _cat in catalogos) {
                        if (_cat.Tabla.name == llf.PKTABLE_NAME) {
                            nombreDisplay = _cat.Campo;
                            break;
                        }
                    }
                    if (nombreDisplay == null) throw new Exception("Error al obtener el nombre a desplegar.");

                    sb.Append(@"
            <mat-form-field class=""example-full-width"">
                <mat-label>" + columna.COLUMN_NAME + @"</mat-label>
                <mat-select [(ngModel)]=""this._seleccion." + columna.COLUMN_NAME + @""" [disabled]=""!this.editable"">
                    @for (elem of this.catalogo" + llf.PKTABLE_NAME + @"; track elem." + llf.PKCOLUMN_NAME + @") {
                        <mat-option [value]=""elem." + llf.PKCOLUMN_NAME + @""">{{elem." + nombreDisplay + @"}}</mat-option>
                    }
                </mat-select>
            </mat-form-field>
");
                    continue;
                }

                if (columna.Tipo.TipoNET == typeof(bool)) {
                    sb.Append(@"
            <mat-checkbox [(ngModel)]=""this._seleccion." + columna.COLUMN_NAME + @""" [disabled]=""!this.editable"">
              " + columna.COLUMN_NAME + @"
            </mat-checkbox>
");
                } else if (columna.Tipo.TipoNET == typeof(int) || columna.Tipo.TipoNET == typeof(double) || columna.Tipo.TipoNET == typeof(float) || columna.Tipo.TipoNET == typeof(decimal)) {
                    sb.Append(@"
            <mat-form-field class=""example-full-width"">
                <mat-label>" + columna.COLUMN_NAME + @"</mat-label>
                <input matInput type=""number"" placeholder=""" + columna.COLUMN_NAME + @""" [(ngModel)]=""this._seleccion." + columna.COLUMN_NAME + @""" [disabled]=""!this.editable"">
            </mat-form-field>
");
                } else if (columna.Tipo.TipoNET == typeof(DateOnly)) {
                    sb.Append(@"
            <mat-form-field class=""example-full-width"">
                <mat-label>" + columna.COLUMN_NAME + @"</mat-label>
                <input matInput type=""date"" placeholder=""" + columna.COLUMN_NAME + @""" [(ngModel)]=""this._seleccion." + columna.COLUMN_NAME + @""" [disabled]=""!this.editable"" [matDatepicker]=""picker"">
                <mat-hint>YYYY-MM-DD</mat-hint>
                <mat-datepicker-toggle matIconSuffix [for]=""dtPicker" + dtPicker + @"""></mat-datepicker-toggle>
                <mat-datepicker #dtPicker" + dtPicker + @"></mat-datepicker>
            </mat-form-field>
");
                    dtPicker++;
                } else if (columna.Tipo.TipoNET == typeof(DateTime)) {
                    sb.Append(@"
            <mat-form-field class=""example-full-width"">
                <mat-label>" + columna.COLUMN_NAME + @"</mat-label>
                <input matInput type=""datetime-local"" placeholder=""" + columna.COLUMN_NAME + @""" [(ngModel)]=""this._seleccion." + columna.COLUMN_NAME + @""" [disabled]=""!this.editable"">
            </mat-form-field>
");
                } else {
                    sb.Append(@"
            <mat-form-field class=""example-full-width"">
                <mat-label>" + columna.COLUMN_NAME + @"</mat-label>
                <input matInput placeholder=""" + columna.COLUMN_NAME + @""" [(ngModel)]=""this._seleccion." + columna.COLUMN_NAME + @""" [disabled]=""!this.editable"">
            </mat-form-field>
");
                }
            }

            sb.Append(@"
        </div>

");
            return sb.ToString();
        }

        public override string ToString()
        {
            StringBuilder res = new StringBuilder();

            res.Append(@"
<mat-card appearance=""outlined"">
    <mat-card-header>
        <mat-card-title>" + tabla.name + @"</mat-card-title>
        <mat-card-subtitle>Descripci&oacute;n del cat&aacute;logo</mat-card-subtitle>
    </mat-card-header>
    <mat-card-content>

");
            res.Append(listadoPrincipal());
            res.Append(agregarPrincipal());
            res.Append(@"

    </mat-card-content>
    <mat-card-actions>
        <button mat-button *ngIf=""this._seleccion == null"" (click)=""this._agregar()"">Agregar</button>

        <button mat-flat-button *ngIf=""this._seleccion != null && this.editable"" (click)=""this._aceptar()"">Aceptar</button>
        <button mat-button *ngIf=""this._seleccion != null"" (click)=""this._cancelar()"">Cancelar</button>
    </mat-card-actions>
    <mat-card-footer></mat-card-footer>
</mat-card>
");
            return res.ToString();
        }
    }
}

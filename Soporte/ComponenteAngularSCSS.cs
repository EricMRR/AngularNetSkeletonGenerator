using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Generation.Soporte
{
    public class ComponenteAngularSCSS
    {
        private Tabla tabla;
        public ComponenteAngularSCSS(Tabla tabla)
        {
            this.tabla = tabla;
        }

        public override string ToString()
        {
            return @"
.mat-mdc-row .mat-mdc-cell {
  border-bottom: 1px solid transparent;
  border-top: 1px solid transparent;
  cursor: pointer;
}

.mat-mdc-row:hover .mat-mdc-cell {
  border-color: currentColor;
}

.example-form {
  min-width: 150px;
  max-width: 500px;
  width: 100%;
}

.example-full-width {
  width: 100%;
}
.example-chip-list{
  width: 50%;
}
";
        }
    }
}

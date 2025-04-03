using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Generation.Soporte
{
    public class ComponenteAngularSPEC
    {
        private Tabla tabla;
        public ComponenteAngularSPEC(Tabla tabla)
        {
            this.tabla = tabla;
        }

        private string name {
            get
            {
                return tabla.name.Substring(0, 1).ToUpper() + ((tabla.name != null && tabla.name.Length > 1) ? tabla.name.Substring(1, tabla.name.Length - 1) : "");
            }
        }

        public override string ToString()
        {
            return @"
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { TestBed } from '@angular/core/testing';

import { " + name + @"Component } from './" + tabla.name.ToLower() + @".component';

describe('" + name + @"Component', () => {
  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [" + name + @"Component],
    }).compileComponents();
  });
});

";
        }
    }
}

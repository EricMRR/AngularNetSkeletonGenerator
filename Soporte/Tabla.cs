using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Generation.Soporte
{
    public class Tabla
    {
        public string name { get; set; }

        [NotMapped]
        public string Namespace { get; set; }

        private Columna[] columnas = null;
        private Indice[] indices = null;
        private LLaveForanea[] foraneas = null;
        private string[] exclusionesInterfaz = null;

        public Indice getIndice(Columna columna) {
            foreach (Indice i in indices) {
                if (i.ColumnName.Equals(columna.COLUMN_NAME)) return i;
            }
            return null;
        }

        [NotMapped]
        public Columna[] Columnas {
            get {
                if (columnas == null) columnas = new Columna[0];
                return columnas;
            }
            set { columnas = value; }
        }

        [NotMapped]
        public Indice[] Indices {
            get {
                if (indices == null) indices = new Indice[0];
                return indices;
            }
            set { indices = value; }
        }

        [NotMapped]
        public LLaveForanea[] Foraneas {
            get {
                if (foraneas == null) foraneas = new LLaveForanea[0];
                return foraneas;
            }
            set { foraneas = value; }
        }

        [NotMapped]
        public string[] ExclusionesInterfaz
        {
            get
            {
                if (exclusionesInterfaz == null) exclusionesInterfaz = new string[0];
                return exclusionesInterfaz;
            }
            set { exclusionesInterfaz = value; }
        }

        public static string sqlTablas() {
            return "SELECT * FROM SYSOBJECTS WHERE xtype = 'U'";
        }

        override public string ToString() {
            StringBuilder sb = new StringBuilder();
            sb.Append(
@"
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace " + Namespace + @"
{
    [Table(""" + name + @""")]
    public class " + name + @"
    {
"
            );

            foreach (Columna c in Columnas) sb.Append(
@"
        " + c.ToString() 
            );

            sb.Append(@"

    }
}
"
            );

            return sb.ToString();
        }

        public string ToStringAngular() {
            string propiedades = "";
            foreach (Columna c in columnas) propiedades += c.ToStringAngular();
            return @"
export interface " + name + @" {
/*PROPIEDADES*/
}
".Replace("/*PROPIEDADES*/", propiedades);            
        }

        public override bool Equals(object? obj)
        {
            if (obj == null) return false;
            if (this.GetType() != obj.GetType()) return false;
            Tabla t = (Tabla)obj;
            if (name != t.name) return false;
            return true;
        }
    }
}

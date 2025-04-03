using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Generation.Soporte
{
    public class LLaveForanea
    {
        public string PKTABLE_NAME { get; set; }
        public string PKCOLUMN_NAME { get; set; }
        public string FKTABLE_NAME { get; set; }
        public string FKCOLUMN_NAME { get; set; }

        [NotMapped]
        public TablaCatalogo Catalogo { get; set; }

        public static LLaveForanea getLlaves(LLaveForanea[] llaves, Columna columna)
        {
            foreach (LLaveForanea llave in llaves) {
                if (llave.FKCOLUMN_NAME.Equals(columna.COLUMN_NAME)) return llave;
            }
            return null;
        }

        public static string sqlLlaves(string tabla)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("EXEC sp_fkeys @fktable_name = '" + tabla + "'");
            return sb.ToString();
        }

        public override bool Equals(object? obj)
        {
            if (obj == null) return false;
            if(this.GetType() != obj.GetType()) return false;
            LLaveForanea llf = (LLaveForanea)obj;
            if (PKTABLE_NAME != llf.PKTABLE_NAME) return false;
            if (PKCOLUMN_NAME != llf.PKCOLUMN_NAME) return false;
            if (FKTABLE_NAME != llf.FKTABLE_NAME) return false;
            if (FKCOLUMN_NAME != llf.FKCOLUMN_NAME) return false;
            return true;
        }
    }

    public class TablaCatalogo { 
        public TablaCatalogo(Tabla tabla, LLaveForanea llave, string campo) {
            this.Tabla = tabla;
            this.Llave = llave;
            this.Campo = campo;
        }

        public Tabla Tabla { get; set; }
        public LLaveForanea Llave { get; set; }
        public string Campo { get; set; }

        public override bool Equals(object? obj)
        {
            if (obj == null) return false;
            if (this.GetType() != obj.GetType()) return false;
            TablaCatalogo tc = (TablaCatalogo)obj;
            if (Tabla != tc.Tabla) return false;
            if (Campo != tc.Campo) return false;
            return true;
        }
    }
}

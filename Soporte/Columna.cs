using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace Generation.Soporte
{
    public class Columna
    {
        public string COLUMN_NAME { get; set; }
        public string DATA_TYPE { get; set; }

        [NotMapped]
        public MapeoTipo Tipo
        {
            get {
                foreach (MapeoTipo t in MapeoTipo.Mapeos)
                {
                    if (t.TipoSQL.Equals(DATA_TYPE)) return t;
                }
                return null;
            }
        }

        private Indice[] indices;

        [NotMapped]
        public Indice[] Indices {
            get {
                if(indices == null) indices = new Indice[0];
                return indices;
            }
            set { indices = value; }
        }

        [NotMapped]
        public LLaveForanea Foranea { get; set; }

        public static string sqlColumnas(string tabla) {
            return "SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = N'" + tabla + "'";
        }

        public override string ToString()
        {
            if (Tipo != null)
            {
                string res = "";
                foreach (Indice i in indices) {
                    if (i.is_primary_key) res += "[Key] ";
                }

                return res + "public " + Tipo.TipoNET.Name + "? " + COLUMN_NAME + " { get; set; } ";
            }
            return "/*ERROR \"" + COLUMN_NAME + "\" SIN TIPO DE MAPEO \"" + DATA_TYPE + "\"*/";
        }

        public string ToStringAngular() {
            if (Tipo != null)
            {
                return "    " + COLUMN_NAME + ": " + Tipo.TipoAngular + @" | null;
";
            }
            return "/*ERROR \"" + COLUMN_NAME + "\" SIN TIPO DE MAPEO \"" + DATA_TYPE + "\"*/";
        }
    }
}

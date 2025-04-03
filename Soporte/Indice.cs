using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Generation.Soporte
{
    public class Indice
    {
        public string ColumnName { get; set; }
        public string name { get; set; }
        public bool is_unique { get; set; }
        public bool is_primary_key { get; set; }

        public static Indice[] getIndices(Indice[] indices, Columna columna) {
            List<Indice> res = new List<Indice>();
            foreach (Indice indice in indices) {
                if (indice.ColumnName.Equals(columna.COLUMN_NAME)) res.Add(indice);
            }
            return res.ToArray();
        }

        public static string sqlIndexes(string tabla) {
            string sqlIndexes = @"SELECT 
     TableName = t.name,
     IndexName = ind.name,
     IndexId = ind.index_id,
     ColumnId = ic.index_column_id,
     ColumnName = col.name
     ,ind.*
     --,ic.*
     --,col.* 
FROM 
     sys.indexes ind 
INNER JOIN 
     sys.index_columns ic ON  ind.object_id = ic.object_id and ind.index_id = ic.index_id 
INNER JOIN 
     sys.columns col ON ic.object_id = col.object_id and ic.column_id = col.column_id 
INNER JOIN 
     sys.tables t ON ind.object_id = t.object_id
WHERE t.name = '" + tabla + @"'";
            return sqlIndexes;
        }

    }
}

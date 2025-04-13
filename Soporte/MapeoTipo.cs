using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Generation.Soporte
{
    public class MapeoTipo
    {
        public string TipoSQL { get; set; }
        public string TipoAngular { get; set; }
        public Type TipoNET { get; set; }
        public string DefaultNET { get; set; }

        private MapeoTipo(string TipoSQL, string TipoAngular, Type TipoNET, string DefaultNET) {
            this.TipoSQL = TipoSQL;
            this.TipoAngular = TipoAngular;
            this.TipoNET = TipoNET;
            this.DefaultNET = DefaultNET;
        }

        private static MapeoTipo[] mapeos = {
            new MapeoTipo("int", "number", typeof(int), "0")
            , new MapeoTipo("nvarchar", "string", typeof(string), "null")
            , new MapeoTipo("bit", "boolean", typeof(bool), "false")
            , new MapeoTipo("datetime", "Date", typeof(DateTime), "DateTime.Now")
            , new MapeoTipo("datetime2", "Date", typeof(DateTime), "DateTime.Now")
            , new MapeoTipo("date", "Date", typeof(DateOnly), "DateOnly.FromDateTime(DateTime.Now)")
            , new MapeoTipo("decimal", "number", typeof(decimal), "0")
            , new MapeoTipo("float", "number", typeof(decimal), "0")
            , new MapeoTipo("numeric", "number", typeof(decimal), "0")
            , new MapeoTipo("uniqueidentifier", "string", typeof(Guid), "null")
            , new MapeoTipo("varbinary", "string", typeof(byte[]), "null")
        };

        public static MapeoTipo[] Mapeos { get { return mapeos; } }
    }
}

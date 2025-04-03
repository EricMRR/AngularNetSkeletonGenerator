using Azure.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Generation.Soporte
{
    public class CampoSistema
    {
        public string Instancia { get; set; }
        public string Nombre { get; set; }
        private string defaultInsert;
        public string DefaultInsert { get { return defaultInsert == null ? "" : defaultInsert; } set { defaultInsert = value; } }
        public string defaultUpdate;
        public string DefaultUpdate { get { return defaultUpdate == null ? "" : defaultUpdate; } set { defaultUpdate = value; } }
        public string defaultWhere;
        public string DefaultWhere { get { return defaultWhere == null ? "" : defaultWhere; } set { defaultWhere = value; } }

        public CampoSistema(string instancia, string nombre, string defaultInsert, string defaultUpdate, string defaultWhere)
        {
            Instancia = instancia;
            Nombre = nombre;
            DefaultInsert = defaultInsert;
            DefaultUpdate = defaultUpdate;
            DefaultWhere = defaultWhere;
        }

        public CampoSistema(string instancia, string nombre) : this(instancia, nombre, null, null, null)
        {
        }
    }
}

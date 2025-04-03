using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Generation.Soporte
{
    public class GeneradorContexto
    {
        public string namespaceTransporte { get; set; }
        public string namespaceAccesoDatos { get; set; }
        public List<Tabla> tablas { get; set; }

        public GeneradorContexto(string namespaceTransporte, string namespaceAccesoDatos)
        {
            this.namespaceTransporte = namespaceTransporte;
            this.namespaceAccesoDatos = namespaceAccesoDatos;
            tablas = new List<Tabla>();
        }

        public override string ToString()
        {
            string _tablas = "";
            foreach (Tabla t in tablas) _tablas += @"        public DbSet<" + t.name + @"> " + t.name + @"s { get; set; }
";

            return @"
using " + namespaceTransporte + @";
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace " + namespaceAccesoDatos + @"
{
    public class Contexto : DbContext
    {
        public Contexto(DbContextOptions<Contexto> options) : base(options) { }
        
/*CONJUNTOS*/
    }
}
".Replace("/*CONJUNTOS*/", _tablas);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Generation.Soporte
{
    public class Controlador
    {
        private Tabla tabla;

        public string nombre { get; set; }
        public string namespaceSolucion { get; set; }
        public string namespaceAPI { get; set; }
        public string namespaceTransporte { get; set; }
        public string namespaceWrapperAPI { get; set; }
        public string namespaceAccesoDatos { get; set; }
        public List<CampoSistema> excepciones { get; set; }

        public Controlador(Tabla tabla, string namespaceSolucion, string namespaceAPI, string namespaceTransporte, string namespaceWrapperAPI, string namespaceAccesoDatos, List<CampoSistema> excepciones)
        {
            this.tabla = tabla;
            this.nombre = tabla.name;
            this.namespaceSolucion = namespaceSolucion;
            this.namespaceAPI = namespaceAPI;
            this.namespaceTransporte = namespaceTransporte;
            this.namespaceWrapperAPI = namespaceWrapperAPI;
            this.namespaceAccesoDatos = namespaceAccesoDatos;
            this.excepciones = excepciones;
        }

        //hay que generar cadenas de texto que signifiquen los ids que se estan actualizando o borrando o lo que sea

        private static Columna GetColumna(Columna[] columnas, Indice indice) {
            foreach (Columna c in columnas) {
                if (c.COLUMN_NAME.Equals(indice.ColumnName)) return c;
            }
            return null;
        }

        private string firmaMetodoGetAll() {
            StringBuilder sb = new StringBuilder();
            foreach(Indice indice in tabla.Indices) {
                Columna columna = GetColumna(tabla.Columnas, indice);
                sb.Append(columna.Tipo.TipoNET.Name + " " + columna.COLUMN_NAME + ", ");
            }
            String res = sb.ToString();
            return res.Length > 0 ? res.Substring(0, res.Length - 2) : res;
        }

        private string whereMetodoGetAll(bool metodo) {
            StringBuilder sb = new StringBuilder();
            sb.Append("x => ");
            foreach (Indice indice in tabla.Indices) {
                Columna columna = GetColumna(tabla.Columnas, indice);
                sb.Append("x." + columna.COLUMN_NAME + " == " + (metodo ? "" : "x.") + columna.COLUMN_NAME + " && ");
            }

            //vamos a meter aqui los campos de sistema que podrian estar filtrando
            foreach (CampoSistema campo in excepciones)
            {
                if (campo.DefaultWhere.Length > 0) sb.Append("x." + campo.Nombre + " == " + campo.DefaultWhere + " && ");
            }

            String res = sb.ToString();
            return (res.Length > 0) ? res.Substring(0, res.Length - 4) : res;
        }

        private string whereMetodoDelete() {
            StringBuilder sb = new StringBuilder();
            foreach (Indice indice in tabla.Indices) {
                Columna columna = GetColumna(tabla.Columnas, indice);
                sb.Append(columna.COLUMN_NAME + " = " + columna.COLUMN_NAME + ", ");
            }
            String res = sb.ToString();
            return (res.Length > 0) ? res.Substring(0, res.Length - 2) : res;
        }

        private string whereMetodoUpdate() {
            StringBuilder sb = new StringBuilder();
            foreach (Columna columna in tabla.Columnas) {
                sb.Append(columna.COLUMN_NAME + " = peticionAPI.Data." + columna.COLUMN_NAME + ", ");
            }
            String res = sb.ToString();
            return (res.Length > 0) ? res.Substring(0, res.Length - 2) : res;
        }

        private string inicializacionSistemaInsert()
        { 
            StringBuilder sb = new StringBuilder();
            foreach (CampoSistema campo in excepciones)
            {
                if(campo.DefaultInsert.Length > 0) sb.Append(@"
                " + campo.Instancia + "." + campo.Nombre + " = " + campo.DefaultInsert + ";");
            }
            return sb.ToString();
        }
        private string inicializacionSistemaUpdate()
        {
            StringBuilder sb = new StringBuilder();
            foreach (CampoSistema campo in excepciones)
            {
                if (campo.DefaultUpdate.Length > 0) sb.Append(@"
                " + campo.Instancia + "." + campo.Nombre + " = " + campo.DefaultUpdate + ";");
            }
            return sb.ToString();
        }
        /*private string inicializacionSistemaWhere()
        {
            StringBuilder sb = new StringBuilder();
            foreach (CampoSistema campo in excepciones)
            {
                if (campo.DefaultWhere.Length > 0) sb.Append(@"
                " + campo.Instancia + "." + campo.Nombre + " = " + campo.DefaultWhere + ";");
            }
            return sb.ToString();
        }*/

        override public string ToString() {
            return @"
using " + namespaceWrapperAPI + @";
using " + namespaceTransporte + @";
using " + namespaceAccesoDatos + @";
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using Microsoft.AspNetCore.Cors;
using System.Text.Json.Serialization;

using " + namespaceSolucion + @".Authentication;
using " + namespaceSolucion + @".Utils;

namespace " + namespaceAPI + @"
{
    [ApiController]
    [Route(""[controller]"")]
    [JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.Unspecified)]
    public class " + nombre + @"Controller : ControllerBase
    {
        private readonly ILogger<" + nombre + @"Controller> logger;
        private readonly Contexto contexto;

        public " + nombre + @"Controller(ILogger<" + nombre  + @"Controller> logger, Contexto contexto)
        {
            this.logger = logger;
            this.contexto = contexto;
        }

        [EnableCors]
        [HttpPost]
        [NoPropertyNamingPolicy]
        public RespuestaAPI<" + nombre + @"> Agregar(PeticionAPI<" + nombre + @"> peticionAPI) {
            RespuestaAPI<" + nombre  + @"> r = new RespuestaAPI<" + nombre + @">();
            try
            {
                new Jwt(this, contexto).VerificarToken();

                " + nombre + @" d = new " + nombre + @"() { " + whereMetodoUpdate() + @" };

                " + inicializacionSistemaInsert() + @"    

                contexto." + nombre + @"s.Add(d);
                contexto.SaveChanges();

                r.Data = d;
                r.Code = 0;
                r.Mensaje = ""OK"";
            }
            catch (Exception ee)
            {
                r.Code = 100;
                r.Mensaje = ee.Message;
            }
            return r;
        }

        [EnableCors]
        [HttpPut]
        [NoPropertyNamingPolicy]
        public RespuestaAPI<" + nombre + @"> Actualizar(PeticionAPI<" + nombre + @"> peticionAPI)
        {
            RespuestaAPI<" + nombre + @"> r = new RespuestaAPI<" + nombre + @">();
            try
            {
                new Jwt(this, contexto).VerificarToken();

                " + nombre + @" d = new " + nombre + @"() { " + whereMetodoUpdate() + @" };

                " + inicializacionSistemaUpdate() + @"

                contexto." + nombre + @"s.Update(d);
                contexto.SaveChanges();

                r.Data = d;
                r.Code = 0;
                r.Mensaje = ""OK"";
            }
            catch (Exception ee)
            {
                r.Code = 100;
                r.Mensaje = ee.Message;
            }
            return r;
        }

        [EnableCors]
        [HttpDelete(""{Id}"")]
        [NoPropertyNamingPolicy]
        public RespuestaAPI<bool> Borrar(" + firmaMetodoGetAll() + @")
        {
            RespuestaAPI<bool> r = new RespuestaAPI<bool>();
            try
            {
                new Jwt(this, contexto).VerificarToken();

                " + nombre + @" d = new " + nombre + @"() { " + whereMetodoDelete() + @" };
                contexto." + nombre + @"s.Remove(d);
                contexto.SaveChanges();

                r.Data = true;
                r.Code = 0;
                r.Mensaje = ""OK"";
            }
            catch (Exception ee)
            {
                r.Code = 100;
                r.Mensaje = ee.Message;
            }
            return r;
        }

        [EnableCors]
        [HttpGet(""{Id}"")]
        [NoPropertyNamingPolicy]
        public RespuestaAPI<" + nombre + @"> Obtener(" + firmaMetodoGetAll() + @")
        {
            RespuestaAPI<" + nombre + @"> r = new RespuestaAPI<" + nombre + @">();
            try
            {
                new Jwt(this, contexto).VerificarToken();

                r.Data = contexto." + nombre + @"s.Where(" + whereMetodoGetAll(true) + @").FirstOrDefault();
                r.Code = 0;
                r.Mensaje = ""OK"";
            }
            catch (Exception ee)
            {
                r.Code = 100;
                r.Mensaje = ee.Message;
            }
            return r;
        }

        [EnableCors]
        [HttpGet]
        [NoPropertyNamingPolicy]
        public RespuestaAPI<IEnumerable<" + nombre + @">> Listar()
        {
            RespuestaAPI<IEnumerable<" + nombre + @">> r = new RespuestaAPI<IEnumerable<" + nombre + @">>();
            try
            {
                new Jwt(this, contexto).VerificarToken();

                r.Data = contexto." + nombre + @"s.Where(" + whereMetodoGetAll(false) + @").ToList();
                r.Code = 0;
                r.Mensaje = ""OK"";
            }
            catch (Exception ee)
            {
                r.Code = 100;
                r.Mensaje = ee.Message;
            }
            return r;
        }

        [EnableCors]
        [HttpPost(""FiltrarCampo"")]
        [NoPropertyNamingPolicy]
        public RespuestaAPI<IEnumerable<" + nombre + @">> Listar(PeticionAPI<" + nombre + @"> peticionAPI) {
            RespuestaAPI<IEnumerable<" + nombre + @">> r = new RespuestaAPI<IEnumerable<" + nombre + @">>();
            try
            {
                new Jwt(this, contexto).VerificarToken();

                " + nombre + @" d = new " + nombre + @"() { " + whereMetodoUpdate() + @" };

                //TODO: realizar filtrado para cada uno de los campos que venga en la peticion

                r.Data = contexto." + nombre + @"s.Where(" + whereMetodoGetAll(false) + @").ToList();
                r.Code = 0;
                r.Mensaje = ""OK"";
            }
            catch (Exception ee)
            {
                r.Code = 100;
                r.Mensaje = ee.Message;
            }
            return r;
        }

        [EnableCors]
        [HttpPost(""FiltrarTodo"")]
        [NoPropertyNamingPolicy]
        public RespuestaAPI<IEnumerable<" + nombre + @">> Listar(PeticionAPI<IEnumerable<string>> peticionAPI) {
            RespuestaAPI<IEnumerable<" + nombre + @">> r = new RespuestaAPI<IEnumerable<" + nombre + @">>();
            try
            {
                new Jwt(this, contexto).VerificarToken();

                string query = QueryGeneration.searchAllColumnsQuery<" + nombre + @">(peticionAPI.Data);
                List<string> parametros = new List<string>(peticionAPI.Data);

                r.Data = contexto." + nombre + @"s.FromSqlRaw(query, parametros.ToArray()).Where(" + whereMetodoGetAll(false) + @").ToList();
                r.Code = 0;
                r.Mensaje = ""OK"";
            }
            catch (Exception ee)
            {
                r.Code = 100;
                r.Mensaje = ee.Message;
            }
            return r;
        }

    }
}
";
        }
    }
}

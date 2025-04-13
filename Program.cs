using Generation.Soporte;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Globalization;
using static System.Net.Mime.MediaTypeNames;

//string cadenaConexion = @"Server=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Proyectos\Resultado\backend\pepDAL\Persistencia.mdf;Integrated Security=True;Connect Timeout=30;";
string cadenaConexion = @"Data Source=ericmrr.pp.ua;Database=ericmr_chatito;Integrated Security=false;User ID=ericmr_chatito;Password=ninguna232323.;TrustServerCertificate=True;MultipleActiveResultSets=false;";
string rutaDestinoClases = @"C:\Proyectos\Resultado\backend\model\Transporte\";
string rutaDestinoControladores = @"C:\Proyectos\Resultado\backend\api\Controllers\";
string rutaDestinoContexto = @"C:\Proyectos\Resultado\backend\dal\";

string namespaceSolucion = @"ChatApp";
string namespaceTransporte = @"ChatAppModels.Transporte";
string namespaceWrapperAPI = @"ChatAppModels.Solucion";
string namespaceAccesoDatos = @"ChatAppDAL";

string rutaAppAngular = @"C:\Proyectos\Resultado\frontend\src\app\";
string rutaModelosAngular = @"C:\Proyectos\Resultado\frontend\src\app\models\";
string rutaServiciosAngular = @"C:\Proyectos\Resultado\frontend\src\app\services\";
string rutaComponentesAngular = @"C:\Proyectos\Resultado\frontend\src\app\components\";
string apiURL = @"https://localhost:7175/";

string key = null;

Console.WriteLine("Conectando a la base de datos...");

Tabla[] tablas = new Tabla[0];

List<TablaCatalogo> catalogos = new List<TablaCatalogo>();

#region "Obtencion de tablas, columnas e indices de la base de datos"
using (DbContext cont = new DbContext(new DbContextOptionsBuilder<DbContext>().UseSqlServer(cadenaConexion).Options)) {
    bool conexion = cont.Database.CanConnect();
    Console.WriteLine("Conexion: " + (conexion ? "Hecha" : "Error"));
    if (!conexion) return;

    tablas = cont.Database.SqlQueryRaw<Tabla>(Tabla.sqlTablas()).ToArray();
    Console.WriteLine(tablas.Length + " tablas");

    for (int i = 0; i < tablas.Length; i++) {
        Tabla tabla = tablas[i];
        tabla.Namespace = namespaceTransporte;

        tabla.Columnas = cont.Database.SqlQueryRaw<Columna>(Columna.sqlColumnas(tabla.name)).ToArray();

        tabla.Indices = cont.Database.SqlQueryRaw<Indice>(Indice.sqlIndexes(tabla.name)).ToArray();

        tabla.Foraneas = cont.Database.SqlQueryRaw<LLaveForanea>(LLaveForanea.sqlLlaves(tabla.name)).ToArray();

        foreach (Columna c in tabla.Columnas) {
            c.Indices = Indice.getIndices(tabla.Indices, c);
            c.Foranea = LLaveForanea.getLlaves(tabla.Foraneas, c);
        }
    }
}
#endregion

Console.WriteLine("Se van a escribir " + tablas.Length + " clases para las tablas, deseas continuar Y/N");
key = "" + Console.ReadKey().KeyChar;
Console.WriteLine(""); if (key == null || !key.ToLower().Equals("y")) return;

#region "Exclusion de tablas"
Console.WriteLine("Deseas excluir tablas? Y/N");
key = "" + Console.ReadKey().KeyChar;
Console.WriteLine(""); if (key != null && key.ToLower().Equals("y")) {
    for(int i=0; i<tablas.Length; i++) Console.WriteLine("[" + i + "] " + tablas[i].name);
    Console.WriteLine("escribe los indices separados por comas eje: 0,2,4");
    string entrada = Console.ReadLine();
    string[] __indices = entrada.Split(',');
    int[] _indices = new int[0];
    List<int> ___indices = new List<int>();
    foreach (string ind in __indices) ___indices.Add(int.Parse(ind));
    _indices = ___indices.ToArray();

    Console.Write("Se excluirán: ");
    foreach (int ind in _indices) Console.Write(ind + ", ");
    Console.WriteLine();

    List<Tabla> restantes = new List<Tabla>();
    for (int i = 0; i < tablas.Length; i++)
    {
        if (!_indices.Contains(i)) restantes.Add(tablas[i]);
    }
    tablas = restantes.ToArray();

    Console.Write("Tablas restantes: ");
    foreach (Tabla t in tablas) Console.Write(t.name + ", ");
    Console.WriteLine();

    Console.WriteLine("Deseas continuar? Y/N");
    key = "" + Console.ReadKey().KeyChar;
    Console.WriteLine(""); if (key == null || !key.ToLower().Equals("y")) return;
}
#endregion

#region "Creacion de catalogos"
string _ucampo = null;
foreach (Tabla t in tablas)
{
    foreach (LLaveForanea llf in t.Foraneas)
    {
        TablaCatalogo tablaCatalogo = null;
        Tabla catalogo = tablas.Where(x => x.name == llf.PKTABLE_NAME).FirstOrDefault();
        foreach (TablaCatalogo anterior in catalogos)
        {
            if (anterior.Tabla == catalogo)
            {
                tablaCatalogo = anterior;
                break;
            }
        }
        if (tablaCatalogo == null)
        {
            Console.WriteLine("La tabla " + catalogo.name + " es un catalogo, ingresa el campo nombre o para desplegar:");
            foreach (Columna _c in catalogo.Columnas) Console.Write(_c.COLUMN_NAME + ", ");
            Console.WriteLine(" (" + _ucampo + ")");
            string _campo = Console.ReadLine();
            if ((_campo == null || _campo.Length == 0) && _ucampo != null && _ucampo.Length > 0) _campo = _ucampo;
            if (_campo == null || _campo.Length == 0) throw new Exception("Error al campo nombre de la tabla: " + t.name);
            _ucampo = _campo;

            tablaCatalogo = new TablaCatalogo(catalogo, null, _campo);
            catalogos.Add(tablaCatalogo);
        }
        llf.Catalogo = tablaCatalogo;
    }
}
#endregion

#region "Liga de foraneas con columnas"
/*foreach (Tabla t in tablas) {
    foreach (Columna c in t.Columnas) {
        foreach (TablaCatalogo tc in catalogos) {
            if (tc.Tabla == t) { 
                //esto no resulto necesario de realizar
            }
        }
    }
}*/
#endregion

#region "Exclusion de capos propios de la solucion y no de negocio en interfaz"
Console.WriteLine("Si la esructura de base de datos cuenta con campos propios de la solucion p.e. para el borrado logico que no deben ser mostrados en interfaz... agrega el nombre separados por comas:");
string[] _camposExcluir = Console.ReadLine().Split(',');
List<CampoSistema> camposExcluir = new List<CampoSistema>();
if(_camposExcluir != null) foreach (string _cpo in _camposExcluir) camposExcluir.Add(new CampoSistema("d", _cpo.Trim()));
if (camposExcluir.Count > 0) {
    Console.Write("Campos a excluir de todas las tablas: ");
    foreach(CampoSistema campo in camposExcluir) Console.Write(campo.Nombre + ", ");
    Console.WriteLine("");
    Console.WriteLine("Deseas continuar Y/N");
    key = "" + Console.ReadKey().KeyChar;
    Console.WriteLine(""); if (key == null || !key.ToLower().Equals("y")) return;
}

foreach (CampoSistema campo in camposExcluir)
{
    Console.Write("Insert - Default para " + campo.Nombre + " (Enter para no usar): ");
    campo.DefaultInsert = Console.ReadLine();
    Console.Write("Update - Default para " + campo.Nombre + " (Enter para no usar): ");
    campo.DefaultUpdate = Console.ReadLine();
    Console.Write("Where - Default para " + campo.Nombre + " (Enter para no usar): ");
    campo.DefaultWhere = Console.ReadLine();
}
#endregion

for (int i = 0; i < tablas.Length; i++) {
    Tabla tabla = tablas[i];

    Console.WriteLine("Escribiendo entidad de transporte tabla: " + tabla.name);

    System.IO.File.WriteAllText(rutaDestinoClases + tabla.name + ".cs", tabla.ToString());

    Console.WriteLine("Escribiendo controlador de transporte tabla: " + tabla.name);
    Controlador cont = new Controlador(tabla, namespaceSolucion, namespaceSolucion, namespaceTransporte, namespaceWrapperAPI, namespaceAccesoDatos, camposExcluir);
    System.IO.File.WriteAllText(rutaDestinoControladores + tabla.name + "Controller.cs", cont.ToString());
}

Console.WriteLine("Escribiendo contexto con " + tablas.Length + " tablas");

GeneradorContexto gcon = new GeneradorContexto(namespaceTransporte, namespaceAccesoDatos);
foreach (Tabla tabla in tablas) gcon.tablas.Add(tabla);
System.IO.File.WriteAllText(rutaDestinoContexto + "Contexto.cs", gcon.ToString());

Console.WriteLine("Escribiendo " + tablas.Length + " modelos de angular");

for (int i = 0; i < tablas.Length; i++)
{
    System.IO.File.WriteAllText(rutaModelosAngular + tablas[i].name + ".ts", tablas[i].ToStringAngular());
}

Console.WriteLine("Escribiendo " + tablas.Length + " servicios de angular");

for (int i = 0; i < tablas.Length; i++)
{
    ServicioAngular sa = new ServicioAngular(tablas[i], rutaModelosAngular, rutaAppAngular, rutaServiciosAngular, apiURL);
    System.IO.File.WriteAllText(rutaServiciosAngular + tablas[i].name.ToLower() + ".service.ts", sa.ToString());
}

Console.WriteLine("Escribiendo " + tablas.Length + " componentes de angular");

for (int i = 0; i < tablas.Length; i++)
{
    System.IO.DirectoryInfo tdi = new System.IO.DirectoryInfo(rutaComponentesAngular + tablas[i].name.ToLower());
    if (!tdi.Exists) tdi.Create();
    System.IO.File.WriteAllText(rutaComponentesAngular + tablas[i].name.ToLower() + "\\" + tablas[i].name.ToLower() + ".component.ts", new ComponenteAngularTS(tablas[i], camposExcluir).ToString());
    System.IO.File.WriteAllText(rutaComponentesAngular + tablas[i].name.ToLower() + "\\" + tablas[i].name.ToLower() + ".component.spec.ts", new ComponenteAngularSPEC(tablas[i]).ToString());
    System.IO.File.WriteAllText(rutaComponentesAngular + tablas[i].name.ToLower() + "\\" + tablas[i].name.ToLower() + ".component.scss", new ComponenteAngularSCSS(tablas[i]).ToString());
    System.IO.File.WriteAllText(rutaComponentesAngular + tablas[i].name.ToLower() + "\\" + tablas[i].name.ToLower() + ".component.html", new ComponenteAngularHTML(tablas[i], catalogos.ToArray(), camposExcluir).ToString());
}

Console.WriteLine("Deseas esrcibir el modulo de ruteo de angular? Y/N");
key = "" + Console.ReadKey().KeyChar;
Console.WriteLine("");
if (key != null && key.ToLower().Equals("y")) {
    Console.WriteLine("Escribiendo router de angular");
    System.IO.File.WriteAllText(rutaAppAngular + "app.routes.ts", new RouterAngular(tablas).ToString());
}

Console.WriteLine("Proceso terminado");

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using Microsoft.Win32;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class Conexion486LP
    {
        private const string CATALOGO = "SneakRushDB";

        // Estas son las instancias candidatas, en orden de preferencia
        // "." es la instancia predeterminada de SQL Server (la que aparece en SSMS en las PCs de la facultad)
        private static readonly string[] InstanciasPosibles = new string[]
        {
            @".",
            @".\SQLEXPRESS",
            @"(localdb)\MSSQLLocalDB"
        };
        private static string _instancia;
        private static readonly object _candado = new object();

        // Cadena de conexión a SneakRushDB en la instancia detectada
        public static string BD
        {
            get { return ConstruirCadena(ResolverInstancia(), CATALOGO); }
        }

        // Cadena de conexión a master en la misma instancia
        public static string Master
        {
            get { return ConstruirCadena(ResolverInstancia(), "master"); }
        }

        public static string InstanciaDetectada
        {
            get { return ResolverInstancia(); }
        }

        // Ejecucion (conectar / leer / escribir / desconectar)

        // Ejecuta una lectura (SP) y devuelve el resultado como DataTable
        public static DataTable EjecutarConsulta(SqlCommand comando)
        {
            using (SqlConnection con = new SqlConnection(BD))
            {
                comando.Connection = con;
                SqlDataAdapter adaptador = new SqlDataAdapter(comando);
                DataTable tabla = new DataTable();
                adaptador.Fill(tabla);
                return tabla;
            }
        }

        // Ejecuta una escritura (INSERT/UPDATE/DELETE) y devuelve filas afectadas. Abre y cierra sola
        public static int EjecutarNoConsulta(SqlCommand comando)
        {
            using (SqlConnection con = new SqlConnection(BD))
            {
                comando.Connection = con;
                con.Open();
                return comando.ExecuteNonQuery();
            }
        }

        // Ejecuta un comando y devuelve un único valor (Existe/COUNT o un Id con SCOPE_IDENTITY)
        public static object EjecutarEscalar(SqlCommand comando)
        {
            using (SqlConnection con = new SqlConnection(BD))
            {
                comando.Connection = con;
                con.Open();
                return comando.ExecuteScalar();
            }
        }

        // Casos con transacción (varias operaciones todo o nada, ej: Carrito+DetalleCarrito)
        // el Mapper comparte una misma conexión+transacción entre comandos, y la DAL la presta abierta

        // Abre una conexión (el llamador la cierra). Solo para casos transaccionales multi-paso.
        public static SqlConnection AbrirConexion()
        {
            SqlConnection con = new SqlConnection(BD);
            con.Open();
            return con;
        }

        public static int EjecutarNoConsultaEnTransaccion(SqlCommand comando, SqlConnection conexion, SqlTransaction transaccion)
        {
            comando.Connection = conexion;
            comando.Transaction = transaccion;
            return comando.ExecuteNonQuery();
        }

        public static object EjecutarEscalarEnTransaccion(SqlCommand comando, SqlConnection conexion, SqlTransaction transaccion)
        {
            comando.Connection = conexion;
            comando.Transaction = transaccion;
            return comando.ExecuteScalar();
        }

        // Solo para Respaldo ALTER/BACKUP/RESTORE DATABASE no admiten transacción explícita,
        // y deben correr contra master (no se puede restaurar una base con una conexión activa usándola)
        public static SqlConnection AbrirConexionMaster()
        {
            SqlConnection con = new SqlConnection(Master);
            con.Open();
            return con;
        }

        public static int EjecutarNoConsultaConConexion(SqlCommand comando, SqlConnection conexion)
        {
            comando.Connection = conexion;
            return comando.ExecuteNonQuery();
        }

   
        // Deteccion de instancia 
        private static string ConstruirCadena(string instancia, string catalogo)
        {
            var b = new SqlConnectionStringBuilder
            {
                DataSource = instancia,
                InitialCatalog = catalogo,
                IntegratedSecurity = true
            };
            return b.ConnectionString;
        }

        // Elige la instancia de SQL Server a usar. Prueba las candidatas en orden y se queda con:
        //   1) la primera que ya tiene la base SneakRushDB,
        //   2) si ninguna la tiene, la primera donde el usuario puede crear bases de datos,
        //   3) si no, la primera que responda.
        // Asi el mismo ejecutable anda con LocalDB, con Express o con la instancia predeterminada, sin tocar ninguna configuracion.
        private static string ResolverInstancia()
        {
            if (_instancia != null) return _instancia;

            lock (_candado)
            {
                if (_instancia != null) return _instancia;

                List<string> candidatas = ListarCandidatas();
                string conBase = null;
                string conPermiso = null;
                string queResponde = null;

                foreach (string inst in candidatas)
                {
                    bool existeBase;
                    bool puedeCrear;
                    if (!Evaluar(inst, out existeBase, out puedeCrear)) continue;

                    if (queResponde == null) queResponde = inst;
                    if (existeBase) { conBase = inst; break; }
                    if (puedeCrear && conPermiso == null) conPermiso = inst;
                }

                string elegida = conBase ?? conPermiso ?? queResponde;
                if (elegida != null)
                    GuardarUltimaInstancia(elegida);

                // si ninguna respondio se devuelve la primera para que el mensaje de error apunte a una instancia concreta
                _instancia = elegida ?? candidatas[0];
                return _instancia;
            }
        }

        // Orden: la ultima que anduvo, la del App.config, las fijas y las instaladas segun el registro de Windows
        private static List<string> ListarCandidatas()
        {
            List<string> lista = new List<string>();

            AgregarCandidata(lista, LeerUltimaInstancia());

            ConnectionStringSettings config = ConfigurationManager.ConnectionStrings["SneakRushDB"];
            if (config != null && !string.IsNullOrWhiteSpace(config.ConnectionString))
            {
                try
                {
                    AgregarCandidata(lista, new SqlConnectionStringBuilder(config.ConnectionString).DataSource);
                }
                catch
                {
                    // Cadena mal escrita en el config: se ignora
                }
            }

            foreach (string inst in InstanciasPosibles)
                AgregarCandidata(lista, inst);

            foreach (string inst in InstanciasDelRegistro())
                AgregarCandidata(lista, inst);

            return lista;
        }

        private static void AgregarCandidata(List<string> lista, string instancia)
        {
            if (string.IsNullOrWhiteSpace(instancia)) return;
            instancia = instancia.Trim();
            if (!lista.Contains(instancia, StringComparer.OrdinalIgnoreCase))
                lista.Add(instancia);
        }

        // Instancias de SQL Server instaladas en la PC (la clave se puede leer sin ser administrador)
        private static List<string> InstanciasDelRegistro()
        {
            List<string> nombres = new List<string>();
            try
            {
                using (RegistryKey raiz = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))
                using (RegistryKey clave = raiz.OpenSubKey(@"SOFTWARE\Microsoft\Microsoft SQL Server\Instance Names\SQL"))
                {
                    if (clave != null)
                    {
                        foreach (string nombre in clave.GetValueNames())
                            nombres.Add(string.Equals(nombre, "MSSQLSERVER", StringComparison.OrdinalIgnoreCase) ? "." : @".\" + nombre);
                    }
                }
            }
            catch
            {
                // Sin acceso al registro: quedan las candidatas fijas
            }
            return nombres;
        }

        private static string ArchivoUltimaInstancia()
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "SneakRush", "instancia.txt");
        }

        private static string LeerUltimaInstancia()
        {
            try
            {
                string ruta = ArchivoUltimaInstancia();
                return File.Exists(ruta) ? File.ReadAllText(ruta).Trim() : null;
            }
            catch
            {
                return null;
            }
        }

        private static void GuardarUltimaInstancia(string instancia)
        {
            try
            {
                string ruta = ArchivoUltimaInstancia();
                Directory.CreateDirectory(Path.GetDirectoryName(ruta));
                File.WriteAllText(ruta, instancia);
            }
            catch
            {
                // Si no se puede guardar, en el proximo arranque se vuelve a detectar
            }
        }

        // Se conecta a master de la instancia y averigua si responde, si ya tiene SneakRushDB y si el usuario puede crear bases
        private static bool Evaluar(string instancia, out bool existeBase, out bool puedeCrear)
        {
            existeBase = false;
            puedeCrear = false;

            var b = new SqlConnectionStringBuilder
            {
                DataSource = instancia,
                InitialCatalog = "master",
                IntegratedSecurity = true,
                // LocalDB arranca la instancia en la primera conexion y puede tardar mas; el resto responde enseguida o no existe
                ConnectTimeout = instancia.StartsWith("(localdb)", StringComparison.OrdinalIgnoreCase) ? 30 : 5
            };

            try
            {
                using (var con = new SqlConnection(b.ConnectionString))
                {
                    con.Open();
                    using (var cmd = new SqlCommand("SELECT DB_ID(@nombre), HAS_PERMS_BY_NAME(NULL, NULL, 'CREATE ANY DATABASE')", con))
                    {
                        cmd.Parameters.AddWithValue("@nombre", CATALOGO);
                        using (SqlDataReader lector = cmd.ExecuteReader())
                        {
                            if (lector.Read())
                            {
                                existeBase = !lector.IsDBNull(0);
                                puedeCrear = !lector.IsDBNull(1) && Convert.ToInt32(lector.GetValue(1)) == 1;
                            }
                        }
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}

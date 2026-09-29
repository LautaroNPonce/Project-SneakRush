
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class Conexion486LP
    {
        private const string CATALOGO = "SneakRushDB";

        // Estas son las instancias candidatas, en orden de preferencia
        private static readonly string[] InstanciasPosibles = new string[]
        {
            @"(localdb)\MSSQLLocalDB",
            @".\SQLEXPRESS"
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

        private static string ResolverInstancia()
        {
            if (_instancia != null) return _instancia;

            lock (_candado)
            {
                if (_instancia != null) return _instancia;
                var config = ConfigurationManager.ConnectionStrings["SneakRushDB"];
                if (config != null && !string.IsNullOrWhiteSpace(config.ConnectionString))
                {
                    try
                    {
                        var b = new SqlConnectionStringBuilder(config.ConnectionString);
                        if (!string.IsNullOrWhiteSpace(b.DataSource))
                        {
                            _instancia = b.DataSource;
                            return _instancia;
                        }
                    }
                    catch
                    {
                        // Cadena mal escrita en el config: se ignora y se autodetecta
                    }
                }

                // Autodeteccion (primera instancia que responda en master)
                foreach (var inst in InstanciasPosibles)
                {
                    if (Responde(inst))
                    {
                        _instancia = inst;
                        return _instancia;
                    }
                }

                // ninguna respondio se devuelve la primera (LocalDB) para que, si algo falla, el mensaje de error apunte al caso mas comun
                _instancia = InstanciasPosibles[0];
                return _instancia;
            }
        }

        private static bool Responde(string instancia)
        {
            var b = new SqlConnectionStringBuilder
            {
                DataSource = instancia,
                InitialCatalog = "master",
                IntegratedSecurity = true,
                ConnectTimeout = 5   // no colgar  si la instancia no existe
            };

            try
            {
                using (var con = new SqlConnection(b.ConnectionString))
                {
                    con.Open();
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}

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
    /// Resuelve automaticamente el motor de SQL Server disponible en la PC. (prueba primero LocalDB y, si no responde, SQL Server Express)
    /// A partir del refactor de arquitectura, esta clase es la UNICA que conecta, desconecta, lee y escribe contra la base. Los Mappers arman el SqlCommand
    /// (nombre del SP + parametros) y se lo pasan a estos metodos para que lo ejecuten; los Mappers ya no abren SqlConnection por su cuenta.
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

        /// Cadena de conexion a la base SneakRushDB en la instancia detectada
        public static string BD
        {
            get { return ConstruirCadena(ResolverInstancia(), CATALOGO); }
        }

        /// Cadena de conexion a 'master' en la misma instancia detectada
        public static string Master
        {
            get { return ConstruirCadena(ResolverInstancia(), "master"); }
        }

        /// Nombre de la instancia que quedo seleccionada (para logs / diagnostico)
        public static string InstanciaDetectada
        {
            get { return ResolverInstancia(); }
        }

        // Ejecucion (conectar / leer / escribir / desconectar)

        /// Ejecuta un comando de LECTURA (normalmente CommandType.StoredProcedure) y devuelve el resultado como DataTable. Abre la conexion, llena la
        /// tabla, y la cierra sola (via el using).
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

        /// Ejecuta un comando de ESCRITURA (INSERT/UPDATE/DELETE) y devuelve la cantidad de filas afectadas. Abre y cierra la conexion sola.
        public static int EjecutarNoConsulta(SqlCommand comando)
        {
            using (SqlConnection con = new SqlConnection(BD))
            {
                comando.Connection = con;
                con.Open();
                return comando.ExecuteNonQuery();
            }
        }

        /// Ejecuta un comando y devuelve un unico valor (para Existe/COUNT o para recuperar un Id generado con SCOPE_IDENTITY). Abre y cierra la conexion sola.
        public static object EjecutarEscalar(SqlCommand comando)
        {
            using (SqlConnection con = new SqlConnection(BD))
            {
                comando.Connection = con;
                con.Open();
                return comando.ExecuteScalar();
            }
        }

        // ------------------------------------------------------------------
        // Casos con TRANSACCION (varias operaciones que tienen que ser todo o
        // nada, ej. Carrito+DetalleCarrito). Ahi el Mapper necesita compartir
        // UNA misma conexion+transaccion entre varios comandos, asi que la DAL
        // le presta la conexion abierta y sobrecargas que reciben la transaccion.
        // ------------------------------------------------------------------

        /// Abre y devuelve una conexion (el llamador es responsable de cerrarla).
        /// Solo se usa para casos transaccionales multi-paso.
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

        // ------------------------------------------------------------------
        // SOLO para Respaldo (Backup/Restore): estos comandos ALTER DATABASE /
        // BACKUP DATABASE / RESTORE DATABASE no admiten una transaccion
        // explicita (SQL Server los rechaza dentro de BEGIN TRAN), y encima
        // tienen que ejecutarse conectados a 'master' (nunca a SneakRushDB,
        // porque no se puede restaurar una base mientras hay una conexion
        // activa usandola). Por eso esta variante: conexion sin transaccion,
        // abierta contra 'master'.
        // ------------------------------------------------------------------

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

        // ------------------------------------------------------------------
        // Deteccion de instancia (SIN CAMBIOS respecto a como ya estaba)
        // ------------------------------------------------------------------

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

                // Ninguna respondio: se devuelve la primera (LocalDB) para que, si algo falla, el mensaje de error apunte al caso mas comun.
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

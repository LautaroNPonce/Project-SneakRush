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
    //public class Conexion486LP
    //{
    //    public static string BD = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=SneakRushDB;Integrated Security=True";
    //}

    /// Resuelve automaticamente el motor de SQL Server disponible en la PC. (prueba primero LocalDB y, si no responde, SQL Server Express)
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

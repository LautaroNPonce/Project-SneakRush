using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace DAL
{
    /// Se asegura de que la base SneakRushDB exista en la instancia detectada por Conexion486LP. Si NO existe, la crea y ejecuta el script
    public static class InicializadorBD486LP
    {
        private const string NOMBRE_BASE = "SneakRushDB";
        private const string ARCHIVO_SCRIPT = "SneakRushDB_Instalador.sql";

        /// Devuelve true si la base ya existia o se creo correctamente y devuelve false y mensaje con el detalle si algo fallo
        public static bool AsegurarBaseDeDatos(out string mensaje)
        {
            mensaje = string.Empty;
            try
            {
                if (ExisteBase())
                    return true; // ya esta creada: no se toca nada

                EjecutarScript();
                return true;
            }
            catch (Exception ex)
            {
                mensaje = ex.Message;
                return false;
            }
        }

        private static bool ExisteBase()
        {
            using (var con = new SqlConnection(Conexion486LP.Master))
            {
                con.Open();
                using (var cmd = new SqlCommand("SELECT DB_ID(@nombre)", con))
                {
                    cmd.Parameters.AddWithValue("@nombre", NOMBRE_BASE);
                    object r = cmd.ExecuteScalar();
                    return r != null && r != DBNull.Value;
                }
            }
        }

        private static void EjecutarScript()
        {
            string ruta = UbicarScript();
            string script = File.ReadAllText(ruta);
            List<string> lotes = SepararPorGo(script);

            using (var con = new SqlConnection(Conexion486LP.Master))
            {
                con.Open();
                foreach (string lote in lotes)
                {
                    if (string.IsNullOrWhiteSpace(lote))
                        continue;

                    using (var cmd = new SqlCommand(lote, con))
                    {
                        cmd.CommandTimeout = 180; // el sembrado de datos puede tardar
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        /// Busca el .sql al lado del ejecutable o en la subcarpeta 'BaseDeDatos'.
        private static string UbicarScript()
        {
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;

            string[] candidatos = new string[]
            {
                Path.Combine(baseDir, ARCHIVO_SCRIPT),
                Path.Combine(baseDir, "BaseDeDatos", ARCHIVO_SCRIPT)
            };

            foreach (string ruta in candidatos)
            {
                if (File.Exists(ruta))
                    return ruta;
            }

            throw new Exception(
                "No se encontro el archivo '" + ARCHIVO_SCRIPT + "'. " +
                "Debe quedar junto al ejecutable o en la subcarpeta 'BaseDeDatos'. " +
                "Se busco en:\r\n - " + string.Join("\r\n - ", candidatos));
        }

        private static List<string> SepararPorGo(string script)
        {
            // Separa por lineas que contengan unicamente 'GO' (igual que SSMS/sqlcmd).
            // ADO.NET no entiende 'GO', por eso hay que partir el script en lotes.
            string[] partes = Regex.Split(
                script,
                @"^\s*GO\s*$",
                RegexOptions.Multiline | RegexOptions.IgnoreCase);

            return partes.ToList();
        }
    }
}

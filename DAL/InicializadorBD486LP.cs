using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace DAL
{
    public static class InicializadorBD486LP
    {
        private const string NOMBRE_BASE = "SneakRushDB";
        private const string ARCHIVO_SCRIPT = "SneakRushDB_Instalador.sql";

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

        public static bool ExisteLaBase()
        {
            return ExisteBase();
        }

        public static void CrearBase()
        {
            EjecutarScript();
        }

        public static int ContarUsuarios()
        {
            using (var con = new SqlConnection(Conexion486LP.BD))
            {
                con.Open();
                using (var cmd = new SqlCommand("SELECT COUNT(*) FROM dbo.Usuarios", con))
                {
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }

        public static void EliminarBase()
        {
            SqlConnection.ClearAllPools();
            if (!ExisteBase()) return;

            using (var con = new SqlConnection(Conexion486LP.Master))
            {
                con.Open();
                string sql = "ALTER DATABASE [" + NOMBRE_BASE + "] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; " +
                             "DROP DATABASE [" + NOMBRE_BASE + "];";
                using (var cmd = new SqlCommand(sql, con))
                {
                    cmd.CommandTimeout = 120;
                    cmd.ExecuteNonQuery();
                }
            }
            SqlConnection.ClearAllPools();
        }

        public static void RespaldarBase(string rutaArchivo)
        {
            using (var con = new SqlConnection(Conexion486LP.Master))
            {
                con.Open();
                string sql = "BACKUP DATABASE [" + NOMBRE_BASE + "] TO DISK = @ruta WITH INIT, FORMAT";
                using (var cmd = new SqlCommand(sql, con))
                {
                    cmd.CommandTimeout = 300;
                    cmd.Parameters.AddWithValue("@ruta", rutaArchivo);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static void RestaurarBase(string rutaArchivo)
        {
            SqlConnection.ClearAllPools();
            using (var con = new SqlConnection(Conexion486LP.Master))
            {
                con.Open();
                string sql = "RESTORE DATABASE [" + NOMBRE_BASE + "] FROM DISK = @ruta WITH REPLACE";
                using (var cmd = new SqlCommand(sql, con))
                {
                    cmd.CommandTimeout = 300;
                    cmd.Parameters.AddWithValue("@ruta", rutaArchivo);
                    cmd.ExecuteNonQuery();
                }
            }
            SqlConnection.ClearAllPools();
        }

        public static string CarpetaRespaldoDelServidor()
        {
            using (var con = new SqlConnection(Conexion486LP.Master))
            {
                con.Open();
                using (var cmd = new SqlCommand("SELECT CONVERT(nvarchar(500), SERVERPROPERTY('InstanceDefaultBackupPath'))", con))
                {
                    object r = cmd.ExecuteScalar();
                    return (r == null || r == DBNull.Value) ? null : r.ToString();
                }
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

        // Busca el .sql al lado del ejecutable o en la subcarpeta BaseDeDatos
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
            // Separa por lineas que contengan unicamente GO (igual que SSMS/sqlcmd) ADO.NET no entiende GO, por eso hay que partir el script en lotes
            string[] partes = Regex.Split(
                script,
                @"^\s*GO\s*$",
                RegexOptions.Multiline | RegexOptions.IgnoreCase);

            return partes.ToList();
        }
    }
}

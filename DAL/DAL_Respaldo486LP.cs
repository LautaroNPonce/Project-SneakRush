using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class DAL_Respaldo486LP
    {
        private string NombreBaseDeDatos()
        {
            SqlConnectionStringBuilder sb = new SqlConnectionStringBuilder(Conexion486LP.BD);
            return sb.InitialCatalog;
        }

        private string CadenaMaster()
        {
            SqlConnectionStringBuilder sb = new SqlConnectionStringBuilder(Conexion486LP.BD);
            sb.InitialCatalog = "master";
            return sb.ConnectionString;
        }

        public bool Backup(string carpetaDestino, out string rutaArchivo, out string mensaje)
        {
            rutaArchivo = "";
            mensaje = "";
            try
            {
                string baseDatos = NombreBaseDeDatos();
                string nombreArchivo = baseDatos + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".bak";
                rutaArchivo = Path.Combine(carpetaDestino, nombreArchivo);

                using (SqlConnection con = new SqlConnection(CadenaMaster()))
                {
                    string query = "BACKUP DATABASE [" + baseDatos + "] TO DISK = @ruta";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@ruta", rutaArchivo);

                    con.Open();
                    cmd.ExecuteNonQuery();
                }
                return true;
            }
            catch (Exception ex)
            {
                mensaje = "Error: " + ex.Message;
                return false;
            }
        }

        public bool Restore(string rutaArchivo, out string mensaje)
        {
            mensaje = "";
            string baseDatos = NombreBaseDeDatos();
            try
            {
                using (SqlConnection con = new SqlConnection(CadenaMaster()))
                {
                    con.Open();

                    // 1) Expulsa cualquier conexión abierta a la base
                    SqlCommand cmd = new SqlCommand("ALTER DATABASE [" + baseDatos + "] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;", con);
                    cmd.ExecuteNonQuery();

                    // 2) Restaurar pisando la base actual
                    cmd = new SqlCommand("RESTORE DATABASE [" + baseDatos + "] FROM DISK = @ruta WITH REPLACE;", con);
                    cmd.Parameters.AddWithValue("@ruta", rutaArchivo);
                    cmd.ExecuteNonQuery();

                    // 3) Volver a multiusuario.
                    cmd = new SqlCommand("ALTER DATABASE [" + baseDatos + "] SET MULTI_USER;", con);
                    cmd.ExecuteNonQuery();
                }
                return true;
            }
            catch (Exception ex)
            {
                mensaje = "Error: " + ex.Message;

                // Si el restore quedó a mitad, la base puede haber quedado en Single_user y sin él no se puede volver a usar. La recuperamos.
                string errorRecuperacion = IntentarMultiUser(baseDatos);
                if (errorRecuperacion != "")
                { 
                    mensaje += " | Además no se pudo devolver la base a MULTI_USER: " + errorRecuperacion; 
                }

                return false;
            }
        }

        // Devuelvo "" si logró volver a MULTI_USER, o el mensaje de error si no pudo.
        private string IntentarMultiUser(string baseDatos)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(CadenaMaster()))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand(
                        "ALTER DATABASE [" + baseDatos + "] SET MULTI_USER;", con);
                    cmd.ExecuteNonQuery();
                }
                return "";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}
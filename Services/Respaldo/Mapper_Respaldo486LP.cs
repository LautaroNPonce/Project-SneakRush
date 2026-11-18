using DAL;
using System;
using Mappers;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
   
    public class Mapper_Respaldo486LP : MapperBase486LP
    {
        private string NombreBaseDeDatos()
        {
            SqlConnectionStringBuilder sb = new SqlConnectionStringBuilder(Conexion486LP.BD);
            return sb.InitialCatalog;
        }

        public bool Backup(string carpetaDestino, out string rutaArchivo, out string mensaje)
        {
            rutaArchivo = "";
            mensaje = "";

            SqlConnection con = null;
            try
            {
                string baseDatos = NombreBaseDeDatos();
                string nombreArchivo = baseDatos + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".bak";
                rutaArchivo = Path.Combine(carpetaDestino, nombreArchivo);

                con = Conexion486LP.AbrirConexionMaster();

                SqlCommand cmd = new SqlCommand("Respaldo_Backup");
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@RutaArchivo", rutaArchivo));
                Conexion486LP.EjecutarNoConsultaConConexion(cmd, con);

                return true;
            }
            catch (Exception ex)
            {
                mensaje = "Error: " + ex.Message;
                return false;
            }
            finally
            {
                con?.Close();
            }
        }

        public bool Restore(string rutaArchivo, out string mensaje)
        {
            mensaje = "";
            SqlConnection con = null;

            try
            {
                con = Conexion486LP.AbrirConexionMaster();

                SqlCommand cmd1 = new SqlCommand("Respaldo_PonerSingleUser");
                cmd1.CommandType = CommandType.StoredProcedure;
                Conexion486LP.EjecutarNoConsultaConConexion(cmd1, con);

                SqlCommand cmd2 = new SqlCommand("Respaldo_Restore");
                cmd2.CommandType = CommandType.StoredProcedure;
                cmd2.Parameters.Add(new SqlParameter("@RutaArchivo", rutaArchivo));
                Conexion486LP.EjecutarNoConsultaConConexion(cmd2, con);

                SqlCommand cmd3 = new SqlCommand("Respaldo_PonerMultiUser");
                cmd3.CommandType = CommandType.StoredProcedure;
                Conexion486LP.EjecutarNoConsultaConConexion(cmd3, con);

                return true;
            }
            catch (Exception ex)
            {
                mensaje = "Error: " + ex.Message;

                string errorRecuperacion = IntentarMultiUser();
                if (errorRecuperacion != "")
                {
                    mensaje += " | Además no se pudo devolver la base a MULTI_USER: " + errorRecuperacion;
                }

                return false;
            }
            finally
            {
                con?.Close();
            }
        }

        private string IntentarMultiUser()
        {
            SqlConnection con = null;
            try
            {
                con = Conexion486LP.AbrirConexionMaster();

                SqlCommand cmd = new SqlCommand("Respaldo_PonerMultiUser");
                cmd.CommandType = CommandType.StoredProcedure;
                Conexion486LP.EjecutarNoConsultaConConexion(cmd, con);

                return "";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            finally
            {
                con?.Close();
            }
        }
    }
}

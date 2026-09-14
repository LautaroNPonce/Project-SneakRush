using DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mappers
{

    /// Habla con SQL para Backup/Restore de la base. Reemplaza a DAL_Respaldo486LP.
    /// DIFERENCIA CLAVE con el resto de los Mappers: estos comandos (BACKUP, RESTORE, ALTER DATABASE SINGLE_USER/MULTI_USER) no admiten transaccion
    /// explicita y tienen que ejecutarse conectados a 'master' (nunca a SneakRushDB directamente - no se puede restaurar una base mientras hay
    /// una conexion activa usandola). Por eso usa Conexion486LP.AbrirConexionMaster() en vez de los metodos EjecutarConsulta/EjecutarNoConsulta normales.
    /// El nombre de la base ("SneakRushDB") queda FIJO dentro de cada SP - no es SQL dinamico, es literal, igual que en los SPs de DV.
    /// UBICACION TRANSITORIA en Mappers (no en Services), misma decision que las demas entidades de seguridad - a confirmar con el profe.
   
    public class Mapper_Respaldo486LP : MapperBase486LP
    {
        // Nombre real de la base, leido del connection string (solo se usa
        // para armar el nombre del archivo .bak, no para el SP - el SP tiene "SneakRushDB" fijo adentro).
        private string NombreBaseDeDatos()
        {
            SqlConnectionStringBuilder sb = new SqlConnectionStringBuilder(Conexion486LP.BD);
            return sb.InitialCatalog;
        }

        // Genera un backup completo de la base en la carpeta indicada.
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

        // Restaura la base desde un archivo .bak. Si algo falla a mitad de camino (la base puede quedar en SINGLE_USER), intenta recuperarla
        // volviendola a MULTI_USER - mismo comportamiento que el DAL original.
        public bool Restore(string rutaArchivo, out string mensaje)
        {
            mensaje = "";
            SqlConnection con = null;

            try
            {
                con = Conexion486LP.AbrirConexionMaster();

                // 1) Expulsa cualquier conexion abierta a la base
                SqlCommand cmd1 = new SqlCommand("Respaldo_PonerSingleUser");
                cmd1.CommandType = CommandType.StoredProcedure;
                Conexion486LP.EjecutarNoConsultaConConexion(cmd1, con);

                // 2) Restaurar pisando la base actual
                SqlCommand cmd2 = new SqlCommand("Respaldo_Restore");
                cmd2.CommandType = CommandType.StoredProcedure;
                cmd2.Parameters.Add(new SqlParameter("@RutaArchivo", rutaArchivo));
                Conexion486LP.EjecutarNoConsultaConConexion(cmd2, con);

                // 3) Volver a multiusuario
                SqlCommand cmd3 = new SqlCommand("Respaldo_PonerMultiUser");
                cmd3.CommandType = CommandType.StoredProcedure;
                Conexion486LP.EjecutarNoConsultaConConexion(cmd3, con);

                return true;
            }
            catch (Exception ex)
            {
                mensaje = "Error: " + ex.Message;

                // Si el restore quedó a mitad, la base puede haber quedado en
                // Single_user y sin él no se puede volver a usar. La recuperamos.
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

        // Devuelve "" si logró volver a MULTI_USER, o el mensaje de error si no pudo.
        // Abre su PROPIA conexion nueva (separada de la del try principal, que puede haber quedado en un estado inconsistente tras la excepcion).
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

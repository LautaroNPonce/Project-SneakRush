using DAL;
using Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mappers
{
    /// Habla con SQL para la entidad Familia. Reemplaza a DAL_Familia486LP.
    /// UBICACION TRANSITORIA en Mappers (no en Services) - mismo motivo que Mapper_Permiso486LP e Idioma, ver Entrada 6 del CHANGELOG.
   
    public class Mapper_Familia486LP : MapperBase486LP
    {
        // Lista todas las familias.
        public List<Familia486LP> Listar()
        {
            try
            {
                SqlCommand cmd = new SqlCommand("Familia_Listar");
                cmd.CommandType = CommandType.StoredProcedure;

                DataTable tabla = Conexion486LP.EjecutarConsulta(cmd);
                return ManejadorMapeo486LP.MapearLista<Familia486LP>(tabla);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al listar familias: " + ex.Message);
            }
        }

        // Da de alta una familia. Actualiza f.Id con el generado.
        public bool Agregar(Familia486LP f, out string mensaje)
        {
            mensaje = "";
            try
            {
                SqlCommand cmd = new SqlCommand("Familia_Agregar");
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@Nombre", f.Nombre));

                object idGenerado = Conexion486LP.EjecutarEscalar(cmd);

                if (idGenerado != null && idGenerado != DBNull.Value)
                {
                    f.Id = Convert.ToInt32(idGenerado);
                    mensaje = "Familia creada correctamente.";
                    return true;
                }

                mensaje = "No se pudo crear la familia.";
                return false;
            }
            catch (Exception ex)
            {
                mensaje = "Error: " + ex.Message;
                return false;
            }
        }

        // Modifica el nombre de una familia.
        public bool Modificar(Familia486LP f, out string mensaje)
        {
            mensaje = "";
            try
            {
                SqlCommand cmd = new SqlCommand("Familia_Modificar");
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@Id", f.Id));
                cmd.Parameters.Add(new SqlParameter("@Nombre", f.Nombre));

                bool resultado = Conexion486LP.EjecutarNoConsulta(cmd) > 0;

                if (resultado)
                {
                    mensaje = "Familia modificada correctamente.";
                    return true;
                }
                mensaje = "No se pudo modificar la familia.";
                return false;
            }
            catch (Exception ex)
            {
                mensaje = "Error: " + ex.Message;
                return false;
            }
        }

        // Elimina una familia. Antes borra sus vinculos (Perfil_Familia y Familia_Permiso) para no violar las foreign keys - todo en una
        // sola transaccion, agrupando 3 SPs atomicos (mismo patron que Carrito).
        public bool Eliminar(int id, out string mensaje)
        {
            mensaje = "";

            SqlConnection con = Conexion486LP.AbrirConexion();
            SqlTransaction tran = con.BeginTransaction();

            try
            {
                SqlCommand cmd1 = new SqlCommand("Familia_EliminarVinculosPerfil");
                cmd1.CommandType = CommandType.StoredProcedure;
                cmd1.Parameters.Add(new SqlParameter("@Id", id));
                Conexion486LP.EjecutarNoConsultaEnTransaccion(cmd1, con, tran);

                SqlCommand cmd2 = new SqlCommand("Familia_EliminarVinculosPermiso");
                cmd2.CommandType = CommandType.StoredProcedure;
                cmd2.Parameters.Add(new SqlParameter("@Id", id));
                Conexion486LP.EjecutarNoConsultaEnTransaccion(cmd2, con, tran);

                SqlCommand cmd3 = new SqlCommand("Familia_Eliminar");
                cmd3.CommandType = CommandType.StoredProcedure;
                cmd3.Parameters.Add(new SqlParameter("@Id", id));
                int filas = Conexion486LP.EjecutarNoConsultaEnTransaccion(cmd3, con, tran);

                if (filas > 0)
                {
                    tran.Commit();
                    mensaje = "Familia eliminada correctamente.";
                    return true;
                }

                tran.Rollback();
                mensaje = "No se pudo eliminar la familia.";
                return false;
            }
            catch (Exception ex)
            {
                try { tran.Rollback(); } catch { }
                mensaje = "Error: " + ex.Message;
                return false;
            }
            finally
            {
                con.Close();
            }
        }

        // Asigna un permiso (patente) a una familia.
        public bool AsignarPermiso(int idFamilia, int idPermiso, out string mensaje)
        {
            mensaje = "";
            try
            {
                SqlCommand cmd = new SqlCommand("Familia_AsignarPermiso");
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@IdFamilia", idFamilia));
                cmd.Parameters.Add(new SqlParameter("@IdPermiso", idPermiso));

                bool resultado = Conexion486LP.EjecutarNoConsulta(cmd) > 0;

                if (resultado)
                {
                    mensaje = "Permiso asignado correctamente.";
                    return true;
                }
                mensaje = "No se pudo asignar el permiso.";
                return false;
            }
            catch (Exception ex)
            {
                mensaje = "Error: " + ex.Message;
                return false;
            }
        }

        // Quita un permiso (patente) de una familia.
        public bool QuitarPermiso(int idFamilia, int idPermiso, out string mensaje)
        {
            mensaje = "";
            try
            {
                SqlCommand cmd = new SqlCommand("Familia_QuitarPermiso");
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@IdFamilia", idFamilia));
                cmd.Parameters.Add(new SqlParameter("@IdPermiso", idPermiso));

                bool resultado = Conexion486LP.EjecutarNoConsulta(cmd) > 0;

                if (resultado)
                {
                    mensaje = "Permiso quitado correctamente.";
                    return true;
                }
                mensaje = "No se pudo quitar el permiso.";
                return false;
            }
            catch (Exception ex)
            {
                mensaje = "Error: " + ex.Message;
                return false;
            }
        }

        // Lista los permisos (patentes) asignados a una familia.
        public List<Permiso486LP> ListarPermisosDeFamilia(int idFamilia)
        {
            try
            {
                SqlCommand cmd = new SqlCommand("Familia_ListarPermisos");
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@IdFamilia", idFamilia));

                DataTable tabla = Conexion486LP.EjecutarConsulta(cmd);
                return ManejadorMapeo486LP.MapearLista<Permiso486LP>(tabla);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al listar permisos de familia: " + ex.Message);
            }
        }

        // Indica si la familia esta asignada a algun perfil (para no dejarla
        // borrar mientras este en uso, si la BLL decide validar eso).
        public bool EstaAsignadaAPerfil(int idFamilia)
        {
            try
            {
                SqlCommand cmd = new SqlCommand("Familia_EstaAsignadaAPerfil");
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@IdFamilia", idFamilia));

                return Convert.ToInt32(Conexion486LP.EjecutarEscalar(cmd)) > 0;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al verificar si la familia está asignada a un perfil: " + ex.Message);
            }
        }
    }
}
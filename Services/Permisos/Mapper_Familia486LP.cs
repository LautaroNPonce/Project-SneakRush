using DAL;
using Mappers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
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

        public bool Modificar(Familia486LP f, out string mensaje)
        {
            mensaje = "";
            try
            {
                SqlCommand cmd = new SqlCommand("Familia_Modificar");
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@Id", f.Id));
                cmd.Parameters.Add(new SqlParameter("@Nombre", f.Nombre));

                Conexion486LP.EjecutarNoConsulta(cmd);
                mensaje = "Familia modificada correctamente.";
                return true;
            }
            catch (Exception ex)
            {
                mensaje = "Error: " + ex.Message;
                return false;
            }
        }

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
                Conexion486LP.EjecutarNoConsultaEnTransaccion(cmd3, con, tran);

                tran.Commit();
                mensaje = "Familia eliminada correctamente.";
                return true;
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

        private bool TienePermiso(int idFamilia, int idPermiso)
        {
            SqlCommand cmd = new SqlCommand("Familia_TienePermiso");
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@IdFamilia", idFamilia));
            cmd.Parameters.Add(new SqlParameter("@IdPermiso", idPermiso));

            object resultado = Conexion486LP.EjecutarEscalar(cmd);
            return resultado != null && resultado != DBNull.Value && Convert.ToInt32(resultado) > 0;
        }

        public bool AsignarPermiso(int idFamilia, int idPermiso, out string mensaje)
        {
            mensaje = "";
            try
            {
                if (TienePermiso(idFamilia, idPermiso))
                {
                    mensaje = "Ese permiso ya estaba asignado a esta familia.";
                    return false;
                }

                SqlCommand cmd = new SqlCommand("Familia_AsignarPermiso");
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@IdFamilia", idFamilia));
                cmd.Parameters.Add(new SqlParameter("@IdPermiso", idPermiso));
                Conexion486LP.EjecutarNoConsulta(cmd);

                if (TienePermiso(idFamilia, idPermiso))
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

        public bool QuitarPermiso(int idFamilia, int idPermiso, out string mensaje)
        {
            mensaje = "";
            try
            {
                if (!TienePermiso(idFamilia, idPermiso))
                {
                    mensaje = "Ese permiso no estaba asignado a esta familia.";
                    return false;
                }

                SqlCommand cmd = new SqlCommand("Familia_QuitarPermiso");
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@IdFamilia", idFamilia));
                cmd.Parameters.Add(new SqlParameter("@IdPermiso", idPermiso));
                Conexion486LP.EjecutarNoConsulta(cmd);

                if (!TienePermiso(idFamilia, idPermiso))
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
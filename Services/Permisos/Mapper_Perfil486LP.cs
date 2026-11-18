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
    public class Mapper_Perfil486LP : MapperBase486LP
    {
        // Lista todos los perfiles.
        public List<Perfil486LP> Listar()
        {
            try
            {
                SqlCommand cmd = new SqlCommand("Perfil_Listar");
                cmd.CommandType = CommandType.StoredProcedure;

                DataTable tabla = Conexion486LP.EjecutarConsulta(cmd);
                return ManejadorMapeo486LP.MapearLista<Perfil486LP>(tabla);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al listar perfiles: " + ex.Message);
            }
        }

        public bool Agregar(Perfil486LP p, out string mensaje)
        {
            mensaje = "";
            try
            {
                SqlCommand cmd = new SqlCommand("Perfil_Agregar");
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@Nombre", p.Nombre));

                Conexion486LP.EjecutarNoConsulta(cmd);
                mensaje = "Perfil creado correctamente.";
                return true;
            }
            catch (Exception ex)
            {
                mensaje = "Error: " + ex.Message;
                return false;
            }
        }

        public bool Modificar(Perfil486LP p, out string mensaje)
        {
            mensaje = "";
            try
            {
                SqlCommand cmd = new SqlCommand("Perfil_Modificar");
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@IdPerfil", p.IdPerfil));
                cmd.Parameters.Add(new SqlParameter("@Nombre", p.Nombre));

                Conexion486LP.EjecutarNoConsulta(cmd);
                mensaje = "Perfil modificado correctamente.";
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
            try
            {
                SqlCommand cmd = new SqlCommand("Perfil_Eliminar");
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@IdPerfil", id));

                Conexion486LP.EjecutarNoConsulta(cmd);
                mensaje = "Perfil eliminado correctamente.";
                return true;
            }
            catch (Exception ex)
            {
                mensaje = "Error: " + ex.Message;
                return false;
            }
        }

        public bool TieneUsuarios(int idPerfil)
        {
            try
            {
                SqlCommand cmd = new SqlCommand("Perfil_TieneUsuarios");
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@IdPerfil", idPerfil));

                return Convert.ToInt32(Conexion486LP.EjecutarEscalar(cmd)) > 0;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al verificar usuarios del perfil: " + ex.Message);
            }
        }

        private bool TieneFamilia(int idPerfil, int idFamilia)
        {
            SqlCommand cmd = new SqlCommand("Perfil_TieneFamilia");
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@IdPerfil", idPerfil));
            cmd.Parameters.Add(new SqlParameter("@IdFamilia", idFamilia));

            object resultado = Conexion486LP.EjecutarEscalar(cmd);
            return resultado != null && resultado != DBNull.Value && Convert.ToInt32(resultado) > 0;
        }

        private bool TienePermiso(int idPerfil, int idPermiso)
        {
            SqlCommand cmd = new SqlCommand("Perfil_TienePermiso");
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@IdPerfil", idPerfil));
            cmd.Parameters.Add(new SqlParameter("@IdPermiso", idPermiso));

            object resultado = Conexion486LP.EjecutarEscalar(cmd);
            return resultado != null && resultado != DBNull.Value && Convert.ToInt32(resultado) > 0;
        }

        public bool AsignarFamilia(int idPerfil, int idFamilia, out string mensaje)
        {
            mensaje = "";
            try
            {
                if (TieneFamilia(idPerfil, idFamilia))
                {
                    mensaje = "Esa familia ya estaba asignada a este perfil.";
                    return false;
                }

                SqlCommand cmd = new SqlCommand("Perfil_AsignarFamilia");
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@IdPerfil", idPerfil));
                cmd.Parameters.Add(new SqlParameter("@IdFamilia", idFamilia));
                Conexion486LP.EjecutarNoConsulta(cmd);

                if (TieneFamilia(idPerfil, idFamilia))
                {
                    mensaje = "Familia asignada correctamente.";
                    return true;
                }
                mensaje = "No se pudo asignar la familia.";
                return false;
            }
            catch (Exception ex)
            {
                mensaje = "Error: " + ex.Message;
                return false;
            }
        }

        public bool QuitarFamilia(int idPerfil, int idFamilia, out string mensaje)
        {
            mensaje = "";
            try
            {
                if (!TieneFamilia(idPerfil, idFamilia))
                {
                    mensaje = "Esa familia no estaba asignada a este perfil.";
                    return false;
                }

                SqlCommand cmd = new SqlCommand("Perfil_QuitarFamilia");
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@IdPerfil", idPerfil));
                cmd.Parameters.Add(new SqlParameter("@IdFamilia", idFamilia));
                Conexion486LP.EjecutarNoConsulta(cmd);

                if (!TieneFamilia(idPerfil, idFamilia))
                {
                    mensaje = "Familia quitada correctamente.";
                    return true;
                }
                mensaje = "No se pudo quitar la familia.";
                return false;
            }
            catch (Exception ex)
            {
                mensaje = "Error: " + ex.Message;
                return false;
            }
        }

        public bool AsignarPermiso(int idPerfil, int idPermiso, out string mensaje)
        {
            mensaje = "";
            try
            {
                if (TienePermiso(idPerfil, idPermiso))
                {
                    mensaje = "Ese permiso ya estaba asignado a este perfil.";
                    return false;
                }

                SqlCommand cmd = new SqlCommand("Perfil_AsignarPermiso");
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@IdPerfil", idPerfil));
                cmd.Parameters.Add(new SqlParameter("@IdPermiso", idPermiso));
                Conexion486LP.EjecutarNoConsulta(cmd);

                if (TienePermiso(idPerfil, idPermiso))
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

        public bool QuitarPermiso(int idPerfil, int idPermiso, out string mensaje)
        {
            mensaje = "";
            try
            {
                if (!TienePermiso(idPerfil, idPermiso))
                {
                    mensaje = "Ese permiso no estaba asignado a este perfil.";
                    return false;
                }

                SqlCommand cmd = new SqlCommand("Perfil_QuitarPermiso");
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@IdPerfil", idPerfil));
                cmd.Parameters.Add(new SqlParameter("@IdPermiso", idPermiso));
                Conexion486LP.EjecutarNoConsulta(cmd);

                if (!TienePermiso(idPerfil, idPermiso))
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

        public List<Familia486LP> ListarFamiliasDePerfil(int idPerfil)
        {
            try
            {
                SqlCommand cmd = new SqlCommand("Perfil_ListarFamilias");
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@IdPerfil", idPerfil));

                DataTable tabla = Conexion486LP.EjecutarConsulta(cmd);
                return ManejadorMapeo486LP.MapearLista<Familia486LP>(tabla);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al listar familias del perfil: " + ex.Message);
            }
        }

        public List<Permiso486LP> ListarPermisosDePerfil(int idPerfil)
        {
            try
            {
                SqlCommand cmd = new SqlCommand("Perfil_ListarPermisos");
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@IdPerfil", idPerfil));

                DataTable tabla = Conexion486LP.EjecutarConsulta(cmd);
                return ManejadorMapeo486LP.MapearLista<Permiso486LP>(tabla);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al listar permisos del perfil: " + ex.Message);
            }
        }

        public List<string> ObtenerNombresPermisosPorRol(string nombreRol)
        {
            List<string> permisos = new List<string>();

            try
            {
                SqlCommand cmd = new SqlCommand("Perfil_ObtenerPermisosPorRol");
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@NombreRol", nombreRol));

                DataTable tabla = Conexion486LP.EjecutarConsulta(cmd);
                foreach (DataRow fila in tabla.Rows)
                {
                    permisos.Add(fila["Nombre"].ToString());
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener permisos por rol: " + ex.Message);
            }

            return permisos;
        }
    }
}

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

    public class Mapper_Usuarios486LP : MapperBase486LP
    {
        public List<Usuario486LP> Listar()
        {
            try
            {
                SqlCommand cmd = new SqlCommand("Usuarios_Listar");
                cmd.CommandType = CommandType.StoredProcedure;

                DataTable tabla = Conexion486LP.EjecutarConsulta(cmd);
                return ManejadorMapeo486LP.MapearLista<Usuario486LP>(tabla);
            }
            catch
            {
                return new List<Usuario486LP>();
            }
        }

        public List<Usuario486LP> ListarActivos()
        {
            try
            {
                SqlCommand cmd = new SqlCommand("Usuarios_ListarActivos");
                cmd.CommandType = CommandType.StoredProcedure;

                DataTable tabla = Conexion486LP.EjecutarConsulta(cmd);
                return ManejadorMapeo486LP.MapearLista<Usuario486LP>(tabla);
            }
            catch
            {
                return new List<Usuario486LP>();
            }
        }

        public Usuario486LP ObtenerPorNombreUsuario(string nombreUsuario)
        {
            try
            {
                SqlCommand cmd = new SqlCommand("Usuarios_ObtenerPorNombreUsuario");
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@NombreUsuario", nombreUsuario));

                DataTable tabla = Conexion486LP.EjecutarConsulta(cmd);
                return tabla.Rows.Count > 0 ? ManejadorMapeo486LP.MapearEntidad<Usuario486LP>(tabla.Rows[0]) : null;
            }
            catch
            {
                return null;
            }
        }

        public bool Agregar(Usuario486LP obj, out string Mensaje)
        {
            Mensaje = string.Empty;
            try
            {
                SqlCommand cmd = new SqlCommand("Usuarios_Agregar");
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@DNI", obj.DNI));
                cmd.Parameters.Add(new SqlParameter("@Nombre", obj.Nombre));
                cmd.Parameters.Add(new SqlParameter("@Apellido", obj.Apellido));
                cmd.Parameters.Add(new SqlParameter("@Email", obj.Email));
                cmd.Parameters.Add(new SqlParameter("@NombreUsuario", obj.NombreUsuario));
                cmd.Parameters.Add(new SqlParameter("@Contraseña", obj.Contraseña));
                cmd.Parameters.Add(new SqlParameter("@Activo", obj.Activo));
                cmd.Parameters.Add(new SqlParameter("@Bloqueado", obj.Bloqueado));
                cmd.Parameters.Add(new SqlParameter("@IntentosFallidos", obj.IntentosFallidos));
                cmd.Parameters.Add(new SqlParameter("@Rol", obj.Rol));
                cmd.Parameters.Add(new SqlParameter("@IdPerfil", (object)obj.IdPerfil ?? DBNull.Value));
                cmd.Parameters.Add(new SqlParameter("@NombreIdioma", (object)obj.NombreIdioma ?? DBNull.Value));
                cmd.Parameters.Add(new SqlParameter("@DebeCambiarContraseña", true));

                Conexion486LP.EjecutarNoConsulta(cmd);
                return true;
            }
            catch (Exception ex)
            {
                Mensaje = ex.Message;
                return false;
            }
        }
        public bool Modificar(Usuario486LP obj, out string Mensaje)
        {
            Mensaje = string.Empty;
            try
            {
                SqlCommand cmd = new SqlCommand("Usuarios_Modificar");
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@IdUsuario", obj.IdUsuario));
                cmd.Parameters.Add(new SqlParameter("@Nombre", obj.Nombre));
                cmd.Parameters.Add(new SqlParameter("@Apellido", obj.Apellido));
                cmd.Parameters.Add(new SqlParameter("@Email", obj.Email));
                cmd.Parameters.Add(new SqlParameter("@Rol", obj.Rol));
                cmd.Parameters.Add(new SqlParameter("@Activo", obj.Activo));
                cmd.Parameters.Add(new SqlParameter("@IdPerfil", (object)obj.IdPerfil ?? DBNull.Value));
                cmd.Parameters.Add(new SqlParameter("@NombreIdioma", (object)obj.NombreIdioma ?? DBNull.Value));

                Conexion486LP.EjecutarNoConsulta(cmd);
                return true;
            }
            catch (Exception ex)
            {
                Mensaje = ex.Message;
                return false;
            }
        }
        public bool Desbloquear(string dni, out string Mensaje)
        {
            Mensaje = string.Empty;
            try
            {
                SqlCommand cmd = new SqlCommand("Usuarios_Desbloquear");
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@DNI", dni));

                Conexion486LP.EjecutarNoConsulta(cmd);
                return true;
            }
            catch (Exception ex)
            {
                Mensaje = ex.Message;
                return false;
            }
        }

        public bool BloquearPorDNI(string dni, out string Mensaje)
        {
            Mensaje = string.Empty;
            try
            {
                SqlCommand cmd = new SqlCommand("Usuarios_BloquearPorDNI");
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@DNI", dni));

                Conexion486LP.EjecutarNoConsulta(cmd);
                return true;
            }
            catch (Exception ex)
            {
                Mensaje = ex.Message;
                return false;
            }
        }

        public bool InvertirActivo(string dni, out string Mensaje)
        {
            Mensaje = string.Empty;
            try
            {
                SqlCommand cmd = new SqlCommand("Usuarios_InvertirActivo");
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@DNI", dni));

                Conexion486LP.EjecutarNoConsulta(cmd);
                return true;
            }
            catch (Exception ex)
            {
                Mensaje = ex.Message;
                return false;
            }
        }

        public bool CambiarContraseña(int idUsuario, string nuevaContraseña, out string Mensaje)
        {
            Mensaje = string.Empty;
            try
            {
                SqlCommand cmd = new SqlCommand("Usuarios_CambiarContraseña");
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@IdUsuario", idUsuario));
                cmd.Parameters.Add(new SqlParameter("@Contraseña", nuevaContraseña));

                Conexion486LP.EjecutarNoConsulta(cmd);
                return true;
            }
            catch (Exception ex)
            {
                Mensaje = ex.Message;
                return false;
            }
        }

        public bool ActualizarIntentos(string nombreUsuario, int intentos, out string Mensaje)
        {
            Mensaje = string.Empty;
            try
            {
                SqlCommand cmd = new SqlCommand("Usuarios_ActualizarIntentos");
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@NombreUsuario", nombreUsuario));
                cmd.Parameters.Add(new SqlParameter("@Intentos", intentos));

                Conexion486LP.EjecutarNoConsulta(cmd);
                return true;
            }
            catch (Exception ex)
            {
                Mensaje = ex.Message;
                return false;
            }
        }

        public bool Bloquear(string nombreUsuario, out string Mensaje)
        {
            Mensaje = string.Empty;
            try
            {
                SqlCommand cmd = new SqlCommand("Usuarios_Bloquear");
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@NombreUsuario", nombreUsuario));

                Conexion486LP.EjecutarNoConsulta(cmd);
                return true;
            }
            catch (Exception ex)
            {
                Mensaje = ex.Message;
                return false;
            }
        }

        public bool Eliminar(int idUsuario, out string Mensaje)
        {
            Mensaje = string.Empty;
            try
            {
                SqlCommand cmd = new SqlCommand("Usuarios_Eliminar");
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@IdUsuario", idUsuario));

                Conexion486LP.EjecutarNoConsulta(cmd);
                return true;
            }
            catch (Exception ex)
            {
                Mensaje = ex.Message;
                return false;
            }
        }
    }
}

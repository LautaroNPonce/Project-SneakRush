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

    /// Habla con SQL para la entidad Usuario. Reemplaza a DAL_Usuarios486LP.
    /// Es la ultima entidad de seguridad que se migra (la mas critica: login, bloqueo, contraseña). UBICACION TRANSITORIA en Mappers (no en Services)
    /// - mismo motivo que las 4 anteriores, ver Entrada 6 del CHANGELOG.

    public class Mapper_Usuarios486LP : MapperBase486LP
    {
        // Devuelve todos los usuarios (Activos + Inactivos).
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

        // Devuelve solo los usuarios activos (para el filtro "Activos" del formulario).
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

        // Busca un usuario por su nombre de usuario (para el login). Null si no existe.
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

        // Da de alta un usuario. La contraseña llega YA hasheada desde la BLL.
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

                return Conexion486LP.EjecutarNoConsulta(cmd) > 0;
            }
            catch (Exception ex)
            {
                Mensaje = ex.Message;
                return false;
            }
        }

        // Modifica los datos de un usuario (no toca contraseña ni intentos/bloqueo).
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

                return Conexion486LP.EjecutarNoConsulta(cmd) > 0;
            }
            catch (Exception ex)
            {
                Mensaje = ex.Message;
                return false;
            }
        }

        // Desbloquea un usuario (lo hace el administrador) y lo obliga a cambiar contraseña.
        public bool Desbloquear(string dni, out string Mensaje)
        {
            Mensaje = string.Empty;
            try
            {
                SqlCommand cmd = new SqlCommand("Usuarios_Desbloquear");
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@DNI", dni));

                return Conexion486LP.EjecutarNoConsulta(cmd) > 0;
            }
            catch (Exception ex)
            {
                Mensaje = ex.Message;
                return false;
            }
        }

        // Bloquear manual por el Administrador (usa DNI, consistente con Desbloquear).
        public bool BloquearPorDNI(string dni, out string Mensaje)
        {
            Mensaje = string.Empty;
            try
            {
                SqlCommand cmd = new SqlCommand("Usuarios_BloquearPorDNI");
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@DNI", dni));

                return Conexion486LP.EjecutarNoConsulta(cmd) > 0;
            }
            catch (Exception ex)
            {
                Mensaje = ex.Message;
                return false;
            }
        }

        // Activar / Desactivar (invierte el bit).
        public bool InvertirActivo(string dni, out string Mensaje)
        {
            Mensaje = string.Empty;
            try
            {
                SqlCommand cmd = new SqlCommand("Usuarios_InvertirActivo");
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@DNI", dni));

                return Conexion486LP.EjecutarNoConsulta(cmd) > 0;
            }
            catch (Exception ex)
            {
                Mensaje = ex.Message;
                return false;
            }
        }

        // Cambia la contraseña (ya hasheada, la BLL hace el hash antes de llamar).
        public bool CambiarContraseña(int idUsuario, string nuevaContraseña, out string Mensaje)
        {
            Mensaje = string.Empty;
            try
            {
                SqlCommand cmd = new SqlCommand("Usuarios_CambiarContraseña");
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@IdUsuario", idUsuario));
                cmd.Parameters.Add(new SqlParameter("@Contraseña", nuevaContraseña));

                return Conexion486LP.EjecutarNoConsulta(cmd) > 0;
            }
            catch (Exception ex)
            {
                Mensaje = ex.Message;
                return false;
            }
        }

        // Actualiza el contador de intentos fallidos de login.
        public bool ActualizarIntentos(string nombreUsuario, int intentos, out string Mensaje)
        {
            Mensaje = string.Empty;
            try
            {
                SqlCommand cmd = new SqlCommand("Usuarios_ActualizarIntentos");
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@NombreUsuario", nombreUsuario));
                cmd.Parameters.Add(new SqlParameter("@Intentos", intentos));

                return Conexion486LP.EjecutarNoConsulta(cmd) > 0;
            }
            catch (Exception ex)
            {
                Mensaje = ex.Message;
                return false;
            }
        }

        // Bloquea al usuario (se llama cuando IntentosFallidos llega a 3).
        public bool Bloquear(string nombreUsuario, out string Mensaje)
        {
            Mensaje = string.Empty;
            try
            {
                SqlCommand cmd = new SqlCommand("Usuarios_Bloquear");
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@NombreUsuario", nombreUsuario));

                return Conexion486LP.EjecutarNoConsulta(cmd) > 0;
            }
            catch (Exception ex)
            {
                Mensaje = ex.Message;
                return false;
            }
        }

        // Baja fisica de un usuario.
        public bool Eliminar(int idUsuario, out string Mensaje)
        {
            Mensaje = string.Empty;
            try
            {
                SqlCommand cmd = new SqlCommand("Usuarios_Eliminar");
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@IdUsuario", idUsuario));

                return Conexion486LP.EjecutarNoConsulta(cmd) > 0;
            }
            catch (Exception ex)
            {
                Mensaje = ex.Message;
                return false;
            }
        }
    }
}

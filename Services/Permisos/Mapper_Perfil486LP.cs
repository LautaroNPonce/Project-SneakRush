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
    /// Habla con SQL para la entidad Perfil. Reemplaza a DAL_Perfil486LP.
    /// UBICACION TRANSITORIA en Mappers (no en Services) - mismo motivo que Mapper_Permiso486LP, Idioma y Familia. Ver Entrada 6 del CHANGELOG.

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

        // Da de alta un perfil. NOTA: igual que el original, no devuelve el Id generado (el DAL viejo tampoco lo hacia).
        // Tampoco se compara el conteo de filas de ExecuteNonQuery para decidir exito - en este entorno no siempre refleja el resultado real. Se confia en la ausencia de excepcion,
        // igual que Mapper_Cliente486LP/Mapper_Carrito486LP.
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

        // Modifica el nombre de un perfil. Mismo criterio que Agregar: no se compara conteo de filas.
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

        // Elimina un perfil. NOTA: igual que el original, no borra antes los vinculos de Perfil_Familia/Perfil_Permiso (se replica el mismo
        // comportamiento tal cual estaba, sin agregar logica nueva). Mismo criterio que Agregar/Modificar: no se compara conteo de filas.
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

        // Indica si hay usuarios usando este perfil.
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

        // Verifica si el perfil ya tiene la familia asignada. Se usa TANTO antes de actuar
        // (mensaje claro si ya estaba en el estado pedido) COMO despues de actuar (para
        // confirmar el resultado real - en este entorno el conteo de filas que devuelve
        // ExecuteNonQuery no siempre refleja si el INSERT/DELETE realmente tuvo efecto,
        // asi que no se usa ese conteo para decidir exito o fracaso).
        private bool TieneFamilia(int idPerfil, int idFamilia)
        {
            SqlCommand cmd = new SqlCommand("Perfil_TieneFamilia");
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@IdPerfil", idPerfil));
            cmd.Parameters.Add(new SqlParameter("@IdFamilia", idFamilia));

            object resultado = Conexion486LP.EjecutarEscalar(cmd);
            return resultado != null && resultado != DBNull.Value && Convert.ToInt32(resultado) > 0;
        }

        // Verifica si el perfil ya tiene el permiso suelto asignado. Mismo criterio que TieneFamilia.
        private bool TienePermiso(int idPerfil, int idPermiso)
        {
            SqlCommand cmd = new SqlCommand("Perfil_TienePermiso");
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@IdPerfil", idPerfil));
            cmd.Parameters.Add(new SqlParameter("@IdPermiso", idPermiso));

            object resultado = Conexion486LP.EjecutarEscalar(cmd);
            return resultado != null && resultado != DBNull.Value && Convert.ToInt32(resultado) > 0;
        }

        // Asigna una familia a un perfil.
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

        // Quita una familia de un perfil.
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

        // Asigna un permiso (patente) suelto a un perfil.
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

        // Quita un permiso (patente) suelto de un perfil.
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

        // Lista las familias asignadas a un perfil.
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

        // Lista los permisos (patentes) sueltos asignados a un perfil.
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

        // Nombres de permisos (directos + via familias) de un rol/perfil por nombre.
        // Devuelve solo strings sueltos, no una entidad - se lee directo del DataTable.
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

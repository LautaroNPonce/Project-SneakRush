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

    /// Habla con SQL para el idioma de un usuario. Reemplaza a DAL_Idioma486LP.
    /// No hay una entidad que mapear (solo un valor escalar), asi que no usa ManejadorMapeo486LP - se lee/escribe directo con EjecutarEscalar/EjecutarNoConsulta.
    /// UBICACION TRANSITORIA en Mappers (no en Services), mismo motivo que Mapper_Permiso486LP - ver Entrada 6 del CHANGELOG.

    public class Mapper_Idioma486LP : MapperBase486LP
    {
        // Obtiene el codigo de idioma asignado a un usuario. Si no encuentra
        // nada, devuelve "es" (español) como default - mismo comportamiento
        // que tenia DAL_Idioma486LP.
        public string ObtenerIdioma(int idUsuario)
        {
            try
            {
                SqlCommand cmd = new SqlCommand("Usuario_ObtenerIdioma");
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@IdUsuario", idUsuario));

                object resultado = Conexion486LP.EjecutarEscalar(cmd);
                return resultado != null && resultado != DBNull.Value ? resultado.ToString() : "es";
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener idioma del usuario: " + ex.Message);
            }
        }

        // Actualiza el idioma asignado a un usuario.
        // NOTA: no se compara el conteo de filas de ExecuteNonQuery para decidir exito -
        // en este entorno no siempre refleja el resultado real. Se confia en la ausencia
        // de excepcion, igual que Mapper_Cliente486LP/Mapper_Carrito486LP.
        public bool GuardarIdioma(int idUsuario, string codigoIdioma, out string mensaje)
        {
            mensaje = "";
            try
            {
                SqlCommand cmd = new SqlCommand("Usuario_ActualizarIdioma");
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@IdUsuario", idUsuario));
                cmd.Parameters.Add(new SqlParameter("@NombreIdioma", codigoIdioma));

                Conexion486LP.EjecutarNoConsulta(cmd);
                mensaje = "Idioma guardado correctamente.";
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al guardar idioma del usuario: " + ex.Message);
            }
        }
    }
}

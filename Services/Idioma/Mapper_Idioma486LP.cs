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
    public class Mapper_Idioma486LP : MapperBase486LP
    {
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

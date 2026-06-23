using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class DAL_Idioma486LP
    {
        public string ObtenerIdioma(int idUsuario)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(Conexion486LP.BD))
                {
                    string query = "SELECT NombreIdioma FROM Usuarios WHERE IdUsuario = @IdUsuario";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@IdUsuario", idUsuario);
                    con.Open();
                    object resultado = cmd.ExecuteScalar();
                    return resultado != null && resultado != DBNull.Value ? resultado.ToString() : "es";
                }
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
                using (SqlConnection con = new SqlConnection(Conexion486LP.BD))
                {
                    string query = "UPDATE Usuarios SET NombreIdioma = @NombreIdioma WHERE IdUsuario = @IdUsuario";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@NombreIdioma", codigoIdioma);
                    cmd.Parameters.AddWithValue("@IdUsuario", idUsuario);
                    con.Open();
                    bool resultado = cmd.ExecuteNonQuery() > 0;
                    if (resultado)
                    {
                        mensaje = "Idioma guardado correctamente.";
                        return true;
                    }
                    mensaje = "No se pudo guardar el idioma.";
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al guardar idioma del usuario: " + ex.Message);
            }
        }
    }
}

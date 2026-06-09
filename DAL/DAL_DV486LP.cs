using Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class DAL_DV486LP
    {
        public DataTable LeerTabla(string nombreTabla)
        {
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection con = new SqlConnection(Conexion486LP.BD))
                {
                    SqlCommand cmd = new SqlCommand($"SELECT * FROM {nombreTabla}", con);
                    con.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        dt.Load(dr);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al leer tabla '{nombreTabla}': {ex.Message}");
            }
            return dt;
        }

        public string ObtenerDVV(string tabla)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(Conexion486LP.BD))
                {
                    SqlCommand cmd = new SqlCommand("SELECT DVV FROM DV WHERE Tabla = @Tabla", con);
                    cmd.Parameters.AddWithValue("@Tabla", tabla);
                    con.Open();
                    object resultado = cmd.ExecuteScalar();
                    return resultado?.ToString() ?? "";
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener DVV de tabla '{tabla}': {ex.Message}");
            }
        }

        public string ObtenerDVH(string tabla)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(Conexion486LP.BD))
                {
                    SqlCommand cmd = new SqlCommand("SELECT DVH FROM DV WHERE Tabla = @Tabla", con);
                    cmd.Parameters.AddWithValue("@Tabla", tabla);
                    con.Open();
                    object resultado = cmd.ExecuteScalar();
                    return resultado?.ToString() ?? "";
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener DVH de tabla '{tabla}': {ex.Message}");
            }
        }

        public void GuardarDV(string tabla, string dvh, string dvv)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(Conexion486LP.BD))
                {
                    SqlCommand cmd = new SqlCommand("UPDATE DV SET DVH = @DVH, DVV = @DVV WHERE Tabla = @Tabla", con);
                    cmd.Parameters.AddWithValue("@Tabla", tabla);
                    cmd.Parameters.AddWithValue("@DVH", dvh);
                    cmd.Parameters.AddWithValue("@DVV", dvv);
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al guardar DV de tabla '{tabla}': {ex.Message}");
            }
        }

        public void RecalcularDVHPorFila(string tabla)
        {
            DataTable dt = LeerTabla(tabla);
            foreach (DataRow fila in dt.Rows)
            {
                StringBuilder sb = new StringBuilder();
                foreach (DataColumn col in dt.Columns)
                {
                    if (col.ColumnName != "DV")
                        sb.Append(fila[col].ToString());
                }
                string hash = Encriptacion486LP.GenerarHash(sb.ToString());
                try
                {
                    using (SqlConnection con = new SqlConnection(Conexion486LP.BD))
                    {
                        SqlCommand cmd = new SqlCommand($"UPDATE {tabla} SET DV = @DV WHERE IdUsuario = @Id", con);
                        cmd.Parameters.AddWithValue("@DV", hash);
                        cmd.Parameters.AddWithValue("@Id", fila["IdUsuario"]);
                        con.Open();
                        int filas = cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error al actualizar DV fila {fila["IdUsuario"]}: {ex.Message}");
                }
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class Conexion486LP
    {
        public static string BD = ConfigurationManager.ConnectionStrings["ConexionBD"].ToString();

        // Ejecuto una consulta SELECT y devuelve los resultados en un DataTable.
        public DataTable Leer(string consulta, bool esProcedimiento = false, List<SqlParameter> parametros = null)
        {
            using (SqlConnection con = new SqlConnection(BD))
            using (SqlCommand cmd = new SqlCommand(consulta, con))
            {
                cmd.CommandType = esProcedimiento ? CommandType.StoredProcedure : CommandType.Text;
                if (parametros != null)
                { 
                    cmd.Parameters.AddRange(parametros.ToArray()); 
                }

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);   // Fill abre y cierra la conexión automáticamente
                return dt;
            }
        }

        //Ejecuto una consulta de escritura (INSERT, UPDATE, DELETE).
        public bool Escribir(string consulta, bool esProcedimiento = false, List<SqlParameter> parametros = null)
        {
            using (SqlConnection con = new SqlConnection(BD))
            using (SqlCommand cmd = new SqlCommand(consulta, con))
            {
                cmd.CommandType = esProcedimiento ? CommandType.StoredProcedure : CommandType.Text;
                if (parametros != null)
                { 
                    cmd.Parameters.AddRange(parametros.ToArray());
                }

                con.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        // Ejecuto una consulta que devuelve un único valor (COUNT, MAX, etc).
        public object LeerScalar(string consulta, bool esProcedimiento = false, List<SqlParameter> parametros = null)
        {
            using (SqlConnection con = new SqlConnection(BD))
            using (SqlCommand cmd = new SqlCommand(consulta, con))
            {
                cmd.CommandType = esProcedimiento ? CommandType.StoredProcedure : CommandType.Text;
                if (parametros != null)
                {
                    cmd.Parameters.AddRange(parametros.ToArray());
                }

                con.Open();
                return cmd.ExecuteScalar();
            }
        }
    }
}

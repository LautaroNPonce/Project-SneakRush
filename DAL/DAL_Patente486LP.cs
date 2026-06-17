using Services;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class DAL_Patente486LP
    {
        public List<Permiso486LP> Listar()
        {
            List<Permiso486LP> lista = new List<Permiso486LP>();

            try
            {
                using (SqlConnection con = new SqlConnection(Conexion486LP.BD))
                {
                    string query = "SELECT IdPermiso, Nombre FROM Permiso";
                    SqlCommand cmd = new SqlCommand(query, con);
                    con.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            Permiso486LP p = new Permiso486LP();
                            p.Id = Convert.ToInt32(dr["IdPermiso"]);
                            p.Nombre = dr["Nombre"].ToString();
                            lista.Add(p);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al listar patentes: " + ex.Message);
            }

            return lista;
        }
    }
}

using BE;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class DAL_Bitacora486LP
    {
        public bool Registrar(BitacoraEvento486LP registro)
        {
            bool respuesta = false;

            try
            {
                using (SqlConnection con = new SqlConnection(Conexion486LP.BD))
                {
                    string query = @"INSERT INTO BitacoraEvento (Fecha, Modulo, Descripcion, Criticidad, DNI)
                                     VALUES (@Fecha, @Modulo, @Descripcion, @Criticidad, @DNI)";

                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.CommandType = CommandType.Text;

                    // Si la fecha viene vacía o inválida, usamos DateTime.Now (igual que tu proyecto)
                    cmd.Parameters.AddWithValue("@Fecha", registro.Fecha < new DateTime(1753, 1, 1) ? DateTime.Now : registro.Fecha);
                    cmd.Parameters.AddWithValue("@Modulo", registro.Modulo);
                    cmd.Parameters.AddWithValue("@Descripcion", registro.Descripcion);
                    cmd.Parameters.AddWithValue("@Criticidad", registro.Criticidad);
                    cmd.Parameters.AddWithValue("@DNI", registro.DNI);

                    con.Open();
                    respuesta = cmd.ExecuteNonQuery() > 0;
                }
            }
            catch
            {
                respuesta = false;
            }

            return respuesta;
        }

        public List<BitacoraEvento486LP> Listar()
        {
            List<BitacoraEvento486LP> lista = new List<BitacoraEvento486LP>();

            try
            {
                using (SqlConnection con = new SqlConnection(Conexion486LP.BD))
                {
                    string query = @"SELECT Numero, Fecha, Modulo, Descripcion, Criticidad, DNI
                                     FROM BitacoraEvento
                                     ORDER BY Fecha DESC";

                    SqlCommand cmd = new SqlCommand(query, con);
                    con.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new BitacoraEvento486LP()
                            {
                                Numero = Convert.ToInt32(dr["Numero"]),
                                Fecha = Convert.ToDateTime(dr["Fecha"]),
                                Modulo = dr["Modulo"].ToString(),
                                Descripcion = dr["Descripcion"].ToString(),
                                Criticidad = dr["Criticidad"].ToString(),
                                DNI = dr["DNI"].ToString()
                            });
                        }
                    }
                }
            }
            catch
            {
                lista = new List<BitacoraEvento486LP>();
            }

            return lista;
        }

        public List<BitacoraEvento486LP> Filtrar(string dni, string modulo,string criticidad, string fechaInicio, string fechaFin)
        {
            List<BitacoraEvento486LP> lista = new List<BitacoraEvento486LP>();

            try
            {
                using (SqlConnection con = new SqlConnection(Conexion486LP.BD))
                {
                    string query = @"SELECT Numero, Fecha, Modulo, Descripcion, Criticidad, DNI
                             FROM BitacoraEvento
                             WHERE (@DNI         IS NULL OR DNI        = @DNI)
                               AND (@Modulo      IS NULL OR Modulo     = @Modulo)
                               AND (@Criticidad  IS NULL OR Criticidad = @Criticidad)
                               AND (@FechaInicio IS NULL OR Fecha     >= @FechaInicio)
                               AND (@FechaFin    IS NULL OR Fecha     <= @FechaFin)
                             ORDER BY Fecha DESC";

                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.CommandType = CommandType.Text;

                    cmd.Parameters.AddWithValue("@DNI", string.IsNullOrEmpty(dni) ? (object)DBNull.Value : dni);
                    cmd.Parameters.AddWithValue("@Modulo", string.IsNullOrEmpty(modulo) ? (object)DBNull.Value : modulo);
                    cmd.Parameters.AddWithValue("@Criticidad", string.IsNullOrEmpty(criticidad) ? (object)DBNull.Value : criticidad);
                    cmd.Parameters.AddWithValue("@FechaInicio", string.IsNullOrEmpty(fechaInicio) ? (object)DBNull.Value : fechaInicio);
                    cmd.Parameters.AddWithValue("@FechaFin", string.IsNullOrEmpty(fechaFin) ? (object)DBNull.Value : fechaFin);

                    con.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new BitacoraEvento486LP()
                            {
                                Numero = Convert.ToInt32(dr["Numero"]),
                                Fecha = Convert.ToDateTime(dr["Fecha"]),
                                Modulo = dr["Modulo"].ToString(),
                                Descripcion = dr["Descripcion"].ToString(),
                                Criticidad = dr["Criticidad"].ToString(),
                                DNI = dr["DNI"].ToString()
                            });
                        }
                    }
                }
            }
            catch
            {
                lista = new List<BitacoraEvento486LP>();
            }

            return lista;
        }
    }
}

using BE;
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
    public class DAL_Bitacora486LP
    {
        public bool Registrar(BitacoraEvento486LP registro)
        {
            bool respuesta = false;

            try
            {
                using (SqlConnection con = new SqlConnection(Conexion486LP.BD))
                {
                    string query = @"INSERT INTO BitacoraEvento (Fecha, Modulo, Descripcion, Criticidad, DNI, NombreUsuario) 
                    VALUES (@Fecha, @Modulo, @Descripcion, @Criticidad, @DNI, @NombreUsuario)";

                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.CommandType = CommandType.Text;

                    cmd.Parameters.AddWithValue("@Fecha", registro.Fecha < new DateTime(1753, 1, 1) ? DateTime.Now : registro.Fecha);
                    cmd.Parameters.AddWithValue("@Modulo", registro.Modulo);
                    cmd.Parameters.AddWithValue("@Descripcion", registro.Descripcion);
                    cmd.Parameters.AddWithValue("@Criticidad", registro.Criticidad);
                    cmd.Parameters.AddWithValue("@DNI", registro.DNI);
                    cmd.Parameters.AddWithValue("@NombreUsuario", registro.NombreUsuario ?? "");

                    con.Open();
                    respuesta = cmd.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception ex)
            {
                // Es temporal para encontrar errores, luego se puede eliminar o reemplazar por un log adecuado
                System.Diagnostics.Debug.WriteLine("ERROR DAL Registrar: " + ex.Message);
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
                    string query = @"SELECT Numero, Fecha, Modulo, Descripcion, Criticidad, DNI, NombreUsuario FROM BitacoraEvento ORDER BY Fecha DESC";
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
                                DNI = dr["DNI"].ToString(),
                                NombreUsuario = dr["NombreUsuario"].ToString()
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
        public List<BitacoraEvento486LP> Filtrar(
            string dni,
            string nombreUsuario,
            string modulo,
            string criticidad,
            string fechaInicio,
            string fechaFin)
        {
            List<BitacoraEvento486LP> lista = new List<BitacoraEvento486LP>();

            try
            {
                using (SqlConnection con = new SqlConnection(Conexion486LP.BD))
                {
                    string query = @"SELECT Numero, Fecha, Modulo, Descripcion, Criticidad, DNI, NombreUsuario FROM BitacoraEvento WHERE (@DNI IS NULL OR DNI = @DNI)
                    AND (@NombreUsuario IS NULL OR NombreUsuario LIKE @NombreUsuario) AND (@Modulo IS NULL OR Modulo = @Modulo) AND (@Criticidad IS NULL OR Criticidad = @Criticidad)
                    AND (@FechaInicio IS NULL OR Fecha >= @FechaInicio) AND (@FechaFin IS NULL OR Fecha <= @FechaFin) ORDER BY Fecha DESC";

                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.CommandType = CommandType.Text;

                    cmd.Parameters.AddWithValue("@DNI", string.IsNullOrEmpty(dni) ? (object)DBNull.Value : dni);
                    cmd.Parameters.AddWithValue("@NombreUsuario", string.IsNullOrEmpty(nombreUsuario) ? (object)DBNull.Value : "%" + nombreUsuario + "%");
                    cmd.Parameters.AddWithValue("@Modulo", string.IsNullOrEmpty(modulo) ? (object)DBNull.Value : modulo);
                    cmd.Parameters.AddWithValue("@Criticidad", string.IsNullOrEmpty(criticidad) ? (object)DBNull.Value : criticidad);
                    cmd.Parameters.AddWithValue("@FechaInicio", string.IsNullOrEmpty(fechaInicio) ? (object)DBNull.Value : (object)fechaInicio);
                    cmd.Parameters.AddWithValue("@FechaFin", string.IsNullOrEmpty(fechaFin) ? (object)DBNull.Value : (object)fechaFin);

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
                                DNI = dr["DNI"].ToString(),
                                NombreUsuario = dr["NombreUsuario"].ToString()
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

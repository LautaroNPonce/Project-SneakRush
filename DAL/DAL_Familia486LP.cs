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
    public class DAL_Familia486LP
    {
        public List<Familia486LP> Listar()
        {
            List<Familia486LP> lista = new List<Familia486LP>();

            try
            {
                using (SqlConnection con = new SqlConnection(Conexion486LP.BD))
                {
                    string query = "SELECT IdFamilia, Nombre FROM Familia";
                    SqlCommand cmd = new SqlCommand(query, con);
                    con.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            Familia486LP f = new Familia486LP();
                            f.Id = Convert.ToInt32(dr["IdFamilia"]);
                            f.Nombre = dr["Nombre"].ToString();
                            lista.Add(f);
                        }
                    }
                }
            }
            catch
            {
                lista = new List<Familia486LP>();
            }

            return lista;
        }

        public bool Agregar(Familia486LP f, out string mensaje)
        {
            mensaje = "";
            try
            {
                using (SqlConnection con = new SqlConnection(Conexion486LP.BD))
                {
                    string query = "INSERT INTO Familia (Nombre) VALUES (@Nombre)";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@Nombre", f.Nombre);

                    con.Open();
                    bool resultado = cmd.ExecuteNonQuery() > 0;

                    if (resultado)
                    {
                        mensaje = "Familia creada correctamente.";
                        return true;
                    }
                    mensaje = "No se pudo crear la familia.";
                    return false;
                }
            }
            catch (Exception ex)
            {
                mensaje = "Error: " + ex.Message;
                return false;
            }
        }

        public bool Modificar(Familia486LP f, out string mensaje)
        {
            mensaje = "";
            try
            {
                using (SqlConnection con = new SqlConnection(Conexion486LP.BD))
                {
                    string query = "UPDATE Familia SET Nombre = @Nombre WHERE IdFamilia = @Id";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@Nombre", f.Nombre);
                    cmd.Parameters.AddWithValue("@Id", f.Id);

                    con.Open();
                    bool resultado = cmd.ExecuteNonQuery() > 0;

                    if (resultado)
                    {
                        mensaje = "Familia modificada correctamente.";
                        return true;
                    }
                    mensaje = "No se pudo modificar la familia.";
                    return false;
                }
            }
            catch (Exception ex)
            {
                mensaje = "Error: " + ex.Message;
                return false;
            }
        }

        public bool Eliminar(int id, out string mensaje)
        {
            mensaje = "";
            try
            {
                using (SqlConnection con = new SqlConnection(Conexion486LP.BD))
                {
                    string query = "DELETE FROM Familia WHERE IdFamilia = @Id";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@Id", id);

                    con.Open();
                    bool resultado = cmd.ExecuteNonQuery() > 0;

                    if (resultado)
                    {
                        mensaje = "Familia eliminada correctamente.";
                        return true;
                    }
                    mensaje = "No se pudo eliminar la familia.";
                    return false;
                }
            }
            catch (Exception ex)
            {
                mensaje = "Error: " + ex.Message;
                return false;
            }
        }

        public bool AsignarPermiso(int idFamilia, int idPermiso, out string mensaje)
        {
            mensaje = "";
            try
            {
                using (SqlConnection con = new SqlConnection(Conexion486LP.BD))
                {
                    string query = "INSERT INTO Familia_Permiso (IdFamilia, IdPermiso) VALUES (@IdFamilia, @IdPermiso)";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@IdFamilia", idFamilia);
                    cmd.Parameters.AddWithValue("@IdPermiso", idPermiso);

                    con.Open();
                    bool resultado = cmd.ExecuteNonQuery() > 0;

                    if (resultado)
                    {
                        mensaje = "Permiso asignado correctamente.";
                        return true;
                    }
                    mensaje = "No se pudo asignar el permiso.";
                    return false;
                }
            }
            catch (Exception ex)
            {
                mensaje = "Error: " + ex.Message;
                return false;
            }
        }

        public bool QuitarPermiso(int idFamilia, int idPermiso, out string mensaje)
        {
            mensaje = "";
            try
            {
                using (SqlConnection con = new SqlConnection(Conexion486LP.BD))
                {
                    string query = "DELETE FROM Familia_Permiso WHERE IdFamilia = @IdFamilia AND IdPermiso = @IdPermiso";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@IdFamilia", idFamilia);
                    cmd.Parameters.AddWithValue("@IdPermiso", idPermiso);

                    con.Open();
                    bool resultado = cmd.ExecuteNonQuery() > 0;

                    if (resultado)
                    {
                        mensaje = "Permiso quitado correctamente.";
                        return true;
                    }
                    mensaje = "No se pudo quitar el permiso.";
                    return false;
                }
            }
            catch (Exception ex)
            {
                mensaje = "Error: " + ex.Message;
                return false;
            }
        }

        public List<Permiso486LP> ListarPermisosDeFamilia(int idFamilia)
        {
            List<Permiso486LP> lista = new List<Permiso486LP>();

            try
            {
                using (SqlConnection con = new SqlConnection(Conexion486LP.BD))
                {
                    string query = @"SELECT p.IdPermiso, p.Nombre FROM Permiso p INNER JOIN Familia_Permiso fp ON p.IdPermiso = fp.IdPermiso WHERE fp.IdFamilia = @IdFamilia";

                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@IdFamilia", idFamilia);
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
            catch
            {
                lista = new List<Permiso486LP>();
            }

            return lista;
        }
    }
}

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
    public class DAL_Perfil486LP
    {
        public List<Perfil486LP> Listar()
        {
            List<Perfil486LP> lista = new List<Perfil486LP>();

            try
            {
                using (SqlConnection con = new SqlConnection(Conexion486LP.BD))
                {
                    string query = "SELECT IdPerfil, Nombre FROM Perfil";
                    SqlCommand cmd = new SqlCommand(query, con);
                    con.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            Perfil486LP p = new Perfil486LP();
                            p.IdPerfil = Convert.ToInt32(dr["IdPerfil"]);
                            p.Nombre = dr["Nombre"].ToString();
                            lista.Add(p);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al listar perfiles: " + ex.Message);
            }

            return lista;
        }

        public bool Agregar(Perfil486LP p, out string mensaje)
        {
            mensaje = "";
            try
            {
                using (SqlConnection con = new SqlConnection(Conexion486LP.BD))
                {
                    string query = "INSERT INTO Perfil (Nombre) VALUES (@Nombre)";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@Nombre", p.Nombre);

                    con.Open();
                    bool resultado = cmd.ExecuteNonQuery() > 0;

                    if (resultado)
                    {
                        mensaje = "Perfil creado correctamente.";
                        return true;
                    }
                    mensaje = "No se pudo crear el perfil.";
                    return false;
                }
            }
            catch (Exception ex)
            {
                mensaje = "Error: " + ex.Message;
                return false;
            }
        }

        public bool Modificar(Perfil486LP p, out string mensaje)
        {
            mensaje = "";
            try
            {
                using (SqlConnection con = new SqlConnection(Conexion486LP.BD))
                {
                    string query = "UPDATE Perfil SET Nombre = @Nombre WHERE IdPerfil = @IdPerfil";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@Nombre", p.Nombre);
                    cmd.Parameters.AddWithValue("@IdPerfil", p.IdPerfil);

                    con.Open();
                    bool resultado = cmd.ExecuteNonQuery() > 0;

                    if (resultado)
                    {
                        mensaje = "Perfil modificado correctamente.";
                        return true;
                    }
                    mensaje = "No se pudo modificar el perfil.";
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
                    string query = "DELETE FROM Perfil WHERE IdPerfil = @IdPerfil";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@IdPerfil", id);

                    con.Open();
                    bool resultado = cmd.ExecuteNonQuery() > 0;

                    if (resultado)
                    {
                        mensaje = "Perfil eliminado correctamente.";
                        return true;
                    }
                    mensaje = "No se pudo eliminar el perfil.";
                    return false;
                }
            }
            catch (Exception ex)
            {
                mensaje = "Error: " + ex.Message;
                return false;
            }
        }

        public bool TieneUsuarios(int idPerfil)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(Conexion486LP.BD))
                {
                    string query = "SELECT COUNT(*) FROM Usuarios WHERE IdPerfil = @IdPerfil";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@IdPerfil", idPerfil);

                    con.Open();
                    int resultado = Convert.ToInt32(cmd.ExecuteScalar());
                    return resultado > 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al verificar usuarios del perfil: " + ex.Message);
            }
        }

        public bool AsignarFamilia(int idPerfil, int idFamilia, out string mensaje)
        {
            mensaje = "";
            try
            {
                using (SqlConnection con = new SqlConnection(Conexion486LP.BD))
                {
                    string query = "INSERT INTO Perfil_Familia (IdPerfil, IdFamilia) VALUES (@IdPerfil, @IdFamilia)";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@IdPerfil", idPerfil);
                    cmd.Parameters.AddWithValue("@IdFamilia", idFamilia);

                    con.Open();
                    bool resultado = cmd.ExecuteNonQuery() > 0;

                    if (resultado)
                    {
                        mensaje = "Familia asignada correctamente.";
                        return true;
                    }
                    mensaje = "No se pudo asignar la familia.";
                    return false;
                }
            }
            catch (Exception ex)
            {
                mensaje = "Error: " + ex.Message;
                return false;
            }
        }

        public bool QuitarFamilia(int idPerfil, int idFamilia, out string mensaje)
        {
            mensaje = "";
            try
            {
                using (SqlConnection con = new SqlConnection(Conexion486LP.BD))
                {
                    string query = "DELETE FROM Perfil_Familia WHERE IdPerfil = @IdPerfil AND IdFamilia = @IdFamilia";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@IdPerfil", idPerfil);
                    cmd.Parameters.AddWithValue("@IdFamilia", idFamilia);

                    con.Open();
                    bool resultado = cmd.ExecuteNonQuery() > 0;

                    if (resultado)
                    {
                        mensaje = "Familia quitada correctamente.";
                        return true;
                    }
                    mensaje = "No se pudo quitar la familia.";
                    return false;
                }
            }
            catch (Exception ex)
            {
                mensaje = "Error: " + ex.Message;
                return false;
            }
        }

        public bool AsignarPermiso(int idPerfil, int idPermiso, out string mensaje)
        {
            mensaje = "";
            try
            {
                using (SqlConnection con = new SqlConnection(Conexion486LP.BD))
                {
                    string query = "INSERT INTO Perfil_Permiso (IdPerfil, IdPermiso) VALUES (@IdPerfil, @IdPermiso)";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@IdPerfil", idPerfil);
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

        public bool QuitarPermiso(int idPerfil, int idPermiso, out string mensaje)
        {
            mensaje = "";
            try
            {
                using (SqlConnection con = new SqlConnection(Conexion486LP.BD))
                {
                    string query = "DELETE FROM Perfil_Permiso WHERE IdPerfil = @IdPerfil AND IdPermiso = @IdPermiso";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@IdPerfil", idPerfil);
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

        public List<Familia486LP> ListarFamiliasDePerfil(int idPerfil)
        {
            List<Familia486LP> lista = new List<Familia486LP>();

            try
            {
                using (SqlConnection con = new SqlConnection(Conexion486LP.BD))
                {
                    string query = @"SELECT f.IdFamilia, f.Nombre FROM Familia f INNER JOIN Perfil_Familia pf ON f.IdFamilia = pf.IdFamilia WHERE pf.IdPerfil = @IdPerfil";

                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@IdPerfil", idPerfil);
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
            catch (Exception ex)
            {
                throw new Exception("Error al listar familias del perfil: " + ex.Message);
            }

            return lista;
        }

        public List<Permiso486LP> ListarPermisosDePerfil(int idPerfil)
        {
            List<Permiso486LP> lista = new List<Permiso486LP>();

            try
            {
                using (SqlConnection con = new SqlConnection(Conexion486LP.BD))
                {
                    string query = @"SELECT p.IdPermiso, p.Nombre FROM Permiso p INNER JOIN Perfil_Permiso pp ON p.IdPermiso = pp.IdPermiso WHERE pp.IdPerfil = @IdPerfil";

                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@IdPerfil", idPerfil);
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
                throw new Exception("Error al listar permisos del perfil: " + ex.Message);
            }

            return lista;
        }

        public List<string> ObtenerNombresPermisosPorRol(string nombreRol)
        {
            List<string> permisos = new List<string>();

            try
            {
                using (SqlConnection con = new SqlConnection(Conexion486LP.BD))
                {
                    string query = @"SELECT DISTINCT p.Nombre FROM Permiso p INNER JOIN Perfil_Permiso pp ON p.IdPermiso = pp.IdPermiso INNER JOIN Perfil per ON pp.IdPerfil = per.IdPerfil
                    WHERE per.Nombre = @NombreRol UNION SELECT DISTINCT p.Nombre FROM Permiso p INNER JOIN Familia_Permiso fp ON p.IdPermiso = fp.IdPermiso
                    INNER JOIN Perfil_Familia pf ON fp.IdFamilia = pf.IdFamilia INNER JOIN Perfil per ON pf.IdPerfil = per.IdPerfil
                    WHERE per.Nombre = @NombreRol2";

                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@NombreRol", nombreRol);
                    cmd.Parameters.AddWithValue("@NombreRol2", nombreRol);
                    con.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            permisos.Add(dr["Nombre"].ToString());
                        }
                    }
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

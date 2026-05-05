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
    public class DAL_Usuarios486LP
    {

        // Devuelve todos los usuarios (Activos + Inactivos)
        public List<Usuario486LP> Listar()
        {
            List<Usuario486LP> lista = new List<Usuario486LP>();

            try
            {
                using (SqlConnection con = new SqlConnection(Conexion486LP.BD))
                {
                    string query = @"SELECT IdUsuario, DNI, Nombre, Apellido, Email, 
                                            NombreUsuario, Contraseña, Activo, Bloqueado, 
                                            IntentosFallidos, Rol, DV, IdPerfil, NombreIdioma
                                     FROM Usuarios";

                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.CommandType = CommandType.Text;
                    con.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new Usuario486LP()
                            {
                                IdUsuario = Convert.ToInt32(dr["IdUsuario"]),
                                DNI = dr["DNI"].ToString(),
                                Nombre = dr["Nombre"].ToString(),
                                Apellido = dr["Apellido"].ToString(),
                                Email = dr["Email"].ToString(),
                                NombreUsuario = dr["NombreUsuario"].ToString(),
                                Contraseña = dr["Contraseña"].ToString(),
                                Activo = Convert.ToBoolean(dr["Activo"]),
                                Bloqueado = Convert.ToBoolean(dr["Bloqueado"]),
                                IntentosFallidos = Convert.ToInt32(dr["IntentosFallidos"]),
                                Rol = dr["Rol"].ToString(),
                                DV = dr["DV"].ToString(),
                                IdPerfil = dr["IdPerfil"] == DBNull.Value ? (int?)null : Convert.ToInt32(dr["IdPerfil"]),
                                NombreIdioma = dr["NombreIdioma"].ToString()
                            });
                        }
                    }
                }
            }
            catch
            {
                lista = new List<Usuario486LP>();
            }

            return lista;
        }

        // Devuelve solo los usuarios activos (para el filtro "Activos" del formulario)
        public List<Usuario486LP> ListarActivos()
        {
            List<Usuario486LP> lista = new List<Usuario486LP>();

            try
            {
                using (SqlConnection con = new SqlConnection(Conexion486LP.BD))
                {
                    string query = @"SELECT IdUsuario, DNI, Nombre, Apellido, Email, 
                                            NombreUsuario, Contraseña, Activo, Bloqueado, 
                                            IntentosFallidos, Rol, DV, IdPerfil, NombreIdioma
                                     FROM Usuarios
                                     WHERE Activo = 1";

                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.CommandType = CommandType.Text;
                    con.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new Usuario486LP()
                            {
                                IdUsuario = Convert.ToInt32(dr["IdUsuario"]),
                                DNI = dr["DNI"].ToString(),
                                Nombre = dr["Nombre"].ToString(),
                                Apellido = dr["Apellido"].ToString(),
                                Email = dr["Email"].ToString(),
                                NombreUsuario = dr["NombreUsuario"].ToString(),
                                Contraseña = dr["Contraseña"].ToString(),
                                Activo = Convert.ToBoolean(dr["Activo"]),
                                Bloqueado = Convert.ToBoolean(dr["Bloqueado"]),
                                IntentosFallidos = Convert.ToInt32(dr["IntentosFallidos"]),
                                Rol = dr["Rol"].ToString(),
                                DV = dr["DV"].ToString(),
                                IdPerfil = dr["IdPerfil"] == DBNull.Value ? (int?)null : Convert.ToInt32(dr["IdPerfil"]),
                                NombreIdioma = dr["NombreIdioma"].ToString()
                            });
                        }
                    }
                }
            }
            catch
            {
                lista = new List<Usuario486LP>();
            }

            return lista;
        }

        public Usuario486LP ObtenerPorNombreUsuario(string nombreUsuario)
        {
            Usuario486LP usuario = null;

            try
            {
                using (SqlConnection con = new SqlConnection(Conexion486LP.BD))
                {
                    string query = @"SELECT IdUsuario, DNI, Nombre, Apellido, Email, 
                                            NombreUsuario, Contraseña, Activo, Bloqueado, 
                                            IntentosFallidos, Rol, DV, IdPerfil, NombreIdioma
                                     FROM Usuarios
                                     WHERE NombreUsuario = @NombreUsuario";

                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@NombreUsuario", nombreUsuario);
                    cmd.CommandType = CommandType.Text;
                    con.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            usuario = new Usuario486LP()
                            {
                                IdUsuario = Convert.ToInt32(dr["IdUsuario"]),
                                DNI = dr["DNI"].ToString(),
                                Nombre = dr["Nombre"].ToString(),
                                Apellido = dr["Apellido"].ToString(),
                                Email = dr["Email"].ToString(),
                                NombreUsuario = dr["NombreUsuario"].ToString(),
                                Contraseña = dr["Contraseña"].ToString(),
                                Activo = Convert.ToBoolean(dr["Activo"]),
                                Bloqueado = Convert.ToBoolean(dr["Bloqueado"]),
                                IntentosFallidos = Convert.ToInt32(dr["IntentosFallidos"]),
                                Rol = dr["Rol"].ToString(),
                                DV = dr["DV"].ToString(),
                                IdPerfil = dr["IdPerfil"] == DBNull.Value ? (int?)null : Convert.ToInt32(dr["IdPerfil"]),
                                NombreIdioma = dr["NombreIdioma"].ToString()
                            };
                        }
                    }
                }
            }
            catch
            {
                usuario = null;
            }

            return usuario;
        }

        public bool Agregar(Usuario486LP obj, out string Mensaje)
        {
            bool resultado = false;
            Mensaje = string.Empty;

            try
            {
                using (SqlConnection con = new SqlConnection(Conexion486LP.BD))
                {
                    string query = @"INSERT INTO Usuarios 
                                        (DNI, Nombre, Apellido, Email, NombreUsuario, 
                                         Contraseña, Activo, Bloqueado, IntentosFallidos, 
                                         Rol, DV, IdPerfil, NombreIdioma)
                                     VALUES 
                                        (@DNI, @Nombre, @Apellido, @Email, @NombreUsuario, 
                                         @Contraseña, @Activo, @Bloqueado, @IntentosFallidos, 
                                         @Rol, @DV, @IdPerfil, @NombreIdioma)";

                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@DNI", obj.DNI);
                    cmd.Parameters.AddWithValue("@Nombre", obj.Nombre);
                    cmd.Parameters.AddWithValue("@Apellido", obj.Apellido);
                    cmd.Parameters.AddWithValue("@Email", obj.Email);
                    cmd.Parameters.AddWithValue("@NombreUsuario", obj.NombreUsuario);
                    cmd.Parameters.AddWithValue("@Contraseña", obj.Contraseña);
                    cmd.Parameters.AddWithValue("@Activo", obj.Activo);
                    cmd.Parameters.AddWithValue("@Bloqueado", obj.Bloqueado);
                    cmd.Parameters.AddWithValue("@IntentosFallidos", obj.IntentosFallidos);
                    cmd.Parameters.AddWithValue("@Rol", obj.Rol);
                    cmd.Parameters.AddWithValue("@DV", obj.DV);
                    cmd.Parameters.AddWithValue("@IdPerfil", (object)obj.IdPerfil ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@NombreIdioma", (object)obj.NombreIdioma ?? DBNull.Value);
                    cmd.CommandType = CommandType.Text;

                    con.Open();
                    resultado = cmd.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception ex)
            {
                resultado = false;
                Mensaje = ex.Message;
            }

            return resultado;
        }

        public bool Modificar(Usuario486LP obj, out string Mensaje)
        {
            bool resultado = false;
            Mensaje = string.Empty;

            try
            {
                using (SqlConnection con = new SqlConnection(Conexion486LP.BD))
                {
                    string query = @"UPDATE Usuarios SET
                                        Nombre       = @Nombre,
                                        Apellido     = @Apellido,
                                        Email        = @Email,
                                        Rol          = @Rol,
                                        Activo       = @Activo,
                                        IdPerfil     = @IdPerfil,
                                        NombreIdioma = @NombreIdioma
                                     WHERE IdUsuario = @IdUsuario";

                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@IdUsuario", obj.IdUsuario);
                    cmd.Parameters.AddWithValue("@Nombre", obj.Nombre);
                    cmd.Parameters.AddWithValue("@Apellido", obj.Apellido);
                    cmd.Parameters.AddWithValue("@Email", obj.Email);
                    cmd.Parameters.AddWithValue("@Rol", obj.Rol);
                    cmd.Parameters.AddWithValue("@Activo", obj.Activo);
                    cmd.Parameters.AddWithValue("@IdPerfil", (object)obj.IdPerfil ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@NombreIdioma", (object)obj.NombreIdioma ?? DBNull.Value);
                    cmd.CommandType = CommandType.Text;

                    con.Open();
                    resultado = cmd.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception ex)
            {
                resultado = false;
                Mensaje = ex.Message;
            }

            return resultado;
        }

        //  Desbloquear Usuario)
        public bool Desbloquear(string dni, out string Mensaje)
        {
            bool resultado = false;
            Mensaje = string.Empty;

            try
            {
                using (SqlConnection con = new SqlConnection(Conexion486LP.BD))
                {
                    string query = @"UPDATE Usuarios SET Bloqueado = 0,
                                        IntentosFallidos = 0
                                     WHERE DNI = @DNI";

                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@DNI", dni);
                    cmd.CommandType = CommandType.Text;

                    con.Open();
                    resultado = cmd.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception ex)
            {
                resultado = false;
                Mensaje = ex.Message;
            }

            return resultado;
        }

        // Bloquear manual por el Administrador (usa DNI, consistente con Desbloquear)
        public bool BloquearPorDNI(string dni, out string Mensaje)
        {
            bool resultado = false;
            Mensaje = string.Empty;

            try
            {
                using (SqlConnection con = new SqlConnection(Conexion486LP.BD))
                {
                    string query = @"UPDATE Usuarios SET Bloqueado = 1, IntentosFallidos = 0 WHERE DNI = @DNI";

                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@DNI", dni);
                    cmd.CommandType = CommandType.Text;

                    con.Open();
                    resultado = cmd.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception ex)
            {
                resultado = false;
                Mensaje = ex.Message;
            }

            return resultado;
        }

        //  (Activar / Desactivar)
        public bool InvertirActivo(string dni, out string Mensaje)
        {
            bool resultado = false;
            Mensaje = string.Empty;

            try
            {
                using (SqlConnection con = new SqlConnection(Conexion486LP.BD))
                {
                    // Invierte el bit directamente en la BD — sin leer primero
                    string query = @"UPDATE Usuarios SET Activo = ~Activo WHERE DNI = @DNI";

                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@DNI", dni);
                    cmd.CommandType = CommandType.Text;

                    con.Open();
                    resultado = cmd.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception ex)
            {
                resultado = false;
                Mensaje = ex.Message;
            }

            return resultado;
        }

        public bool CambiarContraseña(int idUsuario, string nuevaContraseña, out string Mensaje)
        {
            bool resultado = false;
            Mensaje = string.Empty;

            try
            {
                using (SqlConnection con = new SqlConnection(Conexion486LP.BD))
                {
                    string query = @"UPDATE Usuarios SET Contraseña = @Contraseña WHERE IdUsuario = @IdUsuario";

                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@IdUsuario", idUsuario);
                    cmd.Parameters.AddWithValue("@Contraseña", nuevaContraseña);
                    cmd.CommandType = CommandType.Text;

                    con.Open();
                    resultado = cmd.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception ex)
            {
                resultado = false;
                Mensaje = ex.Message;
            }

            return resultado;
        }

        // Se llama tras cada UPDATE 
        public bool ActualizarDV(int idUsuario, string dv, out string Mensaje)
        {
            bool resultado = false;
            Mensaje = string.Empty;

            try
            {
                using (SqlConnection con = new SqlConnection(Conexion486LP.BD))
                {
                    string query = @"UPDATE Usuarios SET DV = @DV WHERE IdUsuario = @IdUsuario";

                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@IdUsuario", idUsuario);
                    cmd.Parameters.AddWithValue("@DV", dv);
                    cmd.CommandType = CommandType.Text;

                    con.Open();
                    resultado = cmd.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception ex)
            {
                resultado = false;
                Mensaje = ex.Message;
            }

            return resultado;
        }

        public bool ActualizarIntentos(string nombreUsuario, int intentos, out string Mensaje)
        {
            bool resultado = false;
            Mensaje = string.Empty;

            try
            {
                using (SqlConnection con = new SqlConnection(Conexion486LP.BD))
                {
                    string query = @"UPDATE Usuarios SET IntentosFallidos = @Intentos WHERE NombreUsuario = @NombreUsuario";

                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@NombreUsuario", nombreUsuario);
                    cmd.Parameters.AddWithValue("@Intentos", intentos);
                    cmd.CommandType = CommandType.Text;

                    con.Open();
                    resultado = cmd.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception ex)
            {
                resultado = false;
                Mensaje = ex.Message;
            }

            return resultado;
        }

        //  Bloquear cuando IntentosFallidos llega a 3
        public bool Bloquear(string nombreUsuario, out string Mensaje)
        {
            bool resultado = false;
            Mensaje = string.Empty;

            try
            {
                using (SqlConnection con = new SqlConnection(Conexion486LP.BD))
                {
                    string query = @"UPDATE Usuarios SET Bloqueado = 1 WHERE NombreUsuario = @NombreUsuario";

                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@NombreUsuario", nombreUsuario);
                    cmd.CommandType = CommandType.Text;

                    con.Open();
                    resultado = cmd.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception ex)
            {
                resultado = false;
                Mensaje = ex.Message;
            }

            return resultado;
        }
        public bool Eliminar(int idUsuario, out string Mensaje)
        {
            bool resultado = false;
            Mensaje = string.Empty;

            try
            {
                using (SqlConnection con = new SqlConnection(Conexion486LP.BD))
                {
                    string query = @"DELETE FROM Usuarios WHERE IdUsuario = @IdUsuario";

                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@IdUsuario", idUsuario);
                    cmd.CommandType = CommandType.Text;

                    con.Open();
                    resultado = cmd.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception ex)
            {
                resultado = false;
                Mensaje = ex.Message;
            }

            return resultado;
        }
    }
}

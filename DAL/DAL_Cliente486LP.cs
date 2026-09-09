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
    public class DAL_Cliente486LP
    {
        // Mapea una fila del DataReader a un Cliente486LP (reutilizado por todos los SELECT).
        // El Correo se devuelve TAL CUAL esta en la base (cifrado en AES);
        // el descifrado lo hace la capa BLL.
        private Cliente486LP Mapear(SqlDataReader dr)
        {
            return new Cliente486LP()
            {
                DNI = dr["DNI"].ToString(),
                Nombre = dr["Nombre"].ToString(),
                Apellido = dr["Apellido"].ToString(),
                Correo = dr["Correo"].ToString(),
                Telefono = dr["Telefono"] == DBNull.Value ? "" : dr["Telefono"].ToString()
            };
        }

        // Lista todos los clientes.
        public List<Cliente486LP> Listar()
        {
            List<Cliente486LP> lista = new List<Cliente486LP>();

            try
            {
                using (SqlConnection con = new SqlConnection(Conexion486LP.BD))
                {
                    string query = @"SELECT DNI, Nombre, Apellido, Correo, Telefono
                    FROM Cliente";

                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.CommandType = CommandType.Text;
                    con.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(Mapear(dr));
                        }
                    }
                }
            }
            catch
            {
                lista = new List<Cliente486LP>();
            }

            return lista;
        }

        // Obtiene un cliente por su DNI (devuelve null si no existe).
        public Cliente486LP Obtener(string dni)
        {
            Cliente486LP cliente = null;

            try
            {
                using (SqlConnection con = new SqlConnection(Conexion486LP.BD))
                {
                    string query = @"SELECT DNI, Nombre, Apellido, Correo, Telefono
                    FROM Cliente WHERE DNI = @DNI";

                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@DNI", dni);
                    cmd.CommandType = CommandType.Text;
                    con.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            cliente = Mapear(dr);
                        }
                    }
                }
            }
            catch
            {
                cliente = null;
            }

            return cliente;
        }

        // Indica si ya existe un cliente con ese DNI.
        public bool Existe(string dni)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(Conexion486LP.BD))
                {
                    string query = @"SELECT COUNT(1) FROM Cliente WHERE DNI = @DNI";

                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@DNI", dni);
                    cmd.CommandType = CommandType.Text;
                    con.Open();

                    int cantidad = Convert.ToInt32(cmd.ExecuteScalar());
                    return cantidad > 0;
                }
            }
            catch
            {
                return false;
            }
        }

        public int Agregar(Cliente486LP cliente)
        {
            using (SqlConnection con = new SqlConnection(Conexion486LP.BD))
            {
                string query = @"INSERT INTO Cliente (DNI, Nombre, Apellido, Correo, Telefono)
                VALUES (@DNI, @Nombre, @Apellido, @Correo, @Telefono)";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@DNI", cliente.DNI);
                cmd.Parameters.AddWithValue("@Nombre", cliente.Nombre);
                cmd.Parameters.AddWithValue("@Apellido", cliente.Apellido);
                cmd.Parameters.AddWithValue("@Correo", cliente.Correo);
                cmd.Parameters.AddWithValue("@Telefono", (object)cliente.Telefono ?? DBNull.Value);
                cmd.CommandType = CommandType.Text;
                con.Open();

                return cmd.ExecuteNonQuery();
            }
        }

        // Modifica los datos de un cliente (por DNI). El Correo llega YA cifrado desde la BLL.
        public int Modificar(Cliente486LP cliente)
        {
            using (SqlConnection con = new SqlConnection(Conexion486LP.BD))
            {
                string query = @"UPDATE Cliente
                SET Nombre = @Nombre, Apellido = @Apellido, Correo = @Correo, Telefono = @Telefono
                WHERE DNI = @DNI";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@DNI", cliente.DNI);
                cmd.Parameters.AddWithValue("@Nombre", cliente.Nombre);
                cmd.Parameters.AddWithValue("@Apellido", cliente.Apellido);
                cmd.Parameters.AddWithValue("@Correo", cliente.Correo);
                cmd.Parameters.AddWithValue("@Telefono", (object)cliente.Telefono ?? DBNull.Value);
                cmd.CommandType = CommandType.Text;
                con.Open();

                return cmd.ExecuteNonQuery();
            }
        }

        // Baja fisica de un cliente por DNI.
        public int Eliminar(string dni)
        {
            using (SqlConnection con = new SqlConnection(Conexion486LP.BD))
            {
                string query = @"DELETE FROM Cliente WHERE DNI = @DNI";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@DNI", dni);
                cmd.CommandType = CommandType.Text;
                con.Open();

                return cmd.ExecuteNonQuery();
            }
        }
    }
}

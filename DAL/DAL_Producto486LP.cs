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
    public class DAL_Producto486LP
    {
        // Mapea una fila del DataReader a un Producto486LP (reutilizado por todos los SELECT)
        private Producto486LP Mapear(SqlDataReader dr)
        {
            return new Producto486LP()
            {
                IdProducto = Convert.ToInt32(dr["IdProducto"]),
                Marca = dr["Marca"].ToString(),
                Modelo = dr["Modelo"].ToString(),
                Color = dr["Color"].ToString(),
                Talle = dr["Talle"].ToString(),
                Precio = Convert.ToDecimal(dr["Precio"]),
                Stock = Convert.ToInt32(dr["Stock"])
            };
        }

        // Lista todos los productos
        public List<Producto486LP> Listar()
        {
            List<Producto486LP> lista = new List<Producto486LP>();

            try
            {
                using (SqlConnection con = new SqlConnection(Conexion486LP.BD))
                {
                    string query = @"SELECT IdProducto, Marca, Modelo, Color, Talle, Precio, Stock
                    FROM Producto";

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
                lista = new List<Producto486LP>();
            }

            return lista;
        }

        // Busca productos por filtros opcionales (Marca / Modelo / Color / Talle) y opcionalmente solo los que tengan stock disponible (Stock > 0).
        // Los filtros vacios se ignoran, si vienen todos vacios y soloConStock = false,devuelve el catalogo completo (equivale a Listar()).
        // NOTA: a diferencia de los otros metodos, este NO traga la excepcion: la deja
        // propagar para que BLL_Producto.Buscar la registre en bitacora (flujo alternativo 7.2).
        public List<Producto486LP> Buscar(string marca, string modelo, string color, string talle, bool soloConStock)
        {
            List<Producto486LP> lista = new List<Producto486LP>();

            using (SqlConnection con = new SqlConnection(Conexion486LP.BD))
            {
                StringBuilder query = new StringBuilder(
                    @"SELECT IdProducto, Marca, Modelo, Color, Talle, Precio, Stock
                    FROM Producto WHERE 1 = 1");

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandType = CommandType.Text;

                if (!string.IsNullOrWhiteSpace(marca))
                {
                    query.Append(" AND Marca LIKE @Marca");
                    cmd.Parameters.AddWithValue("@Marca", "%" + marca.Trim() + "%");
                }
                if (!string.IsNullOrWhiteSpace(modelo))
                {
                    query.Append(" AND Modelo LIKE @Modelo");
                    cmd.Parameters.AddWithValue("@Modelo", "%" + modelo.Trim() + "%");
                }
                if (!string.IsNullOrWhiteSpace(color))
                {
                    query.Append(" AND Color LIKE @Color");
                    cmd.Parameters.AddWithValue("@Color", "%" + color.Trim() + "%");
                }
                if (!string.IsNullOrWhiteSpace(talle))
                {
                    query.Append(" AND Talle LIKE @Talle");
                    cmd.Parameters.AddWithValue("@Talle", "%" + talle.Trim() + "%");
                }
                if (soloConStock)
                {
                    query.Append(" AND Stock > 0");
                }

                cmd.CommandText = query.ToString();
                con.Open();

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(Mapear(dr));
                    }
                }
            }

            return lista;
        }

        // Obtiene un producto por sus atributos (Marca, Modelo, Color, Talle)
        public Producto486LP ObtenerPorAtributos(string marca, string modelo, string color, string talle)
        {
            Producto486LP producto = null;

            try
            {
                using (SqlConnection con = new SqlConnection(Conexion486LP.BD))
                {
                    string query = @"SELECT IdProducto, Marca, Modelo, Color, Talle, Precio, Stock
                    FROM Producto
                    WHERE Marca = @Marca AND Modelo = @Modelo AND Color = @Color AND Talle = @Talle";

                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@Marca", marca);
                    cmd.Parameters.AddWithValue("@Modelo", modelo);
                    cmd.Parameters.AddWithValue("@Color", color);
                    cmd.Parameters.AddWithValue("@Talle", talle);
                    cmd.CommandType = CommandType.Text;
                    con.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            producto = Mapear(dr);
                        }
                    }
                }
            }
            catch
            {
                producto = null;
            }

            return producto;
        }

        // Obtiene un producto por su Id
        public Producto486LP ObtenerPorId(int idProducto)
        {
            Producto486LP producto = null;

            try
            {
                using (SqlConnection con = new SqlConnection(Conexion486LP.BD))
                {
                    string query = @"SELECT IdProducto, Marca, Modelo, Color, Talle, Precio, Stock
                    FROM Producto WHERE IdProducto = @IdProducto";

                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@IdProducto", idProducto);
                    cmd.CommandType = CommandType.Text;
                    con.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            producto = Mapear(dr);
                        }
                    }
                }
            }
            catch
            {
                producto = null;
            }

            return producto;
        }
    }
}

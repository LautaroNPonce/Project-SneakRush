using BE;
using DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mappers
{
    public class Mapper_Producto486LP : MapperBase486LP
    {
        // Lista todos los productos.
        public List<Producto486LP> Listar()
        {
            try
            {
                SqlCommand cmd = new SqlCommand("Producto_Listar");
                cmd.CommandType = CommandType.StoredProcedure;

                DataTable tabla = Conexion486LP.EjecutarConsulta(cmd);
                return ManejadorMapeo486LP.MapearLista<Producto486LP>(tabla);
            }
            catch
            {
                return new List<Producto486LP>();
            }
        }

        // Busca productos por filtros opcionales. Un filtro vacío se manda como NULL al SP, que lo ignora.
        // No traga la excepción: la deja propagar para que BLL_Producto.Buscar la registre en bitácora.
        public List<Producto486LP> Buscar(string marca, string modelo, string color, string talle, bool soloConStock)
        {
            SqlCommand cmd = new SqlCommand("Producto_Buscar");
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@Marca", string.IsNullOrWhiteSpace(marca) ? (object)DBNull.Value : marca.Trim()));
            cmd.Parameters.Add(new SqlParameter("@Modelo", string.IsNullOrWhiteSpace(modelo) ? (object)DBNull.Value : modelo.Trim()));
            cmd.Parameters.Add(new SqlParameter("@Color", string.IsNullOrWhiteSpace(color) ? (object)DBNull.Value : color.Trim()));
            cmd.Parameters.Add(new SqlParameter("@Talle", string.IsNullOrWhiteSpace(talle) ? (object)DBNull.Value : talle.Trim()));
            cmd.Parameters.Add(new SqlParameter("@SoloConStock", soloConStock));

            DataTable tabla = Conexion486LP.EjecutarConsulta(cmd);
            return ManejadorMapeo486LP.MapearLista<Producto486LP>(tabla);
        }

        // Obtiene un producto por sus atributos (Marca, Modelo, Color, Talle).
        public Producto486LP ObtenerPorAtributos(string marca, string modelo, string color, string talle)
        {
            try
            {
                SqlCommand cmd = new SqlCommand("Producto_ObtenerPorAtributos");
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@Marca", marca));
                cmd.Parameters.Add(new SqlParameter("@Modelo", modelo));
                cmd.Parameters.Add(new SqlParameter("@Color", color));
                cmd.Parameters.Add(new SqlParameter("@Talle", talle));

                DataTable tabla = Conexion486LP.EjecutarConsulta(cmd);
                return tabla.Rows.Count > 0 ? ManejadorMapeo486LP.MapearEntidad<Producto486LP>(tabla.Rows[0]) : null;
            }
            catch
            {
                return null;
            }
        }

        // Obtiene un producto por su Id.
        public Producto486LP ObtenerPorId(int idProducto)
        {
            try
            {
                SqlCommand cmd = new SqlCommand("Producto_ObtenerPorId");
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@IdProducto", idProducto));

                DataTable tabla = Conexion486LP.EjecutarConsulta(cmd);
                return tabla.Rows.Count > 0 ? ManejadorMapeo486LP.MapearEntidad<Producto486LP>(tabla.Rows[0]) : null;
            }
            catch
            {
                return null;
            }
        }

        // Descuenta stock tras una venta (CUN04). No compara filas afectadas para decidir éxito, mismo criterio de siempre.
        public bool ActualizarStock(int idProducto, int cantidad, out string mensaje)
        {
            mensaje = "";
            try
            {
                SqlCommand cmd = new SqlCommand("Producto_ActualizarStock");
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@IdProducto", idProducto));
                cmd.Parameters.Add(new SqlParameter("@Cantidad", cantidad));

                Conexion486LP.EjecutarNoConsulta(cmd);
                mensaje = "Stock actualizado correctamente.";
                return true;
            }
            catch (Exception ex)
            {
                mensaje = "Error: " + ex.Message;
                return false;
            }
        }

        // Suma stock tras confirmarse el pago de una compra (CUN08) - separado de ActualizarStock, que resta (Ventas). Mismo criterio: no compara filas afectadas.
        public bool AumentarStock(int idProducto, int cantidad, out string mensaje)
        {
            mensaje = "";
            try
            {
                SqlCommand cmd = new SqlCommand("Producto_AumentarStock");
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@IdProducto", idProducto));
                cmd.Parameters.Add(new SqlParameter("@Cantidad", cantidad));

                Conexion486LP.EjecutarNoConsulta(cmd);
                mensaje = "Stock actualizado correctamente.";
                return true;
            }
            catch (Exception ex)
            {
                mensaje = "Error: " + ex.Message;
                return false;
            }
        }

        // ----- ABM de productos (CUN10) -----
        // Estos metodos no tragan la excepcion: la BLL la registra en bitacora y devuelve el mensaje general.

        // Alta. Devuelve el Id generado
        public int Agregar(Producto486LP producto)
        {
            SqlCommand cmd = new SqlCommand("Producto_Agregar");
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@IdMarca", producto.IdMarca));
            cmd.Parameters.Add(new SqlParameter("@IdCategoria", producto.IdCategoria));
            cmd.Parameters.Add(new SqlParameter("@Modelo", producto.Modelo));
            cmd.Parameters.Add(new SqlParameter("@Color", producto.Color));
            cmd.Parameters.Add(new SqlParameter("@Talle", producto.Talle));
            cmd.Parameters.Add(new SqlParameter("@Precio", producto.Precio));
            cmd.Parameters.Add(new SqlParameter("@Stock", producto.Stock));

            return Convert.ToInt32(Conexion486LP.EjecutarEscalar(cmd));
        }

        // Modifica los datos del producto, menos el stock (el stock solo cambia con ventas y compras)
        public int Modificar(Producto486LP producto)
        {
            SqlCommand cmd = new SqlCommand("Producto_Modificar");
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@IdProducto", producto.IdProducto));
            cmd.Parameters.Add(new SqlParameter("@IdMarca", producto.IdMarca));
            cmd.Parameters.Add(new SqlParameter("@IdCategoria", producto.IdCategoria));
            cmd.Parameters.Add(new SqlParameter("@Modelo", producto.Modelo));
            cmd.Parameters.Add(new SqlParameter("@Color", producto.Color));
            cmd.Parameters.Add(new SqlParameter("@Talle", producto.Talle));
            cmd.Parameters.Add(new SqlParameter("@Precio", producto.Precio));

            return Conexion486LP.EjecutarNoConsulta(cmd);
        }

        // Baja fisica
        public int Eliminar(int idProducto)
        {
            SqlCommand cmd = new SqlCommand("Producto_Eliminar");
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@IdProducto", idProducto));

            return Conexion486LP.EjecutarNoConsulta(cmd);
        }

        // Indica si ya existe otro producto con la misma marca, modelo, color y talle. idExcluir es el que se esta modificando (0 al dar de alta)
        public bool ExisteCombinacion(int idMarca, string modelo, string color, string talle, int idExcluir)
        {
            SqlCommand cmd = new SqlCommand("Producto_ExisteCombinacion");
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@IdMarca", idMarca));
            cmd.Parameters.Add(new SqlParameter("@Modelo", modelo));
            cmd.Parameters.Add(new SqlParameter("@Color", color));
            cmd.Parameters.Add(new SqlParameter("@Talle", talle));
            cmd.Parameters.Add(new SqlParameter("@IdExcluir", idExcluir));

            return Convert.ToInt32(Conexion486LP.EjecutarEscalar(cmd)) > 0;
        }

        // Indica si el producto aparece en algun carrito, venta, solicitud, orden o recepcion
        public bool TieneMovimientos(int idProducto)
        {
            SqlCommand cmd = new SqlCommand("Producto_TieneMovimientos");
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@IdProducto", idProducto));

            return Convert.ToInt32(Conexion486LP.EjecutarEscalar(cmd)) > 0;
        }
    }
}
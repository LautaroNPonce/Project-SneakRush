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
            cmd.Parameters.Add(new SqlParameter("@Stock", producto.Stock));

            return Conexion486LP.EjecutarNoConsulta(cmd);
        }

        public int Eliminar(int idProducto)
        {
            SqlCommand cmd = new SqlCommand("Producto_Eliminar");
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@IdProducto", idProducto));

            return Conexion486LP.EjecutarNoConsulta(cmd);
        }

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

        public bool TieneMovimientos(int idProducto)
        {
            SqlCommand cmd = new SqlCommand("Producto_TieneMovimientos");
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@IdProducto", idProducto));

            return Convert.ToInt32(Conexion486LP.EjecutarEscalar(cmd)) > 0;
        }
    }
}
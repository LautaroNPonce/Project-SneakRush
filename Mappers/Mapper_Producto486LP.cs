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
    /// Habla con SQL para la entidad Producto. Reemplaza a DAL_Producto486LP.
    /// Producto hoy es de solo lectura (no hay ABM todavia, eso es CUN11).
    
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

        // Busca productos por filtros opcionales. Un filtro vacio/blanco se manda
        // como NULL al SP, que lo ignora (mismo comportamiento que tenia la DAL).
        // NOTA: no traga la excepcion, la deja propagar para que BLL_Producto.Buscar
        // la registre en bitacora (flujo alternativo 7.2) - igual que antes.
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
    }
}
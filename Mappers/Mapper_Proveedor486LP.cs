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
    public class Mapper_Proveedor486LP : MapperBase486LP
    {
        public List<Proveedor486LP> Listar()
        {
            try
            {
                SqlCommand cmd = new SqlCommand("Proveedor_Listar");
                cmd.CommandType = CommandType.StoredProcedure;

                DataTable tabla = Conexion486LP.EjecutarConsulta(cmd);
                return ManejadorMapeo486LP.MapearLista<Proveedor486LP>(tabla);
            }
            catch
            {
                return new List<Proveedor486LP>();
            }
        }

        // Suma stock (compra)  separado de ActualizarStock, que resta (usado por Ventas).
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
    }
}

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
    public class Mapper_OrdenCompra486LP : MapperBase486LP
    {
        public bool Agregar(OrdenCompra486LP orden, out string mensaje)
        {
            mensaje = "";

            SqlConnection con = Conexion486LP.AbrirConexion();
            SqlTransaction tran = con.BeginTransaction();

            try
            {
                SqlCommand cmdCab = new SqlCommand("OrdenCompra_Agregar");
                cmdCab.CommandType = CommandType.StoredProcedure;
                cmdCab.Parameters.Add(new SqlParameter("@Fecha", orden.Fecha));
                cmdCab.Parameters.Add(new SqlParameter("@IdProveedor", orden.IdProveedor));
                cmdCab.Parameters.Add(new SqlParameter("@IdSolicitud", orden.IdSolicitud));
                cmdCab.Parameters.Add(new SqlParameter("@CostoTotal", orden.CostoTotal));

                int idOrden = (int)Conexion486LP.EjecutarEscalarEnTransaccion(cmdCab, con, tran);
                orden.IdOrden = idOrden;
                orden.NroOrden = idOrden.ToString("D6"); //El SP genera el NroOrden con el mismo padding(6 digitos)
                orden.Estado = "Pendiente de Recepción";

                foreach (DetalleOrdenCompra486LP det in orden.Detalles)
                {
                    SqlCommand cmdDet = new SqlCommand("DetalleOrdenCompra_Agregar");
                    cmdDet.CommandType = CommandType.StoredProcedure;
                    cmdDet.Parameters.Add(new SqlParameter("@IdOrden", idOrden));
                    cmdDet.Parameters.Add(new SqlParameter("@IdProducto", det.IdProducto));
                    cmdDet.Parameters.Add(new SqlParameter("@Cantidad", det.Cantidad));
                    cmdDet.Parameters.Add(new SqlParameter("@CostoUnitario", det.CostoUnitario));
                    cmdDet.Parameters.Add(new SqlParameter("@Subtotal", det.Subtotal));

                    Conexion486LP.EjecutarNoConsultaEnTransaccion(cmdDet, con, tran);
                }

                tran.Commit();
                mensaje = "Orden de compra registrada correctamente.";
                return true;
            }
            catch (Exception ex)
            {
                try { tran.Rollback(); } catch { }
                mensaje = "Error: " + ex.Message;
                return false;
            }
            finally
            {
                con.Close();
            }
        }

        // Trae una orden ya registrada, con su detalle completo.
        public OrdenCompra486LP ObtenerPorId(int idOrden)
        {
            try
            {
                SqlCommand cmdCab = new SqlCommand("OrdenCompra_ObtenerPorId");
                cmdCab.CommandType = CommandType.StoredProcedure;
                cmdCab.Parameters.Add(new SqlParameter("@IdOrden", idOrden));

                DataTable tablaCab = Conexion486LP.EjecutarConsulta(cmdCab);
                if (tablaCab.Rows.Count == 0) return null;

                OrdenCompra486LP orden = ManejadorMapeo486LP.MapearEntidad<OrdenCompra486LP>(tablaCab.Rows[0]);

                SqlCommand cmdDet = new SqlCommand("DetalleOrdenCompra_ListarPorOrden");
                cmdDet.CommandType = CommandType.StoredProcedure;
                cmdDet.Parameters.Add(new SqlParameter("@IdOrden", idOrden));

                DataTable tablaDet = Conexion486LP.EjecutarConsulta(cmdDet);
                orden.Detalles = ManejadorMapeo486LP.MapearLista<DetalleOrdenCompra486LP>(tablaDet);

                return orden;
            }
            catch
            {
                return null;
            }
        }

        // Costo mas reciente pagado por este producto, dato de referencia para CUN06.
        public decimal? ObtenerUltimoCosto(int idProducto)
        {
            try
            {
                SqlCommand cmd = new SqlCommand("DetalleOrdenCompra_ObtenerUltimoCosto");
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@IdProducto", idProducto));

                object resultado = Conexion486LP.EjecutarEscalar(cmd);
                return (resultado == null || resultado == DBNull.Value) ? (decimal?)null : Convert.ToDecimal(resultado);
            }
            catch
            {
                return null;
            }
        }

        // Lista las ordenes en estado "Pendiente de Recepción" (CUN07 las necesita)
        public List<OrdenCompra486LP> ListarPendientesDeRecepcion()
        {
            try
            {
                SqlCommand cmd = new SqlCommand("OrdenCompra_ListarPendientesDeRecepcion");
                cmd.CommandType = CommandType.StoredProcedure;

                DataTable tabla = Conexion486LP.EjecutarConsulta(cmd);
                return ManejadorMapeo486LP.MapearLista<OrdenCompra486LP>(tabla);
            }
            catch
            {
                return new List<OrdenCompra486LP>();
            }
        }

        // Cambia el Estado de la orden (ej: a "Recibida" cuando CUN07 registra la recepcion, o a "Facturada" cuando CUN08 registra la factura).
        public bool ActualizarEstado(int idOrden, string estado, out string mensaje)
        {
            mensaje = "";
            try
            {
                SqlCommand cmd = new SqlCommand("OrdenCompra_ActualizarEstado");
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@IdOrden", idOrden));
                cmd.Parameters.Add(new SqlParameter("@Estado", estado));

                Conexion486LP.EjecutarNoConsulta(cmd);
                mensaje = "Estado actualizado correctamente.";
                return true;
            }
            catch (Exception ex)
            {
                mensaje = "Error: " + ex.Message;
                return false;
            }
        }

        // Lista las ordenes en estado "Recibida" (CUN08 las necesita para armar la lista de seleccion de facturacion).
        public List<OrdenCompra486LP> ListarPendientesDeFacturacion()
        {
            try
            {
                SqlCommand cmd = new SqlCommand("OrdenCompra_ListarPendientesDeFacturacion");
                cmd.CommandType = CommandType.StoredProcedure;

                DataTable tabla = Conexion486LP.EjecutarConsulta(cmd);
                return ManejadorMapeo486LP.MapearLista<OrdenCompra486LP>(tabla);
            }
            catch
            {
                return new List<OrdenCompra486LP>();
            }
        }
    }
}

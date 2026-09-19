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

    public class Mapper_Venta486LP : MapperBase486LP
    {
        // Da de alta una venta (cabecera + detalle), todo en una sola transaccion - mismo patron que
        // Mapper_Carrito486LP.Guardar (la unica otra entidad de negocio con relacion cabecera-detalle).
        public bool Agregar(Venta486LP venta, out string mensaje)
        {
            mensaje = "";

            SqlConnection con = Conexion486LP.AbrirConexion();
            SqlTransaction tran = con.BeginTransaction();

            try
            {
                SqlCommand cmdCab = new SqlCommand("Venta_Agregar");
                cmdCab.CommandType = CommandType.StoredProcedure;
                cmdCab.Parameters.Add(new SqlParameter("@Fecha", venta.Fecha));
                cmdCab.Parameters.Add(new SqlParameter("@DNICliente", venta.DNICliente));
                cmdCab.Parameters.Add(new SqlParameter("@MedioPago", venta.MedioPago));
                cmdCab.Parameters.Add(new SqlParameter("@Total", venta.Total));

                int idVenta = (int)Conexion486LP.EjecutarEscalarEnTransaccion(cmdCab, con, tran);
                venta.IdVenta = idVenta;

                // El SP genera el NroComprobante con el mismo padding (6 digitos, ej. "000123") - se
                // replica aca el mismo calculo para no tener que hacer una consulta extra solo para leerlo.
                venta.NroComprobante = idVenta.ToString("D6");

                foreach (DetalleVenta486LP det in venta.Detalles)
                {
                    SqlCommand cmdDet = new SqlCommand("DetalleVenta_Agregar");
                    cmdDet.CommandType = CommandType.StoredProcedure;
                    cmdDet.Parameters.Add(new SqlParameter("@IdVenta", idVenta));
                    cmdDet.Parameters.Add(new SqlParameter("@IdProducto", det.IdProducto));
                    cmdDet.Parameters.Add(new SqlParameter("@Cantidad", det.Cantidad));
                    cmdDet.Parameters.Add(new SqlParameter("@PrecioUnitario", det.PrecioUnitario));
                    cmdDet.Parameters.Add(new SqlParameter("@Subtotal", det.Subtotal));

                    Conexion486LP.EjecutarNoConsultaEnTransaccion(cmdDet, con, tran);
                }

                tran.Commit();
                mensaje = "Venta registrada correctamente.";
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

        // Trae una venta ya registrada, con su detalle completo (para "Ver comprobante").
        public Venta486LP ObtenerPorId(int idVenta)
        {
            try
            {
                SqlCommand cmdCab = new SqlCommand("Venta_ObtenerPorId");
                cmdCab.CommandType = CommandType.StoredProcedure;
                cmdCab.Parameters.Add(new SqlParameter("@IdVenta", idVenta));

                DataTable tablaCab = Conexion486LP.EjecutarConsulta(cmdCab);
                if (tablaCab.Rows.Count == 0) return null;

                Venta486LP venta = ManejadorMapeo486LP.MapearEntidad<Venta486LP>(tablaCab.Rows[0]);

                SqlCommand cmdDet = new SqlCommand("DetalleVenta_ListarPorVenta");
                cmdDet.CommandType = CommandType.StoredProcedure;
                cmdDet.Parameters.Add(new SqlParameter("@IdVenta", idVenta));

                DataTable tablaDet = Conexion486LP.EjecutarConsulta(cmdDet);
                venta.Detalles = ManejadorMapeo486LP.MapearLista<DetalleVenta486LP>(tablaDet);

                return venta;
            }
            catch
            {
                return null;
            }
        }
    }
}

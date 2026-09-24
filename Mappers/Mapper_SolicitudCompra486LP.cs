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
    /// Habla con SQL para la entidad SolicitudCompra. Mismo patron que Mapper_Venta486LP: nace
    /// directamente con Mapper (nunca tuvo una DAL_SolicitudCompra486LP propia).

    public class Mapper_SolicitudCompra486LP : MapperBase486LP
    {
        // Da de alta una solicitud (cabecera + detalle), todo en una sola transaccion.
        public bool Agregar(SolicitudCompra486LP solicitud, out string mensaje)
        {
            mensaje = "";

            SqlConnection con = Conexion486LP.AbrirConexion();
            SqlTransaction tran = con.BeginTransaction();

            try
            {
                SqlCommand cmdCab = new SqlCommand("SolicitudCompra_Agregar");
                cmdCab.CommandType = CommandType.StoredProcedure;
                cmdCab.Parameters.Add(new SqlParameter("@Fecha", solicitud.Fecha));
                cmdCab.Parameters.Add(new SqlParameter("@DNIAdministrador", solicitud.DNIAdministrador));
                cmdCab.Parameters.Add(new SqlParameter("@Estado", solicitud.Estado));

                int idSolicitud = (int)Conexion486LP.EjecutarEscalarEnTransaccion(cmdCab, con, tran);
                solicitud.IdSolicitud = idSolicitud;

                foreach (DetalleSolicitudCompra486LP det in solicitud.Detalles)
                {
                    SqlCommand cmdDet = new SqlCommand("DetalleSolicitudCompra_Agregar");
                    cmdDet.CommandType = CommandType.StoredProcedure;
                    cmdDet.Parameters.Add(new SqlParameter("@IdSolicitud", idSolicitud));
                    cmdDet.Parameters.Add(new SqlParameter("@IdProducto", det.IdProducto));
                    cmdDet.Parameters.Add(new SqlParameter("@CantidadSolicitada", det.CantidadSolicitada));

                    Conexion486LP.EjecutarNoConsultaEnTransaccion(cmdDet, con, tran);
                }

                tran.Commit();
                mensaje = "Solicitud de compra registrada correctamente.";
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

        // Trae una solicitud ya registrada, con su detalle completo (CUN06 la va a necesitar).
        public SolicitudCompra486LP ObtenerPorId(int idSolicitud)
        {
            try
            {
                SqlCommand cmdCab = new SqlCommand("SolicitudCompra_ObtenerPorId");
                cmdCab.CommandType = CommandType.StoredProcedure;
                cmdCab.Parameters.Add(new SqlParameter("@IdSolicitud", idSolicitud));

                DataTable tablaCab = Conexion486LP.EjecutarConsulta(cmdCab);
                if (tablaCab.Rows.Count == 0) return null;

                SolicitudCompra486LP solicitud = ManejadorMapeo486LP.MapearEntidad<SolicitudCompra486LP>(tablaCab.Rows[0]);

                SqlCommand cmdDet = new SqlCommand("DetalleSolicitudCompra_ListarPorSolicitud");
                cmdDet.CommandType = CommandType.StoredProcedure;
                cmdDet.Parameters.Add(new SqlParameter("@IdSolicitud", idSolicitud));

                DataTable tablaDet = Conexion486LP.EjecutarConsulta(cmdDet);
                solicitud.Detalles = ManejadorMapeo486LP.MapearLista<DetalleSolicitudCompra486LP>(tablaDet);

                return solicitud;
            }
            catch
            {
                return null;
            }
        }
    }
}

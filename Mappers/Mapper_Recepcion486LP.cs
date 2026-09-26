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
        /// Habla con SQL para la entidad Recepcion. Mismo patron cabecera-detalle que los demas
        /// Mappers de negocio con esa relacion.

        public class Mapper_Recepcion486LP : MapperBase486LP
        {
            // Alta de la recepcion (cabecera + detalle), todo en una sola transaccion.
            public bool Agregar(Recepcion486LP recepcion, out string mensaje)
            {
                mensaje = "";

                SqlConnection con = Conexion486LP.AbrirConexion();
                SqlTransaction tran = con.BeginTransaction();

                try
                {
                    SqlCommand cmdCab = new SqlCommand("Recepcion_Agregar");
                    cmdCab.CommandType = CommandType.StoredProcedure;
                    cmdCab.Parameters.Add(new SqlParameter("@Fecha", recepcion.Fecha));
                    cmdCab.Parameters.Add(new SqlParameter("@IdOrden", recepcion.IdOrden));

                    int idRecepcion = (int)Conexion486LP.EjecutarEscalarEnTransaccion(cmdCab, con, tran);
                    recepcion.IdRecepcion = idRecepcion;

                    // Mismo padding (6 digitos) que el SP genera - se replica aca para no tener que
                    // hacer una consulta extra solo para leerlo.
                    recepcion.NroRecepcion = idRecepcion.ToString("D6");

                    foreach (DetalleRecepcion486LP det in recepcion.Detalles)
                    {
                        SqlCommand cmdDet = new SqlCommand("DetalleRecepcion_Agregar");
                        cmdDet.CommandType = CommandType.StoredProcedure;
                        cmdDet.Parameters.Add(new SqlParameter("@IdRecepcion", idRecepcion));
                        cmdDet.Parameters.Add(new SqlParameter("@IdProducto", det.IdProducto));
                        cmdDet.Parameters.Add(new SqlParameter("@CantidadRecibida", det.CantidadRecibida));
                        cmdDet.Parameters.Add(new SqlParameter("@CantidadFaltante", det.CantidadFaltante));

                        Conexion486LP.EjecutarNoConsultaEnTransaccion(cmdDet, con, tran);
                    }

                    tran.Commit();
                    mensaje = "Recepción registrada correctamente.";
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

            // Trae la Recepcion de una Orden puntual, con su detalle completo (CUN08 la necesita
            // para saber la Cantidad Recibida real de cada producto, y asi aumentar el stock).
            public Recepcion486LP ObtenerPorIdOrden(int idOrden)
            {
                try
                {
                    SqlCommand cmdCab = new SqlCommand("Recepcion_ObtenerPorIdOrden");
                    cmdCab.CommandType = CommandType.StoredProcedure;
                    cmdCab.Parameters.Add(new SqlParameter("@IdOrden", idOrden));

                    DataTable tablaCab = Conexion486LP.EjecutarConsulta(cmdCab);
                    if (tablaCab.Rows.Count == 0) return null;

                    Recepcion486LP recepcion = ManejadorMapeo486LP.MapearEntidad<Recepcion486LP>(tablaCab.Rows[0]);

                    SqlCommand cmdDet = new SqlCommand("DetalleRecepcion_ListarPorRecepcion");
                    cmdDet.CommandType = CommandType.StoredProcedure;
                    cmdDet.Parameters.Add(new SqlParameter("@IdRecepcion", recepcion.IdRecepcion));

                    DataTable tablaDet = Conexion486LP.EjecutarConsulta(cmdDet);
                    recepcion.Detalles = ManejadorMapeo486LP.MapearLista<DetalleRecepcion486LP>(tablaDet);

                    return recepcion;
                }
                catch
                {
                    return null;
                }
            }
        }
    }
}

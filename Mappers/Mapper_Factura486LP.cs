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
    public class Mapper_Factura486LP : MapperBase486LP
    {
        public bool Agregar(Factura486LP factura, out string mensaje)
        {
            mensaje = "";
            try
            {
                SqlCommand cmd = new SqlCommand("Factura_Agregar");
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@Fecha", factura.Fecha));
                cmd.Parameters.Add(new SqlParameter("@IdOrden", factura.IdOrden));
                cmd.Parameters.Add(new SqlParameter("@Total", factura.Total));

                object resultado = Conexion486LP.EjecutarEscalar(cmd);
                int idFactura = Convert.ToInt32(resultado);

                factura.IdFactura = idFactura;
                factura.NroFactura = idFactura.ToString("D6");
                factura.Estado = "Pendiente de Pago";

                mensaje = "Factura registrada correctamente.";
                return true;
            }
            catch (Exception ex)
            {
                mensaje = "Error: " + ex.Message;
                return false;
            }
        }

        public bool ActualizarEstadoYPago(int idFactura, string estado, string medioPago, out string mensaje)
        {
            mensaje = "";
            try
            {
                SqlCommand cmd = new SqlCommand("Factura_ActualizarEstadoYPago");
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@IdFactura", idFactura));
                cmd.Parameters.Add(new SqlParameter("@Estado", estado));
                cmd.Parameters.Add(new SqlParameter("@MedioPago", medioPago));

                Conexion486LP.EjecutarNoConsulta(cmd);
                mensaje = "Estado y medio de pago actualizados correctamente.";
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

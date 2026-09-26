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
    /// Habla con SQL para la entidad Factura. Sin transaccion cabecera-detalle (no tiene
    /// detalle propio) - mas simple que el resto de los Mappers de negocio.

    public class Mapper_Factura486LP : MapperBase486LP
    {
        // Alta de la factura, arranca en "Pendiente de Pago" con MedioPago en null (se completa
        // recien cuando se confirma el pago, via ActualizarEstadoYPago).
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
                // Mismo padding (6 digitos) que el SP genera.
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

        // Cambia el Estado a "Pagada" y guarda el MedioPago usado, en un solo UPDATE.
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

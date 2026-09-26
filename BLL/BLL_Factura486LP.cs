using BE;
using Mappers;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class BLL_Factura486LP
    {
        private Mapper_Factura486LP ObjetoMapper = new Mapper_Factura486LP();
        private BLL_OrdenCompra486LP ObjOrden = new BLL_OrdenCompra486LP();
        private BLL_Producto486LP ObjProducto = new BLL_Producto486LP();
        private BLL_Bitacora486LP ObjBitacora = new BLL_Bitacora486LP();

        // Registra la factura (solo cabecera) a partir de la Orden elegida y su Total ya
        // calculado en pantalla. El pago se confirma aparte, en ConfirmarPago - recien ahi
        // se actualiza stock y se cierra la orden (asi el DV de Factura queda como "alta" acá,
        // y como "actualizacion de una fila" alla, mismo criterio de siempre).
        public Factura486LP RegistrarFactura(Factura486LP factura, out string mensaje)
        {
            mensaje = "";

            try
            {
                if (factura == null || factura.IdOrden <= 0)
                {
                    mensaje = "Debe seleccionar una orden de compra.";
                    return null;
                }

                factura.Fecha = DateTime.Now;

                bool ok = ObjetoMapper.Agregar(factura, out mensaje);
                if (!ok)
                {
                    ObjBitacora.Registrar(new BitacoraEvento486LP("Compras", $"Error al registrar factura de compra: {mensaje}", Criticidad486LP.MuyAlta, "Sistema", "Sistema"));
                    return null;
                }

                string mensajeDV;
                BLL_DV486LP bllDV = new BLL_DV486LP();
                bllDV.RecalcularDV("Factura", out mensajeDV);

                mensaje = "Factura registrada correctamente.";
                return factura;
            }
            catch (Exception ex)
            {
                mensaje = ex.Message;
                ObjBitacora.Registrar(new BitacoraEvento486LP("Compras", $"Error en BLL_Factura.RegistrarFactura(): {ex.Message}", Criticidad486LP.MuyAlta, "Sistema", "Sistema"));
                return null;
            }
        }

        // Confirma el pago (llamado tras la aprobacion simulada de la Entidad Bancaria): pasa
        // la Factura a "Pagada" con el medio de pago usado, aumenta el stock de cada producto
        // segun lo efectivamente recibido (detalleRecepcion), y cierra la Orden como "Facturada".
        // detalleRecepcion se lo pasa el Form, que ya lo consulto para mostrar la pantalla - no
        // le corresponde a esta BLL volver a pedirselo a BLL_Recepcion486LP solo para repetir
        // una consulta que ya se hizo.
        public bool ConfirmarPago(Factura486LP factura, string medioPago, List<DetalleRecepcion486LP> detalleRecepcion, out string mensaje)
        {
            mensaje = "";

            try
            {
                bool ok = ObjetoMapper.ActualizarEstadoYPago(factura.IdFactura, "Pagada", medioPago, out mensaje);
                if (!ok)
                {
                    ObjBitacora.Registrar(new BitacoraEvento486LP("Compras", $"Error al confirmar el pago de la factura: {mensaje}", Criticidad486LP.MuyAlta, "Sistema", "Sistema"));
                    return false;
                }

                foreach (DetalleRecepcion486LP det in detalleRecepcion)
                {
                    string mensajeStock;
                    ObjProducto.AumentarStock(det.IdProducto, det.CantidadRecibida, out mensajeStock);
                }

                string mensajeOrden;
                ObjOrden.ActualizarEstado(factura.IdOrden, "Facturada", out mensajeOrden);

                string dni = SessionManager486LP.ObtenerInstancia().UsuarioActual()?.DNI ?? "Sistema";
                string nombreUsuario = SessionManager486LP.ObtenerInstancia().UsuarioActual()?.NombreUsuario ?? "Sistema";

                ObjBitacora.Registrar(new BitacoraEvento486LP("Compras",
                    $"Factura {factura.NroFactura} pagada ({medioPago}), Total ${factura.Total}. Stock actualizado para {detalleRecepcion.Count} producto(s).",
                    Criticidad486LP.Alta, dni, nombreUsuario));

                string mensajeDV;
                BLL_DV486LP bllDV = new BLL_DV486LP();
                bllDV.RegistrarDVDeFilaNueva("Factura", factura.IdFactura, out mensajeDV);

                mensaje = "Pago confirmado correctamente.";
                return true;
            }
            catch (Exception ex)
            {
                mensaje = ex.Message;
                ObjBitacora.Registrar(new BitacoraEvento486LP("Compras", $"Error en BLL_Factura.ConfirmarPago(): {ex.Message}", Criticidad486LP.MuyAlta, "Sistema", "Sistema"));
                return false;
            }
        }
    }
}

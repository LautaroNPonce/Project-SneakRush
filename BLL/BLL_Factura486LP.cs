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

        // Registra la factura (solo cabecera) con el total ya calculado en pantalla. El pago se confirma aparte, en confirmarpago recién ahí se actualiza stock y se cierra la orden
        public Factura486LP RegistrarFactura(Factura486LP factura, out string mensaje)
        {
            mensaje = "";

            try
            {
                if (factura == null || factura.IdOrden <= 0)
                {
                    mensaje = Mensajes486LP.Obtener("Compra.FacturaSinOrden"); // Muestra: Debe seleccionar una orden de compra
                    return null;
                }

                factura.Fecha = DateTime.Now;

                string mensajeMapper;
                bool ok = ObjetoMapper.Agregar(factura, out mensajeMapper);
                if (!ok)
                {
                    // El texto del Mapper es tecnico: va a la bitacora y el usuario ve un mensaje general
                    ObjBitacora.Registrar(new BitacoraEvento486LP("Compras", $"Error al registrar factura de compra: {mensajeMapper}", Criticidad486LP.MuyAlta, "Sistema", "Sistema"));
                    mensaje = Mensajes486LP.Obtener("General.ErrorInesperado"); // Muestra: Ocurrió un error inesperado. El detalle quedó registrado en la bitácora
                    return null;
                }

                string mensajeDV;
                BLL_DV486LP bllDV = new BLL_DV486LP();
                bllDV.RecalcularDV("Factura", out mensajeDV);

                mensaje = Mensajes486LP.Obtener("Compra.FacturaRegistrada"); // Muestra: Factura registrada correctamente
                return factura;
            }
            catch (Exception ex)
            {
                mensaje = Mensajes486LP.Obtener("General.ErrorInesperado"); // Muestra: Ocurrió un error inesperado. El detalle quedó registrado en la bitácora
                ObjBitacora.Registrar(new BitacoraEvento486LP("Compras", $"Error en BLL_Factura.RegistrarFactura(): {ex.Message}", Criticidad486LP.MuyAlta, "Sistema", "Sistema"));
                return null;
            }
        }

        // Confirma el pago y pasa la factura a pagada, aumenta stock según lo recibido (detalleRecepcion) y cierra la orden facturada
        public bool ConfirmarPago(Factura486LP factura, string medioPago, List<DetalleRecepcion486LP> detalleRecepcion, out string mensaje)
        {
            mensaje = "";

            try
            {
                string mensajeMapper;
                bool ok = ObjetoMapper.ActualizarEstadoYPago(factura.IdFactura, "Pagada", medioPago, out mensajeMapper);
                if (!ok)
                {
                    // El texto del Mapper es tecnico: va a la bitacora y el usuario ve un mensaje general
                    ObjBitacora.Registrar(new BitacoraEvento486LP("Compras", $"Error al confirmar el pago de la factura: {mensajeMapper}", Criticidad486LP.MuyAlta, "Sistema", "Sistema"));
                    mensaje = Mensajes486LP.Obtener("General.ErrorInesperado"); // Muestra: Ocurrió un error inesperado. El detalle quedó registrado en la bitácora
                    return false;
                }

                // Lo utilizo para que el la factura que tengo en memoria quede actualizada con el estado y medio de pago, para el mensaje de bitacora
                factura.Estado = "Pagada";
                factura.MedioPago = medioPago;

                foreach (DetalleRecepcion486LP det in detalleRecepcion)
                {
                    // Un producto que no llego (CantidadRecibida = 0) no suma stock
                    if (det.CantidadRecibida <= 0) continue;

                    string mensajeStock;
                    if (!ObjProducto.AumentarStock(det.IdProducto, det.CantidadRecibida, out mensajeStock))
                    {
                        ObjBitacora.Registrar(new BitacoraEvento486LP("Compras", $"No se pudo sumar el stock del producto Id {det.IdProducto} de la factura {factura.NroFactura}: {mensajeStock}", Criticidad486LP.MuyAlta, "Sistema", "Sistema"));
                    }
                }

                string mensajeOrden;
                if (!ObjOrden.ActualizarEstado(factura.IdOrden, "Facturada", out mensajeOrden))
                {
                    ObjBitacora.Registrar(new BitacoraEvento486LP("Compras", $"No se pudo pasar la Orden #{factura.IdOrden} a Facturada: {mensajeOrden}", Criticidad486LP.MuyAlta, "Sistema", "Sistema"));
                }

                string dni = SessionManager486LP.ObtenerInstancia().UsuarioActual()?.DNI ?? "Sistema";
                string nombreUsuario = SessionManager486LP.ObtenerInstancia().UsuarioActual()?.NombreUsuario ?? "Sistema";

                ObjBitacora.Registrar(new BitacoraEvento486LP("Compras",
                    $"Factura {factura.NroFactura} pagada ({medioPago}), Total ${factura.Total}. Stock actualizado para {detalleRecepcion.Count} producto(s).",
                    Criticidad486LP.Alta, dni, nombreUsuario));

                string mensajeDV;
                BLL_DV486LP bllDV = new BLL_DV486LP();
                bllDV.RegistrarDVDeFilaNueva("Factura", factura.IdFactura, out mensajeDV);

                mensaje = Mensajes486LP.Obtener("Compra.PagoConfirmado"); // Muestra: Pago confirmado correctamente
                return true;
            }
            catch (Exception ex)
            {
                mensaje = Mensajes486LP.Obtener("General.ErrorInesperado"); // Muestra: Ocurrió un error inesperado. El detalle quedó registrado en la bitácora
                ObjBitacora.Registrar(new BitacoraEvento486LP("Compras", $"Error en BLL_Factura.ConfirmarPago(): {ex.Message}", Criticidad486LP.MuyAlta, "Sistema", "Sistema"));
                return false;
            }
        }
    }
}

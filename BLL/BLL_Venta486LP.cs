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
    public class BLL_Venta486LP
    {
        private Mapper_Venta486LP ObjetoMapper = new Mapper_Venta486LP();
        private BLL_Producto486LP ObjProducto = new BLL_Producto486LP();
        private BLL_Carrito486LP ObjCarrito = new BLL_Carrito486LP();
        private BLL_Bitacora486LP ObjBitacora = new BLL_Bitacora486LP();

        // Registra la venta a partir de un carrito ya armado (CUN02): arma la cabecera+detalle, persiste, descuenta stock producto por producto, cierra el carrito de origen (pasa a
        // "Facturado"), recalcula DV de las 3 tablas afectadas (Venta, DetalleVenta, Producto queda resuelto adentro de ActualizarStock) y registra en bitacora. Todo o nada a nivel de negocio:
        // si algun paso posterior a la venta falla, se informa en el mensaje pero la Venta en si ya quedo persistida (siempre se aprueba el pago en este proyecto, no hay reversion posible).
        public Venta486LP RegistrarVenta(Carrito486LP carrito, string medioPago, out string mensaje)
        {
            mensaje = "";

            try
            {
                if (carrito == null || carrito.Detalles == null || carrito.Detalles.Count == 0)
                {
                    mensaje = "El carrito no tiene productos.";
                    return null;
                }
                if (string.IsNullOrEmpty(carrito.DNICliente))
                {
                    mensaje = "El carrito no tiene un cliente asignado.";
                    return null;
                }

                Venta486LP venta = new Venta486LP
                {
                    Fecha = DateTime.Now,
                    DNICliente = carrito.DNICliente,
                    MedioPago = medioPago,
                    Total = carrito.Total
                };

                foreach (DetalleCarrito486LP detCarrito in carrito.Detalles)
                {
                    venta.Detalles.Add(new DetalleVenta486LP
                    {
                        IdProducto = detCarrito.IdProducto,
                        Cantidad = detCarrito.Cantidad,
                        PrecioUnitario = detCarrito.Precio,
                        Subtotal = detCarrito.Subtotal
                    });
                }

                bool ok = ObjetoMapper.Agregar(venta, out mensaje);
                if (!ok)
                {
                    ObjBitacora.Registrar(new BitacoraEvento486LP("Ventas", $"Error al registrar venta (Cliente DNI {carrito.DNICliente}): {mensaje}", Criticidad486LP.MuyAlta, "Sistema", "Sistema"));
                    return null;
                }

                // Paso 11 del escenario principal: actualizar stock de cada producto vendido.
                foreach (DetalleVenta486LP det in venta.Detalles)
                {
                    string mensajeStock;
                    ObjProducto.ActualizarStock(det.IdProducto, det.Cantidad, out mensajeStock);
                }

                // El carrito de origen pasa a "Facturado": no vuelve a aparecer como Activo.
                string mensajeCarrito;
                ObjCarrito.ActualizarEstado(carrito.IdCarrito, "Facturado", out mensajeCarrito);

                string dni = SessionManager486LP.ObtenerInstancia().UsuarioActual()?.DNI ?? "Sistema";
                string nombreUsuario = SessionManager486LP.ObtenerInstancia().UsuarioActual()?.NombreUsuario ?? "Sistema";

                ObjBitacora.Registrar(new BitacoraEvento486LP("Ventas",
                    $"Venta registrada: Comprobante {venta.NroComprobante}, Cliente DNI {venta.DNICliente}, Medio de pago {venta.MedioPago}, Total ${venta.Total}.",
                    Criticidad486LP.Alta, dni, nombreUsuario));

                // DV de Venta y DetalleVenta (el de Producto ya se recalculo, fila por fila, dentro de ActualizarStock).
                string mensajeDV;
                BLL_DV486LP bllDV = new BLL_DV486LP();
                bllDV.RecalcularDV("Venta", out mensajeDV);
                bllDV.RecalcularDV("DetalleVenta", out mensajeDV);

                mensaje = "Venta registrada correctamente.";
                return venta;
            }
            catch (Exception ex)
            {
                mensaje = ex.Message;
                ObjBitacora.Registrar(new BitacoraEvento486LP("Ventas", $"Error en BLL_Venta.RegistrarVenta(): {ex.Message}", Criticidad486LP.MuyAlta, "Sistema", "Sistema"));
                return null;
            }
        }

        // Trae una venta ya registrada, con su detalle (para "Ver comprobante").
        public Venta486LP ObtenerPorId(int idVenta)
        {
            try
            {
                return ObjetoMapper.ObtenerPorId(idVenta);
            }
            catch (Exception ex)
            {
                ObjBitacora.Registrar(new BitacoraEvento486LP("Ventas", $"Error en BLL_Venta.ObtenerPorId(): {ex.Message}", Criticidad486LP.MuyAlta, "Sistema", "Sistema"));
                return null;
            }
        }
    }
}

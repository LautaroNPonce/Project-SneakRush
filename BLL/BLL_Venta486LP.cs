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
    // Copia
    public class BLL_Venta486LP
    {
        private Mapper_Venta486LP ObjetoMapper = new Mapper_Venta486LP();
        private BLL_Producto486LP ObjProducto = new BLL_Producto486LP();
        private BLL_Carrito486LP ObjCarrito = new BLL_Carrito486LP();
        private BLL_Cliente486LP ObjCliente = new BLL_Cliente486LP();
        private BLL_Bitacora486LP ObjBitacora = new BLL_Bitacora486LP();

        // Indica si el cliente del carrito (por DNI) ya esta registrado. Lo usa el Cajero en CUN04 antes de elegir el medio de pago (G02 paso 9):
        // si devuelve false, el form incluye CUN03 para registrarlo.
        public bool ClienteRegistrado(string dni)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(dni)) return false;
                return ObjCliente.Existe(dni.Trim());
            }
            catch (Exception ex)
            {
                ObjBitacora.Registrar(new BitacoraEvento486LP("Ventas", $"Error en BLL_Venta.ClienteRegistrado(): {ex.Message}", Criticidad486LP.MuyAlta, "Sistema", "Sistema"));
                return false;
            }
        }

        // Control de stock de CUN04: verifica que cada producto del carrito tenga stock suficiente (suma las lineas del mismo producto).
        // Devuelve true si alcanza para todo. Si no, devuelve false y en mensaje la lista de productos con faltante (pedidas vs. disponibles), ya traducida.
        public bool VerificarStockCarrito(Carrito486LP carrito, out string mensaje)
        {
            mensaje = "";

            try
            {
                if (carrito == null || carrito.Detalles == null || carrito.Detalles.Count == 0)
                {
                    mensaje = Mensajes486LP.Obtener("Carrito.Vacio"); // Muestra: El carrito no tiene productos
                    return false;
                }

                List<string> faltantes = new List<string>();

                foreach (var grupo in carrito.Detalles.GroupBy(d => d.IdProducto))
                {
                    int pedidas = grupo.Sum(d => d.Cantidad);
                    Producto486LP producto = ObjProducto.ObtenerPorId(grupo.Key);
                    int disponibles = producto == null ? 0 : producto.Stock;

                    if (pedidas > disponibles)
                    {
                        string nombre = producto == null
                            ? "#" + grupo.Key
                            : $"{producto.Marca} {producto.Modelo} - {producto.Color} - {producto.Talle}";
                        faltantes.Add(Mensajes486LP.Obtener("Venta.LineaStock", nombre, pedidas, disponibles)); // Muestra: {0} – pedidas: {1}, disponibles: {2}
                    }
                }

                if (faltantes.Count == 0) return true;

                mensaje = Mensajes486LP.Obtener("Venta.StockInsuficienteCobro", string.Join(Environment.NewLine, faltantes)); // Muestra: No se puede cobrar: no hay stock suficiente de los siguientes productos: (salto de línea) {0}
                return false;
            }
            catch (Exception ex)
            {
                mensaje = Mensajes486LP.Obtener("General.ErrorInesperado"); // Muestra: Ocurrió un error inesperado. El detalle quedó registrado en la bitácora
                ObjBitacora.Registrar(new BitacoraEvento486LP("Ventas", $"Error en BLL_Venta.VerificarStockCarrito(): {ex.Message}", Criticidad486LP.MuyAlta, "Sistema", "Sistema"));
                return false;
            }
        }

        // Cancela el carrito activo (CUN04: el cliente no se registra o desiste de la compra). El carrito NO se elimina: pasa a Estado "Cancelado".
        // BLL_Carrito.ActualizarEstado se ocupa de actualizar el DV del carrito y de registrar el evento en la bitacora.
        public bool CancelarCarrito(int idCarrito, out string mensaje)
        {
            mensaje = "";

            try
            {
                Carrito486LP carrito = ObjCarrito.ObtenerCarrito(idCarrito);
                if (carrito == null)
                {
                    mensaje = Mensajes486LP.Obtener("Venta.CarritoInexistente"); // Muestra: El carrito no existe
                    return false;
                }
                if (carrito.Estado != "Activo")
                {
                    mensaje = Mensajes486LP.Obtener("Venta.SoloCancelarActivo"); // Muestra: Solo se puede cancelar un carrito en estado Activo
                    return false;
                }

                string mensajeEstado;
                if (!ObjCarrito.ActualizarEstado(idCarrito, "Cancelado", out mensajeEstado))
                {
                    mensaje = mensajeEstado;
                    return false;
                }

                mensaje = Mensajes486LP.Obtener("Venta.CarritoCancelado"); // Muestra: Carrito cancelado correctamente
                return true;
            }
            catch (Exception ex)
            {
                mensaje = Mensajes486LP.Obtener("General.ErrorInesperado"); // Muestra: Ocurrió un error inesperado. El detalle quedó registrado en la bitácora
                ObjBitacora.Registrar(new BitacoraEvento486LP("Ventas", $"Error en BLL_Venta.CancelarCarrito(): {ex.Message}", Criticidad486LP.MuyAlta, "Sistema", "Sistema"));
                return false;
            }
        }

        // Registra la venta desde un carrito ya armado: persiste cabecera+detalle, descuenta stock, cierra el carrito ("Facturado"), recalcula DV y registra bitácora.
        // La Venta queda persistida siempre  el pago nunca se rechaza en este proyecto, no hay reversión posible.
        public Venta486LP RegistrarVenta(Carrito486LP carrito, string medioPago, out string mensaje)
        {
            mensaje = "";

            try
            {
                if (carrito == null || carrito.Detalles == null || carrito.Detalles.Count == 0)
                {
                    mensaje = Mensajes486LP.Obtener("Carrito.Vacio"); // Muestra: El carrito no tiene productos
                    return null;
                }
                if (string.IsNullOrEmpty(carrito.DNICliente))
                {
                    mensaje = Mensajes486LP.Obtener("Venta.SinClienteAsignado"); // Muestra: El carrito no tiene un cliente asignado
                    return null;
                }
                // El carrito se guarda solo con el DNI (el Vendedor no registra clientes): la venta exige que el cliente ya este registrado (FK Venta -> Cliente)
                if (!ObjCliente.Existe(carrito.DNICliente))
                {
                    mensaje = Mensajes486LP.Obtener("Venta.ClienteNoRegistrado", carrito.DNICliente); // Muestra: El cliente con DNI {0} no está registrado. Debe registrarlo antes de cobrar
                    return null;
                }

                // Ultimo control de stock antes de grabar: cubre el tiempo que tarda el pago (3 s simulados en tarjeta y transferencia)
                string detalleStock;
                if (!VerificarStockCarrito(carrito, out detalleStock))
                {
                    ObjBitacora.Registrar(new BitacoraEvento486LP("Ventas", $"Venta no registrada por stock insuficiente (Cliente DNI {carrito.DNICliente}): {detalleStock}", Criticidad486LP.Alta, "Sistema", "Sistema"));
                    mensaje = Mensajes486LP.Obtener("Venta.StockCambio"); // Muestra: No se registró la venta porque el stock cambió mientras se procesaba el pago. Revise el carrito y vuelva a intentar
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

                string mensajeMapper;
                bool ok = ObjetoMapper.Agregar(venta, out mensajeMapper);
                if (!ok)
                {
                    // El texto del Mapper es tecnico: va a la bitacora y el usuario ve un mensaje general
                    ObjBitacora.Registrar(new BitacoraEvento486LP("Ventas", $"Error al registrar venta (Cliente DNI {carrito.DNICliente}): {mensajeMapper}", Criticidad486LP.MuyAlta, "Sistema", "Sistema"));
                    mensaje = Mensajes486LP.Obtener("General.ErrorInesperado"); // Muestra: Ocurrió un error inesperado. El detalle quedó registrado en la bitácora
                    return null;
                }

                // se actualiza el stock de cada producto vendido
                foreach (DetalleVenta486LP det in venta.Detalles)
                {
                    string mensajeStock;
                    if (!ObjProducto.ActualizarStock(det.IdProducto, det.Cantidad, out mensajeStock))
                    {
                        ObjBitacora.Registrar(new BitacoraEvento486LP("Ventas", $"No se pudo descontar el stock del producto Id {det.IdProducto} de la venta {venta.NroComprobante}: {mensajeStock}", Criticidad486LP.MuyAlta, "Sistema", "Sistema"));
                    }
                }

                // El carrito de origen pasa a "Facturado" xq no vuelve a aparecer como Activo
                string mensajeCarrito;
                ObjCarrito.ActualizarEstado(carrito.IdCarrito, "Facturado", out mensajeCarrito);

                string dni = SessionManager486LP.ObtenerInstancia().UsuarioActual()?.DNI ?? "Sistema";
                string nombreUsuario = SessionManager486LP.ObtenerInstancia().UsuarioActual()?.NombreUsuario ?? "Sistema";

                ObjBitacora.Registrar(new BitacoraEvento486LP("Ventas",
                    $"Venta registrada: Comprobante {venta.NroComprobante}, Cliente DNI {venta.DNICliente}, Medio de pago {venta.MedioPago}, Total ${venta.Total}.",
                    Criticidad486LP.Alta, dni, nombreUsuario));

                string mensajeDV;
                BLL_DV486LP bllDV = new BLL_DV486LP();
                bllDV.RecalcularDV("Venta", out mensajeDV);
                bllDV.RecalcularDV("DetalleVenta", out mensajeDV);

                mensaje = Mensajes486LP.Obtener("Venta.Registrada"); // Muestra: Venta registrada correctamente
                return venta;
            }
            catch (Exception ex)
            {
                mensaje = Mensajes486LP.Obtener("General.ErrorInesperado"); // Muestra: Ocurrió un error inesperado. El detalle quedó registrado en la bitácora
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

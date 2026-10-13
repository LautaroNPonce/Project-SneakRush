using BE;
using Mappers;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class BLL_Carrito486LP
    {
        private Mapper_Carrito486LP ObjetoMapper = new Mapper_Carrito486LP();
        private BLL_Producto486LP ObjProducto = new BLL_Producto486LP();
        private BLL_Bitacora486LP ObjBitacora = new BLL_Bitacora486LP();

        public bool Agregar(Carrito486LP carrito, Producto486LP producto, int cantidad, out string Mensaje)
        {
            Mensaje = string.Empty;

            try
            {
                if (producto == null)
                {
                    Mensaje = Mensajes486LP.Obtener("Carrito.ProductoInexistente"); return false; // Muestra: El producto no existe.
                }
                if (cantidad <= 0)
                {
                    Mensaje = Mensajes486LP.Obtener("Carrito.CantidadInvalida"); return false; // Muestra: La cantidad debe ser mayor a cero.
                }

                // Cantidad ya cargada de ese producto en el carrito
                int yaCargado = carrito.Detalles.Where(d => d.IdProducto == producto.IdProducto).Sum(d => d.Cantidad);

                if (!ObjProducto.VerificarStock(producto.IdProducto, yaCargado + cantidad))
                {
                    // Se informa cuantas unidades todavia se pueden sumar (el stock menos lo que ya esta en el carrito)
                    int quedan = Math.Max(0, ObjProducto.ObtenerStockDisponible(producto.IdProducto) - yaCargado);
                    Mensaje = Mensajes486LP.Obtener("Carrito.StockInsuficiente", quedan); return false; // Muestra: Stock insuficiente. Unidades disponibles: {0}.
                }

                DetalleCarrito486LP det = new DetalleCarrito486LP()
                {
                    IdProducto = producto.IdProducto,
                    Cantidad = cantidad,
                    Precio = producto.Precio,
                    Subtotal = producto.Precio * cantidad,
                    Producto = producto
                };
                carrito.Detalles.Add(det);
                carrito.Total = carrito.Detalles.Sum(d => d.Subtotal);

                return true;
            }
            catch (Exception ex)
            {
                Mensaje = Mensajes486LP.Obtener("General.ErrorInesperado"); // Muestra: Ocurrió un error inesperado. El detalle quedó registrado en la bitácora.
                ObjBitacora.Registrar(new BitacoraEvento486LP("Ventas", $"Error en BLL_Carrito.Agregar(): {ex.Message}", Criticidad486LP.MuyAlta, "Sistema", "Sistema"));
                return false;
            }
        }

        public bool ModificarCantidad(Carrito486LP carrito, DetalleCarrito486LP detalle, int nuevaCantidad, out string Mensaje)
        {
            Mensaje = string.Empty;

            try
            {
                if (carrito == null || detalle == null)
                {
                    Mensaje = Mensajes486LP.Obtener("Carrito.ProductoInexistente"); return false; // Muestra: El producto no existe.
                }
                if (nuevaCantidad <= 0)
                {
                    Mensaje = Mensajes486LP.Obtener("Carrito.CantidadInvalida"); return false; // Muestra: La cantidad debe ser mayor a cero.
                }

                int enOtrasLineas = carrito.Detalles.Where(d => d != detalle && d.IdProducto == detalle.IdProducto).Sum(d => d.Cantidad);

                if (!ObjProducto.VerificarStock(detalle.IdProducto, enOtrasLineas + nuevaCantidad))
                {
                    int quedan = Math.Max(0, ObjProducto.ObtenerStockDisponible(detalle.IdProducto) - enOtrasLineas);
                    Mensaje = Mensajes486LP.Obtener("Carrito.StockInsuficiente", quedan); return false; // Muestra: Stock insuficiente. Unidades disponibles: {0}.
                }

                detalle.Cantidad = nuevaCantidad;
                detalle.Subtotal = detalle.Precio * nuevaCantidad;
                carrito.Total = carrito.Detalles.Sum(d => d.Subtotal);
                return true;
            }
            catch (Exception ex)
            {
                Mensaje = Mensajes486LP.Obtener("General.ErrorInesperado"); // Muestra: Ocurrió un error inesperado. El detalle quedó registrado en la bitácora.
                ObjBitacora.Registrar(new BitacoraEvento486LP("Ventas", $"Error en BLL_Carrito.ModificarCantidad(): {ex.Message}", Criticidad486LP.MuyAlta, "Sistema", "Sistema"));
                return false;
            }
        }

        public void QuitarProducto(Carrito486LP carrito, DetalleCarrito486LP detalle)
        {
            carrito.Detalles.Remove(detalle);
            carrito.Total = carrito.Detalles.Sum(d => d.Subtotal);
        }


        public bool GuardarCarrito(Carrito486LP carrito, out string Mensaje)
        {
            Mensaje = string.Empty;

            try
            {
                if (string.IsNullOrEmpty(carrito.DNICliente))
                {
                    Mensaje = Mensajes486LP.Obtener("Carrito.SinCliente"); return false; // Muestra: Debe asignar el DNI del cliente al carrito.
                }
                if (!Regex.IsMatch(carrito.DNICliente, @"^\d{7,8}$"))
                {
                    Mensaje = Mensajes486LP.Obtener("Cliente.DNIInvalido"); return false; // Muestra: El DNI debe tener 7 u 8 dígitos numéricos.
                }
                if (carrito.Detalles == null || carrito.Detalles.Count == 0)
                {
                    Mensaje = Mensajes486LP.Obtener("Carrito.Vacio"); return false; // Muestra: El carrito no tiene productos.
                } 

                carrito.Fecha = DateTime.Now;
                carrito.Estado = "Activo";
                carrito.Total = carrito.Detalles.Sum(d => d.Subtotal);

                string mensajeMapper;
                int idGenerado = ObjetoMapper.Guardar(carrito, out mensajeMapper);
                bool resultado = idGenerado > 0;

                if (!resultado)
                {
                    // El texto del Mapper es tecnico: va a la bitacora y el usuario ve un mensaje general
                    ObjBitacora.Registrar(new BitacoraEvento486LP("Ventas", $"Error al guardar el carrito (Cliente DNI: {carrito.DNICliente}): {mensajeMapper}", Criticidad486LP.MuyAlta, "Sistema", "Sistema"));
                    Mensaje = Mensajes486LP.Obtener("General.ErrorInesperado"); // Muestra: Ocurrió un error inesperado. El detalle quedó registrado en la bitácora.
                }

                if (resultado)
                {
                    carrito.IdCarrito = idGenerado;

                    // Registrar en bitacora
                    ObjBitacora.Registrar(new BitacoraEvento486LP("Ventas", $"Alta de carrito de venta #{idGenerado} (Cliente DNI: {carrito.DNICliente}, Total: {carrito.Total}).", Criticidad486LP.Alta,
                        SessionManager486LP.ObtenerInstancia().UsuarioActual()?.DNI ?? "Sistema",
                        SessionManager486LP.ObtenerInstancia().UsuarioActual()?.NombreUsuario ?? "Sistema"));

                    // Recalcular DV de las tablas afectadas
                    string mensajeDV;
                    BLL_DV486LP bllDV = new BLL_DV486LP();
                    bllDV.RecalcularDV("Carrito", out mensajeDV);
                    bllDV.RecalcularDV("DetalleCarrito", out mensajeDV);
                }

                return resultado;
            }
            catch (Exception ex)
            {
                Mensaje = Mensajes486LP.Obtener("General.ErrorInesperado"); // Muestra: Ocurrió un error inesperado. El detalle quedó registrado en la bitácora.
                ObjBitacora.Registrar(new BitacoraEvento486LP("Ventas", $"Error en BLL_Carrito.GuardarCarrito(): {ex.Message}", Criticidad486LP.MuyAlta, "Sistema", "Sistema"));
                return false;
            }
        }

        public Carrito486LP ObtenerCarrito(int idCarrito)
        {
            try
            {
                return ObjetoMapper.Obtener(idCarrito);
            }
            catch (Exception ex)
            {
                ObjBitacora.Registrar(new BitacoraEvento486LP("Ventas", $"Error en BLL_Carrito.ObtenerCarrito(): {ex.Message}", Criticidad486LP.MuyAlta, "Sistema", "Sistema"));
                return null;
            }
        }

        // Busca el carrito Activo de un DNI
        public Carrito486LP ObtenerActivoPorDNI(string dni)
        {
            try
            {
                return ObjetoMapper.ObtenerActivoPorDNI(dni);
            }
            catch (Exception ex)
            {
                ObjBitacora.Registrar(new BitacoraEvento486LP("Ventas", $"Error en BLL_Carrito.ObtenerActivoPorDNI(): {ex.Message}", Criticidad486LP.MuyAlta, "Sistema", "Sistema"));
                return null;
            }
        }

        // Cambia el Estado del carrito: "Facturado" cuando se completa la venta o "Cancelado" cuando el Cajero cancela la operacion (en ambos casos deja de aparecer como Activo)
        // Actualiza el DV de la fila del carrito y registra el evento en la bitacora
        public bool ActualizarEstado(int idCarrito, string estado, out string mensaje)
        {
            mensaje = "";
            try
            {
                string mensajeMapper;
                bool ok = ObjetoMapper.ActualizarEstado(idCarrito, estado, out mensajeMapper);

                if (!ok)
                {
                    ObjBitacora.Registrar(new BitacoraEvento486LP("Ventas", $"Error al actualizar el estado del carrito #{idCarrito} a '{estado}': {mensajeMapper}", Criticidad486LP.MuyAlta, "Sistema", "Sistema"));
                    mensaje = Mensajes486LP.Obtener("General.ErrorInesperado"); // Muestra: Ocurrió un error inesperado. El detalle quedó registrado en la bitácora.
                }

                if (ok)
                {
                    string mensajeDV;
                    BLL_DV486LP bllDV = new BLL_DV486LP();
                    bllDV.RegistrarDVDeFilaNueva("Carrito", idCarrito, out mensajeDV);

                    ObjBitacora.Registrar(new BitacoraEvento486LP("Ventas", $"Carrito #{idCarrito} actualizado a estado '{estado}'.", Criticidad486LP.Media,
                        SessionManager486LP.ObtenerInstancia().UsuarioActual()?.DNI ?? "Sistema",
                        SessionManager486LP.ObtenerInstancia().UsuarioActual()?.NombreUsuario ?? "Sistema"));
                }

                return ok;
            }
            catch (Exception ex)
            {
                mensaje = Mensajes486LP.Obtener("General.ErrorInesperado"); // Muestra: Ocurrió un error inesperado. El detalle quedó registrado en la bitácora.
                ObjBitacora.Registrar(new BitacoraEvento486LP("Ventas", $"Error en BLL_Carrito.ActualizarEstado(): {ex.Message}", Criticidad486LP.MuyAlta, "Sistema", "Sistema"));
                return false;
            }
        }
    }
}

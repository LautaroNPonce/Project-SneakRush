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
                    Mensaje = "El producto no existe."; return false;
                }
                if (cantidad <= 0)
                {
                    Mensaje = "La cantidad debe ser mayor a cero."; return false;
                }

                // Cantidad ya cargada de ese producto en el carrito
                int yaCargado = carrito.Detalles.Where(d => d.IdProducto == producto.IdProducto).Sum(d => d.Cantidad);

                if (!ObjProducto.VerificarStock(producto.IdProducto, yaCargado + cantidad))
                {
                    Mensaje = "Sin stock disponible para la cantidad solicitada."; return false;
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
                Mensaje = ex.Message;
                ObjBitacora.Registrar(new BitacoraEvento486LP("Ventas", $"Error en BLL_Carrito.Agregar(): {ex.Message}", Criticidad486LP.MuyAlta, "Sistema", "Sistema"));
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
                    Mensaje = "Debe asignar un cliente al carrito."; return false;
                }
                if (carrito.Detalles == null || carrito.Detalles.Count == 0)
                {
                    Mensaje = "El carrito no tiene productos."; return false;
                }

                carrito.Fecha = DateTime.Now;
                carrito.Estado = "Activo";
                carrito.Total = carrito.Detalles.Sum(d => d.Subtotal);

                int idGenerado = ObjetoMapper.Guardar(carrito, out Mensaje);
                bool resultado = idGenerado > 0;

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
                Mensaje = ex.Message;
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

        // Cambia el Estado del carrito (ej: a facturado cuando completa la venta, para que no vuelva a aparecer como activo)
        // No recalcula DV acá lo hace BLL_Venta486LP.RegistrarVenta() junto con el resto de las tablas afectadas
        public bool ActualizarEstado(int idCarrito, string estado, out string mensaje)
        {
            mensaje = "";
            try
            {
                bool ok = ObjetoMapper.ActualizarEstado(idCarrito, estado, out mensaje);

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
                mensaje = ex.Message;
                ObjBitacora.Registrar(new BitacoraEvento486LP("Ventas", $"Error en BLL_Carrito.ActualizarEstado(): {ex.Message}", Criticidad486LP.MuyAlta, "Sistema", "Sistema"));
                return false;
            }
        }
    }
}

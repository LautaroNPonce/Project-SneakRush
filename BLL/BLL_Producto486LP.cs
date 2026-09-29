using BE;
using DAL;
using Mappers;
using Mappers.Mappers;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class BLL_Producto486LP
    {
        private Mapper_Producto486LP ObjetoMapper = new Mapper_Producto486LP();
        private BLL_Bitacora486LP ObjBitacora = new BLL_Bitacora486LP();

        // Lista todos los productos
        public List<Producto486LP> Listar()
        {
            try
            {
                return ObjetoMapper.Listar();
            }
            catch (Exception ex)
            {
                ObjBitacora.Registrar(new BitacoraEvento486LP("Ventas", $"Error en BLL_Producto.Listar(): {ex.Message}", Criticidad486LP.MuyAlta, "Sistema", "Sistema"));
                return new List<Producto486LP>();
            }
        }

        public List<Producto486LP> Buscar(string marca, string modelo, string color, string talle, bool soloConStock)
        {
            try
            {
                return ObjetoMapper.Buscar(marca, modelo, color, talle, soloConStock);
            }
            catch (Exception ex)
            {
                ObjBitacora.Registrar(new BitacoraEvento486LP("Ventas", $"Error en BLL_Producto.Buscar(): {ex.Message}", Criticidad486LP.MuyAlta, "Sistema", "Sistema"));
                return new List<Producto486LP>();
            }
        }

        public Producto486LP ObtenerProducto(string marca, string modelo, string color, string talle)
        {
            try
            {
                return ObjetoMapper.ObtenerPorAtributos(marca, modelo, color, talle);
            }
            catch (Exception ex)
            {
                ObjBitacora.Registrar(new BitacoraEvento486LP("Ventas", $"Error en BLL_Producto.ObtenerProducto(): {ex.Message}", Criticidad486LP.MuyAlta, "Sistema", "Sistema"));
                return null;
            }
        }

        public bool VerificarStock(int idProducto, int cantidad)
        {
            try
            {
                Producto486LP p = ObjetoMapper.ObtenerPorId(idProducto);
                return p != null && p.Stock >= cantidad;
            }
            catch (Exception ex)
            {
                ObjBitacora.Registrar(new BitacoraEvento486LP("Ventas", $"Error en BLL_Producto.VerificarStock(): {ex.Message}", Criticidad486LP.MuyAlta, "Sistema", "Sistema"));
                return false;
            }
        }
        public bool ActualizarStock(int idProducto, int cantidad, out string mensaje)
        {
            mensaje = "";
            try
            {
                bool ok = ObjetoMapper.ActualizarStock(idProducto, cantidad, out mensaje);

                if (ok)
                {
                    string mensajeDV;
                    BLL_DV486LP bllDV = new BLL_DV486LP();
                    bllDV.RegistrarDVDeFilaNueva("Producto", idProducto, out mensajeDV);

                    ObjBitacora.Registrar(new BitacoraEvento486LP("Ventas", $"Stock actualizado: Producto Id {idProducto}, se descontaron {cantidad} unidades.", Criticidad486LP.Media,
                        SessionManager486LP.ObtenerInstancia().UsuarioActual()?.DNI ?? "Sistema",
                        SessionManager486LP.ObtenerInstancia().UsuarioActual()?.NombreUsuario ?? "Sistema"));
                }

                return ok;
            }
            catch (Exception ex)
            {
                mensaje = ex.Message;
                ObjBitacora.Registrar(new BitacoraEvento486LP("Ventas", $"Error en BLL_Producto.ActualizarStock(): {ex.Message}", Criticidad486LP.MuyAlta, "Sistema", "Sistema"));
                return false;
            }
        }

        // Obtiene un producto por su Id. Hace falta para completar el Producto de cada DetalleCarrito486LP (llega en null desde el Mapper de Carrito) y así mostrar
        // Marca/Modelo/Color/Talle en la grilla de Cobrar venta
        public Producto486LP ObtenerPorId(int idProducto)
        {
            try
            {
                return ObjetoMapper.ObtenerPorId(idProducto);
            }
            catch (Exception ex)
            {
                ObjBitacora.Registrar(new BitacoraEvento486LP("Ventas", $"Error en BLL_Producto.ObtenerPorId(): {ex.Message}", Criticidad486LP.MuyAlta, "Sistema", "Sistema"));
                return null;
            }
        }

        // Aumenta stock tras confirmarse el pago de una compra. Mismo criterio que actualizarStock: recalcula el DV de esa única fila, porque esto corre en cada compra pagada
        public bool AumentarStock(int idProducto, int cantidad, out string mensaje)
        {
            mensaje = "";
            try
            {
                bool ok = ObjetoMapper.AumentarStock(idProducto, cantidad, out mensaje);

                if (ok)
                {
                    string mensajeDV;
                    BLL_DV486LP bllDV = new BLL_DV486LP();
                    bllDV.RegistrarDVDeFilaNueva("Producto", idProducto, out mensajeDV);

                    ObjBitacora.Registrar(new BitacoraEvento486LP("Compras", $"Stock actualizado: Producto Id {idProducto}, se sumaron {cantidad} unidades.", Criticidad486LP.Media,
                        SessionManager486LP.ObtenerInstancia().UsuarioActual()?.DNI ?? "Sistema",
                        SessionManager486LP.ObtenerInstancia().UsuarioActual()?.NombreUsuario ?? "Sistema"));
                }

                return ok;
            }
            catch (Exception ex)
            {
                mensaje = ex.Message;
                ObjBitacora.Registrar(new BitacoraEvento486LP("Compras", $"Error en BLL_Producto.AumentarStock(): {ex.Message}", Criticidad486LP.MuyAlta, "Sistema", "Sistema"));
                return false;
            }
        }
    }
}

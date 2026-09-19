using BE;
using DAL;
using Mappers;
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

        // Busca productos por filtros opcionales (Marca / Modelo / Color / Talle) y, opcionalmente, solo los que tengan stock disponible.
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

        // Obtiene un producto por sus atributos (Marca, Modelo, Color, Talle)
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

        // Verifica que haya stock suficiente del producto para la cantidad pedida
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

        // Descuenta stock tras una venta (CUN04, paso 11). Primer metodo de escritura de esta BLL -
        // por eso, a partir de ahora, Producto tambien recalcula su DV (RegistrarDVDeFilaNueva: solo
        // esa fila puntual, igual criterio que Bitacora, porque esto se ejecuta en CADA venta).
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

        // Obtiene un producto por su Id. NUEVO para CUN04: hace falta para completar el objeto Producto relacionado de cada DetalleCarrito486LP (que llega en null desde el Mapper de
        // Carrito) y asi poder mostrar Marca/Modelo/Color/Talle en la grilla de Cobrar venta.
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
    }
}

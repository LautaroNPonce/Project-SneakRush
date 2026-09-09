using BE;
using DAL;
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
        private DAL_Producto486LP ObjetoDAL = new DAL_Producto486LP();
        private BLL_Bitacora486LP ObjBitacora = new BLL_Bitacora486LP();

        // Lista todos los productos
        public List<Producto486LP> Listar()
        {
            try
            {
                return ObjetoDAL.Listar();
            }
            catch (Exception ex)
            {
                ObjBitacora.Registrar(new BitacoraEvento486LP("Ventas", $"Error en BLL_Producto.Listar(): {ex.Message}", Criticidad486LP.MuyAlta, "Sistema", "Sistema"));
                return new List<Producto486LP>();
            }
        }

        // Busca productos por filtros opcionales (Marca / Modelo / Color / Talle) y,
        // opcionalmente, solo los que tengan stock disponible.
        // Ante un error de acceso a datos, registra el evento en bitacora (flujo alternativo 7.2)
        // y devuelve una lista vacia para que la GUI muestre "sin resultados" sin caerse.
        public List<Producto486LP> Buscar(string marca, string modelo, string color, string talle, bool soloConStock)
        {
            try
            {
                return ObjetoDAL.Buscar(marca, modelo, color, talle, soloConStock);
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
                return ObjetoDAL.ObtenerPorAtributos(marca, modelo, color, talle);
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
                Producto486LP p = ObjetoDAL.ObtenerPorId(idProducto);
                return p != null && p.Stock >= cantidad;
            }
            catch (Exception ex)
            {
                ObjBitacora.Registrar(new BitacoraEvento486LP("Ventas", $"Error en BLL_Producto.VerificarStock(): {ex.Message}", Criticidad486LP.MuyAlta, "Sistema", "Sistema"));
                return false;
            }
        }
    }
}

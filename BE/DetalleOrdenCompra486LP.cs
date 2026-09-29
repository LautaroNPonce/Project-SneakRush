using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BE
{
    public class DetalleOrdenCompra486LP
    {
        [ColumnaTabla486LP]
        public int IdDetalleOrden { get; set; }
        [ColumnaTabla486LP]
        public int IdOrden { get; set; }
        [ColumnaTabla486LP]
        public int IdProducto { get; set; }
        [ColumnaTabla486LP]
        public int Cantidad { get; set; }
        [ColumnaTabla486LP]
        public decimal CostoUnitario { get; set; }
        [ColumnaTabla486LP]
        public decimal Subtotal { get; set; }

        // Vienen del JOIN con Producto en el SP DetalleOrdenCompra_ListarPorOrden - no son columnas reales de la tabla DetalleOrdenCompra (por eso no participan del mecanismo
        // de DV). Llevan [ColumnaTabla486LP] igual, mismo patron que DetalleVenta486LP y DetalleSolicitudCompra486LP, para que el mapeo por reflexion las complete solas.
        [ColumnaTabla486LP]
        public string Marca { get; set; }
        [ColumnaTabla486LP]
        public string Modelo { get; set; }
        [ColumnaTabla486LP]
        public string Color { get; set; }
        [ColumnaTabla486LP]
        public string Talle { get; set; }

        public DetalleOrdenCompra486LP() { }
        public decimal? UltimoCosto { get; set; }

        public DetalleOrdenCompra486LP(int idDetalleOrden, int idOrden, int idProducto, int cantidad, decimal costoUnitario, decimal subtotal)
        {
            IdDetalleOrden = idDetalleOrden;
            IdOrden = idOrden;
            IdProducto = idProducto;
            Cantidad = cantidad;
            CostoUnitario = costoUnitario;
            Subtotal = subtotal;
        }

        public override string ToString()
        {
            return $"Detalle #{IdDetalleOrden} - Producto {IdProducto} x{Cantidad}";
        }
    }
}

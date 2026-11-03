using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BE
{
    public class DetalleVenta486LP
    {
        [ColumnaTabla486LP]
        public int IdDetalleVenta { get; set; }
        [ColumnaTabla486LP]
        public int IdVenta { get; set; }
        [ColumnaTabla486LP]
        public int IdProducto { get; set; }
        [ColumnaTabla486LP]
        public int Cantidad { get; set; }
        [ColumnaTabla486LP]
        public decimal PrecioUnitario { get; set; }
        [ColumnaTabla486LP]
        public decimal Subtotal { get; set; }
        [ColumnaTabla486LP]
        public string Marca { get; set; }
        [ColumnaTabla486LP]
        public string Modelo { get; set; }
        [ColumnaTabla486LP]
        public string Color { get; set; }
        [ColumnaTabla486LP]
        public string Talle { get; set; }

        public DetalleVenta486LP() { }

        public DetalleVenta486LP(int idDetalleVenta, int idVenta, int idProducto, int cantidad, decimal precioUnitario, decimal subtotal)
        {
            IdDetalleVenta = idDetalleVenta;
            IdVenta = idVenta;
            IdProducto = idProducto;
            Cantidad = cantidad;
            PrecioUnitario = precioUnitario;
            Subtotal = subtotal;
        }

        public override string ToString()
        {
            return $"Detalle #{IdDetalleVenta} - Producto {IdProducto} x{Cantidad} = ${Subtotal}";
        }
    }
}

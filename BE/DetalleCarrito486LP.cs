using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BE
{
    public class DetalleCarrito486LP
    {
        [ColumnaTabla486LP]
        public int IdDetalle { get; set; }
        [ColumnaTabla486LP]
        public int IdCarrito { get; set; }
        [ColumnaTabla486LP]
        public int IdProducto { get; set; }
        [ColumnaTabla486LP]
        public int Cantidad { get; set; }
        [ColumnaTabla486LP]
        public decimal Precio { get; set; }
        [ColumnaTabla486LP]
        public decimal Subtotal { get; set; }

        // Referencia al producto (para mostrar Marca/Modelo/Color/Talle en la grilla).
        // NO llevan [ColumnaTabla486LP]: no son columnas de DetalleCarrito, son el objeto relacionado y propiedades calculadas de solo lectura.
        public Producto486LP Producto { get; set; }
        public string Marca { get { return Producto?.Marca; } }
        public string Modelo { get { return Producto?.Modelo; } }
        public string Color { get { return Producto?.Color; } }
        public string Talle { get { return Producto?.Talle; } }

        public DetalleCarrito486LP() { }

        public DetalleCarrito486LP(int idDetalle, int idCarrito, int idProducto, int cantidad, decimal precio, decimal subtotal)
        {
            IdDetalle = idDetalle;
            IdCarrito = idCarrito;
            IdProducto = idProducto;
            Cantidad = cantidad;
            Precio = precio;
            Subtotal = subtotal;
        }

        public override string ToString()
        {
            return $"Detalle #{IdDetalle} - Producto {IdProducto} x{Cantidad} = ${Subtotal}";
        }
    }
}

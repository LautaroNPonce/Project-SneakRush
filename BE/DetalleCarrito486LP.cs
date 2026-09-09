using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BE
{
    public class DetalleCarrito486LP
    {
        public int IdDetalle { get; set; }
        public int IdCarrito { get; set; }
        public int IdProducto { get; set; }
        public int Cantidad { get; set; }
        public decimal Precio { get; set; }
        public decimal Subtotal { get; set; }

        // Referencia al producto (para mostrar Marca/Modelo/Color/Talle en la grilla)
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

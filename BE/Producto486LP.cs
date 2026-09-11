using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BE
{
    public class Producto486LP
    {
        [ColumnaTabla486LP]
        public int IdProducto { get; set; }
        [ColumnaTabla486LP]
        public string Marca { get; set; }
        [ColumnaTabla486LP]
        public string Modelo { get; set; }
        [ColumnaTabla486LP]
        public string Color { get; set; }
        [ColumnaTabla486LP]
        public string Talle { get; set; }
        [ColumnaTabla486LP]
        public decimal Precio { get; set; }
        [ColumnaTabla486LP]
        public int Stock { get; set; }

        public Producto486LP() { }

        public Producto486LP(int idProducto, string marca, string modelo, string color, string talle, decimal precio, int stock)
        {
            IdProducto = idProducto;
            Marca = marca;
            Modelo = modelo;
            Color = color;
            Talle = talle;
            Precio = precio;
            Stock = stock;
        }

        public override string ToString()
        {
            return $"{Marca} {Modelo} - {Color} - Talle {Talle} (${Precio})";
        }
    }
}

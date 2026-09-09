using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BE
{
    public class Producto486LP
    {
        public int IdProducto { get; set; }
        public string Marca { get; set; }
        public string Modelo { get; set; }
        public string Color { get; set; }
        public string Talle { get; set; }
        public decimal Precio { get; set; }
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

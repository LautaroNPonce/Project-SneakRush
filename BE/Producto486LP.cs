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
        // IdMarca / IdCategoria son las FK de la tabla Producto; Marca y Categoria son los nombres, que traen los SP con JOIN
        [ColumnaTabla486LP]
        public int IdMarca { get; set; }
        [ColumnaTabla486LP]
        public string Marca { get; set; }
        [ColumnaTabla486LP]
        public int IdCategoria { get; set; }
        [ColumnaTabla486LP]
        public string Categoria { get; set; }
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

        public Producto486LP(int idProducto, int idMarca, string marca, int idCategoria, string categoria, string modelo, string color, string talle, decimal precio, int stock)
        {
            IdProducto = idProducto;
            IdMarca = idMarca;
            Marca = marca;
            IdCategoria = idCategoria;
            Categoria = categoria;
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

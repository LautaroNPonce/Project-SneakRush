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

    public class ProductoCambio486LP
    {
        public const string OPERACION_INICIAL = "I";
        public const string OPERACION_ALTA = "A";
        public const string OPERACION_MODIFICACION = "M";
        public const string OPERACION_BAJA = "B";

        [ColumnaTabla486LP]
        public int IdCambio { get; set; }
        [ColumnaTabla486LP]
        public int IdProducto { get; set; }
        [ColumnaTabla486LP]
        public DateTime FechaHora { get; set; }
        [ColumnaTabla486LP]
        public string Operacion { get; set; }
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
        [ColumnaTabla486LP]
        public bool Vigente { get; set; }

        public override string ToString()
        {
            return $"{Marca} {Modelo} - {Color} - Talle {Talle}";
        }
    }
}

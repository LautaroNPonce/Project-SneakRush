using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BE
{
    public class ReporteStockProducto486LP
    {
        public const int SIN_STOCK = 0;    // 0 unidades
        public const int STOCK_BAJO = 1;   // de 1 hasta el umbral (5 unidades)
        public const int NORMAL = 2;       // mas que el umbral

        [ColumnaTabla486LP]
        public string Marca { get; set; }
        [ColumnaTabla486LP]
        public string Modelo { get; set; }
        [ColumnaTabla486LP]
        public string Color { get; set; }
        [ColumnaTabla486LP]
        public string Talle { get; set; }
        [ColumnaTabla486LP]
        public int Stock { get; set; }
        [ColumnaTabla486LP]
        public decimal Precio { get; set; }
        [ColumnaTabla486LP]
        public decimal ValorStock { get; set; }
        [ColumnaTabla486LP]
        public int Estado { get; set; }

        public ReporteStockProducto486LP() { }

        public override string ToString()
        {
            return $"{Marca} {Modelo} {Color} T{Talle}: {Stock} u.";
        }
    }
}

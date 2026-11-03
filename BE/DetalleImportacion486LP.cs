using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BE
{
    // Las lineas importadas (Alta / SumaStock) nunca se vuelven a procesar, las Rechazadas si, cuando llegan corregidas.
    public class DetalleImportacion486LP
    {
        public const string ACCION_ALTA = "Alta";
        public const string ACCION_SUMA_STOCK = "SumaStock";
        public const string ACCION_RECHAZADA = "Rechazada";

        [ColumnaTabla486LP]
        public int IdDetalleImportacion { get; set; }
        [ColumnaTabla486LP]
        public int IdImportacion { get; set; }
        [ColumnaTabla486LP]
        public int Linea { get; set; }
        [ColumnaTabla486LP]
        public string Marca { get; set; }
        [ColumnaTabla486LP]
        public string Categoria { get; set; }
        [ColumnaTabla486LP]
        public string Modelo { get; set; }
        [ColumnaTabla486LP]
        public string Color { get; set; }
        [ColumnaTabla486LP]
        public string Talle { get; set; }
        [ColumnaTabla486LP]
        public string Cantidad { get; set; }
        [ColumnaTabla486LP]
        public string Precio { get; set; }
        [ColumnaTabla486LP]
        public string Accion { get; set; }
        [ColumnaTabla486LP]
        public int IdProducto { get; set; }
        [ColumnaTabla486LP]
        public string Motivo { get; set; }

        public DetalleImportacion486LP() { }

        public override string ToString()
        {
            return $"Linea {Linea}: {Marca} {Modelo} {Color} {Talle} ({Accion})";
        }
    }
}

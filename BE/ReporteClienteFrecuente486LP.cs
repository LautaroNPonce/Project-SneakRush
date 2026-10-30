using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BE
{
    public class ReporteClienteFrecuente486LP
    {
        [ColumnaTabla486LP]
        public string DNI { get; set; }
        [ColumnaTabla486LP]
        public string Nombre { get; set; }
        [ColumnaTabla486LP]
        public string Apellido { get; set; }
        [ColumnaTabla486LP]
        public int CantidadCompras { get; set; }
        [ColumnaTabla486LP]
        public decimal TotalGastado { get; set; }
        [ColumnaTabla486LP]
        public DateTime UltimaCompra { get; set; }

        public string NombreCompleto
        {
            get { return $"{Apellido}, {Nombre}"; }
        }

        public ReporteClienteFrecuente486LP() { }

        public override string ToString()
        {
            return $"{DNI} - {NombreCompleto}: {CantidadCompras} compras, ${TotalGastado}";
        }
    }
}

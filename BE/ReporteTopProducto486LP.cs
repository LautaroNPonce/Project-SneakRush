using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BE
{
    public class ReporteTopProducto486LP
    {
        [ColumnaTabla486LP]
        public string Marca { get; set; }
        [ColumnaTabla486LP]
        public string Modelo { get; set; }
        [ColumnaTabla486LP]
        public int Unidades { get; set; }
        [ColumnaTabla486LP]
        public decimal Monto { get; set; }

        public string NombreCompleto
        {
            get { return $"{Marca} {Modelo}"; }
        }

        public ReporteTopProducto486LP() { }

        public override string ToString()
        {
            return $"{NombreCompleto}: {Unidades} unidades, ${Monto}";
        }
    }
}

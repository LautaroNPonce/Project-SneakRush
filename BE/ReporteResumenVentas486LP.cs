using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BE
{
    public class ReporteResumenVentas486LP
    {
        [ColumnaTabla486LP]
        public int CantidadVentas { get; set; }
        [ColumnaTabla486LP]
        public decimal MontoTotal { get; set; }
        [ColumnaTabla486LP]
        public int Unidades { get; set; }
        public decimal TicketPromedio
        {
            get { return CantidadVentas > 0 ? Math.Round(MontoTotal / CantidadVentas, 2) : 0m; }
        }

        public ReporteResumenVentas486LP() { }
    }
}

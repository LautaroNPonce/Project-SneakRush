using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BE
{
    // Totales de un rango de fechas (SP Reporte_ResumenVentas). Siempre devuelve una fila, aunque no haya ventas (todo en 0).
    public class ReporteResumenVentas486LP
    {
        [ColumnaTabla486LP]
        public int CantidadVentas { get; set; }
        [ColumnaTabla486LP]
        public decimal MontoTotal { get; set; }
        [ColumnaTabla486LP]
        public int Unidades { get; set; }

        // Monto total dividido la cantidad de ventas (0 si no hubo ventas)
        public decimal TicketPromedio
        {
            get { return CantidadVentas > 0 ? Math.Round(MontoTotal / CantidadVentas, 2) : 0m; }
        }

        public ReporteResumenVentas486LP() { }
    }
}

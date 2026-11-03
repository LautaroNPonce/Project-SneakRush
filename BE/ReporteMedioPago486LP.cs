using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BE
{
    // Una fila del SP Reporte_VentasMedioPago: lo vendido con cada medio de pago.
    public class ReporteMedioPago486LP
    {
        [ColumnaTabla486LP]
        public string MedioPago { get; set; }
        [ColumnaTabla486LP]
        public int CantidadVentas { get; set; }
        [ColumnaTabla486LP]
        public decimal MontoTotal { get; set; }

        public ReporteMedioPago486LP() { }

        public override string ToString()
        {
            return $"{MedioPago}: {CantidadVentas} ventas, ${MontoTotal}";
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BE
{
    public class ReporteVentaPeriodo486LP
    {
        // Primer dia del periodo (dia 1 del mes, o lunes de la semana)
        [ColumnaTabla486LP]
        public DateTime Periodo { get; set; }
        [ColumnaTabla486LP]
        public int CantidadVentas { get; set; }
        [ColumnaTabla486LP]
        public decimal MontoTotal { get; set; }
        [ColumnaTabla486LP]
        public int Unidades { get; set; }

        public ReporteVentaPeriodo486LP() { }

        public override string ToString()
        {
            return $"{Periodo:dd/MM/yyyy}: {CantidadVentas} ventas, ${MontoTotal}";
        }
    }
}

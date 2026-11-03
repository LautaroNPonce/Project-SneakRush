using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BE
{
    public class ReportePuntoSerie486LP
    {
        public const int COMPLETO = 0;
        public const int EN_CURSO = 1;
        public const int PROYECTADO = 2;
        public DateTime Periodo { get; set; }
        public int Tipo { get; set; }
        public int CantidadVentas { get; set; }
        public decimal? MontoReal { get; set; }
        public decimal? MontoProyectado { get; set; }

        public ReportePuntoSerie486LP() { }
    }
}

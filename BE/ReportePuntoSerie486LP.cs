using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BE
{
    // Un periodo de la serie que se grafica y se muestra en la grilla: puede ser real (ya termino), en curso (todavia no termino) o proyectado (futuro).
    public class ReportePuntoSerie486LP
    {
        public const int COMPLETO = 0;
        public const int EN_CURSO = 1;
        public const int PROYECTADO = 2;

        // Primer dia del periodo
        public DateTime Periodo { get; set; }
        public int Tipo { get; set; }
        public int CantidadVentas { get; set; }
        // Lo vendido de verdad (en un periodo en curso es lo que va hasta hoy). null en los proyectados.
        public decimal? MontoReal { get; set; }
        // Lo proyectado por el modelo (en un periodo en curso es la proyeccion de cierre). null en los completos.
        public decimal? MontoProyectado { get; set; }

        public ReportePuntoSerie486LP() { }
    }
}

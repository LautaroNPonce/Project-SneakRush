using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BE
{
    // Resultado completo del reporte de reporte inteligente: lo que se muestra en las tarjetas, los graficos y la grilla.
    public class ReporteInteligente486LP
    {
        // Rango realmente consultado (el rango pedido se ajusta a periodos enteros y no pasa del periodo actual)
        public DateTime DesdeEfectivo { get; set; }
        public DateTime HastaEfectivo { get; set; }
        public int Agrupacion { get; set; }
        public int Metodo { get; set; }
        public int PeriodosAProyectar { get; set; }

        public ReporteResumenVentas486LP Resumen { get; set; }
        public List<ReportePuntoSerie486LP> Serie { get; set; }
        public List<ReporteTopProducto486LP> TopProductos { get; set; }
        public List<ReporteMedioPago486LP> MediosPago { get; set; }

        // Periodos que ya terminaron dentro del rango (los unicos que usa el modelo para proyectar)
        public int PeriodosCompletos { get; set; }
        // true si hay periodos completos suficientes para proyectar
        public bool DatosSuficientes { get; set; }

        // Ultimo periodo completo y su comparacion con el anterior
        public DateTime? PeriodoUltimoCompleto { get; set; }
        public decimal MontoUltimoCompleto { get; set; }
        // Variacion porcentual contra el periodo completo anterior (null si no hay anterior o vendio 0)
        public decimal? VariacionUltimoVsAnterior { get; set; }

        // Periodo en curso (el que incluye el dia de hoy)
        public bool HayEnCurso { get; set; }
        public DateTime? PeriodoEnCurso { get; set; }
        public decimal MontoEnCurso { get; set; }
        public int DiasTranscurridos { get; set; }
        public int DiasDelPeriodo { get; set; }
        // Cierre del periodo en curso si se mantiene el ritmo diario de lo que va
        public decimal CierreAlRitmoActual { get; set; }
        // Cierre del periodo en curso segun el modelo (null si faltan datos)
        public decimal? ProyeccionCierre { get; set; }

        // Primer periodo que todavia no empezo y su proyeccion (null si faltan datos)
        public DateTime? PeriodoProximo { get; set; }
        public decimal? ProyeccionProximo { get; set; }
        // Variacion porcentual del proximo periodo contra el ultimo periodo completo (null si no se puede calcular)
        public decimal? VariacionProximoVsUltimo { get; set; }

        public ReporteInteligente486LP()
        {
            Resumen = new ReporteResumenVentas486LP();
            Serie = new List<ReportePuntoSerie486LP>();
            TopProductos = new List<ReporteTopProducto486LP>();
            MediosPago = new List<ReporteMedioPago486LP>();
        }
    }
}

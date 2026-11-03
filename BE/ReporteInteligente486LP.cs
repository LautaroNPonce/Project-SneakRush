using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BE
{
    public class ReporteInteligente486LP
    {
        public DateTime DesdeEfectivo { get; set; }
        public DateTime HastaEfectivo { get; set; }
        public int Agrupacion { get; set; }
        public int Metodo { get; set; }
        public int PeriodosAProyectar { get; set; }

        public ReporteResumenVentas486LP Resumen { get; set; }
        public List<ReportePuntoSerie486LP> Serie { get; set; }
        public List<ReporteTopProducto486LP> TopProductos { get; set; }
        public List<ReporteMedioPago486LP> MediosPago { get; set; }
        public int PeriodosCompletos { get; set; }
        public bool DatosSuficientes { get; set; }
        public DateTime? PeriodoUltimoCompleto { get; set; }
        public decimal MontoUltimoCompleto { get; set; }
        public decimal? VariacionUltimoVsAnterior { get; set; }
        public bool HayEnCurso { get; set; }
        public DateTime? PeriodoEnCurso { get; set; }
        public decimal MontoEnCurso { get; set; }
        public int DiasTranscurridos { get; set; }
        public int DiasDelPeriodo { get; set; }
        public decimal CierreAlRitmoActual { get; set; }
        public decimal? ProyeccionCierre { get; set; }
        public DateTime? PeriodoProximo { get; set; }
        public decimal? ProyeccionProximo { get; set; }
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

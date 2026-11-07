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


    // Un modelo (suma de todos sus colores y talles) con lo que se vendio en el periodo y el stock que hay hoy
    public class ReporteRotacionProducto486LP
    {
        public const int COMPRAR = 1;
        public const int MANTENER = 2;
        public const int NO_COMPRAR = 3;

        [ColumnaTabla486LP]
        public string Marca { get; set; }
        [ColumnaTabla486LP]
        public string Modelo { get; set; }
        [ColumnaTabla486LP]
        public int Unidades { get; set; }
        [ColumnaTabla486LP]
        public decimal Monto { get; set; }
        [ColumnaTabla486LP]
        public int Stock { get; set; }

        public decimal UnidadesPorMes { get; set; }
        public decimal? MesesDeStock { get; set; }   // null cuando no se vendio nada en el periodo
        public int Recomendacion { get; set; }
        public int CantidadSugerida { get; set; }

        public string NombreCompleto
        {
            get { return $"{Marca} {Modelo}"; }
        }

        public ReporteRotacionProducto486LP() { }

        public override string ToString()
        {
            return $"{NombreCompleto}: {Unidades} vendidas, stock {Stock}";
        }
    }

    // Reporte de rotacion: que modelos se venden mas, cuales menos y cuanto conviene comprar de cada uno
    public class ReporteRotacion486LP
    {
        public DateTime Desde { get; set; }
        public DateTime Hasta { get; set; }
        public decimal MesesPeriodo { get; set; }
        public int MesesACubrir { get; set; }

        public List<ReporteRotacionProducto486LP> Productos { get; set; }     // ordenados: primero comprar, despues mantener, despues no comprar
        public List<ReporteRotacionProducto486LP> MasVendidos { get; set; }
        public List<ReporteRotacionProducto486LP> MenosVendidos { get; set; }

        public int TotalUnidades { get; set; }
        public decimal TotalMonto { get; set; }
        public int ModelosAnalizados { get; set; }
        public int ModelosConVentas { get; set; }
        public int ModelosSinVentas { get; set; }
        public int ModelosAComprar { get; set; }
        public int UnidadesAComprar { get; set; }
        public int ModelosNoComprar { get; set; }
        public ReporteRotacionProducto486LP MasVendido { get; set; }
        public ReporteRotacionProducto486LP MenosVendido { get; set; }   // el que menos vendio entre los que tuvieron ventas

        public ReporteRotacion486LP()
        {
            Productos = new List<ReporteRotacionProducto486LP>();
            MasVendidos = new List<ReporteRotacionProducto486LP>();
            MenosVendidos = new List<ReporteRotacionProducto486LP>();
        }
    }
}

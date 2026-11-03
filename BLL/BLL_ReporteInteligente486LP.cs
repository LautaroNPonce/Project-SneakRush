using BE;
using Mappers;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    // Reporte inteligente: ventas por periodo, comparacion con el periodo anterior y proyeccion de ventas.
    // La proyeccion se calcula aca, en C#, con dos metodos simples y explicables (tendencia lineal y promedio movil).
    public class BLL_ReporteInteligente486LP
    {
        private Mapper_ReporteInteligente486LP ObjetoMapper = new Mapper_ReporteInteligente486LP();
        private BLL_Bitacora486LP ObjBitacora = new BLL_Bitacora486LP();

        private const string MODULO = "Ventas";

        // Como se agrupan las ventas
        public const int AGRUPAR_MES = 1;
        public const int AGRUPAR_SEMANA = 2;

        // Como se proyecta
        public const int METODO_TENDENCIA = 1;
        public const int METODO_PROMEDIO_MOVIL = 2;

        // Periodos completos que hacen falta para poder proyectar
        public const int MIN_PERIODOS = 3;
        // Cuantos periodos futuros se pueden proyectar como maximo
        public const int MAX_PERIODOS_PROYECTAR = 6;
        // Cuantos periodos usa el promedio movil
        public const int VENTANA_PROMEDIO = 3;
        // Cuantos modelos se muestran en el grafico de mas vendidos
        public const int CANTIDAD_TOP = 5;

        // ===================== Validaciones =====================

        public bool ValidarRangoFechas(DateTime desde, DateTime hasta)
        {
            return desde.Date <= hasta.Date;
        }

        public bool ValidarAgrupacion(int agrupacion)
        {
            return agrupacion == AGRUPAR_MES || agrupacion == AGRUPAR_SEMANA;
        }

        public bool ValidarMetodo(int metodo)
        {
            return metodo == METODO_TENDENCIA || metodo == METODO_PROMEDIO_MOVIL;
        }

        public bool ValidarPeriodosAProyectar(int periodos)
        {
            return periodos >= 1 && periodos <= MAX_PERIODOS_PROYECTAR;
        }

        // ===================== Periodos =====================

        // Primer dia del periodo que contiene la fecha: dia 1 del mes, o el lunes de esa semana
        public static DateTime InicioPeriodo(DateTime fecha, int agrupacion)
        {
            fecha = fecha.Date;
            if (agrupacion == AGRUPAR_SEMANA)
                return fecha.AddDays(-(((int)fecha.DayOfWeek + 6) % 7));
            return new DateTime(fecha.Year, fecha.Month, 1);
        }

        // Primer dia del periodo que sigue
        public static DateTime SiguientePeriodo(DateTime inicio, int agrupacion)
        {
            return agrupacion == AGRUPAR_SEMANA ? inicio.AddDays(7) : inicio.AddMonths(1);
        }

        // Ultimo dia del periodo
        public static DateTime FinPeriodo(DateTime inicio, int agrupacion)
        {
            return SiguientePeriodo(inicio, agrupacion).AddDays(-1);
        }

        // El rango pedido se ajusta a periodos enteros (para no comparar un mes completo con medio mes) y no pasa del periodo de hoy
        public static void RangoEfectivo(DateTime desde, DateTime hasta, int agrupacion, DateTime hoy, out DateTime desdeEfectivo, out DateTime hastaEfectivo)
        {
            DateTime tope = hasta.Date > hoy.Date ? hoy.Date : hasta.Date;
            desdeEfectivo = InicioPeriodo(desde, agrupacion);
            hastaEfectivo = FinPeriodo(InicioPeriodo(tope, agrupacion), agrupacion);
        }

        // ===================== Generar =====================

        // Devuelve el reporte armado, un reporte vacio si los filtros no son validos, o null si hubo un error (queda registrado en la bitacora).
        public ReporteInteligente486LP Generar(DateTime desde, DateTime hasta, int agrupacion, int metodo, int periodosAProyectar)
        {
            try
            {
                if (!ValidarRangoFechas(desde, hasta) || !ValidarAgrupacion(agrupacion) || !ValidarMetodo(metodo) || !ValidarPeriodosAProyectar(periodosAProyectar))
                    return new ReporteInteligente486LP();

                DateTime hoy = DateTime.Today;
                DateTime desdeEf, hastaEf;
                RangoEfectivo(desde, hasta, agrupacion, hoy, out desdeEf, out hastaEf);

                List<ReporteVentaPeriodo486LP> ventas = ObjetoMapper.VentasPorPeriodo(desdeEf, hastaEf, agrupacion);
                ReporteResumenVentas486LP resumen = ObjetoMapper.Resumen(desdeEf, hastaEf);
                List<ReporteTopProducto486LP> top = ObjetoMapper.TopProductos(desdeEf, hastaEf, CANTIDAD_TOP);
                List<ReporteMedioPago486LP> medios = ObjetoMapper.VentasPorMedioPago(desdeEf, hastaEf);

                return Calcular(desdeEf, hastaEf, agrupacion, metodo, periodosAProyectar, hoy, ventas, resumen, top, medios);
            }
            catch (Exception ex)
            {
                ObjBitacora.Registrar(new BitacoraEvento486LP(MODULO, $"Error en BLL_ReporteInteligente.Generar(): {ex.Message}", Criticidad486LP.MuyAlta, "Sistema", "Sistema"));
                return null;
            }
        }

        // Arma el reporte a partir de los datos ya leidos de la base. No toca la base, asi se puede probar solo.
        // "hoy" es el dia desde el que se mira: los periodos que terminaron antes son completos, el que incluye hoy esta en curso.
        public ReporteInteligente486LP Calcular(DateTime desdeEfectivo, DateTime hastaEfectivo, int agrupacion, int metodo, int periodosAProyectar, DateTime hoy,
            List<ReporteVentaPeriodo486LP> ventas, ReporteResumenVentas486LP resumen, List<ReporteTopProducto486LP> top, List<ReporteMedioPago486LP> medios)
        {
            hoy = hoy.Date;

            ReporteInteligente486LP r = new ReporteInteligente486LP();
            r.DesdeEfectivo = desdeEfectivo.Date;
            r.HastaEfectivo = hastaEfectivo.Date;
            r.Agrupacion = agrupacion;
            r.Metodo = metodo;
            r.PeriodosAProyectar = periodosAProyectar;
            r.Resumen = resumen ?? new ReporteResumenVentas486LP();
            r.TopProductos = top ?? new List<ReporteTopProducto486LP>();
            r.MediosPago = medios ?? new List<ReporteMedioPago486LP>();

            Dictionary<DateTime, ReporteVentaPeriodo486LP> porPeriodo = new Dictionary<DateTime, ReporteVentaPeriodo486LP>();
            if (ventas != null)
            {
                foreach (ReporteVentaPeriodo486LP v in ventas) porPeriodo[v.Periodo.Date] = v;
            }

            // Serie real: un punto por cada periodo del rango, tambien los que no tuvieron ventas (valen 0)
            List<decimal> completos = new List<decimal>();
            DateTime ultimoInicio = InicioPeriodo(hastaEfectivo, agrupacion);
            DateTime inicio = InicioPeriodo(desdeEfectivo, agrupacion);
            DateTime inicioUltimoCompleto = DateTime.MinValue;

            while (inicio <= ultimoInicio)
            {
                ReporteVentaPeriodo486LP v;
                porPeriodo.TryGetValue(inicio, out v);

                ReportePuntoSerie486LP punto = new ReportePuntoSerie486LP();
                punto.Periodo = inicio;
                punto.CantidadVentas = v != null ? v.CantidadVentas : 0;
                punto.MontoReal = v != null ? v.MontoTotal : 0m;

                DateTime fin = FinPeriodo(inicio, agrupacion);
                if (fin < hoy)
                {
                    punto.Tipo = ReportePuntoSerie486LP.COMPLETO;
                    completos.Add(punto.MontoReal.Value);
                    inicioUltimoCompleto = inicio;
                }
                else if (inicio <= hoy)
                {
                    punto.Tipo = ReportePuntoSerie486LP.EN_CURSO;
                }
                else
                {
                    break;   // periodo que todavia no empezo: no hay nada real que mostrar
                }

                r.Serie.Add(punto);
                inicio = SiguientePeriodo(inicio, agrupacion);
            }

            r.PeriodosCompletos = completos.Count;
            r.DatosSuficientes = completos.Count >= MIN_PERIODOS;

            // Ultimo periodo completo y comparacion con el anterior
            if (completos.Count > 0)
            {
                r.PeriodoUltimoCompleto = inicioUltimoCompleto;
                r.MontoUltimoCompleto = completos[completos.Count - 1];

                if (completos.Count >= 2)
                {
                    decimal anterior = completos[completos.Count - 2];
                    if (anterior > 0m)
                        r.VariacionUltimoVsAnterior = Math.Round((r.MontoUltimoCompleto - anterior) / anterior * 100m, 1);
                }
            }

            // Periodo en curso: lo que va y como cerraria si sigue al mismo ritmo
            ReportePuntoSerie486LP enCurso = r.Serie.FirstOrDefault(x => x.Tipo == ReportePuntoSerie486LP.EN_CURSO);
            if (enCurso != null)
            {
                r.HayEnCurso = true;
                r.PeriodoEnCurso = enCurso.Periodo;
                r.MontoEnCurso = enCurso.MontoReal ?? 0m;
                r.DiasTranscurridos = (hoy - enCurso.Periodo).Days + 1;
                r.DiasDelPeriodo = (FinPeriodo(enCurso.Periodo, agrupacion) - enCurso.Periodo).Days + 1;
                r.CierreAlRitmoActual = Math.Round(r.MontoEnCurso / r.DiasTranscurridos * r.DiasDelPeriodo, 2);
            }

            // Proyeccion: solo con los periodos completos. Si hay un periodo en curso, el primer valor proyectado es su cierre.
            if (r.DatosSuficientes)
            {
                int pasos = periodosAProyectar + (r.HayEnCurso ? 1 : 0);
                List<decimal> proyectados = Proyectar(completos, pasos, metodo);

                DateTime periodo = SiguientePeriodo(inicioUltimoCompleto, agrupacion);
                for (int i = 0; i < proyectados.Count; i++)
                {
                    if (i == 0 && enCurso != null)
                    {
                        // el cierre del periodo en curso nunca puede ser menor a lo que ya se vendio
                        decimal cierre = Math.Max(proyectados[i], r.MontoEnCurso);
                        enCurso.MontoProyectado = cierre;
                        r.ProyeccionCierre = cierre;
                    }
                    else
                    {
                        ReportePuntoSerie486LP futuro = new ReportePuntoSerie486LP();
                        futuro.Periodo = periodo;
                        futuro.Tipo = ReportePuntoSerie486LP.PROYECTADO;
                        futuro.MontoProyectado = proyectados[i];
                        r.Serie.Add(futuro);
                    }
                    periodo = SiguientePeriodo(periodo, agrupacion);
                }

                ReportePuntoSerie486LP proximo = r.Serie.FirstOrDefault(x => x.Tipo == ReportePuntoSerie486LP.PROYECTADO);
                if (proximo != null)
                {
                    r.PeriodoProximo = proximo.Periodo;
                    r.ProyeccionProximo = proximo.MontoProyectado;
                    if (r.MontoUltimoCompleto > 0m)
                        r.VariacionProximoVsUltimo = Math.Round((proximo.MontoProyectado.Value - r.MontoUltimoCompleto) / r.MontoUltimoCompleto * 100m, 1);
                }
            }

            return r;
        }

        // ===================== Proyeccion =====================

        // Proyecta los "pasos" periodos siguientes a partir del historico (un monto por periodo completo, en orden).
        //  - Tendencia lineal: recta de minimos cuadrados y = a + b*x sobre los periodos pasados, prolongada hacia adelante.
        //  - Promedio movil: promedio de los ultimos 3 periodos; para proyectar mas de uno, cada proyeccion entra en el promedio siguiente.
        // Nunca devuelve montos negativos.
        public static List<decimal> Proyectar(List<decimal> historico, int pasos, int metodo)
        {
            List<decimal> resultado = new List<decimal>();
            if (historico == null || historico.Count == 0 || pasos < 1) return resultado;

            if (metodo == METODO_PROMEDIO_MOVIL)
            {
                List<decimal> trabajo = new List<decimal>(historico);
                for (int i = 0; i < pasos; i++)
                {
                    int n = trabajo.Count;
                    int ventana = Math.Min(VENTANA_PROMEDIO, n);
                    decimal suma = 0m;
                    for (int j = n - ventana; j < n; j++) suma += trabajo[j];
                    decimal promedio = Math.Round(suma / ventana, 2);
                    trabajo.Add(promedio);
                    resultado.Add(promedio);
                }
            }
            else
            {
                int n = historico.Count;
                double sx = 0, sy = 0, sxy = 0, sxx = 0;
                for (int i = 0; i < n; i++)
                {
                    double y = (double)historico[i];
                    sx += i;
                    sy += y;
                    sxy += i * y;
                    sxx += (double)i * i;
                }
                double denominador = n * sxx - sx * sx;
                double b = denominador == 0 ? 0 : (n * sxy - sx * sy) / denominador;   // pendiente
                double a = (sy - b * sx) / n;                                             // ordenada

                for (int k = 1; k <= pasos; k++)
                {
                    double y = a + b * (n - 1 + k);
                    if (y < 0) y = 0;
                    resultado.Add(Math.Round((decimal)y, 2));
                }
            }

            return resultado;
        }

        // ===================== Bitacora =====================

        // Deja constancia en la bitacora de que se exporto el reporte a PDF
        public void RegistrarExportacion(int periodos)
        {
            try
            {
                var usuario = SessionManager486LP.ObtenerInstancia().UsuarioActual();
                ObjBitacora.Registrar(new BitacoraEvento486LP(MODULO,
                    $"Reporte inteligente exportado a PDF ({periodos} periodos).",
                    Criticidad486LP.Baja,
                    usuario?.DNI ?? "Sistema",
                    usuario?.NombreUsuario ?? "Sistema"));
            }
            catch (Exception ex)
            {
                ObjBitacora.Registrar(new BitacoraEvento486LP(MODULO, $"Error en BLL_ReporteInteligente.RegistrarExportacion(): {ex.Message}", Criticidad486LP.MuyAlta, "Sistema", "Sistema"));
            }
        }
    }
}

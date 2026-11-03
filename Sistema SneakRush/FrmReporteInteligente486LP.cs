using BE;
using BLL;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Services;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Charting = System.Windows.Forms.DataVisualization.Charting;
using PdfFont = iTextSharp.text.Font;
using PdfRectangle = iTextSharp.text.Rectangle;

namespace Sistema_SneakRush
{
    public partial class FrmReporteInteligente486LP : Form, IObserver486LP, IRefrescable486LP
    {
        private BLL_ReporteInteligente486LP _bll = new BLL_ReporteInteligente486LP();
        private readonly string f = "FrmReporteInteligente486LP";
        private bool _puedeGenerar;
        private bool _puedeLimpiar;
        private bool _puedeExportar;
        private bool _puedeCerrar;
        private bool _cargado = false;
        private bool _generado = false;
        private ReporteInteligente486LP _reporte = new ReporteInteligente486LP();
        private List<FilaPeriodo486LP> _filas = new List<FilaPeriodo486LP>();

        private static readonly Color Azul = Color.FromArgb(12, 68, 124);
        private static readonly Color Naranja = Color.FromArgb(240, 90, 40);
        private static readonly Color AzulClaro = Color.FromArgb(130, 165, 210);

        public FrmReporteInteligente486LP()
        {
            InitializeComponent();
            Program.LanguageManager.Agregar(this);
            this.FormClosing += FrmReporteInteligente486LP_FormClosing;
        }

        private void FrmReporteInteligente486LP_Load(object sender, EventArgs e)
        {
            if (!TienePatenteAcceso())
            {
                var lm = Program.LanguageManager;
                MessageBox.Show(
                    lm.ObtenerTexto(f, "Msg.SinPermiso", "No tiene permiso para ver el reporte inteligente."),
                    lm.ObtenerTexto(f, "Msg.SinPermiso.Title", "Acceso denegado"),
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);

                this.BeginInvoke(new Action(() =>
                {
                    this.DialogResult = DialogResult.Cancel;
                    this.Close();
                }));
                return;
            }

            ConfigurarGrilla();
            ConfigurarGraficos();
            RellenarCombos();
            ValoresIniciales();
            AjustarBotonesSegunPerfil();
            AplicarPermisosBotones();
            _cargado = true;
            ActualizarIdioma();
        }

        private bool TienePatenteAcceso()
        {
            var usuario = SessionManager486LP.ObtenerInstancia().UsuarioActual();
            if (usuario == null) return false;

            List<string> permisos = new BLL_Perfil486LP().ObtenerPermisosPorRol(usuario.Rol);
            return permisos.Contains("REPORTE_INTELIGENTE");
        }

        private void AjustarBotonesSegunPerfil()
        {
            var usuario = SessionManager486LP.ObtenerInstancia().UsuarioActual();
            if (usuario == null) return;

            List<string> permisos = new BLL_Perfil486LP().ObtenerPermisosPorRol(usuario.Rol);

            _puedeGenerar = permisos.Contains("REPORTE_INTELIGENTE_GENERAR");
            _puedeLimpiar = permisos.Contains("REPORTE_INTELIGENTE_LIMPIAR");
            _puedeExportar = permisos.Contains("REPORTE_INTELIGENTE_EXPORTAR");
            _puedeCerrar = permisos.Contains("REPORTE_INTELIGENTE_CERRAR");
        }

        private void AplicarPermisosBotones()
        {
            btnGenerar.Enabled = _puedeGenerar;
            btnLimpiar.Enabled = _puedeLimpiar;
            btnExportarPDF.Enabled = _puedeExportar;
            btnCerrar.Enabled = _puedeCerrar;
        }

        private void ValoresIniciales()
        {
            dtpHasta.Value = DateTime.Today;
            dtpDesde.Value = DateTime.Today.AddMonths(-12);
            cmbAgrupar.SelectedIndex = 0;// por mes
            nudProyectar.Value = 3;
            cmbMetodo.SelectedIndex = 0; // tendencia lineal
        }

        // Los combos guardan la opcion por posicion (0 = mes / tendencia lineal, 1 = semana / promedio movil) solo cambia el texto segun el idioma
        private void RellenarCombos()
        {
            var lm = Program.LanguageManager;

            int agrupar = cmbAgrupar.SelectedIndex < 0 ? 0 : cmbAgrupar.SelectedIndex;
            cmbAgrupar.Items.Clear();
            cmbAgrupar.Items.Add(lm.ObtenerTexto(f, "agrupar.Mes", "Mes"));
            cmbAgrupar.Items.Add(lm.ObtenerTexto(f, "agrupar.Semana", "Semana"));
            cmbAgrupar.SelectedIndex = agrupar;

            int metodo = cmbMetodo.SelectedIndex < 0 ? 0 : cmbMetodo.SelectedIndex;
            cmbMetodo.Items.Clear();
            cmbMetodo.Items.Add(lm.ObtenerTexto(f, "metodo.Tendencia", "Tendencia lineal"));
            cmbMetodo.Items.Add(lm.ObtenerTexto(f, "metodo.Promedio", "Promedio móvil (3 períodos)"));
            cmbMetodo.SelectedIndex = metodo;
        }

        private int AgrupacionElegida()
        {
            return cmbAgrupar.SelectedIndex == 1 ? BLL_ReporteInteligente486LP.AGRUPAR_SEMANA : BLL_ReporteInteligente486LP.AGRUPAR_MES;
        }

        private int MetodoElegido()
        {
            return cmbMetodo.SelectedIndex == 1 ? BLL_ReporteInteligente486LP.METODO_PROMEDIO_MOVIL : BLL_ReporteInteligente486LP.METODO_TENDENCIA;
        }

        // Acomoda los filtros de izquierda a derecha segun el ancho del texto (cambia con el idioma)
        private void DistribuirFiltros()
        {
            int x = 18;
            Ubicar(ref x, lblDesde, dtpDesde);
            Ubicar(ref x, lblHasta, dtpHasta);
            Ubicar(ref x, lblAgrupar, cmbAgrupar);

            x = 18;
            Ubicar(ref x, lblProyectar, nudProyectar);
            Ubicar(ref x, lblMetodo, cmbMetodo);
        }

        private void Ubicar(ref int x, Label etiqueta, Control control)
        {
            etiqueta.Left = x;
            control.Left = etiqueta.Right + 8;
            x = control.Right + 26;
        }

        private void ConfigurarGrilla()
        {
            dgvPeriodos.AutoGenerateColumns = false;
            dgvPeriodos.AllowUserToAddRows = false;
            dgvPeriodos.AllowUserToDeleteRows = false;
            dgvPeriodos.ReadOnly = true;
            dgvPeriodos.MultiSelect = false;
            dgvPeriodos.RowHeadersVisible = false;
            dgvPeriodos.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvPeriodos.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvPeriodos.BackgroundColor = Color.White;
            dgvPeriodos.EnableHeadersVisualStyles = false;
            dgvPeriodos.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;

            DataGridViewCellStyle estiloHeader = new DataGridViewCellStyle();
            estiloHeader.BackColor = Azul;
            estiloHeader.ForeColor = Color.White;
            estiloHeader.Font = new System.Drawing.Font("Segoe UI", 9.75F, FontStyle.Bold);
            estiloHeader.SelectionBackColor = Azul;
            estiloHeader.SelectionForeColor = Color.White;
            estiloHeader.WrapMode = DataGridViewTriState.True;
            dgvPeriodos.ColumnHeadersDefaultCellStyle = estiloHeader;

            DataGridViewCellStyle estiloFilas = new DataGridViewCellStyle();
            estiloFilas.BackColor = Color.White;
            estiloFilas.SelectionBackColor = Naranja;
            estiloFilas.SelectionForeColor = Color.White;
            dgvPeriodos.RowsDefaultCellStyle = estiloFilas;

            dgvPeriodos.Columns.Clear();
            dgvPeriodos.Columns.Add(new DataGridViewTextBoxColumn { Name = "colPeriodo", DataPropertyName = "Periodo", FillWeight = 22 });
            dgvPeriodos.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colCantidadVentas", DataPropertyName = "CantidadVentas", FillWeight = 14,
                DefaultCellStyle = new DataGridViewCellStyle { NullValue = "-", Alignment = DataGridViewContentAlignment.MiddleCenter }
            });
            dgvPeriodos.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colMontoReal", DataPropertyName = "MontoReal", FillWeight = 22,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "C2", NullValue = "-", Alignment = DataGridViewContentAlignment.MiddleRight }
            });
            dgvPeriodos.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colMontoProyectado", DataPropertyName = "MontoProyectado", FillWeight = 22,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "C2", NullValue = "-", Alignment = DataGridViewContentAlignment.MiddleRight }
            });
            dgvPeriodos.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colEstado", DataPropertyName = "Estado", FillWeight = 20,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter }
            });

            TraducirColumnas();
        }

        private void ConfigurarGraficos()
        {
            chartVentas.Series.Clear();
            chartVentas.ChartAreas.Clear();
            chartVentas.Legends.Clear();
            chartVentas.Titles.Clear();

            Charting.ChartArea areaV = new Charting.ChartArea("areaVentas");
            areaV.BackColor = Color.White;
            areaV.AxisX.MajorGrid.Enabled = false;
            areaV.AxisX.LabelStyle.Font = new System.Drawing.Font("Segoe UI", 8F);
            areaV.AxisY.LabelStyle.Format = "C0";
            areaV.AxisY.LabelStyle.Font = new System.Drawing.Font("Segoe UI", 8F);
            areaV.AxisY.MajorGrid.LineColor = Color.Gainsboro;
            chartVentas.ChartAreas.Add(areaV);

            Charting.Legend leyendaV = new Charting.Legend("leyendaVentas");
            leyendaV.Docking = Charting.Docking.Bottom;
            leyendaV.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            chartVentas.Legends.Add(leyendaV);

            Charting.Series serieReal = new Charting.Series("serieReal");
            serieReal.ChartType = Charting.SeriesChartType.Column;
            serieReal.ChartArea = "areaVentas";
            serieReal.Legend = "leyendaVentas";
            serieReal.Color = Azul;
            serieReal.IsXValueIndexed = true;
            serieReal.Font = new System.Drawing.Font("Segoe UI", 7.5F);
            chartVentas.Series.Add(serieReal);

            Charting.Series serieProy = new Charting.Series("serieProyeccion");
            serieProy.ChartType = Charting.SeriesChartType.Line;
            serieProy.ChartArea = "areaVentas";
            serieProy.Legend = "leyendaVentas";
            serieProy.Color = Naranja;
            serieProy.BorderWidth = 3;
            serieProy.BorderDashStyle = Charting.ChartDashStyle.Dash;
            serieProy.MarkerStyle = Charting.MarkerStyle.Circle;
            serieProy.MarkerSize = 8;
            serieProy.IsXValueIndexed = true;
            chartVentas.Series.Add(serieProy);

            chartVentas.Titles.Add(new Charting.Title("", Charting.Docking.Top, new System.Drawing.Font("Segoe UI", 10F, FontStyle.Bold), Azul));
            chartTop.Series.Clear();
            chartTop.ChartAreas.Clear();
            chartTop.Legends.Clear();
            chartTop.Titles.Clear();

            Charting.ChartArea areaT = new Charting.ChartArea("areaTop");
            areaT.BackColor = Color.White;
            areaT.AxisX.MajorGrid.Enabled = false;
            areaT.AxisX.IsReversed = true;  // el mas vendido queda arriba
            areaT.AxisX.LabelStyle.Interval = 1;
            areaT.AxisX.LabelStyle.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            areaT.AxisY.LabelStyle.Format = "N0";
            areaT.AxisY.LabelStyle.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            areaT.AxisY.MajorGrid.LineColor = Color.Gainsboro;
            chartTop.ChartAreas.Add(areaT);

            Charting.Series serieTop = new Charting.Series("serieTop");
            serieTop.ChartType = Charting.SeriesChartType.Bar;
            serieTop.ChartArea = "areaTop";
            serieTop.Color = Azul;
            serieTop.IsValueShownAsLabel = true;
            serieTop.LabelFormat = "N0";
            serieTop.Font = new System.Drawing.Font("Segoe UI", 8.25F, FontStyle.Bold);
            chartTop.Series.Add(serieTop);

            chartTop.Titles.Add(new Charting.Title("", Charting.Docking.Top, new System.Drawing.Font("Segoe UI", 10F, FontStyle.Bold), Azul));
            chartMedios.Series.Clear();
            chartMedios.ChartAreas.Clear();
            chartMedios.Legends.Clear();
            chartMedios.Titles.Clear();

            Charting.ChartArea areaM = new Charting.ChartArea("areaMedios");
            areaM.BackColor = Color.White;
            chartMedios.ChartAreas.Add(areaM);

            Charting.Legend leyendaM = new Charting.Legend("leyendaMedios");
            leyendaM.Docking = Charting.Docking.Right;
            leyendaM.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            chartMedios.Legends.Add(leyendaM);

            Charting.Series serieMedios = new Charting.Series("serieMedios");
            serieMedios.ChartType = Charting.SeriesChartType.Pie;
            serieMedios.ChartArea = "areaMedios";
            serieMedios.Legend = "leyendaMedios";
            serieMedios.IsValueShownAsLabel = true;
            serieMedios.Label = "#PERCENT{P0}";
            serieMedios.Font = new System.Drawing.Font("Segoe UI", 8.25F, FontStyle.Bold);
            serieMedios["PieLabelStyle"] = "Inside";
            chartMedios.Series.Add(serieMedios);

            chartMedios.Titles.Add(new Charting.Title("", Charting.Docking.Top, new System.Drawing.Font("Segoe UI", 10F, FontStyle.Bold), Azul));
        }

        private string EtiquetaPeriodo(DateTime periodo, int agrupacion)
        {
            if (agrupacion == BLL_ReporteInteligente486LP.AGRUPAR_SEMANA)
                return periodo.ToString("dd/MM/yy", CultureInfo.CurrentCulture);

            string mes = periodo.ToString("MMM yyyy", CultureInfo.CurrentCulture);
            return mes.Length > 0 ? char.ToUpper(mes[0], CultureInfo.CurrentCulture) + mes.Substring(1) : mes;
        }

        private string NombreMetodo(int metodo)
        {
            var lm = Program.LanguageManager;
            return metodo == BLL_ReporteInteligente486LP.METODO_PROMEDIO_MOVIL
                ? lm.ObtenerTexto(f, "metodo.Promedio", "Promedio móvil (3 períodos)")
                : lm.ObtenerTexto(f, "metodo.Tendencia", "Tendencia lineal");
        }

        private string NombreEstado(int tipo)
        {
            var lm = Program.LanguageManager;
            if (tipo == ReportePuntoSerie486LP.EN_CURSO) return lm.ObtenerTexto(f, "estado.EnCurso", "En curso");
            if (tipo == ReportePuntoSerie486LP.PROYECTADO) return lm.ObtenerTexto(f, "estado.Proyectado", "Proyectado");
            return lm.ObtenerTexto(f, "estado.Completo", "Completo");
        }

        private List<FilaPeriodo486LP> ArmarFilas()
        {
            List<FilaPeriodo486LP> filas = new List<FilaPeriodo486LP>();
            if (!_generado) return filas;

            foreach (ReportePuntoSerie486LP p in _reporte.Serie)
            {
                FilaPeriodo486LP fila = new FilaPeriodo486LP();
                fila.Periodo = EtiquetaPeriodo(p.Periodo, _reporte.Agrupacion);
                fila.CantidadVentas = p.Tipo == ReportePuntoSerie486LP.PROYECTADO ? (int?)null : p.CantidadVentas;
                fila.MontoReal = p.MontoReal;
                fila.MontoProyectado = p.MontoProyectado;
                fila.Tipo = p.Tipo;
                fila.Estado = NombreEstado(p.Tipo);
                filas.Add(fila);
            }
            return filas;
        }

        private void ActualizarGrilla()
        {
            _filas = ArmarFilas();
            dgvPeriodos.DataSource = null;
            dgvPeriodos.DataSource = _filas;

            for (int i = 0; i < dgvPeriodos.Rows.Count && i < _filas.Count; i++)
            {
                if (_filas[i].Tipo == ReportePuntoSerie486LP.PROYECTADO)
                    dgvPeriodos.Rows[i].DefaultCellStyle.BackColor = Color.FromArgb(253, 235, 222);
                else if (_filas[i].Tipo == ReportePuntoSerie486LP.EN_CURSO)
                    dgvPeriodos.Rows[i].DefaultCellStyle.BackColor = Color.FromArgb(255, 248, 220);
            }
            dgvPeriodos.ClearSelection();
        }

        private void ActualizarGraficoVentas()
        {
            var lm = Program.LanguageManager;
            if (chartVentas.Series.Count < 2) return;

            if (chartVentas.Titles.Count > 0)
                chartVentas.Titles[0].Text = lm.ObtenerTexto(f, "chart.Ventas.Titulo", "Ventas por período y proyección");

            Charting.Series real = chartVentas.Series["serieReal"];
            Charting.Series proy = chartVentas.Series["serieProyeccion"];
            real.Points.Clear();
            proy.Points.Clear();
            real.LegendText = lm.ObtenerTexto(f, "chart.Ventas.Real", "Ventas reales");
            proy.LegendText = lm.ObtenerTexto(f, "chart.Ventas.Proyeccion", "Proyección");

            if (!_generado) return;

            bool conEtiquetas = _reporte.Serie.Count <= 14;
            real.IsValueShownAsLabel = conEtiquetas;
            real.LabelFormat = "C0";
            chartVentas.ChartAreas["areaVentas"].AxisX.LabelStyle.Interval = _reporte.Serie.Count <= 26 ? 1 : 0;
            chartVentas.ChartAreas["areaVentas"].AxisX.LabelStyle.Angle = conEtiquetas ? 0 : -45;

            int indiceUltimoCompleto = -1;
            for (int i = 0; i < _reporte.Serie.Count; i++)
            {
                if (_reporte.Serie[i].Tipo == ReportePuntoSerie486LP.COMPLETO) indiceUltimoCompleto = i;
            }

            for (int i = 0; i < _reporte.Serie.Count; i++)
            {
                ReportePuntoSerie486LP p = _reporte.Serie[i];
                string etiqueta = EtiquetaPeriodo(p.Periodo, _reporte.Agrupacion);

                int idxReal = real.Points.AddXY(etiqueta, p.MontoReal.HasValue ? (double)p.MontoReal.Value : 0.0);
                if (!p.MontoReal.HasValue) real.Points[idxReal].IsEmpty = true;
                else if (p.Tipo == ReportePuntoSerie486LP.EN_CURSO) real.Points[idxReal].Color = AzulClaro;

                double? valorProy = null;
                if (p.MontoProyectado.HasValue) valorProy = (double)p.MontoProyectado.Value;
                else if (_reporte.DatosSuficientes && i == indiceUltimoCompleto && p.MontoReal.HasValue) valorProy = (double)p.MontoReal.Value;

                int idxProy = proy.Points.AddXY(etiqueta, valorProy.HasValue ? valorProy.Value : 0.0);
                if (!valorProy.HasValue) proy.Points[idxProy].IsEmpty = true;
            }
        }

        private void ActualizarGraficoTop()
        {
            var lm = Program.LanguageManager;
            if (chartTop.Series.Count == 0) return;

            if (chartTop.Titles.Count > 0)
                chartTop.Titles[0].Text = string.Format(lm.ObtenerTexto(f, "chart.Top.Titulo", "Top {0} modelos más vendidos (unidades)"), BLL_ReporteInteligente486LP.CANTIDAD_TOP);

            Charting.Series serie = chartTop.Series["serieTop"];
            serie.Points.Clear();
            if (!_generado) return;

            foreach (ReporteTopProducto486LP t in _reporte.TopProductos)
            {
                serie.Points.AddXY(t.NombreCompleto, t.Unidades);
            }
        }

        private void ActualizarGraficoMedios()
        {
            var lm = Program.LanguageManager;
            if (chartMedios.Series.Count == 0) return;

            if (chartMedios.Titles.Count > 0)
                chartMedios.Titles[0].Text = lm.ObtenerTexto(f, "chart.Medios.Titulo", "Ventas por medio de pago");

            Charting.Series serie = chartMedios.Series["serieMedios"];
            serie.Points.Clear();
            if (!_generado) return;

            Color[] paleta = { Azul, Naranja, Color.FromArgb(46, 139, 87), Color.FromArgb(150, 110, 190), Color.FromArgb(120, 120, 120) };
            int i = 0;
            foreach (ReporteMedioPago486LP m in _reporte.MediosPago)
            {
                int idx = serie.Points.AddXY(m.MedioPago, (double)m.MontoTotal);
                serie.Points[idx].Color = paleta[i % paleta.Length];
                serie.Points[idx].LegendText = m.MedioPago;
                serie.Points[idx].AxisLabel = m.MedioPago;
                i++;
            }
        }


        private string Variacion(decimal variacion)
        {
            string flecha = variacion >= 0 ? "▲" : "▼";
            return flecha + " " + Math.Abs(variacion).ToString("0.0", CultureInfo.CurrentCulture) + "%";
        }

        private void ActualizarTarjetas()
        {
            var lm = Program.LanguageManager;

            lblCardTitulo1.Text = lm.ObtenerTexto(f, "card.Total", "Total vendido");
            lblCardTitulo2.Text = lm.ObtenerTexto(f, "card.Ticket", "Ticket promedio");
            lblCardTitulo3.Text = lm.ObtenerTexto(f, "card.UltimoCompleto", "Último período completo");
            lblCardTitulo4.Text = lm.ObtenerTexto(f, "card.EnCurso", "Período en curso");
            lblCardTitulo5.Text = lm.ObtenerTexto(f, "card.Cierre", "Proyección de cierre");
            lblCardTitulo6.Text = lm.ObtenerTexto(f, "card.Proximo", "Próximo período proyectado");

            if (!_generado)
            {
                Label[] valores = { lblCardValor1, lblCardValor2, lblCardValor3, lblCardValor4, lblCardValor5, lblCardValor6 };
                Label[] detalles = { lblCardDetalle1, lblCardDetalle2, lblCardDetalle3, lblCardDetalle4, lblCardDetalle5, lblCardDetalle6 };
                foreach (Label l in valores) l.Text = "-";
                foreach (Label l in detalles) l.Text = "";
                return;
            }

            ReporteInteligente486LP r = _reporte;
            string faltan = string.Format(lm.ObtenerTexto(f, "card.DatosInsuficientes", "Datos insuficientes: se necesitan {0} períodos completos y hay {1}."),
                BLL_ReporteInteligente486LP.MIN_PERIODOS, r.PeriodosCompletos);

            // 1) Total vendido en el rango
            lblCardValor1.Text = r.Resumen.MontoTotal.ToString("C0");
            lblCardDetalle1.Text = string.Format(lm.ObtenerTexto(f, "card.Total.Detalle", "{0} ventas | {1} unidades"), r.Resumen.CantidadVentas, r.Resumen.Unidades);

            // 2) Ticket promedio
            lblCardValor2.Text = r.Resumen.TicketPromedio.ToString("C0");
            lblCardDetalle2.Text = string.Format(lm.ObtenerTexto(f, "card.Ticket.Detalle", "Del {0} al {1}"),
                r.DesdeEfectivo.ToString("dd/MM/yyyy"), r.HastaEfectivo.ToString("dd/MM/yyyy"));

            // 3) Ultimo periodo completo, comparado con el anterior
            if (r.PeriodoUltimoCompleto.HasValue)
            {
                string etiqueta = EtiquetaPeriodo(r.PeriodoUltimoCompleto.Value, r.Agrupacion);
                lblCardValor3.Text = r.MontoUltimoCompleto.ToString("C0");
                lblCardDetalle3.Text = r.VariacionUltimoVsAnterior.HasValue
                    ? string.Format(lm.ObtenerTexto(f, "card.UltimoCompleto.Detalle", "{0} | {1} vs el anterior"), etiqueta, Variacion(r.VariacionUltimoVsAnterior.Value))
                    : string.Format(lm.ObtenerTexto(f, "card.UltimoCompleto.SinComparar", "{0} | sin período anterior para comparar"), etiqueta);
            }
            else
            {
                lblCardValor3.Text = "-";
                lblCardDetalle3.Text = lm.ObtenerTexto(f, "card.SinCompletos", "Todavía no terminó ningún período del rango.");
            }

            // 4) Periodo en curso: lo que va y como cerraria al ritmo de hoy
            if (r.HayEnCurso)
            {
                lblCardValor4.Text = r.MontoEnCurso.ToString("C0");
                lblCardDetalle4.Text = string.Format(lm.ObtenerTexto(f, "card.EnCurso.Detalle", "{0} | día {1} de {2}. Al ritmo actual: {3}"),
                    EtiquetaPeriodo(r.PeriodoEnCurso.Value, r.Agrupacion), r.DiasTranscurridos, r.DiasDelPeriodo, r.CierreAlRitmoActual.ToString("C0"));
            }
            else
            {
                lblCardValor4.Text = "-";
                lblCardDetalle4.Text = lm.ObtenerTexto(f, "card.SinEnCurso", "El rango no incluye el período actual.");
            }

            // 5) Proyeccion de cierre del periodo en curso
            if (r.HayEnCurso && r.ProyeccionCierre.HasValue)
            {
                lblCardValor5.Text = r.ProyeccionCierre.Value.ToString("C0");
                lblCardDetalle5.Text = string.Format("{0} | {1}", EtiquetaPeriodo(r.PeriodoEnCurso.Value, r.Agrupacion), NombreMetodo(r.Metodo));
            }
            else
            {
                lblCardValor5.Text = "-";
                lblCardDetalle5.Text = r.HayEnCurso ? faltan : lm.ObtenerTexto(f, "card.SinEnCurso", "El rango no incluye el período actual.");
            }

            // 6) Proximo periodo proyectado
            if (r.ProyeccionProximo.HasValue)
            {
                lblCardValor6.Text = r.ProyeccionProximo.Value.ToString("C0");
                string etiqueta = EtiquetaPeriodo(r.PeriodoProximo.Value, r.Agrupacion);
                lblCardDetalle6.Text = r.VariacionProximoVsUltimo.HasValue
                    ? string.Format(lm.ObtenerTexto(f, "card.Proximo.Detalle", "{0} | {1} vs el último completo"), etiqueta, Variacion(r.VariacionProximoVsUltimo.Value))
                    : etiqueta;
            }
            else
            {
                lblCardValor6.Text = "-";
                lblCardDetalle6.Text = faltan;
            }
        }

        private string TextoResumen()
        {
            var lm = Program.LanguageManager;
            if (!_generado) return lm.ObtenerTexto(f, "lbl.SinGenerar", "Aún no se generó el reporte.");

            string texto = string.Format(lm.ObtenerTexto(f, "lbl.Rango", "Rango consultado (ajustado a períodos enteros): {0} al {1}. Método de proyección: {2}."),
                _reporte.DesdeEfectivo.ToString("dd/MM/yyyy"), _reporte.HastaEfectivo.ToString("dd/MM/yyyy"), NombreMetodo(_reporte.Metodo));

            if (_reporte.DatosSuficientes)
                texto += " " + lm.ObtenerTexto(f, "lbl.NotaProyeccion", "La proyección usa solo períodos completos: el período en curso no entra en el cálculo.");
            else
                texto += " " + string.Format(lm.ObtenerTexto(f, "lbl.DatosInsuficientes", "No se proyecta: hacen falta al menos {0} períodos completos y hay {1}."),
                    BLL_ReporteInteligente486LP.MIN_PERIODOS, _reporte.PeriodosCompletos);

            return texto;
        }

        private void MostrarDatos()
        {
            ActualizarGrilla();
            ActualizarGraficoVentas();
            ActualizarGraficoTop();
            ActualizarGraficoMedios();
            ActualizarTarjetas();
            lblResumen.Text = TextoResumen();
        }

        private void btnGenerar_Click(object sender, EventArgs e)
        {
            var lm = Program.LanguageManager;

            DateTime desde = dtpDesde.Value.Date;
            DateTime hasta = dtpHasta.Value.Date;
            int agrupacion = AgrupacionElegida();
            int metodo = MetodoElegido();
            int periodos = (int)nudProyectar.Value;

            if (!_bll.ValidarRangoFechas(desde, hasta))
            {
                MessageBox.Show(lm.ObtenerTexto(f, "Msg.RangoInvalido", "La fecha 'Desde' no puede ser posterior a la fecha 'Hasta'."),
                    lm.ObtenerTexto(f, "Msg.RangoInvalido.Title", "Filtros inválidos"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!_bll.ValidarPeriodosAProyectar(periodos))
            {
                MessageBox.Show(string.Format(lm.ObtenerTexto(f, "Msg.PeriodosInvalido", "La cantidad de períodos a proyectar debe estar entre 1 y {0}."), BLL_ReporteInteligente486LP.MAX_PERIODOS_PROYECTAR),
                    lm.ObtenerTexto(f, "Msg.RangoInvalido.Title", "Filtros inválidos"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            ReporteInteligente486LP resultado = _bll.Generar(desde, hasta, agrupacion, metodo, periodos);

            if (resultado == null)
            {
                MessageBox.Show(lm.ObtenerTexto(f, "Msg.ErrorGenerar", "No se pudo generar el reporte. El error quedó registrado en la bitácora."),
                    lm.ObtenerTexto(f, "Msg.Error.Title", "Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            _reporte = resultado;
            _generado = true;
            MostrarDatos();

            if (_reporte.Resumen.CantidadVentas == 0)
            {
                MessageBox.Show(lm.ObtenerTexto(f, "Msg.SinResultados", "No hay ventas en el rango elegido."),
                    lm.ObtenerTexto(f, "Msg.SinResultados.Title", "Sin resultados"), MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else if (!_reporte.DatosSuficientes)
            {
                MessageBox.Show(string.Format(lm.ObtenerTexto(f, "Msg.DatosInsuficientes", "Datos insuficientes para proyectar: hacen falta al menos {0} períodos completos y el rango tiene {1}. Probá ampliar el rango de fechas o agrupar por semana."),
                    BLL_ReporteInteligente486LP.MIN_PERIODOS, _reporte.PeriodosCompletos),
                    lm.ObtenerTexto(f, "Msg.DatosInsuficientes.Title", "Datos insuficientes"), MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            Resetear();
        }

        public void Refrescar()
        {
            Resetear();
        }

        private void Resetear()
        {
            ValoresIniciales();
            _reporte = new ReporteInteligente486LP();
            _generado = false;
            MostrarDatos();
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // Todo esto es lo de PDF
        private iTextSharp.text.Image ImagenDe(Charting.Chart grafico, float ancho, float alto)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                grafico.SaveImage(ms, Charting.ChartImageFormat.Png);
                iTextSharp.text.Image img = iTextSharp.text.Image.GetInstance(ms.ToArray());
                img.ScaleToFit(ancho, alto);
                img.Alignment = Element.ALIGN_CENTER;
                return img;
            }
        }

        private void btnExportarPDF_Click(object sender, EventArgs e)
        {
            var lm = Program.LanguageManager;

            if (!_generado || _reporte.Resumen.CantidadVentas == 0)
            {
                MessageBox.Show(lm.ObtenerTexto(f, "Msg.SinDatosPDF", "Primero generá el reporte con al menos una venta para poder exportarlo."),
                    lm.ObtenerTexto(f, "Msg.PDF.Title", "Exportar PDF"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Title = lm.ObtenerTexto(f, "Msg.GuardarPDF", "Guardar reporte inteligente");
                sfd.Filter = "Archivo PDF (*.pdf)|*.pdf";
                sfd.FileName = $"ReporteInteligente_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";

                if (sfd.ShowDialog() != DialogResult.OK) return;

                try
                {
                    Document doc = new Document(PageSize.A4.Rotate(), 20f, 20f, 25f, 20f);
                    PdfWriter.GetInstance(doc, new FileStream(sfd.FileName, FileMode.Create));
                    doc.Open();

                    PdfFont fuenteTitulo = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 14f, BaseColor.WHITE);
                    PdfFont fuenteSubtitulo = FontFactory.GetFont(FontFactory.HELVETICA, 9f, new BaseColor(80, 80, 80));
                    PdfFont fuenteHeader = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 8f, BaseColor.WHITE);
                    PdfFont fuenteDato = FontFactory.GetFont(FontFactory.HELVETICA, 8f, BaseColor.BLACK);
                    PdfFont fuenteCardTitulo = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 7f, new BaseColor(90, 90, 90));
                    PdfFont fuenteCardValor = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 11f, new BaseColor(12, 68, 124));
                    PdfFont fuenteCardDetalle = FontFactory.GetFont(FontFactory.HELVETICA, 6.5f, new BaseColor(90, 90, 90));
                    PdfFont fuenteNota = FontFactory.GetFont(FontFactory.HELVETICA_OBLIQUE, 7.5f, new BaseColor(80, 80, 80));
                    PdfPTable encabezado = new PdfPTable(1) { WidthPercentage = 100 };
                    encabezado.AddCell(new PdfPCell(new Phrase("SneakRush — " + lm.ObtenerTexto(f, "pdf.Titulo", "Reporte inteligente: ventas y proyección"), fuenteTitulo))
                    {
                        BackgroundColor = new BaseColor(33, 90, 160),
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        Padding = 8f, Border = PdfRectangle.NO_BORDER
                    });

                    string usuario = SessionManager486LP.ObtenerInstancia().UsuarioActual().NombreUsuario;
                    string agrupacionTexto = _reporte.Agrupacion == BLL_ReporteInteligente486LP.AGRUPAR_SEMANA
                        ? lm.ObtenerTexto(f, "agrupar.Semana", "Semana") : lm.ObtenerTexto(f, "agrupar.Mes", "Mes");
                    string periodo = string.Format(lm.ObtenerTexto(f, "pdf.Periodo", "Período: {0} al {1} | Agrupado por: {2} | Método: {3} | Períodos a proyectar: {4}"),
                        _reporte.DesdeEfectivo.ToString("dd/MM/yyyy"), _reporte.HastaEfectivo.ToString("dd/MM/yyyy"),
                        agrupacionTexto, NombreMetodo(_reporte.Metodo), _reporte.PeriodosAProyectar);
                    string subtexto = string.Format(lm.ObtenerTexto(f, "pdf.Generado", "Generado: {0} | Usuario: {1}"),
                        DateTime.Now.ToString("dd/MM/yyyy HH:mm"), usuario);

                    encabezado.AddCell(new PdfPCell(new Phrase(periodo + "\n" + subtexto, fuenteSubtitulo))
                    {
                        BackgroundColor = new BaseColor(220, 230, 245),
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        Padding = 5f, Border = PdfRectangle.NO_BORDER
                    });
                    doc.Add(encabezado);
                    doc.Add(new Paragraph(" ", fuenteNota));
                    string[] titulos = { lblCardTitulo1.Text, lblCardTitulo2.Text, lblCardTitulo3.Text, lblCardTitulo4.Text, lblCardTitulo5.Text, lblCardTitulo6.Text };
                    string[] valores = { lblCardValor1.Text, lblCardValor2.Text, lblCardValor3.Text, lblCardValor4.Text, lblCardValor5.Text, lblCardValor6.Text };
                    string[] detalles = { lblCardDetalle1.Text, lblCardDetalle2.Text, lblCardDetalle3.Text, lblCardDetalle4.Text, lblCardDetalle5.Text, lblCardDetalle6.Text };
                    for (int k = 0; k < detalles.Length; k++)
                        detalles[k] = detalles[k].Replace("▲ ", "+").Replace("▼ ", "-");

                    PdfPTable tarjetas = new PdfPTable(6) { WidthPercentage = 100 };
                    for (int i = 0; i < 6; i++)
                    {
                        PdfPCell celda = new PdfPCell
                        {
                            BackgroundColor = i >= 4 ? new BaseColor(253, 240, 232) : new BaseColor(245, 248, 255),
                            BorderColor = new BaseColor(200, 210, 230),
                            Padding = 5f
                        };
                        celda.AddElement(new Paragraph(titulos[i], fuenteCardTitulo));
                        celda.AddElement(new Paragraph(valores[i], fuenteCardValor));
                        celda.AddElement(new Paragraph(detalles[i], fuenteCardDetalle));
                        tarjetas.AddCell(celda);
                    }
                    doc.Add(tarjetas);
                    doc.Add(new Paragraph(" ", fuenteNota));

                    doc.Add(ImagenDe(chartVentas, 600f, 190f));

                    PdfPTable graficos = new PdfPTable(2) { WidthPercentage = 100, SpacingBefore = 4f };
                    graficos.AddCell(new PdfPCell(ImagenDe(chartTop, 390f, 150f), true) { Border = PdfRectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_CENTER });
                    graficos.AddCell(new PdfPCell(ImagenDe(chartMedios, 390f, 150f), true) { Border = PdfRectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_CENTER });
                    doc.Add(graficos);

                    Paragraph nota = new Paragraph(lblResumen.Text, fuenteNota);
                    nota.Alignment = Element.ALIGN_CENTER;
                    nota.SpacingBefore = 4f;
                    doc.Add(nota);
                    doc.Add(new Paragraph(" ", fuenteNota));

                    float[] anchos = { 22f, 14f, 22f, 22f, 20f };
                    PdfPTable tabla = new PdfPTable(anchos.Length) { WidthPercentage = 100, SpacingBefore = 4f };
                    tabla.SetWidths(anchos);

                    BaseColor bgClaro = new BaseColor(245, 248, 255);

                    string[] headers =
                    {
                        lm.ObtenerTexto(f, "col.Periodo", "Período"),
                        lm.ObtenerTexto(f, "col.CantidadVentas", "Cantidad de ventas"),
                        lm.ObtenerTexto(f, "col.MontoReal", "Monto real"),
                        lm.ObtenerTexto(f, "col.MontoProyectado", "Monto proyectado"),
                        lm.ObtenerTexto(f, "col.Estado", "Estado")
                    };
                    foreach (string h in headers)
                    {
                        tabla.AddCell(new PdfPCell(new Phrase(h, fuenteHeader))
                        {
                            BackgroundColor = new BaseColor(33, 90, 160),
                            HorizontalAlignment = Element.ALIGN_CENTER,
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            Padding = 5f
                        });
                    }

                    bool filaAlterna = false;
                    foreach (FilaPeriodo486LP fila in _filas)
                    {
                        BaseColor bgFila = filaAlterna ? bgClaro : BaseColor.WHITE;
                        if (fila.Tipo == ReportePuntoSerie486LP.PROYECTADO) bgFila = new BaseColor(253, 235, 222);
                        else if (fila.Tipo == ReportePuntoSerie486LP.EN_CURSO) bgFila = new BaseColor(255, 248, 220);
                        filaAlterna = !filaAlterna;

                        AgregarCelda(tabla, fila.Periodo, fuenteDato, bgFila, Element.ALIGN_CENTER);
                        AgregarCelda(tabla, fila.CantidadVentas.HasValue ? fila.CantidadVentas.Value.ToString() : "-", fuenteDato, bgFila, Element.ALIGN_CENTER);
                        AgregarCelda(tabla, fila.MontoReal.HasValue ? fila.MontoReal.Value.ToString("C2") : "-", fuenteDato, bgFila, Element.ALIGN_RIGHT);
                        AgregarCelda(tabla, fila.MontoProyectado.HasValue ? fila.MontoProyectado.Value.ToString("C2") : "-", fuenteDato, bgFila, Element.ALIGN_RIGHT);
                        AgregarCelda(tabla, fila.Estado, fuenteDato, bgFila, Element.ALIGN_CENTER);
                    }

                    doc.Add(tabla);
                    doc.Add(new Paragraph(" ", fuenteNota));

                    Paragraph pie = new Paragraph("SneakRush 2026 — Universidad Abierta Interamericana — Ingeniería de Software", FontFactory.GetFont(FontFactory.HELVETICA, 7f, new BaseColor(150, 150, 150)));
                    pie.Alignment = Element.ALIGN_CENTER;
                    doc.Add(pie);
                    doc.Close();

                    _bll.RegistrarExportacion(_filas.Count);

                    MessageBox.Show(string.Format(lm.ObtenerTexto(f, "Msg.PDFExportado", "PDF exportado correctamente:\n{0}"), sfd.FileName),
                        lm.ObtenerTexto(f, "Msg.PDF.Title", "Exportar PDF"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                    System.Diagnostics.Process.Start(sfd.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(string.Format(lm.ObtenerTexto(f, "Msg.ErrorPDF", "Error al exportar PDF:\n{0}"), ex.Message),
                        lm.ObtenerTexto(f, "Msg.Error.Title", "Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void AgregarCelda(PdfPTable tabla, string texto, PdfFont fuente, BaseColor fondo, int alineacion)
        {
            tabla.AddCell(new PdfPCell(new Phrase(texto, fuente))
            {
                BackgroundColor = fondo,
                HorizontalAlignment = alineacion,
                VerticalAlignment = Element.ALIGN_MIDDLE,
                Padding = 4f,
                BorderColor = new BaseColor(200, 210, 230),
                BorderWidth = 0.5f
            });
        }

        public void ActualizarIdioma()
        {
            var lm = Program.LanguageManager;

            this.Text = lm.ObtenerTexto(f, "Title", "Reporte inteligente");
            lblTitulo.Text = lm.ObtenerTexto(f, "Title", "Reporte inteligente");

            grpFiltros.Text = lm.ObtenerTexto(f, "grpFiltros", "Filtros");
            lblDesde.Text = lm.ObtenerTexto(f, "lblDesde", "Desde");
            lblHasta.Text = lm.ObtenerTexto(f, "lblHasta", "Hasta");
            lblAgrupar.Text = lm.ObtenerTexto(f, "lblAgrupar", "Agrupar por");
            lblProyectar.Text = lm.ObtenerTexto(f, "lblProyectar", "Períodos a proyectar");
            lblMetodo.Text = lm.ObtenerTexto(f, "lblMetodo", "Método de proyección");

            btnGenerar.Text = lm.ObtenerTexto(f, "btnGenerar", "Generar");
            btnLimpiar.Text = lm.ObtenerTexto(f, "btnLimpiar", "Limpiar");
            btnExportarPDF.Text = lm.ObtenerTexto(f, "btnExportarPDF", "Exportar PDF");
            btnCerrar.Text = lm.ObtenerTexto(f, "btnCerrar", "Cerrar");

            if (!_cargado) return;

            RellenarCombos();
            DistribuirFiltros();
            TraducirColumnas();
            MostrarDatos();
        }

        private void TraducirColumnas()
        {
            var lm = Program.LanguageManager;

            if (dgvPeriodos.Columns.Contains("colPeriodo"))
                dgvPeriodos.Columns["colPeriodo"].HeaderText = lm.ObtenerTexto(f, "col.Periodo", "Período");
            if (dgvPeriodos.Columns.Contains("colCantidadVentas"))
                dgvPeriodos.Columns["colCantidadVentas"].HeaderText = lm.ObtenerTexto(f, "col.CantidadVentas", "Cantidad de ventas");
            if (dgvPeriodos.Columns.Contains("colMontoReal"))
                dgvPeriodos.Columns["colMontoReal"].HeaderText = lm.ObtenerTexto(f, "col.MontoReal", "Monto real");
            if (dgvPeriodos.Columns.Contains("colMontoProyectado"))
                dgvPeriodos.Columns["colMontoProyectado"].HeaderText = lm.ObtenerTexto(f, "col.MontoProyectado", "Monto proyectado");
            if (dgvPeriodos.Columns.Contains("colEstado"))
                dgvPeriodos.Columns["colEstado"].HeaderText = lm.ObtenerTexto(f, "col.Estado", "Estado");
        }

        private void FrmReporteInteligente486LP_FormClosing(object sender, FormClosingEventArgs e)
        {
            Program.LanguageManager.Quitar(this);
        }
    }

    public class FilaPeriodo486LP
    {
        public string Periodo { get; set; }
        public int? CantidadVentas { get; set; }
        public decimal? MontoReal { get; set; }
        public decimal? MontoProyectado { get; set; }
        public string Estado { get; set; }
        public int Tipo { get; set; }
    }
}

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
        private bool _cargado = false;
        private bool _generado = false;
        private ReporteInteligente486LP _reporte = new ReporteInteligente486LP();
        private List<FilaPeriodo486LP> _filas = new List<FilaPeriodo486LP>();

        private const int VISTA_ROTACION = 0;
        private const int VISTA_VENTAS = 1;
        private int _vista = VISTA_ROTACION;
        private bool _ajustando = false;   // true mientras el codigo cambia fechas / combo de periodo rapido (para no disparar los eventos)
        private ReporteRotacion486LP _rotacion = new ReporteRotacion486LP();
        private List<FilaRotacion486LP> _filasRot = new List<FilaRotacion486LP>();
        private static readonly int[] MesesRango = { 0, 6, 12, 24, 36, 48 };   // 0 = personalizado

        private static readonly Color Azul = Color.FromArgb(12, 68, 124);
        private static readonly Color Naranja = Color.FromArgb(240, 90, 40);
        private static readonly Color AzulClaro = Color.FromArgb(130, 165, 210);
        private static readonly Color Verde = Color.FromArgb(46, 139, 87);

        public FrmReporteInteligente486LP()
        {
            InitializeComponent();
            Ayuda486LP.Vincular(this, "reporte-inteligente");
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
            ConfigurarGrillaRotacion();
            ConfigurarGraficos();
            RellenarCombos();
            ValoresIniciales();
            AplicarVista();
            dtpDesde.ValueChanged += Fecha_ValueChanged;
            dtpHasta.ValueChanged += Fecha_ValueChanged;
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

        private void ValoresIniciales()
        {
            _ajustando = true;
            dtpHasta.Value = DateTime.Today;
            dtpDesde.Value = DateTime.Today.AddMonths(-24);
            cmbRango.SelectedIndex = 3;  // ultimos 2 anios
            _ajustando = false;

            cmbAgrupar.SelectedIndex = 0;// por mes
            nudProyectar.Value = 3;
            cmbMetodo.SelectedIndex = 0; // tendencia lineal
            nudCubrir.Value = BLL_ReporteInteligente486LP.MESES_CUBRIR_DEFECTO;
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

            // El periodo rapido guarda la opcion por posicion (ver MesesRango); mientras se rellena no se tocan las fechas
            _ajustando = true;
            int rango = cmbRango.SelectedIndex < 0 ? 3 : cmbRango.SelectedIndex;
            cmbRango.Items.Clear();
            cmbRango.Items.Add(lm.ObtenerTexto(f, "rango.Personalizado", "Personalizado"));
            cmbRango.Items.Add(lm.ObtenerTexto(f, "rango.6m", "Últimos 6 meses"));
            cmbRango.Items.Add(lm.ObtenerTexto(f, "rango.1a", "Último año"));
            cmbRango.Items.Add(lm.ObtenerTexto(f, "rango.2a", "Últimos 2 años"));
            cmbRango.Items.Add(lm.ObtenerTexto(f, "rango.3a", "Últimos 3 años"));
            cmbRango.Items.Add(lm.ObtenerTexto(f, "rango.4a", "Últimos 4 años"));
            cmbRango.SelectedIndex = rango;
            _ajustando = false;
        }

        private void cmbRango_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_cargado || _ajustando) return;

            int indice = cmbRango.SelectedIndex;
            if (indice <= 0 || indice >= MesesRango.Length) return;   // "personalizado" no toca las fechas

            _ajustando = true;
            dtpHasta.Value = DateTime.Today;
            dtpDesde.Value = DateTime.Today.AddMonths(-MesesRango[indice]);
            _ajustando = false;
        }

        // Si la persona cambia una fecha a mano, el periodo rapido pasa a "personalizado"
        private void Fecha_ValueChanged(object sender, EventArgs e)
        {
            if (!_cargado || _ajustando) return;

            _ajustando = true;
            cmbRango.SelectedIndex = 0;
            _ajustando = false;
        }

        private void btnVistaRotacion_Click(object sender, EventArgs e)
        {
            _vista = VISTA_ROTACION;
            AplicarVista();
        }

        private void btnVistaVentas_Click(object sender, EventArgs e)
        {
            _vista = VISTA_VENTAS;
            AplicarVista();
        }

        // Muestra solo lo de la vista elegida: rotacion y compras (por defecto) o ventas y proyeccion
        private void AplicarVista()
        {
            bool rotacion = _vista == VISTA_ROTACION;

            chartMas.Visible = rotacion;
            chartMenos.Visible = rotacion;
            dgvRotacion.Visible = rotacion;
            lblCubrir.Visible = rotacion;
            nudCubrir.Visible = rotacion;

            chartVentas.Visible = !rotacion;
            dgvPeriodos.Visible = !rotacion;
            lblAgrupar.Visible = !rotacion;
            cmbAgrupar.Visible = !rotacion;
            lblProyectar.Visible = !rotacion;
            nudProyectar.Visible = !rotacion;
            lblMetodo.Visible = !rotacion;
            cmbMetodo.Visible = !rotacion;

            btnVistaRotacion.BackColor = rotacion ? Naranja : Azul;
            btnVistaVentas.BackColor = rotacion ? Azul : Naranja;

            if (_cargado)
            {
                DistribuirFiltros();
                MostrarDatos();
            }
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
            // Fila 1: fechas y periodo rapido (para las dos vistas)
            int x = 18;
            Ubicar(ref x, lblDesde, dtpDesde);
            Ubicar(ref x, lblHasta, dtpHasta);
            Ubicar(ref x, lblRango, cmbRango);

            // Fila 2: depende de la vista
            lblAgrupar.Top = lblProyectar.Top;
            cmbAgrupar.Top = cmbMetodo.Top;

            x = 18;
            if (_vista == VISTA_ROTACION)
            {
                Ubicar(ref x, lblCubrir, nudCubrir);
            }
            else
            {
                Ubicar(ref x, lblAgrupar, cmbAgrupar);
                Ubicar(ref x, lblProyectar, nudProyectar);
                Ubicar(ref x, lblMetodo, cmbMetodo);
            }
        }

        private void Ubicar(ref int x, Label etiqueta, Control control)
        {
            etiqueta.Left = x;
            control.Left = etiqueta.Right + 8;
            x = control.Right + 26;
        }

        private void EstilizarGrilla(DataGridView g)
        {
            g.AutoGenerateColumns = false;
            g.AllowUserToAddRows = false;
            g.AllowUserToDeleteRows = false;
            g.ReadOnly = true;
            g.MultiSelect = false;
            g.RowHeadersVisible = false;
            g.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            g.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            g.BackgroundColor = Color.White;
            g.EnableHeadersVisualStyles = false;
            g.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;

            DataGridViewCellStyle estiloHeader = new DataGridViewCellStyle();
            estiloHeader.BackColor = Azul;
            estiloHeader.ForeColor = Color.White;
            estiloHeader.Font = new System.Drawing.Font("Segoe UI", 9.75F, FontStyle.Bold);
            estiloHeader.SelectionBackColor = Azul;
            estiloHeader.SelectionForeColor = Color.White;
            estiloHeader.WrapMode = DataGridViewTriState.True;
            g.ColumnHeadersDefaultCellStyle = estiloHeader;

            DataGridViewCellStyle estiloFilas = new DataGridViewCellStyle();
            estiloFilas.BackColor = Color.White;
            estiloFilas.SelectionBackColor = Naranja;
            estiloFilas.SelectionForeColor = Color.White;
            g.RowsDefaultCellStyle = estiloFilas;
        }

        private void ConfigurarGrilla()
        {
            EstilizarGrilla(dgvPeriodos);

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

        private void ConfigurarGrillaRotacion()
        {
            EstilizarGrilla(dgvRotacion);

            DataGridViewCellStyle centro = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter };

            dgvRotacion.Columns.Clear();
            dgvRotacion.Columns.Add(new DataGridViewTextBoxColumn { Name = "colRotModelo", DataPropertyName = "Modelo", FillWeight = 30 });
            dgvRotacion.Columns.Add(new DataGridViewTextBoxColumn { Name = "colRotUnidades", DataPropertyName = "Unidades", FillWeight = 11, DefaultCellStyle = centro });
            dgvRotacion.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colRotPorMes", DataPropertyName = "PorMes", FillWeight = 11,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "N1", Alignment = DataGridViewContentAlignment.MiddleCenter }
            });
            dgvRotacion.Columns.Add(new DataGridViewTextBoxColumn { Name = "colRotStock", DataPropertyName = "Stock", FillWeight = 11, DefaultCellStyle = centro });
            dgvRotacion.Columns.Add(new DataGridViewTextBoxColumn { Name = "colRotCobertura", DataPropertyName = "Cobertura", FillWeight = 14, DefaultCellStyle = centro });
            dgvRotacion.Columns.Add(new DataGridViewTextBoxColumn { Name = "colRotRecomendacion", DataPropertyName = "Recomendacion", FillWeight = 13, DefaultCellStyle = centro });
            dgvRotacion.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colRotCantidad", DataPropertyName = "CantidadSugerida", FillWeight = 12,
                DefaultCellStyle = new DataGridViewCellStyle { NullValue = "-", Alignment = DataGridViewContentAlignment.MiddleCenter }
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

            ConfigurarGraficoRanking(chartMas, "areaMas", "serieMas", Azul);
            ConfigurarGraficoRanking(chartMenos, "areaMenos", "serieMenos", Naranja);
        }

        private void ConfigurarGraficoRanking(Charting.Chart grafico, string nombreArea, string nombreSerie, Color color)
        {
            grafico.Series.Clear();
            grafico.ChartAreas.Clear();
            grafico.Legends.Clear();
            grafico.Titles.Clear();

            Charting.ChartArea area = new Charting.ChartArea(nombreArea);
            area.BackColor = Color.White;
            area.AxisX.MajorGrid.Enabled = false;
            area.AxisX.IsReversed = true;  // el primero de la lista queda arriba
            area.AxisX.LabelStyle.Interval = 1;
            area.AxisX.LabelStyle.Font = new System.Drawing.Font("Segoe UI", 8F);
            area.AxisY.LabelStyle.Format = "N0";
            area.AxisY.LabelStyle.Font = new System.Drawing.Font("Segoe UI", 8F);
            area.AxisY.MajorGrid.LineColor = Color.Gainsboro;
            grafico.ChartAreas.Add(area);

            Charting.Series serie = new Charting.Series(nombreSerie);
            serie.ChartType = Charting.SeriesChartType.Bar;
            serie.ChartArea = nombreArea;
            serie.Color = color;
            serie.IsValueShownAsLabel = true;
            serie.LabelFormat = "N0";
            serie.Font = new System.Drawing.Font("Segoe UI", 8F, FontStyle.Bold);
            grafico.Series.Add(serie);

            grafico.Titles.Add(new Charting.Title("", Charting.Docking.Top, new System.Drawing.Font("Segoe UI", 10F, FontStyle.Bold), Azul));
        }

        // Grafico de torta de ventas por medio de pago. Se arma al momento (para la ventana y para el PDF).
        private Charting.Chart CrearGraficoMedios(int ancho, int alto)
        {
            var lm = Program.LanguageManager;

            Charting.Chart grafico = new Charting.Chart();
            grafico.Size = new Size(ancho, alto);
            grafico.BackColor = Color.White;

            Charting.ChartArea area = new Charting.ChartArea("areaMedios");
            area.BackColor = Color.White;
            grafico.ChartAreas.Add(area);

            Charting.Legend leyenda = new Charting.Legend("leyendaMedios");
            leyenda.Docking = Charting.Docking.Right;
            leyenda.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            grafico.Legends.Add(leyenda);

            Charting.Series serie = new Charting.Series("serieMedios");
            serie.ChartType = Charting.SeriesChartType.Pie;
            serie.ChartArea = "areaMedios";
            serie.Legend = "leyendaMedios";
            serie.IsValueShownAsLabel = true;
            serie.Label = "#PERCENT{P0}";
            serie.Font = new System.Drawing.Font("Segoe UI", 8.25F, FontStyle.Bold);
            serie["PieLabelStyle"] = "Inside";
            grafico.Series.Add(serie);

            grafico.Titles.Add(new Charting.Title(lm.ObtenerTexto(f, "chart.Medios.Titulo", "Ventas por medio de pago"),
                Charting.Docking.Top, new System.Drawing.Font("Segoe UI", 10F, FontStyle.Bold), Azul));

            Color[] paleta = { Azul, Naranja, Verde, Color.FromArgb(150, 110, 190), Color.FromArgb(120, 120, 120) };
            int i = 0;
            foreach (ReporteMedioPago486LP m in _reporte.MediosPago)
            {
                int idx = serie.Points.AddXY(m.MedioPago, (double)m.MontoTotal);
                serie.Points[idx].Color = paleta[i % paleta.Length];
                serie.Points[idx].LegendText = string.Format("{0} ({1})", m.MedioPago, m.MontoTotal.ToString("C0"));
                serie.Points[idx].AxisLabel = m.MedioPago;
                i++;
            }
            return grafico;
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

        private string NombreRecomendacion(int tipo)
        {
            var lm = Program.LanguageManager;
            if (tipo == ReporteRotacionProducto486LP.COMPRAR) return lm.ObtenerTexto(f, "rec.Comprar", "Comprar");
            if (tipo == ReporteRotacionProducto486LP.NO_COMPRAR) return lm.ObtenerTexto(f, "rec.NoComprar", "No comprar");
            return lm.ObtenerTexto(f, "rec.Mantener", "Mantener");
        }

        private List<FilaRotacion486LP> ArmarFilasRotacion()
        {
            var lm = Program.LanguageManager;
            List<FilaRotacion486LP> filas = new List<FilaRotacion486LP>();
            if (!_generado) return filas;

            foreach (ReporteRotacionProducto486LP p in _rotacion.Productos)
            {
                FilaRotacion486LP fila = new FilaRotacion486LP();
                fila.Modelo = p.NombreCompleto;
                fila.Unidades = p.Unidades;
                fila.PorMes = p.UnidadesPorMes;
                fila.Stock = p.Stock;
                fila.Cobertura = p.MesesDeStock.HasValue
                    ? string.Format(lm.ObtenerTexto(f, "rot.Cobertura", "{0} meses"), p.MesesDeStock.Value.ToString("0.0", CultureInfo.CurrentCulture))
                    : lm.ObtenerTexto(f, "rot.SinVentas", "Sin ventas");
                fila.Tipo = p.Recomendacion;
                fila.Recomendacion = NombreRecomendacion(p.Recomendacion);
                fila.CantidadSugerida = p.Recomendacion == ReporteRotacionProducto486LP.COMPRAR ? (int?)p.CantidadSugerida : null;
                filas.Add(fila);
            }
            return filas;
        }

        private void ActualizarGrillaRotacion()
        {
            _filasRot = ArmarFilasRotacion();
            dgvRotacion.DataSource = null;
            dgvRotacion.DataSource = _filasRot;

            for (int i = 0; i < dgvRotacion.Rows.Count && i < _filasRot.Count; i++)
            {
                if (_filasRot[i].Tipo == ReporteRotacionProducto486LP.COMPRAR)
                    dgvRotacion.Rows[i].DefaultCellStyle.BackColor = Color.FromArgb(226, 244, 230);
                else if (_filasRot[i].Tipo == ReporteRotacionProducto486LP.NO_COMPRAR)
                    dgvRotacion.Rows[i].DefaultCellStyle.BackColor = Color.FromArgb(250, 228, 228);
            }
            dgvRotacion.ClearSelection();
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

        private void ActualizarGraficosRanking()
        {
            var lm = Program.LanguageManager;
            if (chartMas.Series.Count == 0 || chartMenos.Series.Count == 0) return;

            int n = BLL_ReporteInteligente486LP.CANTIDAD_RANKING;
            if (chartMas.Titles.Count > 0)
                chartMas.Titles[0].Text = string.Format(lm.ObtenerTexto(f, "chart.Mas.Titulo", "Los {0} modelos más vendidos (unidades)"), n);
            if (chartMenos.Titles.Count > 0)
                chartMenos.Titles[0].Text = string.Format(lm.ObtenerTexto(f, "chart.Menos.Titulo", "Los {0} modelos menos vendidos (unidades)"), n);

            Charting.Series serieMas = chartMas.Series["serieMas"];
            Charting.Series serieMenos = chartMenos.Series["serieMenos"];
            serieMas.Points.Clear();
            serieMenos.Points.Clear();
            if (!_generado) return;

            foreach (ReporteRotacionProducto486LP p in _rotacion.MasVendidos)
                serieMas.Points.AddXY(p.NombreCompleto, p.Unidades);

            foreach (ReporteRotacionProducto486LP p in _rotacion.MenosVendidos)
                serieMenos.Points.AddXY(p.NombreCompleto, p.Unidades);
        }

        private string Variacion(decimal variacion)
        {
            string flecha = variacion >= 0 ? "▲" : "▼";
            return flecha + " " + Math.Abs(variacion).ToString("0.0", CultureInfo.CurrentCulture) + "%";
        }

        private void TarjetasVentas(string[] t, string[] v, string[] d)
        {
            var lm = Program.LanguageManager;

            t[0] = lm.ObtenerTexto(f, "card.Total", "Total vendido");
            t[1] = lm.ObtenerTexto(f, "card.Ticket", "Ticket promedio");
            t[2] = lm.ObtenerTexto(f, "card.UltimoCompleto", "Último período completo");
            t[3] = lm.ObtenerTexto(f, "card.EnCurso", "Período en curso");
            t[4] = lm.ObtenerTexto(f, "card.Cierre", "Proyección de cierre");
            t[5] = lm.ObtenerTexto(f, "card.Proximo", "Próximo período proyectado");

            if (!_generado)
            {
                for (int i = 0; i < 6; i++)
                {
                    v[i] = "-";
                    d[i] = "";
                }
                return;
            }

            ReporteInteligente486LP r = _reporte;
            string faltan = string.Format(lm.ObtenerTexto(f, "card.DatosInsuficientes", "Datos insuficientes: se necesitan {0} períodos completos y hay {1}."),
                BLL_ReporteInteligente486LP.MIN_PERIODOS, r.PeriodosCompletos);

            // 1) Total vendido en el rango
            v[0] = r.Resumen.MontoTotal.ToString("C0");
            d[0] = string.Format(lm.ObtenerTexto(f, "card.Total.Detalle", "{0} ventas | {1} unidades"), r.Resumen.CantidadVentas, r.Resumen.Unidades);

            // 2) Ticket promedio
            v[1] = r.Resumen.TicketPromedio.ToString("C0");
            d[1] = string.Format(lm.ObtenerTexto(f, "card.Ticket.Detalle", "Del {0} al {1}"),
                r.DesdeEfectivo.ToString("dd/MM/yyyy"), r.HastaEfectivo.ToString("dd/MM/yyyy"));

            // 3) Ultimo periodo completo, comparado con el anterior
            if (r.PeriodoUltimoCompleto.HasValue)
            {
                string etiqueta = EtiquetaPeriodo(r.PeriodoUltimoCompleto.Value, r.Agrupacion);
                v[2] = r.MontoUltimoCompleto.ToString("C0");
                d[2] = r.VariacionUltimoVsAnterior.HasValue
                    ? string.Format(lm.ObtenerTexto(f, "card.UltimoCompleto.Detalle", "{0} | {1} vs el anterior"), etiqueta, Variacion(r.VariacionUltimoVsAnterior.Value))
                    : string.Format(lm.ObtenerTexto(f, "card.UltimoCompleto.SinComparar", "{0} | sin período anterior para comparar"), etiqueta);
            }
            else
            {
                v[2] = "-";
                d[2] = lm.ObtenerTexto(f, "card.SinCompletos", "Todavía no terminó ningún período del rango.");
            }

            // 4) Periodo en curso: lo que va y como cerraria al ritmo de hoy
            if (r.HayEnCurso)
            {
                v[3] = r.MontoEnCurso.ToString("C0");
                d[3] = string.Format(lm.ObtenerTexto(f, "card.EnCurso.Detalle", "{0} | día {1} de {2}. Al ritmo actual: {3}"),
                    EtiquetaPeriodo(r.PeriodoEnCurso.Value, r.Agrupacion), r.DiasTranscurridos, r.DiasDelPeriodo, r.CierreAlRitmoActual.ToString("C0"));
            }
            else
            {
                v[3] = "-";
                d[3] = lm.ObtenerTexto(f, "card.SinEnCurso", "El rango no incluye el período actual.");
            }

            // 5) Proyeccion de cierre del periodo en curso
            if (r.HayEnCurso && r.ProyeccionCierre.HasValue)
            {
                v[4] = r.ProyeccionCierre.Value.ToString("C0");
                d[4] = string.Format("{0} | {1}", EtiquetaPeriodo(r.PeriodoEnCurso.Value, r.Agrupacion), NombreMetodo(r.Metodo));
            }
            else
            {
                v[4] = "-";
                d[4] = r.HayEnCurso ? faltan : lm.ObtenerTexto(f, "card.SinEnCurso", "El rango no incluye el período actual.");
            }

            // 6) Proximo periodo proyectado
            if (r.ProyeccionProximo.HasValue)
            {
                v[5] = r.ProyeccionProximo.Value.ToString("C0");
                string etiqueta = EtiquetaPeriodo(r.PeriodoProximo.Value, r.Agrupacion);
                d[5] = r.VariacionProximoVsUltimo.HasValue
                    ? string.Format(lm.ObtenerTexto(f, "card.Proximo.Detalle", "{0} | {1} vs el último completo"), etiqueta, Variacion(r.VariacionProximoVsUltimo.Value))
                    : etiqueta;
            }
            else
            {
                v[5] = "-";
                d[5] = faltan;
            }
        }

        private void TarjetasRotacion(string[] t, string[] v, string[] d)
        {
            var lm = Program.LanguageManager;

            t[0] = lm.ObtenerTexto(f, "rcard.Unidades", "Unidades vendidas");
            t[1] = lm.ObtenerTexto(f, "rcard.Modelos", "Modelos analizados");
            t[2] = lm.ObtenerTexto(f, "rcard.Comprar", "Modelos a comprar");
            t[3] = lm.ObtenerTexto(f, "rcard.NoComprar", "No hace falta comprar");
            t[4] = lm.ObtenerTexto(f, "rcard.MasVendido", "Modelo más vendido");
            t[5] = lm.ObtenerTexto(f, "rcard.MenosVendido", "Modelo menos vendido");

            if (!_generado)
            {
                for (int i = 0; i < 6; i++)
                {
                    v[i] = "-";
                    d[i] = "";
                }
                return;
            }

            ReporteRotacion486LP r = _rotacion;
            string unidad = lm.ObtenerTexto(f, "rcard.Un", "un.");

            v[0] = r.TotalUnidades.ToString("N0");
            d[0] = string.Format(lm.ObtenerTexto(f, "rcard.Unidades.Detalle", "Facturación: {0}"), r.TotalMonto.ToString("C0"));

            v[1] = r.ModelosAnalizados.ToString("N0");
            d[1] = string.Format(lm.ObtenerTexto(f, "rcard.Modelos.Detalle", "{0} con ventas | {1} sin ventas"), r.ModelosConVentas, r.ModelosSinVentas);

            v[2] = r.ModelosAComprar.ToString("N0");
            d[2] = string.Format(lm.ObtenerTexto(f, "rcard.Comprar.Detalle", "{0} unidades sugeridas para {1} meses"), r.UnidadesAComprar.ToString("N0"), r.MesesACubrir);

            v[3] = r.ModelosNoComprar.ToString("N0");
            d[3] = string.Format(lm.ObtenerTexto(f, "rcard.NoComprar.Detalle", "Sin ventas o con stock para más de {0} meses"), r.MesesACubrir * BLL_ReporteInteligente486LP.FACTOR_SOBRESTOCK);

            if (r.MasVendido != null)
            {
                v[4] = r.MasVendido.Unidades.ToString("N0") + " " + unidad;
                d[4] = r.MasVendido.NombreCompleto;
            }
            else
            {
                v[4] = "-";
                d[4] = lm.ObtenerTexto(f, "rcard.SinVentas", "No hubo ventas en el período.");
            }

            if (r.MenosVendido != null)
            {
                v[5] = r.MenosVendido.Unidades.ToString("N0") + " " + unidad;
                d[5] = r.MenosVendido.NombreCompleto;
            }
            else
            {
                v[5] = "-";
                d[5] = lm.ObtenerTexto(f, "rcard.SinVentas", "No hubo ventas en el período.");
            }
        }

        private void ActualizarTarjetas()
        {
            string[] t = new string[6];
            string[] v = new string[6];
            string[] d = new string[6];

            if (_vista == VISTA_ROTACION) TarjetasRotacion(t, v, d);
            else TarjetasVentas(t, v, d);

            Label[] titulos = { lblCardTitulo1, lblCardTitulo2, lblCardTitulo3, lblCardTitulo4, lblCardTitulo5, lblCardTitulo6 };
            Label[] valores = { lblCardValor1, lblCardValor2, lblCardValor3, lblCardValor4, lblCardValor5, lblCardValor6 };
            Label[] detalles = { lblCardDetalle1, lblCardDetalle2, lblCardDetalle3, lblCardDetalle4, lblCardDetalle5, lblCardDetalle6 };
            for (int i = 0; i < 6; i++)
            {
                titulos[i].Text = t[i];
                valores[i].Text = v[i];
                detalles[i].Text = d[i];
            }
        }

        private string TextoResumenVentas()
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

        private string TextoResumenRotacion()
        {
            var lm = Program.LanguageManager;
            if (!_generado) return lm.ObtenerTexto(f, "lbl.SinGenerar", "Aún no se generó el reporte.");

            return string.Format(lm.ObtenerTexto(f, "lbl.RangoRotacion",
                "Período analizado: {0} al {1} ({2} meses). Se sugiere comprar cuando el stock actual no alcanza para {3} meses de ventas al ritmo del período; no comprar si no hubo ventas o si el stock alcanza para más de {4} meses."),
                _rotacion.Desde.ToString("dd/MM/yyyy"), _rotacion.Hasta.ToString("dd/MM/yyyy"),
                _rotacion.MesesPeriodo.ToString("0.#", CultureInfo.CurrentCulture),
                _rotacion.MesesACubrir, _rotacion.MesesACubrir * BLL_ReporteInteligente486LP.FACTOR_SOBRESTOCK);
        }

        private string TextoResumen()
        {
            return _vista == VISTA_ROTACION ? TextoResumenRotacion() : TextoResumenVentas();
        }

        private void MostrarDatos()
        {
            ActualizarGrilla();
            ActualizarGrillaRotacion();
            ActualizarGraficoVentas();
            ActualizarGraficosRanking();
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
            int mesesACubrir = (int)nudCubrir.Value;

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

            if (!_bll.ValidarMesesACubrir(mesesACubrir))
            {
                MessageBox.Show(string.Format(lm.ObtenerTexto(f, "Msg.MesesInvalido", "Los meses de ventas a cubrir deben estar entre {0} y {1}."), BLL_ReporteInteligente486LP.MIN_MESES_CUBRIR, BLL_ReporteInteligente486LP.MAX_MESES_CUBRIR),
                    lm.ObtenerTexto(f, "Msg.RangoInvalido.Title", "Filtros inválidos"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            ReporteInteligente486LP resultado = _bll.Generar(desde, hasta, agrupacion, metodo, periodos);
            ReporteRotacion486LP rotacion = _bll.GenerarRotacion(desde, hasta, mesesACubrir);

            if (resultado == null || rotacion == null)
            {
                MessageBox.Show(lm.ObtenerTexto(f, "Msg.ErrorGenerar", "No se pudo generar el reporte. El error quedó registrado en la bitácora."),
                    lm.ObtenerTexto(f, "Msg.Error.Title", "Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            _reporte = resultado;
            _rotacion = rotacion;
            _generado = true;
            MostrarDatos();

            if (_vista == VISTA_ROTACION)
            {
                if (_rotacion.ModelosConVentas == 0)
                {
                    MessageBox.Show(lm.ObtenerTexto(f, "Msg.SinResultados", "No hay ventas en el rango elegido."),
                        lm.ObtenerTexto(f, "Msg.SinResultados.Title", "Sin resultados"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else if (_reporte.Resumen.CantidadVentas == 0)
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

        private void btnMedios_Click(object sender, EventArgs e)
        {
            var lm = Program.LanguageManager;

            if (!_generado || _reporte.MediosPago.Count == 0)
            {
                MessageBox.Show(lm.ObtenerTexto(f, "Msg.SinMedios", "Primero generá el reporte con al menos una venta para ver los medios de pago."),
                    lm.ObtenerTexto(f, "chart.Medios.Titulo", "Ventas por medio de pago"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using (Form dialogo = new Form())
            using (Charting.Chart grafico = CrearGraficoMedios(640, 400))
            using (Button cerrar = new Button())
            {
                dialogo.Text = lm.ObtenerTexto(f, "chart.Medios.Titulo", "Ventas por medio de pago");
                dialogo.StartPosition = FormStartPosition.CenterParent;
                dialogo.FormBorderStyle = FormBorderStyle.FixedDialog;
                dialogo.MaximizeBox = false;
                dialogo.MinimizeBox = false;
                dialogo.ShowInTaskbar = false;
                dialogo.BackColor = Color.White;
                dialogo.ClientSize = new Size(640, 440);

                grafico.Dock = DockStyle.Fill;

                cerrar.Text = lm.ObtenerTexto(f, "btnCerrar", "Cerrar");
                cerrar.Dock = DockStyle.Bottom;
                cerrar.Height = 40;
                cerrar.FlatStyle = FlatStyle.Flat;
                cerrar.FlatAppearance.BorderSize = 0;
                cerrar.BackColor = Azul;
                cerrar.ForeColor = Color.White;
                cerrar.Font = new System.Drawing.Font("Segoe UI", 9.75F, FontStyle.Bold);
                cerrar.DialogResult = DialogResult.OK;

                dialogo.Controls.Add(grafico);
                dialogo.Controls.Add(cerrar);
                dialogo.AcceptButton = cerrar;
                dialogo.ShowDialog(this);
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
            _rotacion = new ReporteRotacion486LP();
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
            // Un grafico de la vista que no se esta mostrando esta oculto: se muestra un instante (sin repintar) para que se dibuje completo
            bool estabaVisible = grafico.Visible;
            if (!estabaVisible) grafico.Visible = true;
            try
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
            finally
            {
                if (!estabaVisible) grafico.Visible = false;
            }
        }

        private void btnExportarPDF_Click(object sender, EventArgs e)
        {
            var lm = Program.LanguageManager;

            if (!_generado || (_reporte.Resumen.CantidadVentas == 0 && _rotacion.TotalUnidades == 0))
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
                    PdfFont fuenteSeccion = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10f, BaseColor.WHITE);
                    PdfFont fuenteSubtitulo = FontFactory.GetFont(FontFactory.HELVETICA, 9f, new BaseColor(80, 80, 80));
                    PdfFont fuenteHeader = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 8f, BaseColor.WHITE);
                    PdfFont fuenteDato = FontFactory.GetFont(FontFactory.HELVETICA, 8f, BaseColor.BLACK);
                    PdfFont fuenteCardTitulo = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 7f, new BaseColor(90, 90, 90));
                    PdfFont fuenteCardValor = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 11f, new BaseColor(12, 68, 124));
                    PdfFont fuenteCardDetalle = FontFactory.GetFont(FontFactory.HELVETICA, 6.5f, new BaseColor(90, 90, 90));
                    PdfFont fuenteNota = FontFactory.GetFont(FontFactory.HELVETICA_OBLIQUE, 7.5f, new BaseColor(80, 80, 80));
                    BaseColor bgClaro = new BaseColor(245, 248, 255);

                    // Encabezado
                    PdfPTable encabezado = new PdfPTable(1) { WidthPercentage = 100 };
                    encabezado.AddCell(new PdfPCell(new Phrase("SneakRush — " + lm.ObtenerTexto(f, "pdf.Titulo", "Reporte inteligente: rotación de productos y ventas"), fuenteTitulo))
                    {
                        BackgroundColor = new BaseColor(33, 90, 160),
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        Padding = 8f, Border = PdfRectangle.NO_BORDER
                    });

                    string usuario = SessionManager486LP.ObtenerInstancia().UsuarioActual().NombreUsuario;
                    string subtexto = string.Format(lm.ObtenerTexto(f, "pdf.Generado", "Generado: {0} | Usuario: {1}"),
                        DateTime.Now.ToString("dd/MM/yyyy HH:mm"), usuario);

                    encabezado.AddCell(new PdfPCell(new Phrase(subtexto, fuenteSubtitulo))
                    {
                        BackgroundColor = new BaseColor(220, 230, 245),
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        Padding = 5f, Border = PdfRectangle.NO_BORDER
                    });
                    doc.Add(encabezado);
                    doc.Add(new Paragraph(" ", fuenteNota));

                    // ------------------------------------------------ 1) Rotacion y compras
                    string periodoRot = string.Format(lm.ObtenerTexto(f, "pdf.PeriodoRotacion", "Período analizado: {0} al {1} | Meses de ventas a cubrir con la compra: {2}"),
                        _rotacion.Desde.ToString("dd/MM/yyyy"), _rotacion.Hasta.ToString("dd/MM/yyyy"), _rotacion.MesesACubrir);
                    AgregarSeccionPDF(doc, lm.ObtenerTexto(f, "pdf.SeccionRotacion", "1. Rotación de productos y compra sugerida"), periodoRot, fuenteSeccion, fuenteSubtitulo);

                    string[] t = new string[6];
                    string[] v = new string[6];
                    string[] d = new string[6];
                    TarjetasRotacion(t, v, d);
                    AgregarTarjetasPDF(doc, t, v, d, fuenteCardTitulo, fuenteCardValor, fuenteCardDetalle, 2);
                    doc.Add(new Paragraph(" ", fuenteNota));

                    PdfPTable graficosRot = new PdfPTable(2) { WidthPercentage = 100, SpacingBefore = 4f };
                    graficosRot.AddCell(new PdfPCell(ImagenDe(chartMas, 390f, 160f), true) { Border = PdfRectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_CENTER });
                    graficosRot.AddCell(new PdfPCell(ImagenDe(chartMenos, 390f, 160f), true) { Border = PdfRectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_CENTER });
                    doc.Add(graficosRot);

                    Paragraph notaRot = new Paragraph(TextoResumenRotacion(), fuenteNota);
                    notaRot.Alignment = Element.ALIGN_CENTER;
                    notaRot.SpacingBefore = 4f;
                    doc.Add(notaRot);
                    doc.Add(new Paragraph(" ", fuenteNota));

                    float[] anchosRot = { 30f, 11f, 11f, 11f, 14f, 13f, 12f };
                    PdfPTable tablaRot = new PdfPTable(anchosRot.Length) { WidthPercentage = 100, SpacingBefore = 4f };
                    tablaRot.SetWidths(anchosRot);
                    tablaRot.HeaderRows = 1;

                    string[] headersRot =
                    {
                        lm.ObtenerTexto(f, "rcol.Modelo", "Modelo"),
                        lm.ObtenerTexto(f, "rcol.Unidades", "Unidades vendidas"),
                        lm.ObtenerTexto(f, "rcol.PorMes", "Ventas por mes"),
                        lm.ObtenerTexto(f, "rcol.Stock", "Stock actual"),
                        lm.ObtenerTexto(f, "rcol.Cobertura", "Stock alcanza para"),
                        lm.ObtenerTexto(f, "rcol.Recomendacion", "Recomendación"),
                        lm.ObtenerTexto(f, "rcol.Cantidad", "Cantidad sugerida")
                    };
                    foreach (string h in headersRot)
                    {
                        tablaRot.AddCell(new PdfPCell(new Phrase(h, fuenteHeader))
                        {
                            BackgroundColor = new BaseColor(33, 90, 160),
                            HorizontalAlignment = Element.ALIGN_CENTER,
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            Padding = 5f
                        });
                    }

                    foreach (FilaRotacion486LP fila in _filasRot)
                    {
                        BaseColor bgFila = BaseColor.WHITE;
                        if (fila.Tipo == ReporteRotacionProducto486LP.COMPRAR) bgFila = new BaseColor(226, 244, 230);
                        else if (fila.Tipo == ReporteRotacionProducto486LP.NO_COMPRAR) bgFila = new BaseColor(250, 228, 228);

                        AgregarCelda(tablaRot, fila.Modelo, fuenteDato, bgFila, Element.ALIGN_LEFT);
                        AgregarCelda(tablaRot, fila.Unidades.ToString("N0"), fuenteDato, bgFila, Element.ALIGN_CENTER);
                        AgregarCelda(tablaRot, fila.PorMes.ToString("N1"), fuenteDato, bgFila, Element.ALIGN_CENTER);
                        AgregarCelda(tablaRot, fila.Stock.ToString("N0"), fuenteDato, bgFila, Element.ALIGN_CENTER);
                        AgregarCelda(tablaRot, fila.Cobertura, fuenteDato, bgFila, Element.ALIGN_CENTER);
                        AgregarCelda(tablaRot, fila.Recomendacion, fuenteDato, bgFila, Element.ALIGN_CENTER);
                        AgregarCelda(tablaRot, fila.CantidadSugerida.HasValue ? fila.CantidadSugerida.Value.ToString("N0") : "-", fuenteDato, bgFila, Element.ALIGN_CENTER);
                    }
                    doc.Add(tablaRot);

                    // ------------------------------------------------ 2) Ventas y proyeccion
                    doc.NewPage();

                    string agrupacionTexto = _reporte.Agrupacion == BLL_ReporteInteligente486LP.AGRUPAR_SEMANA
                        ? lm.ObtenerTexto(f, "agrupar.Semana", "Semana") : lm.ObtenerTexto(f, "agrupar.Mes", "Mes");
                    string periodoVentas = string.Format(lm.ObtenerTexto(f, "pdf.Periodo", "Período: {0} al {1} | Agrupado por: {2} | Método: {3} | Períodos a proyectar: {4}"),
                        _reporte.DesdeEfectivo.ToString("dd/MM/yyyy"), _reporte.HastaEfectivo.ToString("dd/MM/yyyy"),
                        agrupacionTexto, NombreMetodo(_reporte.Metodo), _reporte.PeriodosAProyectar);
                    AgregarSeccionPDF(doc, lm.ObtenerTexto(f, "pdf.SeccionVentas", "2. Ventas por período y proyección"), periodoVentas, fuenteSeccion, fuenteSubtitulo);

                    TarjetasVentas(t, v, d);
                    AgregarTarjetasPDF(doc, t, v, d, fuenteCardTitulo, fuenteCardValor, fuenteCardDetalle, 4);
                    doc.Add(new Paragraph(" ", fuenteNota));

                    doc.Add(ImagenDe(chartVentas, 780f, 165f));

                    if (_reporte.MediosPago.Count > 0)
                    {
                        using (Charting.Chart medios = CrearGraficoMedios(640, 260))
                        {
                            doc.Add(ImagenDe(medios, 400f, 130f));
                        }
                    }

                    Paragraph nota = new Paragraph(TextoResumenVentas(), fuenteNota);
                    nota.Alignment = Element.ALIGN_CENTER;
                    nota.SpacingBefore = 4f;
                    doc.Add(nota);
                    doc.Add(new Paragraph(" ", fuenteNota));

                    float[] anchos = { 22f, 14f, 22f, 22f, 20f };
                    PdfPTable tabla = new PdfPTable(anchos.Length) { WidthPercentage = 100, SpacingBefore = 4f };
                    tabla.SetWidths(anchos);
                    tabla.HeaderRows = 1;

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

                    _bll.RegistrarExportacion(_filas.Count, _filasRot.Count);

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

        // Barra de titulo de una seccion del PDF (con el periodo que cubre)
        private void AgregarSeccionPDF(Document doc, string titulo, string detalle, PdfFont fuenteSeccion, PdfFont fuenteDetalle)
        {
            PdfPTable seccion = new PdfPTable(1) { WidthPercentage = 100 };
            seccion.AddCell(new PdfPCell(new Phrase(titulo, fuenteSeccion))
            {
                BackgroundColor = new BaseColor(240, 90, 40),
                HorizontalAlignment = Element.ALIGN_LEFT,
                Padding = 5f, Border = PdfRectangle.NO_BORDER
            });
            seccion.AddCell(new PdfPCell(new Phrase(detalle, fuenteDetalle))
            {
                BackgroundColor = new BaseColor(253, 240, 232),
                HorizontalAlignment = Element.ALIGN_LEFT,
                Padding = 4f, Border = PdfRectangle.NO_BORDER
            });
            doc.Add(seccion);
            doc.Add(new Paragraph(" ", fuenteDetalle));
        }

        // Fila de 6 tarjetas; desdeResaltada es la primera que va con fondo naranja claro
        private void AgregarTarjetasPDF(Document doc, string[] titulos, string[] valores, string[] detalles,
            PdfFont fuenteTitulo, PdfFont fuenteValor, PdfFont fuenteDetalle, int desdeResaltada)
        {
            PdfPTable tarjetas = new PdfPTable(6) { WidthPercentage = 100 };
            for (int i = 0; i < 6; i++)
            {
                string detalle = (detalles[i] ?? "").Replace("▲ ", "+").Replace("▼ ", "-");
                PdfPCell celda = new PdfPCell
                {
                    BackgroundColor = i >= desdeResaltada ? new BaseColor(253, 240, 232) : new BaseColor(245, 248, 255),
                    BorderColor = new BaseColor(200, 210, 230),
                    Padding = 5f
                };
                celda.AddElement(new Paragraph(titulos[i], fuenteTitulo));
                celda.AddElement(new Paragraph(valores[i], fuenteValor));
                celda.AddElement(new Paragraph(detalle, fuenteDetalle));
                tarjetas.AddCell(celda);
            }
            doc.Add(tarjetas);
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
            lblRango.Text = lm.ObtenerTexto(f, "lblRango", "Período rápido");
            lblCubrir.Text = lm.ObtenerTexto(f, "lblCubrir", "Meses de ventas a cubrir");
            btnVistaRotacion.Text = lm.ObtenerTexto(f, "btnVistaRotacion", "Rotación y compras");
            btnVistaVentas.Text = lm.ObtenerTexto(f, "btnVistaVentas", "Ventas y proyección");
            btnMedios.Text = lm.ObtenerTexto(f, "btnMedios", "Medios de pago");

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

            if (dgvRotacion.Columns.Contains("colRotModelo"))
                dgvRotacion.Columns["colRotModelo"].HeaderText = lm.ObtenerTexto(f, "rcol.Modelo", "Modelo");
            if (dgvRotacion.Columns.Contains("colRotUnidades"))
                dgvRotacion.Columns["colRotUnidades"].HeaderText = lm.ObtenerTexto(f, "rcol.Unidades", "Unidades vendidas");
            if (dgvRotacion.Columns.Contains("colRotPorMes"))
                dgvRotacion.Columns["colRotPorMes"].HeaderText = lm.ObtenerTexto(f, "rcol.PorMes", "Ventas por mes");
            if (dgvRotacion.Columns.Contains("colRotStock"))
                dgvRotacion.Columns["colRotStock"].HeaderText = lm.ObtenerTexto(f, "rcol.Stock", "Stock actual");
            if (dgvRotacion.Columns.Contains("colRotCobertura"))
                dgvRotacion.Columns["colRotCobertura"].HeaderText = lm.ObtenerTexto(f, "rcol.Cobertura", "Stock alcanza para");
            if (dgvRotacion.Columns.Contains("colRotRecomendacion"))
                dgvRotacion.Columns["colRotRecomendacion"].HeaderText = lm.ObtenerTexto(f, "rcol.Recomendacion", "Recomendación");
            if (dgvRotacion.Columns.Contains("colRotCantidad"))
                dgvRotacion.Columns["colRotCantidad"].HeaderText = lm.ObtenerTexto(f, "rcol.Cantidad", "Cantidad sugerida");
        }

        private void FrmReporteInteligente486LP_FormClosing(object sender, FormClosingEventArgs e)
        {
            Program.LanguageManager.Quitar(this);
        }
    }

    public class FilaRotacion486LP
    {
        public string Modelo { get; set; }
        public int Unidades { get; set; }
        public decimal PorMes { get; set; }
        public int Stock { get; set; }
        public string Cobertura { get; set; }
        public string Recomendacion { get; set; }
        public int? CantidadSugerida { get; set; }
        public int Tipo { get; set; }
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

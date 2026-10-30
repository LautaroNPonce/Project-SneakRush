using BE;
using BLL;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Services;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Charting = System.Windows.Forms.DataVisualization.Charting;
using PdfFont = iTextSharp.text.Font;
using PdfRectangle = iTextSharp.text.Rectangle;

namespace Sistema_SneakRush
{
    public partial class FrmReporteVenta486LP : Form, IObserver486LP, IRefrescable486LP
    {
        private BLL_Reporte486LP _bll = new BLL_Reporte486LP();
        private readonly string f = "FrmReporteVenta486LP";
        private bool _puedeGenerar;
        private bool _puedeLimpiar;
        private bool _puedeExportar;
        private bool _puedeCerrar;
        private List<ReporteClienteFrecuente486LP> _datos = new List<ReporteClienteFrecuente486LP>();
        private bool _generado = false;
        private DateTime _desdeUsado;
        private DateTime _hastaUsado;
        private int _minUsado;

        public FrmReporteVenta486LP()
        {
            InitializeComponent();
            Program.LanguageManager.Agregar(this);
            this.FormClosing += FrmReporteVenta486LP_FormClosing;
        }

        private void FrmReporteVenta486LP_Load(object sender, EventArgs e)
        {
            if (!TienePatenteAcceso())
            {
                var lm = Program.LanguageManager;
                MessageBox.Show(
                    lm.ObtenerTexto(f, "Msg.SinPermiso", "No tiene permiso para ver el reporte de clientes frecuentes."),
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
            ConfigurarGrafico();
            ValoresIniciales();
            AjustarBotonesSegunPerfil();
            AplicarPermisosBotones();
            ActualizarIdioma();
        }

        private bool TienePatenteAcceso()
        {
            var usuario = SessionManager486LP.ObtenerInstancia().UsuarioActual();
            if (usuario == null) return false;

            List<string> permisos = new BLL_Perfil486LP().ObtenerPermisosPorRol(usuario.Rol);
            return permisos.Contains("REPORTE_VENTAS");
        }

        private void AjustarBotonesSegunPerfil()
        {
            var usuario = SessionManager486LP.ObtenerInstancia().UsuarioActual();
            if (usuario == null) return;

            List<string> permisos = new BLL_Perfil486LP().ObtenerPermisosPorRol(usuario.Rol);

            _puedeGenerar = permisos.Contains("REPORTE_VENTAS_GENERAR");
            _puedeLimpiar = permisos.Contains("REPORTE_VENTAS_LIMPIAR");
            _puedeExportar = permisos.Contains("REPORTE_VENTAS_EXPORTAR");
            _puedeCerrar = permisos.Contains("REPORTE_VENTAS_CERRAR");
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
            nudMinCompras.Value = 1;
        }

        private void ConfigurarGrilla()
        {
            dgvClientes.AutoGenerateColumns = false;
            dgvClientes.AllowUserToAddRows = false;
            dgvClientes.AllowUserToDeleteRows = false;
            dgvClientes.ReadOnly = true;
            dgvClientes.MultiSelect = false;
            dgvClientes.RowHeadersVisible = false;
            dgvClientes.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvClientes.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvClientes.BackgroundColor = Color.White;
            dgvClientes.EnableHeadersVisualStyles = false;
            dgvClientes.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;

            DataGridViewCellStyle estiloHeader = new DataGridViewCellStyle();
            estiloHeader.BackColor = Color.FromArgb(12, 68, 124);
            estiloHeader.ForeColor = Color.White;
            estiloHeader.Font = new System.Drawing.Font("Segoe UI", 9.75F, FontStyle.Bold);
            estiloHeader.SelectionBackColor = Color.FromArgb(12, 68, 124);
            estiloHeader.SelectionForeColor = Color.White;
            estiloHeader.WrapMode = DataGridViewTriState.True;
            dgvClientes.ColumnHeadersDefaultCellStyle = estiloHeader;

            DataGridViewCellStyle estiloFilas = new DataGridViewCellStyle();
            estiloFilas.BackColor = Color.White;
            estiloFilas.SelectionBackColor = Color.FromArgb(240, 90, 40);
            estiloFilas.SelectionForeColor = Color.White;
            dgvClientes.RowsDefaultCellStyle = estiloFilas;

            DataGridViewCellStyle estiloAlterno = new DataGridViewCellStyle();
            estiloAlterno.BackColor = Color.FromArgb(238, 243, 250);
            dgvClientes.AlternatingRowsDefaultCellStyle = estiloAlterno;

            dgvClientes.Columns.Clear();
            dgvClientes.Columns.Add(new DataGridViewTextBoxColumn { Name = "colDNI", DataPropertyName = "DNI", FillWeight = 16 });
            dgvClientes.Columns.Add(new DataGridViewTextBoxColumn { Name = "colNombre", DataPropertyName = "Nombre", FillWeight = 18 });
            dgvClientes.Columns.Add(new DataGridViewTextBoxColumn { Name = "colApellido", DataPropertyName = "Apellido", FillWeight = 18 });
            dgvClientes.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colCantidadCompras", DataPropertyName = "CantidadCompras", FillWeight = 14,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter }
            });
            dgvClientes.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colTotalGastado", DataPropertyName = "TotalGastado", FillWeight = 18,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "C2", Alignment = DataGridViewContentAlignment.MiddleRight }
            });
            dgvClientes.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colUltimaCompra", DataPropertyName = "UltimaCompra", FillWeight = 16,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "dd/MM/yyyy", Alignment = DataGridViewContentAlignment.MiddleCenter }
            });

            TraducirColumnas();
        }

        private void ConfigurarGrafico()
        {
            chartTop.Series.Clear();
            chartTop.ChartAreas.Clear();
            chartTop.Legends.Clear();
            chartTop.Titles.Clear();

            Charting.ChartArea area = new Charting.ChartArea("areaTop");
            area.BackColor = Color.White;
            area.AxisX.MajorGrid.Enabled = false;
            area.AxisX.IsReversed = true;                       // el cliente que mas gasto queda arriba
            area.AxisX.LabelStyle.Interval = 1;
            area.AxisX.LabelStyle.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            area.AxisY.LabelStyle.Format = "C0";
            area.AxisY.LabelStyle.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            area.AxisY.MajorGrid.LineColor = Color.Gainsboro;
            chartTop.ChartAreas.Add(area);

            Charting.Series serie = new Charting.Series("serieTop");
            serie.ChartType = Charting.SeriesChartType.Bar;
            serie.ChartArea = "areaTop";
            serie.Color = Color.FromArgb(12, 68, 124);
            serie.IsValueShownAsLabel = true;
            serie.LabelFormat = "C0";
            serie.Font = new System.Drawing.Font("Segoe UI", 8.25F, FontStyle.Bold);
            chartTop.Series.Add(serie);

            chartTop.Titles.Add(new Charting.Title("", Charting.Docking.Top, new System.Drawing.Font("Segoe UI", 10F, FontStyle.Bold), Color.FromArgb(12, 68, 124)));
        }

        private void ActualizarGrafico()
        {
            var lm = Program.LanguageManager;

            if (chartTop.Series.Count == 0) return;  

            if (chartTop.Titles.Count > 0)
                chartTop.Titles[0].Text = string.Format(lm.ObtenerTexto(f, "chart.Titulo", "Top {0} clientes por monto gastado"), BLL_Reporte486LP.CANTIDAD_GRAFICO);

            Charting.Series serie = chartTop.Series["serieTop"];
            serie.Points.Clear();

            foreach (ReporteClienteFrecuente486LP c in _bll.TopPorMonto(_datos))
            {
                serie.Points.AddXY(c.NombreCompleto, (double)c.TotalGastado);
            }
        }

        private void btnGenerar_Click(object sender, EventArgs e)
        {
            var lm = Program.LanguageManager;

            DateTime desde = dtpDesde.Value.Date;
            DateTime hasta = dtpHasta.Value.Date;
            int minCompras = (int)nudMinCompras.Value;

            if (!_bll.ValidarRangoFechas(desde, hasta))
            {
                MessageBox.Show(lm.ObtenerTexto(f, "Msg.RangoInvalido", "La fecha 'Desde' no puede ser posterior a la fecha 'Hasta'."),
                    lm.ObtenerTexto(f, "Msg.RangoInvalido.Title", "Filtros inválidos"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!_bll.ValidarMinimoCompras(minCompras))
            {
                MessageBox.Show(lm.ObtenerTexto(f, "Msg.MinInvalido", "El mínimo de compras debe ser 1 o más."),
                    lm.ObtenerTexto(f, "Msg.RangoInvalido.Title", "Filtros inválidos"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            List<ReporteClienteFrecuente486LP> lista = _bll.ClientesFrecuentes(desde, hasta, minCompras);

            if (lista == null)
            {
                MessageBox.Show(lm.ObtenerTexto(f, "Msg.ErrorGenerar", "No se pudo generar el reporte. El error quedó registrado en la bitácora."),
                    lm.ObtenerTexto(f, "Msg.Error.Title", "Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            _datos = lista;
            _generado = true;
            _desdeUsado = desde;
            _hastaUsado = hasta;
            _minUsado = minCompras;

            MostrarDatos();

            if (_datos.Count == 0)
            {
                MessageBox.Show(lm.ObtenerTexto(f, "Msg.SinResultados", "No hay clientes que cumplan con los filtros elegidos."),
                    lm.ObtenerTexto(f, "Msg.SinResultados.Title", "Sin resultados"), MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void MostrarDatos()
        {
            dgvClientes.DataSource = null;
            dgvClientes.DataSource = _datos;
            dgvClientes.ClearSelection();
            ActualizarGrafico();
            ActualizarResumen();
        }

        private void ActualizarResumen()
        {
            var lm = Program.LanguageManager;

            if (!_generado)
            {
                lblResumen.Text = lm.ObtenerTexto(f, "lbl.SinGenerar", "Aún no se generó el reporte.");
                return;
            }

            lblResumen.Text = string.Format(lm.ObtenerTexto(f, "lbl.Resumen", "Clientes: {0} | Total gastado: {1}"),
                _datos.Count, _bll.TotalGeneral(_datos).ToString("C2"));
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
            _datos = new List<ReporteClienteFrecuente486LP>();
            _generado = false;
            MostrarDatos();
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnExportarPDF_Click(object sender, EventArgs e)
        {
            var lm = Program.LanguageManager;

            if (!_generado || _datos.Count == 0)
            {
                MessageBox.Show(lm.ObtenerTexto(f, "Msg.SinDatosPDF", "Primero generá el reporte con al menos un cliente para poder exportarlo."),
                    lm.ObtenerTexto(f, "Msg.PDF.Title", "Exportar PDF"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Title = lm.ObtenerTexto(f, "Msg.GuardarPDF", "Guardar reporte de clientes frecuentes");
                sfd.Filter = "Archivo PDF (*.pdf)|*.pdf";
                sfd.FileName = $"ClientesFrecuentes_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";

                if (sfd.ShowDialog() != DialogResult.OK) return;

                try
                {
                    Document doc = new Document(PageSize.A4.Rotate(), 20f, 20f, 30f, 20f);
                    PdfWriter.GetInstance(doc, new FileStream(sfd.FileName, FileMode.Create));
                    doc.Open();

                    PdfFont fuenteTitulo = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 14f, BaseColor.WHITE);
                    PdfFont fuenteSubtitulo = FontFactory.GetFont(FontFactory.HELVETICA, 9f, new BaseColor(80, 80, 80));
                    PdfFont fuenteHeader = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 8f, BaseColor.WHITE);
                    PdfFont fuenteDato = FontFactory.GetFont(FontFactory.HELVETICA, 8f, BaseColor.BLACK);
                    PdfFont fuenteResumen = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 9f, new BaseColor(33, 90, 160));

                    PdfPTable encabezado = new PdfPTable(1) { WidthPercentage = 100 };
                    encabezado.AddCell(new PdfPCell(new Phrase("SneakRush — " + lm.ObtenerTexto(f, "pdf.Titulo", "Clientes frecuentes"), fuenteTitulo))
                    {
                        BackgroundColor = new BaseColor(33, 90, 160),
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        Padding = 8f, Border = PdfRectangle.NO_BORDER
                    });

                    string usuario = SessionManager486LP.ObtenerInstancia().UsuarioActual().NombreUsuario;
                    string periodo = string.Format(lm.ObtenerTexto(f, "pdf.Periodo", "Período: {0} al {1} | Mínimo de compras: {2}"),
                        _desdeUsado.ToString("dd/MM/yyyy"), _hastaUsado.ToString("dd/MM/yyyy"), _minUsado);
                    string subtexto = string.Format(lm.ObtenerTexto(f, "pdf.Generado", "Generado: {0} | Usuario: {1} | Clientes: {2}"),
                        DateTime.Now.ToString("dd/MM/yyyy HH:mm"), usuario, _datos.Count);

                    encabezado.AddCell(new PdfPCell(new Phrase(periodo + "\n" + subtexto, fuenteSubtitulo))
                    {
                        BackgroundColor = new BaseColor(220, 230, 245),
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        Padding = 5f, Border = PdfRectangle.NO_BORDER
                    });
                    doc.Add(encabezado);
                    doc.Add(new Paragraph(" "));

                    // Grafico de barras
                    using (MemoryStream ms = new MemoryStream())
                    {
                        chartTop.SaveImage(ms, Charting.ChartImageFormat.Png);
                        iTextSharp.text.Image imgGrafico = iTextSharp.text.Image.GetInstance(ms.ToArray());
                        imgGrafico.ScaleToFit(420f, 260f);
                        imgGrafico.Alignment = Element.ALIGN_CENTER;
                        doc.Add(imgGrafico);
                    }

                    Paragraph resumen = new Paragraph(lblResumen.Text, fuenteResumen);
                    resumen.Alignment = Element.ALIGN_CENTER;
                    resumen.SpacingBefore = 6f;
                    doc.Add(resumen);
                    doc.Add(new Paragraph(" "));

                    float[] anchos = { 14f, 20f, 20f, 12f, 18f, 16f };
                    PdfPTable tabla = new PdfPTable(anchos.Length) { WidthPercentage = 100, SpacingBefore = 4f };
                    tabla.SetWidths(anchos);

                    BaseColor bgClaro = new BaseColor(245, 248, 255);

                    string[] headers =
                    {
                        lm.ObtenerTexto(f, "col.DNI", "DNI"),
                        lm.ObtenerTexto(f, "col.Nombre", "Nombre"),
                        lm.ObtenerTexto(f, "col.Apellido", "Apellido"),
                        lm.ObtenerTexto(f, "col.CantidadCompras", "Cantidad de compras"),
                        lm.ObtenerTexto(f, "col.TotalGastado", "Total gastado"),
                        lm.ObtenerTexto(f, "col.UltimaCompra", "Fecha de la última compra")
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
                    foreach (ReporteClienteFrecuente486LP c in _datos)
                    {
                        BaseColor bgFila = filaAlterna ? bgClaro : BaseColor.WHITE;
                        filaAlterna = !filaAlterna;

                        AgregarCelda(tabla, c.DNI, fuenteDato, bgFila, Element.ALIGN_CENTER);
                        AgregarCelda(tabla, c.Nombre, fuenteDato, bgFila, Element.ALIGN_LEFT);
                        AgregarCelda(tabla, c.Apellido, fuenteDato, bgFila, Element.ALIGN_LEFT);
                        AgregarCelda(tabla, c.CantidadCompras.ToString(), fuenteDato, bgFila, Element.ALIGN_CENTER);
                        AgregarCelda(tabla, c.TotalGastado.ToString("C2"), fuenteDato, bgFila, Element.ALIGN_RIGHT);
                        AgregarCelda(tabla, c.UltimaCompra.ToString("dd/MM/yyyy"), fuenteDato, bgFila, Element.ALIGN_CENTER);
                    }

                    doc.Add(tabla);
                    doc.Add(new Paragraph(" "));

                    Paragraph pie = new Paragraph("SneakRush 2026 — Universidad Abierta Interamericana — Ingeniería de Software", FontFactory.GetFont(FontFactory.HELVETICA, 7f, new BaseColor(150, 150, 150)));
                    pie.Alignment = Element.ALIGN_CENTER;
                    doc.Add(pie);
                    doc.Close();

                    _bll.RegistrarExportacionClientesFrecuentes(_datos.Count);

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

            this.Text = lm.ObtenerTexto(f, "Title", "Clientes frecuentes");
            lblTitulo.Text = lm.ObtenerTexto(f, "Title", "Clientes frecuentes");

            grpFiltros.Text = lm.ObtenerTexto(f, "grpFiltros", "Filtros");
            lblDesde.Text = lm.ObtenerTexto(f, "lblDesde", "Desde");
            lblHasta.Text = lm.ObtenerTexto(f, "lblHasta", "Hasta");
            lblMinCompras.Text = lm.ObtenerTexto(f, "lblMinCompras", "Mínimo de compras");
            // el selector numerico se corre a la derecha del texto, que puede cambiar de largo segun el idioma
            nudMinCompras.Left = lblMinCompras.Right + 10;

            btnGenerar.Text = lm.ObtenerTexto(f, "btnGenerar", "Generar");
            btnLimpiar.Text = lm.ObtenerTexto(f, "btnLimpiar", "Limpiar");
            btnExportarPDF.Text = lm.ObtenerTexto(f, "btnExportarPDF", "Exportar PDF");
            btnCerrar.Text = lm.ObtenerTexto(f, "btnCerrar", "Cerrar");

            TraducirColumnas();
            ActualizarResumen();
            if (chartTop.Titles.Count > 0)
                chartTop.Titles[0].Text = string.Format(lm.ObtenerTexto(f, "chart.Titulo", "Top {0} clientes por monto gastado"), BLL_Reporte486LP.CANTIDAD_GRAFICO);
        }

        private void TraducirColumnas()
        {
            var lm = Program.LanguageManager;

            if (dgvClientes.Columns.Contains("colDNI"))
                dgvClientes.Columns["colDNI"].HeaderText = lm.ObtenerTexto(f, "col.DNI", "DNI");
            if (dgvClientes.Columns.Contains("colNombre"))
                dgvClientes.Columns["colNombre"].HeaderText = lm.ObtenerTexto(f, "col.Nombre", "Nombre");
            if (dgvClientes.Columns.Contains("colApellido"))
                dgvClientes.Columns["colApellido"].HeaderText = lm.ObtenerTexto(f, "col.Apellido", "Apellido");
            if (dgvClientes.Columns.Contains("colCantidadCompras"))
                dgvClientes.Columns["colCantidadCompras"].HeaderText = lm.ObtenerTexto(f, "col.CantidadCompras", "Cantidad de compras");
            if (dgvClientes.Columns.Contains("colTotalGastado"))
                dgvClientes.Columns["colTotalGastado"].HeaderText = lm.ObtenerTexto(f, "col.TotalGastado", "Total gastado");
            if (dgvClientes.Columns.Contains("colUltimaCompra"))
                dgvClientes.Columns["colUltimaCompra"].HeaderText = lm.ObtenerTexto(f, "col.UltimaCompra", "Fecha de la última compra");
        }

        private void FrmReporteVenta486LP_FormClosing(object sender, FormClosingEventArgs e)
        {
            Program.LanguageManager.Quitar(this);
        }
    }
}

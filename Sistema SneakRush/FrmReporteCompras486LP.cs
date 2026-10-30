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
    // Reporte "Estado de stock y reposicion" (menu Reporte > Estado de stock). Se mantiene el nombre de clase FrmReporteCompras486LP.
    public partial class FrmReporteCompras486LP : Form, IObserver486LP, IRefrescable486LP
    {
        private BLL_Reporte486LP _bll = new BLL_Reporte486LP();
        private readonly string f = "FrmReporteCompras486LP";

        // Patentes granulares por boton (ademas de la patente de acceso al form, REPORTE_COMPRAS).
        private bool _puedeGenerar;
        private bool _puedeLimpiar;
        private bool _puedeExportar;
        private bool _puedeCerrar;

        // Opciones de los filtros de marca y talle (salen de los productos cargados)
        private List<string> _marcas = new List<string>();
        private List<string> _talles = new List<string>();

        // Ultimo reporte generado (es lo que se ve en la grilla y el grafico, y lo que se exporta)
        private List<ReporteStockProducto486LP> _datos = new List<ReporteStockProducto486LP>();
        private bool _generado = false;

        // Colores de cada estado: 0 sin stock (rojo), 1 stock bajo (naranja), 2 normal (verde)
        private readonly Color[] _coloresEstado =
        {
            Color.FromArgb(192, 57, 43),
            Color.FromArgb(230, 140, 20),
            Color.FromArgb(39, 150, 80)
        };

        public FrmReporteCompras486LP()
        {
            InitializeComponent();
            Program.LanguageManager.Agregar(this);
            this.FormClosing += FrmReporteCompras486LP_FormClosing;
            chkSoloReponer.CheckedChanged += chkSoloReponer_CheckedChanged;
        }

        private void FrmReporteCompras486LP_Load(object sender, EventArgs e)
        {
            // Verificacion de acceso al form completo: un usuario sin la patente REPORTE_COMPRAS (ej. el Cajero) no puede entrar.
            if (!TienePatenteAcceso())
            {
                var lm = Program.LanguageManager;
                MessageBox.Show(
                    lm.ObtenerTexto(f, "Msg.SinPermiso", "No tiene permiso para ver el reporte de estado de stock."),
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
            CargarOpcionesFiltros();
            AjustarBotonesSegunPerfil();
            AplicarPermisosBotones();
            ActualizarIdioma();
        }

        // Verifica que el usuario en sesion tenga la patente de acceso al form.
        private bool TienePatenteAcceso()
        {
            var usuario = SessionManager486LP.ObtenerInstancia().UsuarioActual();
            if (usuario == null) return false;

            List<string> permisos = new BLL_Perfil486LP().ObtenerPermisosPorRol(usuario.Rol);
            return permisos.Contains("REPORTE_COMPRAS");
        }

        // Patentes granulares por boton.
        private void AjustarBotonesSegunPerfil()
        {
            var usuario = SessionManager486LP.ObtenerInstancia().UsuarioActual();
            if (usuario == null) return;

            List<string> permisos = new BLL_Perfil486LP().ObtenerPermisosPorRol(usuario.Rol);

            _puedeGenerar = permisos.Contains("REPORTE_COMPRAS_GENERAR");
            _puedeLimpiar = permisos.Contains("REPORTE_COMPRAS_LIMPIAR");
            _puedeExportar = permisos.Contains("REPORTE_COMPRAS_EXPORTAR");
            _puedeCerrar = permisos.Contains("REPORTE_COMPRAS_CERRAR");
        }

        private void AplicarPermisosBotones()
        {
            btnGenerar.Enabled = _puedeGenerar;
            btnLimpiar.Enabled = _puedeLimpiar;
            btnExportarPDF.Enabled = _puedeExportar;
            btnCerrar.Enabled = _puedeCerrar;
        }

        // Las marcas y talles del filtro salen de todos los productos cargados
        private void CargarOpcionesFiltros()
        {
            List<ReporteStockProducto486LP> todos = _bll.EstadoStock(null, null, null, false);
            if (todos == null)
            {
                var lm = Program.LanguageManager;
                MessageBox.Show(lm.ObtenerTexto(f, "Msg.ErrorGenerar", "No se pudo generar el reporte. El error quedó registrado en la bitácora."),
                    lm.ObtenerTexto(f, "Msg.Error.Title", "Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            _marcas = _bll.MarcasDe(todos);
            _talles = _bll.TallesDe(todos);
        }

        // Llena los 3 combos (la primera opcion de cada uno es "Todos"/"Todas"); conserva lo que estaba elegido
        private void LlenarCombos()
        {
            var lm = Program.LanguageManager;
            string todos = lm.ObtenerTexto(f, "cmb.Todos", "Todos");

            int idxMarca = cmbMarca.SelectedIndex;
            int idxTalle = cmbTalle.SelectedIndex;
            int idxEstado = cmbEstado.SelectedIndex;

            cmbMarca.Items.Clear();
            cmbMarca.Items.Add(todos);
            foreach (string m in _marcas) cmbMarca.Items.Add(m);
            cmbMarca.SelectedIndex = (idxMarca >= 0 && idxMarca < cmbMarca.Items.Count) ? idxMarca : 0;

            cmbTalle.Items.Clear();
            cmbTalle.Items.Add(todos);
            foreach (string t in _talles) cmbTalle.Items.Add(t);
            cmbTalle.SelectedIndex = (idxTalle >= 0 && idxTalle < cmbTalle.Items.Count) ? idxTalle : 0;

            cmbEstado.Items.Clear();
            cmbEstado.Items.Add(todos);
            for (int i = 0; i <= 2; i++) cmbEstado.Items.Add(EstadoTexto(i));
            cmbEstado.SelectedIndex = (idxEstado >= 0 && idxEstado < cmbEstado.Items.Count) ? idxEstado : 0;
        }

        private string EstadoTexto(int estado)
        {
            var lm = Program.LanguageManager;
            switch (estado)
            {
                case ReporteStockProducto486LP.SIN_STOCK: return lm.ObtenerTexto(f, "estado.SinStock", "Sin stock");
                case ReporteStockProducto486LP.STOCK_BAJO: return lm.ObtenerTexto(f, "estado.StockBajo", "Stock bajo");
                default: return lm.ObtenerTexto(f, "estado.Normal", "Normal");
            }
        }

        // "Solo productos a reponer" ya incluye los estados "sin stock" y "stock bajo": en ese caso el combo de estado no se usa
        private void chkSoloReponer_CheckedChanged(object sender, EventArgs e)
        {
            if (chkSoloReponer.Checked)
            {
                cmbEstado.SelectedIndex = 0;
                cmbEstado.Enabled = false;
            }
            else
            {
                cmbEstado.Enabled = true;
            }
        }

        private void ConfigurarGrilla()
        {
            dgvProductos.AutoGenerateColumns = false;
            dgvProductos.AllowUserToAddRows = false;
            dgvProductos.AllowUserToDeleteRows = false;
            dgvProductos.ReadOnly = true;
            dgvProductos.MultiSelect = false;
            dgvProductos.RowHeadersVisible = false;
            dgvProductos.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvProductos.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvProductos.BackgroundColor = Color.White;
            dgvProductos.EnableHeadersVisualStyles = false;
            dgvProductos.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;

            DataGridViewCellStyle estiloHeader = new DataGridViewCellStyle();
            estiloHeader.BackColor = Color.FromArgb(12, 68, 124);
            estiloHeader.ForeColor = Color.White;
            estiloHeader.Font = new System.Drawing.Font("Segoe UI", 9.75F, FontStyle.Bold);
            estiloHeader.SelectionBackColor = Color.FromArgb(12, 68, 124);
            estiloHeader.SelectionForeColor = Color.White;
            estiloHeader.WrapMode = DataGridViewTriState.True;
            dgvProductos.ColumnHeadersDefaultCellStyle = estiloHeader;

            DataGridViewCellStyle estiloFilas = new DataGridViewCellStyle();
            estiloFilas.BackColor = Color.White;
            estiloFilas.SelectionBackColor = Color.FromArgb(240, 90, 40);
            estiloFilas.SelectionForeColor = Color.White;
            dgvProductos.RowsDefaultCellStyle = estiloFilas;

            DataGridViewCellStyle estiloAlterno = new DataGridViewCellStyle();
            estiloAlterno.BackColor = Color.FromArgb(238, 243, 250);
            dgvProductos.AlternatingRowsDefaultCellStyle = estiloAlterno;

            dgvProductos.Columns.Clear();
            dgvProductos.Columns.Add(new DataGridViewTextBoxColumn { Name = "colMarca", DataPropertyName = "Marca", FillWeight = 17 });
            dgvProductos.Columns.Add(new DataGridViewTextBoxColumn { Name = "colModelo", DataPropertyName = "Modelo", FillWeight = 24 });
            dgvProductos.Columns.Add(new DataGridViewTextBoxColumn { Name = "colColor", DataPropertyName = "Color", FillWeight = 11 });
            dgvProductos.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colTalle", DataPropertyName = "Talle", FillWeight = 8,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter }
            });
            dgvProductos.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colStock", DataPropertyName = "Stock", FillWeight = 9,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter }
            });
            dgvProductos.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colPrecio", DataPropertyName = "Precio", FillWeight = 13,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "C2", Alignment = DataGridViewContentAlignment.MiddleRight }
            });
            dgvProductos.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colValorStock", DataPropertyName = "ValorStock", FillWeight = 14,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "C2", Alignment = DataGridViewContentAlignment.MiddleRight }
            });
            dgvProductos.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colEstado", DataPropertyName = "Estado", FillWeight = 12,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter }
            });

            dgvProductos.CellFormatting += dgvProductos_CellFormatting;

            TraducirColumnas();
        }

        // El estado viene como numero (0, 1 o 2): en la grilla se muestra su nombre (traducido) y con el color de su estado
        private void dgvProductos_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;
            if (dgvProductos.Columns[e.ColumnIndex].Name != "colEstado") return;

            if (e.Value is int estado && estado >= 0 && estado <= 2)
            {
                e.Value = EstadoTexto(estado);
                e.CellStyle.ForeColor = _coloresEstado[estado];
                e.CellStyle.Font = new System.Drawing.Font("Segoe UI", 9.75F, FontStyle.Bold);
                e.FormattingApplied = true;
            }
        }

        private void ConfigurarGrafico()
        {
            chartEstado.Series.Clear();
            chartEstado.ChartAreas.Clear();
            chartEstado.Legends.Clear();
            chartEstado.Titles.Clear();

            Charting.ChartArea area = new Charting.ChartArea("areaEstado");
            area.BackColor = Color.White;
            area.AxisX.MajorGrid.Enabled = false;
            area.AxisX.LabelStyle.Interval = 1;
            area.AxisX.LabelStyle.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            area.AxisY.LabelStyle.Format = "0";
            area.AxisY.LabelStyle.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            area.AxisY.MajorGrid.LineColor = Color.Gainsboro;
            chartEstado.ChartAreas.Add(area);

            Charting.Series serie = new Charting.Series("serieEstado");
            serie.ChartType = Charting.SeriesChartType.Column;
            serie.ChartArea = "areaEstado";
            serie.IsValueShownAsLabel = true;
            serie.Font = new System.Drawing.Font("Segoe UI", 9F, FontStyle.Bold);
            chartEstado.Series.Add(serie);

            chartEstado.Titles.Add(new Charting.Title("", Charting.Docking.Top, new System.Drawing.Font("Segoe UI", 10F, FontStyle.Bold), Color.FromArgb(12, 68, 124)));
        }

        private void ActualizarGrafico()
        {
            var lm = Program.LanguageManager;

            if (chartEstado.Series.Count == 0) return;   // el form se esta cerrando por falta de permiso y nunca llego a armar el grafico

            if (chartEstado.Titles.Count > 0)
                chartEstado.Titles[0].Text = lm.ObtenerTexto(f, "chart.Titulo", "Cantidad de productos por estado de stock");

            Charting.Series serie = chartEstado.Series["serieEstado"];
            serie.Points.Clear();

            int[] cantidades = _bll.ContarPorEstado(_datos);
            for (int i = 0; i <= 2; i++)
            {
                int idx = serie.Points.AddXY(EstadoTexto(i), cantidades[i]);
                serie.Points[idx].Color = _coloresEstado[i];
            }
        }

        private void btnGenerar_Click(object sender, EventArgs e)
        {
            var lm = Program.LanguageManager;

            string marca = cmbMarca.SelectedIndex > 0 ? cmbMarca.SelectedItem.ToString() : null;
            string talle = cmbTalle.SelectedIndex > 0 ? cmbTalle.SelectedItem.ToString() : null;
            int? estado = cmbEstado.SelectedIndex > 0 ? (int?)(cmbEstado.SelectedIndex - 1) : null;
            bool soloReponer = chkSoloReponer.Checked;

            List<ReporteStockProducto486LP> lista = _bll.EstadoStock(marca, talle, estado, soloReponer);

            if (lista == null)
            {
                MessageBox.Show(lm.ObtenerTexto(f, "Msg.ErrorGenerar", "No se pudo generar el reporte. El error quedó registrado en la bitácora."),
                    lm.ObtenerTexto(f, "Msg.Error.Title", "Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            _datos = lista;
            _generado = true;
            MostrarDatos();

            if (_datos.Count == 0)
            {
                MessageBox.Show(lm.ObtenerTexto(f, "Msg.SinResultados", "No hay productos que cumplan con los filtros elegidos."),
                    lm.ObtenerTexto(f, "Msg.SinResultados.Title", "Sin resultados"), MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void MostrarDatos()
        {
            dgvProductos.DataSource = null;
            dgvProductos.DataSource = _datos;
            dgvProductos.ClearSelection();
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

            lblResumen.Text = string.Format(lm.ObtenerTexto(f, "lbl.Resumen", "Productos: {0} | Unidades: {1} | Valor total del stock: {2}"),
                _datos.Count, _bll.TotalUnidades(_datos), _bll.ValorTotalStock(_datos).ToString("C2"));
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            Resetear();
        }

        // Si el reporte ya estaba abierto y se vuelve a elegir desde el menu, se limpian los filtros y el resultado
        public void Refrescar()
        {
            Resetear();
        }

        private void Resetear()
        {
            chkSoloReponer.Checked = false;
            if (cmbMarca.Items.Count > 0) cmbMarca.SelectedIndex = 0;
            if (cmbTalle.Items.Count > 0) cmbTalle.SelectedIndex = 0;
            if (cmbEstado.Items.Count > 0) cmbEstado.SelectedIndex = 0;
            _datos = new List<ReporteStockProducto486LP>();
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
                MessageBox.Show(lm.ObtenerTexto(f, "Msg.SinDatosPDF", "Primero generá el reporte con al menos un producto para poder exportarlo."),
                    lm.ObtenerTexto(f, "Msg.PDF.Title", "Exportar PDF"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Title = lm.ObtenerTexto(f, "Msg.GuardarPDF", "Guardar reporte de estado de stock");
                sfd.Filter = "Archivo PDF (*.pdf)|*.pdf";
                sfd.FileName = $"EstadoStock_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";

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
                    PdfFont fuenteEstado = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 8f, BaseColor.WHITE);

                    PdfPTable encabezado = new PdfPTable(1) { WidthPercentage = 100 };
                    encabezado.AddCell(new PdfPCell(new Phrase("SneakRush — " + lm.ObtenerTexto(f, "pdf.Titulo", "Estado de stock y reposición"), fuenteTitulo))
                    {
                        BackgroundColor = new BaseColor(33, 90, 160),
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        Padding = 8f, Border = PdfRectangle.NO_BORDER
                    });

                    string usuario = SessionManager486LP.ObtenerInstancia().UsuarioActual().NombreUsuario;
                    string filtros = string.Format(lm.ObtenerTexto(f, "pdf.Filtros", "Marca: {0} | Talle: {1} | Estado: {2} | Solo a reponer: {3}"),
                        cmbMarca.SelectedItem, cmbTalle.SelectedItem, cmbEstado.SelectedItem,
                        chkSoloReponer.Checked ? lm.ObtenerTexto(f, "pdf.Si", "Sí") : lm.ObtenerTexto(f, "pdf.No", "No"));
                    string subtexto = string.Format(lm.ObtenerTexto(f, "pdf.Generado", "Generado: {0} | Usuario: {1} | Productos: {2}"),
                        DateTime.Now.ToString("dd/MM/yyyy HH:mm"), usuario, _datos.Count);

                    encabezado.AddCell(new PdfPCell(new Phrase(filtros + "\n" + subtexto, fuenteSubtitulo))
                    {
                        BackgroundColor = new BaseColor(220, 230, 245),
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        Padding = 5f, Border = PdfRectangle.NO_BORDER
                    });
                    doc.Add(encabezado);
                    doc.Add(new Paragraph(" "));

                    // Grafico de barras (se toma la imagen del grafico que se ve en pantalla)
                    using (MemoryStream ms = new MemoryStream())
                    {
                        chartEstado.SaveImage(ms, Charting.ChartImageFormat.Png);
                        iTextSharp.text.Image imgGrafico = iTextSharp.text.Image.GetInstance(ms.ToArray());
                        imgGrafico.ScaleToFit(380f, 240f);
                        imgGrafico.Alignment = Element.ALIGN_CENTER;
                        doc.Add(imgGrafico);
                    }

                    Paragraph resumen = new Paragraph(lblResumen.Text, fuenteResumen);
                    resumen.Alignment = Element.ALIGN_CENTER;
                    resumen.SpacingBefore = 6f;
                    doc.Add(resumen);
                    doc.Add(new Paragraph(" "));

                    float[] anchos = { 13f, 17f, 13f, 7f, 8f, 14f, 15f, 13f };
                    PdfPTable tabla = new PdfPTable(anchos.Length) { WidthPercentage = 100, SpacingBefore = 4f };
                    tabla.SetWidths(anchos);

                    BaseColor bgClaro = new BaseColor(245, 248, 255);

                    string[] headers =
                    {
                        lm.ObtenerTexto(f, "col.Marca", "Marca"),
                        lm.ObtenerTexto(f, "col.Modelo", "Modelo"),
                        lm.ObtenerTexto(f, "col.Color", "Color"),
                        lm.ObtenerTexto(f, "col.Talle", "Talle"),
                        lm.ObtenerTexto(f, "col.Stock", "Stock"),
                        lm.ObtenerTexto(f, "col.Precio", "Precio"),
                        lm.ObtenerTexto(f, "col.ValorStock", "Valor del stock"),
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
                    foreach (ReporteStockProducto486LP p in _datos)
                    {
                        BaseColor bgFila = filaAlterna ? bgClaro : BaseColor.WHITE;
                        filaAlterna = !filaAlterna;

                        Color c = _coloresEstado[p.Estado >= 0 && p.Estado <= 2 ? p.Estado : 2];
                        BaseColor bgEstado = new BaseColor(c.R, c.G, c.B);

                        AgregarCelda(tabla, p.Marca, fuenteDato, bgFila, Element.ALIGN_LEFT);
                        AgregarCelda(tabla, p.Modelo, fuenteDato, bgFila, Element.ALIGN_LEFT);
                        AgregarCelda(tabla, p.Color, fuenteDato, bgFila, Element.ALIGN_LEFT);
                        AgregarCelda(tabla, p.Talle, fuenteDato, bgFila, Element.ALIGN_CENTER);
                        AgregarCelda(tabla, p.Stock.ToString(), fuenteDato, bgFila, Element.ALIGN_CENTER);
                        AgregarCelda(tabla, p.Precio.ToString("C2"), fuenteDato, bgFila, Element.ALIGN_RIGHT);
                        AgregarCelda(tabla, p.ValorStock.ToString("C2"), fuenteDato, bgFila, Element.ALIGN_RIGHT);
                        AgregarCelda(tabla, EstadoTexto(p.Estado), fuenteEstado, bgEstado, Element.ALIGN_CENTER);
                    }

                    doc.Add(tabla);
                    doc.Add(new Paragraph(" "));

                    Paragraph pie = new Paragraph("SneakRush 2026 — Universidad Abierta Interamericana — Ingeniería de Software", FontFactory.GetFont(FontFactory.HELVETICA, 7f, new BaseColor(150, 150, 150)));
                    pie.Alignment = Element.ALIGN_CENTER;
                    doc.Add(pie);
                    doc.Close();

                    _bll.RegistrarExportacionEstadoStock(_datos.Count);

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

            this.Text = lm.ObtenerTexto(f, "Title", "Estado de stock y reposición");
            lblTitulo.Text = lm.ObtenerTexto(f, "Title", "Estado de stock y reposición");

            grpFiltros.Text = lm.ObtenerTexto(f, "grpFiltros", "Filtros");
            lblMarca.Text = lm.ObtenerTexto(f, "lblMarca", "Marca");
            lblTalle.Text = lm.ObtenerTexto(f, "lblTalle", "Talle");
            lblEstado.Text = lm.ObtenerTexto(f, "lblEstado", "Estado");
            chkSoloReponer.Text = lm.ObtenerTexto(f, "chkSoloReponer", "Solo productos a reponer");

            LlenarCombos();

            // los controles se acomodan uno a continuacion del otro, porque cada texto puede cambiar de largo segun el idioma
            lblMarca.Left = 18;
            cmbMarca.Left = lblMarca.Right + 8;
            lblTalle.Left = cmbMarca.Right + 20;
            cmbTalle.Left = lblTalle.Right + 8;
            lblEstado.Left = cmbTalle.Right + 20;
            cmbEstado.Left = lblEstado.Right + 8;
            chkSoloReponer.Left = cmbEstado.Right + 25;

            btnGenerar.Text = lm.ObtenerTexto(f, "btnGenerar", "Generar");
            btnLimpiar.Text = lm.ObtenerTexto(f, "btnLimpiar", "Limpiar");
            btnExportarPDF.Text = lm.ObtenerTexto(f, "btnExportarPDF", "Exportar PDF");
            btnCerrar.Text = lm.ObtenerTexto(f, "btnCerrar", "Cerrar");

            TraducirColumnas();
            dgvProductos.Invalidate();   // vuelve a dibujar la columna Estado con el texto del nuevo idioma
            ActualizarResumen();
            ActualizarGrafico();
        }

        private void TraducirColumnas()
        {
            var lm = Program.LanguageManager;

            if (dgvProductos.Columns.Contains("colMarca"))
                dgvProductos.Columns["colMarca"].HeaderText = lm.ObtenerTexto(f, "col.Marca", "Marca");
            if (dgvProductos.Columns.Contains("colModelo"))
                dgvProductos.Columns["colModelo"].HeaderText = lm.ObtenerTexto(f, "col.Modelo", "Modelo");
            if (dgvProductos.Columns.Contains("colColor"))
                dgvProductos.Columns["colColor"].HeaderText = lm.ObtenerTexto(f, "col.Color", "Color");
            if (dgvProductos.Columns.Contains("colTalle"))
                dgvProductos.Columns["colTalle"].HeaderText = lm.ObtenerTexto(f, "col.Talle", "Talle");
            if (dgvProductos.Columns.Contains("colStock"))
                dgvProductos.Columns["colStock"].HeaderText = lm.ObtenerTexto(f, "col.Stock", "Stock");
            if (dgvProductos.Columns.Contains("colPrecio"))
                dgvProductos.Columns["colPrecio"].HeaderText = lm.ObtenerTexto(f, "col.Precio", "Precio");
            if (dgvProductos.Columns.Contains("colValorStock"))
                dgvProductos.Columns["colValorStock"].HeaderText = lm.ObtenerTexto(f, "col.ValorStock", "Valor del stock");
            if (dgvProductos.Columns.Contains("colEstado"))
                dgvProductos.Columns["colEstado"].HeaderText = lm.ObtenerTexto(f, "col.Estado", "Estado");
        }

        private void FrmReporteCompras486LP_FormClosing(object sender, FormClosingEventArgs e)
        {
            Program.LanguageManager.Quitar(this);
        }
    }
}

using BLL;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PdfFont = iTextSharp.text.Font;
using PdfRectangle = iTextSharp.text.Rectangle;

namespace Sistema_SneakRush
{
    public partial class FrmBitacoraEvento486LP : Form, IObserver486LP
    {
        private BLL_Bitacora486LP _bll = new BLL_Bitacora486LP();
        private readonly string[] _codigosModulo =
        {
            "", "Login", "Logout", "Cambiar Contraseña",
            "Gestión Usuarios", "Gestión Perfiles", "Gestión Familias", "Gestión Respaldos"
        };
        private bool _filtroAplicado = true;   // arranca igual que el diseñador (botón = "Cancelar")
        private int _ultimoTotal = 0;

        public FrmBitacoraEvento486LP()
        {
            InitializeComponent();
            AjustarBotonesSegunPerfil();
            Program.LanguageManager.Agregar(this);
            this.FormClosing += FrmBitacoraEvento486LP_FormClosing;
        }

        private void FrmBitacoraEvento486LP_Load(object sender, EventArgs e)
        {
            ConfigurarDgv();
            CargarCombos();
            CargarGrilla();
            ActualizarIdioma();
        }

        private void ConfigurarDgv()
        {
            dgtBitacoraEvento.AutoGenerateColumns = false;
            dgtBitacoraEvento.ReadOnly = true;
            dgtBitacoraEvento.AllowUserToAddRows = false;
            dgtBitacoraEvento.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgtBitacoraEvento.MultiSelect = false;
            dgtBitacoraEvento.Columns.Clear();

            dgtBitacoraEvento.Columns.Add(new DataGridViewTextBoxColumn { Name = "colNumero", DataPropertyName = "Numero", HeaderText = "N°", Width = 60  });
            dgtBitacoraEvento.Columns.Add(new DataGridViewTextBoxColumn { Name = "colFecha", DataPropertyName = "Fecha", HeaderText = "Fecha y Hora", Width = 160, DefaultCellStyle = { Format = "dd/MM/yyyy HH:mm:ss" } });
            dgtBitacoraEvento.Columns.Add(new DataGridViewTextBoxColumn { Name = "colNombreUsuario", DataPropertyName = "NombreUsuario", HeaderText = "Usuario", Width = 120 });
            dgtBitacoraEvento.Columns.Add(new DataGridViewTextBoxColumn { Name = "colDNI", DataPropertyName = "DNI", HeaderText = "DNI", Width = 100 });
            dgtBitacoraEvento.Columns.Add(new DataGridViewTextBoxColumn { Name = "colModulo", DataPropertyName = "Modulo", HeaderText = "Módulo", Width = 150 });
            dgtBitacoraEvento.Columns.Add(new DataGridViewTextBoxColumn { Name = "colDescripcion", DataPropertyName = "Descripcion", HeaderText = "Descripción", Width = 200 });
            dgtBitacoraEvento.Columns.Add(new DataGridViewTextBoxColumn { Name = "colCriticidad", DataPropertyName = "Criticidad", HeaderText = "Criticidad", Width = 110 });

            dgtBitacoraEvento.ShowCellToolTips = true;

            TraducirColumnas();
        }

        private void CargarCombos()
        {
            var lm = Program.LanguageManager;
            string f = "FrmBitacoraEvento486LP";

            int idxModulo = cmbModulo.SelectedIndex;
            int idxCrit = cmbCriticidad.SelectedIndex;

            cmbModulo.Items.Clear();
            cmbModulo.Items.AddRange(new object[]
            {
                "",
                lm.ObtenerTexto(f, "modBit.Login", "Login"),
                lm.ObtenerTexto(f, "modBit.Logout", "Logout"),
                lm.ObtenerTexto(f, "modBit.CambiarContrasena", "Cambiar Contraseña"),
                lm.ObtenerTexto(f, "modBit.GestionUsuarios", "Gestión Usuarios"),
                lm.ObtenerTexto(f, "modBit.GestionPerfiles", "Gestión Perfiles"),
                lm.ObtenerTexto(f, "modBit.GestionFamilias", "Gestión Familias"),
                lm.ObtenerTexto(f, "modBit.GestionRespaldos", "Gestión Respaldos")
            });
            cmbModulo.SelectedIndex = (idxModulo >= 0) ? idxModulo : 0;

            cmbCriticidad.Items.Clear();
            cmbCriticidad.Items.AddRange(new object[]
            {
                "",
                lm.ObtenerTexto(f, "crit.MuyAlta", "1 - Muy Alta"),
                lm.ObtenerTexto(f, "crit.Alta", "2 - Alta"),
                lm.ObtenerTexto(f, "crit.Media", "3 - Media"),
                lm.ObtenerTexto(f, "crit.Baja", "4 - Baja"),
                lm.ObtenerTexto(f, "crit.MuyBaja", "5 - Muy Baja")
            });
            cmbCriticidad.SelectedIndex = (idxCrit >= 0) ? idxCrit : 0;
        }

        private void CargarGrilla()
        {
            string fechaInicio = DateTime.Today.AddDays(-3).ToString("yyyy-MM-dd");
            string fechaFin = DateTime.Today.ToString("yyyy-MM-dd") + " 23:59:59";
            List<BitacoraEvento486LP> lista = _bll.Filtrar("", "", "", null, fechaInicio, fechaFin);
            ActualizarGrilla(lista);
        }

        private void ActualizarGrilla(List<BitacoraEvento486LP> lista)
        {
            dgtBitacoraEvento.DataSource = null;
            dgtBitacoraEvento.DataSource = lista;
            _ultimoTotal = lista.Count;
            ActualizarLabelTotal();
        }

        private void btnAplicar_Click(object sender, EventArgs e)
        {
            var lm = Program.LanguageManager;
            string f = "FrmBitacoraEvento486LP";
            string dni = txtDNI.Text.Trim();
            string modulo = (cmbModulo.SelectedIndex >= 0) ? _codigosModulo[cmbModulo.SelectedIndex] : "";
            string fechaInicio = dtpFechaDesde.Checked ? dtpFechaDesde.Value.ToString("yyyy-MM-dd") : "";
            string fechaFin = dtpFechaHasta.Checked ? dtpFechaHasta.Value.ToString("yyyy-MM-dd") + " 23:59:59" : "";

            // El índice del combo coincide directamente con el número de criticidad
            int? criticidad = cmbCriticidad.SelectedIndex > 0 ? cmbCriticidad.SelectedIndex : (int?)null;

            if (dtpFechaDesde.Checked && dtpFechaHasta.Checked && dtpFechaDesde.Value.Date > dtpFechaHasta.Value.Date)
            {
                MessageBox.Show(lm.ObtenerTexto(f, "Msg.FechasInvalidas", "La fecha desde no puede ser mayor a la fecha hasta."),
                    lm.ObtenerTexto(f, "Msg.FechasInvalidas.Title", "Fechas inválidas"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            List<BitacoraEvento486LP> lista = _bll.Filtrar(dni, "", modulo, criticidad, fechaInicio, fechaFin);
            ActualizarGrilla(lista);
            _filtroAplicado = true;
            ActualizarBotonCancelar();

            if (lista.Count == 0)
            {
                MessageBox.Show(lm.ObtenerTexto(f, "Msg.SinResultados", "No se encontraron registros con los filtros aplicados."),
                    lm.ObtenerTexto(f, "Msg.SinResultados.Title", "Sin resultados"), MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            if (_filtroAplicado)
            {
                Resetear();
            }
            else
            {
                this.Close();
            }
        }

        private void Resetear()
        {
            txtDNI.Clear();
            cmbModulo.SelectedIndex = 0;
            cmbCriticidad.SelectedIndex = 0;
            dtpFechaDesde.Checked = false;
            dtpFechaHasta.Checked = false;
            dtpFechaDesde.Value = DateTime.Today;
            dtpFechaHasta.Value = DateTime.Today;
            _filtroAplicado = false;
            ActualizarBotonCancelar();
            CargarGrilla();
            txtDetNumero.Clear();
            txtDetFecha.Clear();
            txtDetUsuario.Clear();
            txtDetDNI.Clear();
            txtDetModulo.Clear();
            txtDetCriticidad.Clear();
            txtDetDescripcion.Clear();
        }

        private void AjustarBotonesSegunPerfil()
        {
            var usuario = SessionManager486LP.ObtenerInstancia().UsuarioActual();
            if (usuario == null) return;

            List<string> permisos = new BLL_Perfil486LP().ObtenerPermisosPorRol(usuario.Rol);

            btnAplicar.Enabled = permisos.Contains("BITACORA_APLICAR");
            btnLimpiar.Enabled = permisos.Contains("BITACORA_LIMPIAR");
            btnCancelar.Enabled = permisos.Contains("BITACORA_CANCELAR");
            btnExportarPDF.Enabled = permisos.Contains("BITACORA_EXPORTAR");
        }

        private void dgtBitacoraEvento_SelectionChanged(object sender, EventArgs e)
        {
            if (dgtBitacoraEvento.CurrentRow == null)
            { 
                return; 
            }


            BitacoraEvento486LP ev = dgtBitacoraEvento.CurrentRow.DataBoundItem as BitacoraEvento486LP;
            if (ev == null) 
            { 
                return; 
            }

            txtDetNumero.Text = ev.Numero.ToString();
            txtDetFecha.Text = ev.Fecha.ToString("dd/MM/yyyy HH:mm:ss");
            txtDetUsuario.Text = !string.IsNullOrEmpty(ev.Nombre) ? $"{ev.Nombre} {ev.Apellido}" : ev.NombreUsuario;
            txtDetDNI.Text = ev.DNI;
            txtDetModulo.Text = ev.Modulo;
            txtDetDescripcion.Text = ev.Descripcion;
            txtDetCriticidad.Text = $"{ev.Criticidad} - {Criticidad486LP.ATexto(ev.Criticidad)}";
        }

        private void btnExportarPDF_Click(object sender, EventArgs e)
        {
            var lm = Program.LanguageManager;
            string f = "FrmBitacoraEvento486LP";  

            if (dgtBitacoraEvento.Rows.Count == 0)
            {
                MessageBox.Show(lm.ObtenerTexto(f, "Msg.SinRegistrosPDF", "No hay registros para exportar."),
                    lm.ObtenerTexto(f, "Msg.SinRegistrosPDF.Title", "Exportar PDF"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Title = "Guardar reporte de bitácora";
                sfd.Filter = "Archivo PDF (*.pdf)|*.pdf";
                sfd.FileName = $"Bitacora_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";

                if (sfd.ShowDialog() != DialogResult.OK) return;

                try
                {
                    Document doc = new Document(PageSize.A4.Rotate(), 20f, 20f, 30f, 20f);
                    PdfWriter.GetInstance(doc, new FileStream(sfd.FileName, FileMode.Create));
                    doc.Open();

                    PdfFont fuenteTitulo = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 14f, BaseColor.WHITE);
                    PdfFont fuenteSubtitulo = FontFactory.GetFont(FontFactory.HELVETICA, 9f, new BaseColor(80, 80, 80));
                    PdfFont fuenteHeader = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 8f, BaseColor.WHITE);
                    PdfFont fuenteDato = FontFactory.GetFont(FontFactory.HELVETICA, 7f, BaseColor.BLACK);

                    PdfPTable encabezado = new PdfPTable(1) { WidthPercentage = 100 };
                    encabezado.AddCell(new PdfPCell(new Phrase("SneakRush — Bitácora de Eventos", fuenteTitulo))
                    {
                        BackgroundColor = new BaseColor(33, 90, 160),
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        Padding = 8f, Border = PdfRectangle.NO_BORDER
                    });

                    string usuario = SessionManager486LP.ObtenerInstancia().UsuarioActual().NombreUsuario;
                    string subtexto = $"Generado: {DateTime.Now:dd/MM/yyyy HH:mm} | Usuario: {usuario} | Registros: {dgtBitacoraEvento.Rows.Count}";
                    encabezado.AddCell(new PdfPCell(new Phrase(subtexto, fuenteSubtitulo))
                    {
                        BackgroundColor = new BaseColor(220, 230, 245),
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        Padding = 5f, Border = PdfRectangle.NO_BORDER
                    });
                    doc.Add(encabezado);
                    doc.Add(new Paragraph(" "));

                    float[] anchos = { 4f, 13f, 11f, 9f, 10f, 12f, 41f };
                    PdfPTable tabla = new PdfPTable(anchos.Length) { WidthPercentage = 100, SpacingBefore = 4f };
                    tabla.SetWidths(anchos);

                    BaseColor colorMuyAlta = new BaseColor(220, 53,  69);
                    BaseColor colorAlta = new BaseColor(255, 140,  0);
                    BaseColor colorMedia = new BaseColor(255, 193,  7);
                    BaseColor colorBaja = new BaseColor(40,  167, 69);
                    BaseColor colorMuyBaja = new BaseColor(108, 117, 125);
                    BaseColor bgClaro = new BaseColor(245, 248, 255);

                    string[] headers = { "N°", "Fecha y Hora", "Módulo", "Criticidad", "DNI", "Usuario", "Descripción" };
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

                    foreach (DataGridViewRow fila in dgtBitacoraEvento.Rows)
                    {
                        BaseColor bgFila = filaAlterna ? bgClaro : BaseColor.WHITE;
                        filaAlterna = !filaAlterna;

                        string numero = fila.Cells["colNumero"].Value?.ToString() ?? "";
                        string fecha = fila.Cells["colFecha"].Value?.ToString() ?? "";
                        string modulo = fila.Cells["colModulo"].Value?.ToString() ?? "";
                        string dni = fila.Cells["colDNI"].Value?.ToString() ?? "";
                        string nombreUsr = fila.Cells["colNombreUsuario"].Value?.ToString() ?? "";
                        string descripcion = fila.Cells["colDescripcion"].Value?.ToString() ?? "";

                        // Criticidad es int y se convierte a texto para el PDF 
                        int critValor = 0;
                        int.TryParse(fila.Cells["colCriticidad"].Value?.ToString(), out critValor);
                        string critTexto = Criticidad486LP.ATexto(critValor);

                        BaseColor bgCrit;
                        BaseColor fuenteCritColor;

                        if (critValor == Criticidad486LP.MuyAlta)
                        {
                            { bgCrit = colorMuyAlta; fuenteCritColor = BaseColor.WHITE; }
                        }
                        else if (critValor == Criticidad486LP.Alta) 
                        {
                            { bgCrit = colorAlta; fuenteCritColor = BaseColor.WHITE; }
                        }
                        else if (critValor == Criticidad486LP.Media) 
                        {
                            { bgCrit = colorMedia; fuenteCritColor = BaseColor.BLACK; }
                        }
                        else if (critValor == Criticidad486LP.Baja) 
                        {
                            { bgCrit = colorBaja; fuenteCritColor = BaseColor.WHITE; }
                        }
                        else 
                        {
                            { bgCrit = colorMuyBaja; fuenteCritColor = BaseColor.WHITE; }
                        }                                        

                        PdfFont fuenteCrit = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 7f, fuenteCritColor);

                        AgregarCelda(tabla, numero, fuenteDato, bgFila, Element.ALIGN_CENTER);
                        AgregarCelda(tabla, fecha, fuenteDato, bgFila, Element.ALIGN_CENTER);
                        AgregarCelda(tabla, modulo, fuenteDato, bgFila, Element.ALIGN_LEFT);
                        AgregarCelda(tabla, critTexto, fuenteCrit, bgCrit, Element.ALIGN_CENTER);
                        AgregarCelda(tabla, dni, fuenteDato, bgFila, Element.ALIGN_CENTER);
                        AgregarCelda(tabla, nombreUsr, fuenteDato, bgFila, Element.ALIGN_LEFT);
                        AgregarCelda(tabla, descripcion, fuenteDato, bgFila, Element.ALIGN_LEFT);
                    }

                    doc.Add(tabla);
                    doc.Add(new Paragraph(" "));

                    Paragraph pie = new Paragraph("SneakRush 2026 — Universidad Abierta Interamericana — Ingeniería de Software",FontFactory.GetFont(FontFactory.HELVETICA, 7f, new BaseColor(150, 150, 150)));
                    pie.Alignment = Element.ALIGN_CENTER;
                    doc.Add(pie);
                    doc.Close();

                    MessageBox.Show(string.Format(lm.ObtenerTexto(f, "Msg.PDFExportado", "PDF exportado correctamente:\n{0}"), sfd.FileName),lm.ObtenerTexto(f, "Msg.PDFExportado.Title", "Exportar PDF"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                    System.Diagnostics.Process.Start(sfd.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(string.Format(lm.ObtenerTexto(f, "Msg.ErrorPDF", "Error al exportar PDF:\n{0}"), ex.Message),
                        lm.ObtenerTexto(f, "Msg.ErrorPDF.Title", "Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            Resetear();
        }

        public void ActualizarIdioma()
        {
            var lm = Program.LanguageManager;
            string f = "FrmBitacoraEvento486LP";

            this.Text = lm.ObtenerTexto(f, "Title");
            label1.Text = lm.ObtenerTexto(f, "Title");

            // filtros
            label2.Text = lm.ObtenerTexto(f, "lblDNI");
            label3.Text = lm.ObtenerTexto(f, "lblModulo");
            label6.Text = lm.ObtenerTexto(f, "lblFechaDesde");
            label4.Text = lm.ObtenerTexto(f, "lblFechaHasta");
            label5.Text = lm.ObtenerTexto(f, "lblCriticidad");

            // panel de detalle
            grpDetalle.Text = lm.ObtenerTexto(f, "grpDetalle");
            label7.Text = lm.ObtenerTexto(f, "lblDetNumero");
            label8.Text = lm.ObtenerTexto(f, "lblDetFecha");
            label9.Text = lm.ObtenerTexto(f, "lblDetUsuario");
            label10.Text = lm.ObtenerTexto(f, "lblDetDNI");
            label11.Text = lm.ObtenerTexto(f, "lblDetModulo");
            label12.Text = lm.ObtenerTexto(f, "lblDetCriticidad");
            label13.Text = lm.ObtenerTexto(f, "lblDetDescripcion");

            // botones
            btnAplicar.Text = lm.ObtenerTexto(f, "btnAplicar");
            btnLimpiar.Text = lm.ObtenerTexto(f, "btnLimpiar");
            btnExportarPDF.Text = lm.ObtenerTexto(f, "btnExportarPDF");

            TraducirColumnas();
            ActualizarBotonCancelar();
            ActualizarLabelTotal();
            CargarCombos();
        }

        private void TraducirColumnas()
        {
            var lm = Program.LanguageManager;
            string f = "FrmBitacoraEvento486LP";

            if (dgtBitacoraEvento.Columns.Contains("colNumero"))
                dgtBitacoraEvento.Columns["colNumero"].HeaderText = lm.ObtenerTexto(f, "col.Numero");
            if (dgtBitacoraEvento.Columns.Contains("colFecha"))
                dgtBitacoraEvento.Columns["colFecha"].HeaderText = lm.ObtenerTexto(f, "col.FechaHora");
            if (dgtBitacoraEvento.Columns.Contains("colNombreUsuario"))
                dgtBitacoraEvento.Columns["colNombreUsuario"].HeaderText = lm.ObtenerTexto(f, "col.Usuario");
            if (dgtBitacoraEvento.Columns.Contains("colDNI"))
                dgtBitacoraEvento.Columns["colDNI"].HeaderText = lm.ObtenerTexto(f, "col.DNI");
            if (dgtBitacoraEvento.Columns.Contains("colModulo"))
                dgtBitacoraEvento.Columns["colModulo"].HeaderText = lm.ObtenerTexto(f, "col.Modulo");
            if (dgtBitacoraEvento.Columns.Contains("colDescripcion"))
                dgtBitacoraEvento.Columns["colDescripcion"].HeaderText = lm.ObtenerTexto(f, "col.Descripcion");
            if (dgtBitacoraEvento.Columns.Contains("colCriticidad"))
                dgtBitacoraEvento.Columns["colCriticidad"].HeaderText = lm.ObtenerTexto(f, "col.Criticidad");
        }

        private void ActualizarBotonCancelar()
        {
            var lm = Program.LanguageManager;
            string f = "FrmBitacoraEvento486LP";
            btnCancelar.Text = _filtroAplicado
                ? lm.ObtenerTexto(f, "btnCancelar", "Cancelar")
                : lm.ObtenerTexto(f, "btnSalir", "Salir");
        }

        private void ActualizarLabelTotal()
        {
            var lm = Program.LanguageManager;
            string f = "FrmBitacoraEvento486LP";
            lblTotal.Text = string.Format(lm.ObtenerTexto(f, "lbl.Total", "Registros: {0}"), _ultimoTotal);
        }

        private void FrmBitacoraEvento486LP_FormClosing(object sender, FormClosingEventArgs e)
        {
            Program.LanguageManager.Quitar(this);
        }
    }
    
}

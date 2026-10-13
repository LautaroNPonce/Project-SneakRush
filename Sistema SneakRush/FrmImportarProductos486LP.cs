using BE;
using BLL;
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

namespace Sistema_SneakRush
{
    // Dialogo que se abre con el boton "Importar XML" de Procesar productos (CUN10)
    public partial class FrmImportarProductos486LP : Form, IObserver486LP
    {
        private BLL_ImportacionProducto486LP _bllImportacion = new BLL_ImportacionProducto486LP();
        private readonly string f = "FrmImportarProductos486LP";

        // Resultado de la ultima importacion: de aca sale lo que se exporta como "rechazadas"
        private ResultadoImportacion486LP _resultado;

        // Patentes granulares por boton (ademas de la patente de acceso al dialogo, MAESTRO_PRODUCTOS_IMPORTAR).
        private bool _puedeExaminar;
        private bool _puedeEjecutar;
        private bool _puedeExportar;
        private bool _puedeCerrar;

        public FrmImportarProductos486LP()
        {
            InitializeComponent();
            Program.LanguageManager.Agregar(this);
            this.FormClosing += FrmImportarProductos486LP_FormClosing;
        }

        private void FrmImportarProductos486LP_Load(object sender, EventArgs e)
        {
            // Verificacion de acceso al dialogo (mismo patron que los otros forms de Maestro).
            if (!TienePatenteAcceso())
            {
                var lm = Program.LanguageManager;
                MessageBox.Show(
                    lm.ObtenerTexto(f, "Frm.ImportarProductos.Msg.SinPermiso", "No tiene permiso para importar productos desde un XML."),
                    lm.ObtenerTexto(f, "Frm.ImportarProductos.Msg.SinPermiso.Title", "Acceso denegado"),
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);

                this.BeginInvoke(new Action(() =>
                {
                    this.DialogResult = DialogResult.Cancel;
                    this.Close();
                }));
                return;
            }

            // El cuadro del archivo es de solo lectura (se completa con Examinar); el largo maximo es el de una ruta de Windows
            txtArchivo.MaxLength = 260;

            CargarProveedores();
            ConfigurarColumnas();
            AjustarBotonesSegunPerfil();
            ActualizarIdioma();
            ActualizarBotones();
        }

        // Verifica que el usuario en sesion tenga la patente de acceso al dialogo.
        private bool TienePatenteAcceso()
        {
            var usuario = SessionManager486LP.ObtenerInstancia().UsuarioActual();
            if (usuario == null) return false;

            List<string> permisos = new BLL_Perfil486LP().ObtenerPermisosPorRol(usuario.Rol);
            return permisos.Contains("MAESTRO_PRODUCTOS_IMPORTAR");
        }

        // Patentes granulares por boton.
        private void AjustarBotonesSegunPerfil()
        {
            var usuario = SessionManager486LP.ObtenerInstancia().UsuarioActual();
            if (usuario == null) return;

            BLL_Perfil486LP bllPerfil = new BLL_Perfil486LP();
            List<string> permisos = bllPerfil.ObtenerPermisosPorRol(usuario.Rol);

            _puedeExaminar = permisos.Contains("MAESTRO_PRODUCTOS_IMPORTAR_EXAMINAR");
            _puedeEjecutar = permisos.Contains("MAESTRO_PRODUCTOS_IMPORTAR_EJECUTAR");
            _puedeExportar = permisos.Contains("MAESTRO_PRODUCTOS_IMPORTAR_EXPORTAR");
            _puedeCerrar = permisos.Contains("MAESTRO_PRODUCTOS_IMPORTAR_CERRAR");
        }

        private void CargarProveedores()
        {
            cmbProveedor.DataSource = null;
            cmbProveedor.DisplayMember = "Nombre";
            cmbProveedor.ValueMember = "IdProveedor";
            cmbProveedor.DataSource = _bllImportacion.ListarProveedores();
            cmbProveedor.SelectedIndex = -1;
        }

        private void ConfigurarColumnas()
        {
            dgvLineas.AutoGenerateColumns = false;
            dgvLineas.Columns.Clear();

            dgvLineas.Columns.Add(new DataGridViewTextBoxColumn { Name = "colLinea", Width = 50 });
            dgvLineas.Columns.Add(new DataGridViewTextBoxColumn { Name = "colMarca", Width = 80 });
            dgvLineas.Columns.Add(new DataGridViewTextBoxColumn { Name = "colModelo", Width = 130 });
            dgvLineas.Columns.Add(new DataGridViewTextBoxColumn { Name = "colColor", Width = 80 });
            dgvLineas.Columns.Add(new DataGridViewTextBoxColumn { Name = "colTalle", Width = 50 });
            dgvLineas.Columns.Add(new DataGridViewTextBoxColumn { Name = "colCantidad", Width = 65 });
            dgvLineas.Columns.Add(new DataGridViewTextBoxColumn { Name = "colResultado", Width = 100 });
            dgvLineas.Columns.Add(new DataGridViewTextBoxColumn { Name = "colDetalle", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });

            AplicarEncabezadosColumnas();
        }

        private void AplicarEncabezadosColumnas()
        {
            if (dgvLineas.Columns.Count == 0) return;

            var lm = Program.LanguageManager;
            dgvLineas.Columns["colLinea"].HeaderText = lm.ObtenerTexto(f, "Frm.ImportarProductos.Col.Linea", "Línea");
            dgvLineas.Columns["colMarca"].HeaderText = lm.ObtenerTexto(f, "Frm.ImportarProductos.Col.Marca", "Marca");
            dgvLineas.Columns["colModelo"].HeaderText = lm.ObtenerTexto(f, "Frm.ImportarProductos.Col.Modelo", "Modelo");
            dgvLineas.Columns["colColor"].HeaderText = lm.ObtenerTexto(f, "Frm.ImportarProductos.Col.Color", "Color");
            dgvLineas.Columns["colTalle"].HeaderText = lm.ObtenerTexto(f, "Frm.ImportarProductos.Col.Talle", "Talle");
            dgvLineas.Columns["colCantidad"].HeaderText = lm.ObtenerTexto(f, "Frm.ImportarProductos.Col.Cantidad", "Cantidad");
            dgvLineas.Columns["colResultado"].HeaderText = lm.ObtenerTexto(f, "Frm.ImportarProductos.Col.Resultado", "Resultado");
            dgvLineas.Columns["colDetalle"].HeaderText = lm.ObtenerTexto(f, "Frm.ImportarProductos.Col.Detalle", "Detalle");
        }

        private void btnExaminar_Click(object sender, EventArgs e)
        {
            var lm = Program.LanguageManager;

            using (OpenFileDialog dialogo = new OpenFileDialog())
            {
                dialogo.Title = lm.ObtenerTexto(f, "Frm.ImportarProductos.Examinar.Title", "Seleccione el XML del remito");
                dialogo.Filter = lm.ObtenerTexto(f, "Frm.ImportarProductos.Filtro", "Archivos XML (*.xml)|*.xml");
                dialogo.CheckFileExists = true;
                dialogo.Multiselect = false;

                if (dialogo.ShowDialog(this) == DialogResult.OK)
                    txtArchivo.Text = dialogo.FileName;
            }
        }

        private void btnImportar_Click(object sender, EventArgs e)
        {
            var lm = Program.LanguageManager;

            if (!ValidarEntrada()) return;

            int idProveedor = (int)cmbProveedor.SelectedValue;
            ResultadoImportacion486LP resultado;
            string mensaje;
            bool ok;

            // Mientras importa no se puede volver a tocar el boton (evita mandar el mismo archivo dos veces seguidas)
            btnImportar.Enabled = false;
            Cursor = Cursors.WaitCursor;
            try
            {
                ok = _bllImportacion.Importar(idProveedor, txtArchivo.Text.Trim(), out resultado, out mensaje);
            }
            finally
            {
                Cursor = Cursors.Default;
                btnImportar.Enabled = _puedeEjecutar;
            }

            // Archivo rechazado completo (no se importo nada)
            if (!ok)
            {
                MostrarAviso(mensaje);
                return;
            }

            _resultado = resultado;
            MostrarResultado();
            txtArchivo.Clear();
            ActualizarBotones();

            string texto = mensaje;
            if (resultado.Rechazadas > 0)
                texto += Environment.NewLine + Environment.NewLine + lm.ObtenerTexto(f, "Frm.ImportarProductos.Msg.HayRechazadas",
                    "Revise la grilla: cada línea rechazada indica el motivo. Puede exportarlas a un XML, corregirlas y volver a importarlo; las líneas ya importadas no se repiten.");

            MessageBox.Show(texto, lm.ObtenerTexto(f, "Frm.ImportarProductos.Msg.Title", "Importar productos"), MessageBoxButtons.OK,
                resultado.Rechazadas > 0 ? MessageBoxIcon.Warning : MessageBoxIcon.Information);
        }

        // Valida solo lo que corresponde a la GUI (proveedor elegido y archivo seleccionado). Las reglas del archivo y de cada linea las hace la BLL
        private bool ValidarEntrada()
        {
            var lm = Program.LanguageManager;

            if (cmbProveedor.SelectedIndex < 0 || cmbProveedor.SelectedValue == null)
            {
                MostrarAviso(lm.ObtenerTexto(f, "Frm.ImportarProductos.Msg.ProveedorObligatorio", "Seleccione el proveedor del remito."));
                cmbProveedor.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtArchivo.Text))
            {
                MostrarAviso(lm.ObtenerTexto(f, "Frm.ImportarProductos.Msg.ArchivoObligatorio", "Seleccione el archivo XML del remito con el botón Examinar."));
                btnExaminar.Focus();
                return false;
            }
            return true;
        }

        private void btnExportar_Click(object sender, EventArgs e)
        {
            var lm = Program.LanguageManager;
            if (_resultado == null) return;

            string nombreSugerido = "remito_" + _resultado.NroRemito + "_rechazadas.xml";
            foreach (char c in Path.GetInvalidFileNameChars())
                nombreSugerido = nombreSugerido.Replace(c, '_');

            using (SaveFileDialog dialogo = new SaveFileDialog())
            {
                dialogo.Title = lm.ObtenerTexto(f, "Frm.ImportarProductos.Exportar.Title", "Guardar las líneas rechazadas");
                dialogo.Filter = lm.ObtenerTexto(f, "Frm.ImportarProductos.Filtro", "Archivos XML (*.xml)|*.xml");
                dialogo.FileName = nombreSugerido;
                dialogo.OverwritePrompt = true;

                if (dialogo.ShowDialog(this) != DialogResult.OK) return;

                string mensaje;
                bool ok = _bllImportacion.ExportarRechazadas(_resultado, dialogo.FileName, out mensaje);
                if (ok)
                    MessageBox.Show(mensaje, lm.ObtenerTexto(f, "Frm.ImportarProductos.Msg.Title", "Importar productos"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                else
                    MostrarAviso(mensaje);
            }
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // Vuelca el reporte en la grilla (una fila por linea del XML) y arma el resumen
        private void MostrarResultado()
        {
            var lm = Program.LanguageManager;

            dgvLineas.Rows.Clear();
            if (_resultado == null)
            {
                lblResumen.Text = "";
                return;
            }

            foreach (LineaImportacion486LP l in _resultado.Lineas)
            {
                int i = dgvLineas.Rows.Add(l.Linea, l.Marca, l.Modelo, l.Color, l.Talle, l.Cantidad, TextoResultado(l.Resultado), l.Detalle);
                dgvLineas.Rows[i].DefaultCellStyle.BackColor = ColorResultado(l.Resultado);
            }
            dgvLineas.ClearSelection();

            string resumen = string.Format(lm.ObtenerTexto(f, "Frm.ImportarProductos.Resumen",
                "Productos nuevos: {0}. Stock sumado: {1}. Líneas rechazadas: {2}. Ya importadas antes: {3}."),
                _resultado.Altas, _resultado.Sumas, _resultado.Rechazadas, _resultado.YaImportadas);

            string estado = _resultado.EstadoRemito == ImportacionProducto486LP.ESTADO_COMPLETO
                ? string.Format(lm.ObtenerTexto(f, "Frm.ImportarProductos.Estado.Completo", "Remito {0}: completo. Ya no se puede volver a importar."), _resultado.NroRemito)
                : string.Format(lm.ObtenerTexto(f, "Frm.ImportarProductos.Estado.ConRechazos", "Remito {0}: quedan líneas rechazadas. Corríjalas y vuelva a importar el archivo."), _resultado.NroRemito);

            lblResumen.Text = resumen + Environment.NewLine + estado;
        }

        private string TextoResultado(string resultado)
        {
            var lm = Program.LanguageManager;
            switch (resultado)
            {
                case LineaImportacion486LP.RESULTADO_ALTA: return lm.ObtenerTexto(f, "Frm.ImportarProductos.Res.Alta", "Producto nuevo");
                case LineaImportacion486LP.RESULTADO_SUMA_STOCK: return lm.ObtenerTexto(f, "Frm.ImportarProductos.Res.SumaStock", "Stock sumado");
                case LineaImportacion486LP.RESULTADO_YA_IMPORTADA: return lm.ObtenerTexto(f, "Frm.ImportarProductos.Res.YaImportada", "Ya importada");
                default: return lm.ObtenerTexto(f, "Frm.ImportarProductos.Res.Rechazada", "Rechazada");
            }
        }

        private Color ColorResultado(string resultado)
        {
            switch (resultado)
            {
                case LineaImportacion486LP.RESULTADO_ALTA: return Color.FromArgb(226, 245, 230);
                case LineaImportacion486LP.RESULTADO_SUMA_STOCK: return Color.FromArgb(226, 236, 250);
                case LineaImportacion486LP.RESULTADO_YA_IMPORTADA: return Color.FromArgb(238, 238, 238);
                default: return Color.FromArgb(253, 226, 226);
            }
        }

        // Cada boton depende de su patente. Exportar ademas solo tiene sentido si la ultima importacion dejo lineas rechazadas que se puedan corregir
        private void ActualizarBotones()
        {
            btnExaminar.Enabled = _puedeExaminar;
            btnImportar.Enabled = _puedeEjecutar;
            btnCerrar.Enabled = _puedeCerrar;
            btnExportar.Enabled = _puedeExportar && _resultado != null &&
                _resultado.Lineas.Any(l => l.Resultado == LineaImportacion486LP.RESULTADO_RECHAZADA && l.Exportable);
        }

        private void MostrarAviso(string texto)
        {
            var lm = Program.LanguageManager;
            MessageBox.Show(texto, lm.ObtenerTexto(f, "Frm.ImportarProductos.Msg.Title", "Importar productos"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        public void ActualizarIdioma()
        {
            var lm = Program.LanguageManager;

            this.Text = lm.ObtenerTexto(f, "Frm.ImportarProductos.Titulo", "Importar productos desde XML");
            lblTitulo.Text = lm.ObtenerTexto(f, "Frm.ImportarProductos.Titulo", "Importar productos desde XML");
            grpOrigen.Text = lm.ObtenerTexto(f, "Frm.ImportarProductos.Origen", "Remito del proveedor");
            lblProveedor.Text = lm.ObtenerTexto(f, "Frm.ImportarProductos.Proveedor", "Proveedor:");
            lblArchivo.Text = lm.ObtenerTexto(f, "Frm.ImportarProductos.Archivo", "Archivo XML:");
            lblNota.Text = lm.ObtenerTexto(f, "Frm.ImportarProductos.Nota",
                "Cada línea suma stock si el producto ya existe (no cambia el precio) o lo da de alta si es nuevo. Las líneas con errores se informan y no se importan.");
            btnExaminar.Text = lm.ObtenerTexto(f, "Frm.ImportarProductos.Examinar", "Examinar...");
            btnImportar.Text = lm.ObtenerTexto(f, "Frm.ImportarProductos.Importar", "Importar");
            lblDetalle.Text = lm.ObtenerTexto(f, "Frm.ImportarProductos.Reporte", "Detalle de la importación:");
            btnExportar.Text = lm.ObtenerTexto(f, "Frm.ImportarProductos.Exportar", "Exportar rechazadas");
            btnCerrar.Text = lm.ObtenerTexto(f, "Frm.ImportarProductos.Cerrar", "Cerrar");

            AplicarEncabezadosColumnas();
            MostrarResultado();
        }

        private void FrmImportarProductos486LP_FormClosing(object sender, FormClosingEventArgs e)
        {
            Program.LanguageManager.Quitar(this);
        }
    }
}

using BE;
using BLL;
using Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sistema_SneakRush
{
    public partial class FrmHistorialProductos486LP : Form, IObserver486LP, IRefrescable486LP
    {
        private BLL_Producto486LP _bllProducto = new BLL_Producto486LP();
        private BLL_Marca486LP _bllMarca = new BLL_Marca486LP();
        private readonly string f = "FrmHistorialProductos486LP";
        private bool _puedeAplicar;
        private bool _puedeLimpiar;
        private bool _puedeRestaurar;
        private bool _puedeCerrar;
        private bool _cargado;

        private static readonly Color ColorVigente = Color.FromArgb(212, 237, 218);
        private static readonly Color ColorBaja = Color.FromArgb(248, 215, 218);

        public FrmHistorialProductos486LP()
        {
            InitializeComponent();
            Ayuda486LP.Vincular(this, "historial-productos");
            EstiloBotonHabilitacion(btnAplicar);
            EstiloBotonHabilitacion(btnLimpiar);
            EstiloBotonHabilitacion(btnRestaurar);
            EstiloBotonHabilitacion(btnCerrar);
            Program.LanguageManager.Agregar(this);
            this.FormClosing += FrmHistorialProductos486LP_FormClosing;
        }

        // Un boton sin patente se ve gris y apagado, igual que en el dialogo de importar productos
        private static void EstiloBotonHabilitacion(Button boton)
        {
            Color colorActivo = boton.BackColor;
            Color colorInactivo = Color.FromArgb(222, 224, 228);

            boton.EnabledChanged += (s, e) =>
            {
                boton.BackColor = boton.Enabled ? colorActivo : colorInactivo;
                boton.Invalidate();
            };
            boton.Paint += (s, e) =>
            {
                if (boton.Enabled)
                {
                    return;
                }
                e.Graphics.Clear(colorInactivo);
                TextRenderer.DrawText(e.Graphics, boton.Text, boton.Font, boton.ClientRectangle, Color.FromArgb(95, 99, 108),
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
            };

            boton.BackColor = boton.Enabled ? colorActivo : colorInactivo;
        }

        private void FrmHistorialProductos486LP_Load(object sender, EventArgs e)
        {
            if (!TienePatenteAcceso())
            {
                var lm = Program.LanguageManager;
                MessageBox.Show(
                    lm.ObtenerTexto(f, "Frm.HistorialProductos.Msg.SinPermiso", "No tiene permiso para ver el historial de cambios de productos."),
                    lm.ObtenerTexto(f, "Frm.HistorialProductos.Msg.SinPermiso.Title", "Acceso denegado"),
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);

                this.BeginInvoke(new Action(() =>
                {
                    this.DialogResult = DialogResult.Cancel;
                    this.Close();
                }));
                return;
            }

            AjustarBotonesSegunPerfil();
            AplicarPermisosBotones();
            CargarMarcas();
            ConfigurarColumnas();
            ValoresIniciales();
            _cargado = true;
            ActualizarIdioma();
            Consultar();
        }

        // Si la ventana ya estaba abierta y se vuelve a entrar desde el menu, se recargan marcas y versiones
        public void Refrescar()
        {
            if (!_cargado) return;

            object marcaElegida = cmbMarca.SelectedValue;
            CargarMarcas();
            if (marcaElegida != null)
            {
                cmbMarca.SelectedValue = marcaElegida;
            }
            Consultar();
        }

        private bool TienePatenteAcceso()
        {
            var usuario = SessionManager486LP.ObtenerInstancia().UsuarioActual();
            if (usuario == null) return false;

            List<string> permisos = new BLL_Perfil486LP().ObtenerPermisosPorRol(usuario.Rol);
            return permisos.Contains("MAESTRO_PRODUCTOS_HISTORIAL");
        }

        private void AjustarBotonesSegunPerfil()
        {
            var usuario = SessionManager486LP.ObtenerInstancia().UsuarioActual();
            if (usuario == null) return;

            List<string> permisos = new BLL_Perfil486LP().ObtenerPermisosPorRol(usuario.Rol);

            _puedeAplicar = permisos.Contains("MAESTRO_PRODUCTOS_HISTORIAL_APLICAR");
            _puedeLimpiar = permisos.Contains("MAESTRO_PRODUCTOS_HISTORIAL_LIMPIAR");
            _puedeRestaurar = permisos.Contains("MAESTRO_PRODUCTOS_HISTORIAL_RESTAURAR");
            _puedeCerrar = permisos.Contains("MAESTRO_PRODUCTOS_HISTORIAL_CERRAR");
        }

        private void AplicarPermisosBotones()
        {
            btnAplicar.Enabled = _puedeAplicar;
            btnLimpiar.Enabled = _puedeLimpiar;
            btnRestaurar.Enabled = _puedeRestaurar;
            btnCerrar.Enabled = _puedeCerrar;
        }

        private void CargarMarcas()
        {
            cmbMarca.DataSource = null;
            cmbMarca.DisplayMember = "Nombre";
            cmbMarca.ValueMember = "IdMarca";
            cmbMarca.DataSource = _bllMarca.Listar();
            cmbMarca.SelectedIndex = -1;
        }

        private void ValoresIniciales()
        {
            cmbMarca.SelectedIndex = -1;
            txtModelo.Clear();
            txtColor.Clear();
            txtTalle.Clear();
            dtpHasta.Value = DateTime.Today;
            dtpDesde.Value = DateTime.Today.AddMonths(-1);
        }

        private void ConfigurarColumnas()
        {
            dgvCambios.AutoGenerateColumns = false;
            dgvCambios.Columns.Clear();
            dgvCambios.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            dgvCambios.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "FechaHora", Name = "colFecha", Width = 85, DefaultCellStyle = new DataGridViewCellStyle { Format = "dd/MM/yyyy" } });
            dgvCambios.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "FechaHora", Name = "colHora", Width = 70, DefaultCellStyle = new DataGridViewCellStyle { Format = "HH:mm:ss" } });
            dgvCambios.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Operacion", Name = "colOperacion", Width = 90 });
            dgvCambios.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Marca", Name = "colMarca", Width = 85 });
            dgvCambios.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Categoria", Name = "colCategoria", Width = 95 });
            dgvCambios.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Modelo", Name = "colModelo", Width = 120 });
            dgvCambios.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Color", Name = "colColor", Width = 70 });
            dgvCambios.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Talle", Name = "colTalle", Width = 70 });
            dgvCambios.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Precio", Name = "colPrecio", Width = 95, DefaultCellStyle = new DataGridViewCellStyle { Format = "C2" } });
            dgvCambios.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Stock", Name = "colStock", Width = 70 });
            dgvCambios.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Vigente", Name = "colVigente", Width = 60 });

            dgvCambios.CellFormatting += dgvCambios_CellFormatting;
            dgvCambios.DataBindingComplete += dgvCambios_DataBindingComplete;

            AplicarEncabezadosColumnas();
        }

        private void AplicarEncabezadosColumnas()
        {
            if (dgvCambios.Columns.Count == 0) return;

            var lm = Program.LanguageManager;
            dgvCambios.Columns["colFecha"].HeaderText = lm.ObtenerTexto(f, "Frm.HistorialProductos.Col.Fecha", "Fecha");
            dgvCambios.Columns["colHora"].HeaderText = lm.ObtenerTexto(f, "Frm.HistorialProductos.Col.Hora", "Hora");
            dgvCambios.Columns["colOperacion"].HeaderText = lm.ObtenerTexto(f, "Frm.HistorialProductos.Col.Operacion", "Operación");
            dgvCambios.Columns["colMarca"].HeaderText = lm.ObtenerTexto(f, "Frm.HistorialProductos.Col.Marca", "Marca");
            dgvCambios.Columns["colCategoria"].HeaderText = lm.ObtenerTexto(f, "Frm.HistorialProductos.Col.Categoria", "Categoría");
            dgvCambios.Columns["colModelo"].HeaderText = lm.ObtenerTexto(f, "Frm.HistorialProductos.Col.Modelo", "Modelo");
            dgvCambios.Columns["colColor"].HeaderText = lm.ObtenerTexto(f, "Frm.HistorialProductos.Col.Color", "Color");
            dgvCambios.Columns["colTalle"].HeaderText = lm.ObtenerTexto(f, "Frm.HistorialProductos.Col.Talle", "Talle");
            dgvCambios.Columns["colPrecio"].HeaderText = lm.ObtenerTexto(f, "Frm.HistorialProductos.Col.Precio", "Precio");
            dgvCambios.Columns["colStock"].HeaderText = lm.ObtenerTexto(f, "Frm.HistorialProductos.Col.Stock", "Stock");
            dgvCambios.Columns["colVigente"].HeaderText = lm.ObtenerTexto(f, "Frm.HistorialProductos.Col.Vigente", "Vigente");
        }

        // La operacion y el vigente se guardan como codigo / bool y se muestran con su texto en el idioma activo
        private void dgvCambios_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || e.Value == null) return;

            var lm = Program.LanguageManager;
            string columna = dgvCambios.Columns[e.ColumnIndex].Name;

            if (columna == "colOperacion")
            {
                e.Value = TextoOperacion(e.Value.ToString());
                e.FormattingApplied = true;
            }
            else if (columna == "colVigente")
            {
                bool vigente = Convert.ToBoolean(e.Value);
                e.Value = vigente ? lm.ObtenerTexto(f, "Frm.HistorialProductos.Si", "Sí") : lm.ObtenerTexto(f, "Frm.HistorialProductos.No", "No");
                e.FormattingApplied = true;
            }
        }

        private string TextoOperacion(string codigo)
        {
            var lm = Program.LanguageManager;
            switch (codigo)
            {
                case ProductoCambio486LP.OPERACION_INICIAL: return lm.ObtenerTexto(f, "Frm.HistorialProductos.Op.Inicial", "Inicial");
                case ProductoCambio486LP.OPERACION_ALTA: return lm.ObtenerTexto(f, "Frm.HistorialProductos.Op.Alta", "Alta");
                case ProductoCambio486LP.OPERACION_MODIFICACION: return lm.ObtenerTexto(f, "Frm.HistorialProductos.Op.Modificacion", "Modificación");
                case ProductoCambio486LP.OPERACION_BAJA: return lm.ObtenerTexto(f, "Frm.HistorialProductos.Op.Baja", "Baja");
                default: return codigo;
            }
        }

        // Verde para la version vigente y rojo suave para las bajas
        private void dgvCambios_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            foreach (DataGridViewRow fila in dgvCambios.Rows)
            {
                ProductoCambio486LP cambio = fila.DataBoundItem as ProductoCambio486LP;
                if (cambio == null) continue;

                if (cambio.Operacion == ProductoCambio486LP.OPERACION_BAJA)
                    fila.DefaultCellStyle.BackColor = ColorBaja;
                else if (cambio.Vigente)
                    fila.DefaultCellStyle.BackColor = ColorVigente;
            }

            dgvCambios.ClearSelection();
        }

        private void Consultar()
        {
            var lm = Program.LanguageManager;

            string mensaje;
            if (!_bllProducto.ValidarFechasHistorial(dtpDesde.Value, dtpHasta.Value, out mensaje))
            {
                MostrarAviso(mensaje);
                return;
            }

            string marca = cmbMarca.SelectedIndex >= 0 ? cmbMarca.Text : null;
            List<ProductoCambio486LP> cambios = _bllProducto.ListarCambios(marca, txtModelo.Text, txtColor.Text, txtTalle.Text, dtpDesde.Value, dtpHasta.Value);

            dgvCambios.DataSource = null;
            dgvCambios.DataSource = cambios;
            lblCantidad.Text = string.Format(lm.ObtenerTexto(f, "Frm.HistorialProductos.Cantidad", "Versiones encontradas: {0}"), cambios.Count);
        }

        private ProductoCambio486LP CambioSeleccionado()
        {
            if (dgvCambios.SelectedRows.Count == 0) return null;
            return dgvCambios.SelectedRows[0].DataBoundItem as ProductoCambio486LP;
        }

        private void btnAplicar_Click(object sender, EventArgs e)
        {
            Consultar();
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            ValoresIniciales();
            Consultar();
        }

        private void btnRestaurar_Click(object sender, EventArgs e)
        {
            var lm = Program.LanguageManager;
            ProductoCambio486LP cambio = CambioSeleccionado();

            string mensaje;
            if (!_bllProducto.ValidarRestauracion(cambio, out mensaje))
            {
                MostrarAviso(mensaje);
                return;
            }

            // Restaurar pisa el estado actual (stock incluido) y puede afectar ventas y compras posteriores: se pide confirmacion con No como opcion por defecto
            DialogResult respuesta = MessageBox.Show(
                string.Format(lm.ObtenerTexto(f, "Frm.HistorialProductos.Msg.ConfirmarRestauracion",
                    "Va a restaurar el producto {0} a la versión del {1}.\n\nSe reemplaza todo el estado actual del producto con los datos de esa versión, incluido el stock, aunque después haya habido ventas o compras. Si el producto fue eliminado, se vuelve a crear.\n\nEsto puede afectar a las ventas y compras posteriores y queda bajo su responsabilidad.\n\n¿Desea continuar?"),
                    cambio, cambio.FechaHora.ToString("dd/MM/yyyy HH:mm:ss")),
                lm.ObtenerTexto(f, "Frm.HistorialProductos.Msg.Title", "Historial de cambios"),
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);

            if (respuesta != DialogResult.Yes) return;

            if (_bllProducto.RestaurarCambio(cambio, out mensaje))
            {
                MessageBox.Show(mensaje, lm.ObtenerTexto(f, "Frm.HistorialProductos.Msg.Title", "Historial de cambios"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                Consultar();
            }
            else
            {
                MostrarAviso(mensaje);
            }
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void MostrarAviso(string texto)
        {
            var lm = Program.LanguageManager;
            MessageBox.Show(texto, lm.ObtenerTexto(f, "Frm.HistorialProductos.Msg.Title", "Historial de cambios"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        public void ActualizarIdioma()
        {
            var lm = Program.LanguageManager;

            this.Text = lm.ObtenerTexto(f, "Frm.HistorialProductos.Titulo", "Historial de cambios de productos");
            lblTitulo.Text = lm.ObtenerTexto(f, "Frm.HistorialProductos.Titulo", "Historial de cambios de productos");
            grpFiltros.Text = lm.ObtenerTexto(f, "Frm.HistorialProductos.Filtros", "Filtros");
            lblMarca.Text = lm.ObtenerTexto(f, "Frm.HistorialProductos.Marca", "Marca:");
            lblModelo.Text = lm.ObtenerTexto(f, "Frm.HistorialProductos.Modelo", "Modelo:");
            lblColor.Text = lm.ObtenerTexto(f, "Frm.HistorialProductos.Color", "Color:");
            lblTalle.Text = lm.ObtenerTexto(f, "Frm.HistorialProductos.Talle", "Talle:");
            lblDesde.Text = lm.ObtenerTexto(f, "Frm.HistorialProductos.Desde", "Desde:");
            lblHasta.Text = lm.ObtenerTexto(f, "Frm.HistorialProductos.Hasta", "Hasta:");
            btnAplicar.Text = lm.ObtenerTexto(f, "Frm.HistorialProductos.Aplicar", "Aplicar");
            btnLimpiar.Text = lm.ObtenerTexto(f, "Frm.HistorialProductos.Limpiar", "Limpiar");
            btnRestaurar.Text = lm.ObtenerTexto(f, "Frm.HistorialProductos.Restaurar", "Restaurar");
            btnCerrar.Text = lm.ObtenerTexto(f, "Frm.HistorialProductos.Cerrar", "Cerrar");

            AplicarEncabezadosColumnas();

            if (_cargado)
            {
                dgvCambios.Invalidate();
                int cantidad = dgvCambios.Rows.Count;
                lblCantidad.Text = string.Format(lm.ObtenerTexto(f, "Frm.HistorialProductos.Cantidad", "Versiones encontradas: {0}"), cantidad);
            }
        }

        private void FrmHistorialProductos486LP_FormClosing(object sender, FormClosingEventArgs e)
        {
            Program.LanguageManager.Quitar(this);
        }
    }
}

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
    public partial class FrmGenerarSolicitud486LP : Form, IObserver486LP
    {
        private BLL_Producto486LP _bllProducto = new BLL_Producto486LP();
        private BLL_SolicitudCompra486LP _bllSolicitud = new BLL_SolicitudCompra486LP();
        private BLL_Perfil486LP _bllPerfil = new BLL_Perfil486LP();
        private readonly string f = "FrmGenerarSolicitud486LP";

        private const int UmbralStockBajo = 3; // Umbral para el resaltado de stock bajo (naranja). Stock = 0 siempre es rojo.

        private List<Producto486LP> _productosDisponibles = new List<Producto486LP>();
        private SolicitudCompra486LP _solicitudEnConstruccion = new SolicitudCompra486LP();
        private Producto486LP _productoSeleccionado;
        private bool _puedeAgregar;
        private bool _puedeQuitar;
        private bool _puedeConfirmar;
        private bool _puedeCancelar;

        public FrmGenerarSolicitud486LP()
        {
            InitializeComponent();
            this.Load += FrmGenerarSolicitud486LP_Load;
            Program.LanguageManager.Agregar(this);
            this.FormClosing += FrmGenerarSolicitud486LP_FormClosing;

            btnLimpiarFiltros.Click += btnLimpiarFiltros_Click;
            cmbMarca.SelectedIndexChanged += Filtro_SelectedIndexChanged;
            cmbTalle.SelectedIndexChanged += Filtro_SelectedIndexChanged;
            dgvCatalogo.SelectionChanged += dgvCatalogo_SelectionChanged;
            btnAgregar.Click += btnAgregar_Click;
            btnQuitar.Click += btnQuitar_Click;
            btnConfirmar.Click += btnConfirmar_Click;
            btnCancelar.Click += btnCancelar_Click;
        }

        private void FrmGenerarSolicitud486LP_Load(object sender, EventArgs e)
        {
            if (!TienePatenteAcceso())
            {
                var lm = Program.LanguageManager;
                MessageBox.Show(
                    lm.ObtenerTexto(f, "Frm.GenerarSolicitud.Msg.SinPermiso", "No tiene permiso para generar solicitudes de compra."),
                    lm.ObtenerTexto(f, "Frm.GenerarSolicitud.Msg.SinPermiso.Title", "Acceso denegado"),
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);

                this.BeginInvoke(new Action(() =>
                {
                    this.DialogResult = DialogResult.Cancel;
                    this.Close();
                }));
                return;
            }

            ConfigurarColumnas();
            AjustarBotonesSegunPerfil();
            AplicarPermisosBotones();
            CargarCatalogo();
            ActualizarIdioma();
        }

        private bool TienePatenteAcceso()
        {
            var usuario = SessionManager486LP.ObtenerInstancia().UsuarioActual();
            if (usuario == null) return false;

            List<string> permisos = _bllPerfil.ObtenerPermisosPorRol(usuario.Rol);
            return permisos.Contains("COMPRA_GENERAR_SOLICITUD");
        }

        private void AjustarBotonesSegunPerfil()
        {
            var usuario = SessionManager486LP.ObtenerInstancia().UsuarioActual();
            if (usuario == null) return;

            List<string> permisos = _bllPerfil.ObtenerPermisosPorRol(usuario.Rol);

            _puedeAgregar = permisos.Contains("SOLICITUD_AGREGAR");
            _puedeQuitar = permisos.Contains("SOLICITUD_QUITAR");
            _puedeConfirmar = permisos.Contains("SOLICITUD_CONFIRMAR");
            _puedeCancelar = permisos.Contains("SOLICITUD_CANCELAR");
        }

        private void AplicarPermisosBotones()
        {
            btnAgregar.Enabled = _puedeAgregar;
            btnQuitar.Enabled = _puedeQuitar;
            btnConfirmar.Enabled = _puedeConfirmar;
            btnCancelar.Enabled = _puedeCancelar;
        }

        private void CargarCatalogo()
        {
            _productosDisponibles = _bllProducto.Listar().OrderBy(p => p.Stock).ToList();
            CargarCombosFiltro();
            MostrarCatalogo(_productosDisponibles);
        }

        private void CargarCombosFiltro()
        {
            var lm = Program.LanguageManager;

            string marcaPrevia = cmbMarca.SelectedIndex > 0 ? cmbMarca.Text : null;
            string tallePrevio = cmbTalle.SelectedIndex > 0 ? cmbTalle.Text : null;

            cmbMarca.Items.Clear();
            cmbMarca.Items.Add(lm.ObtenerTexto(f, "Frm.GenerarSolicitud.Filtro.Todas", "(Todas)"));
            cmbMarca.Items.AddRange(_productosDisponibles.Select(p => p.Marca).Distinct().OrderBy(m => m).ToArray());
            cmbMarca.SelectedIndex = marcaPrevia != null && cmbMarca.Items.Contains(marcaPrevia) ? cmbMarca.Items.IndexOf(marcaPrevia) : 0;

            cmbTalle.Items.Clear();
            cmbTalle.Items.Add(lm.ObtenerTexto(f, "Frm.GenerarSolicitud.Filtro.Todos", "(Todos)"));
            cmbTalle.Items.AddRange(_productosDisponibles.Select(p => p.Talle).Distinct().OrderBy(t => t).ToArray());
            cmbTalle.SelectedIndex = tallePrevio != null && cmbTalle.Items.Contains(tallePrevio) ? cmbTalle.Items.IndexOf(tallePrevio) : 0;
        }

        private void Filtro_SelectedIndexChanged(object sender, EventArgs e)
        {
            AplicarFiltros();
        }

        private void AplicarFiltros()
        {
            string marca = cmbMarca.SelectedIndex > 0 ? cmbMarca.Text : "";
            string talle = cmbTalle.SelectedIndex > 0 ? cmbTalle.Text : "";

            var filtrados = _productosDisponibles.Where(p =>
                (string.IsNullOrEmpty(marca) || p.Marca == marca) &&
                (string.IsNullOrEmpty(talle) || p.Talle == talle)
            ).ToList();

            MostrarCatalogo(filtrados);
        }

        private void btnLimpiarFiltros_Click(object sender, EventArgs e)
        {
            cmbMarca.SelectedIndex = 0;
            cmbTalle.SelectedIndex = 0;
            MostrarCatalogo(_productosDisponibles);
        }

        private void MostrarCatalogo(List<Producto486LP> lista)
        {
            dgvCatalogo.AutoGenerateColumns = false;
            dgvCatalogo.DataSource = null;
            dgvCatalogo.DataSource = new BindingList<Producto486LP>(lista);
            AplicarResaltadoStock();
        }

        // Rojo si Stock=0, naranja si Stock<=UmbralStockBajo para que el Administrador detecte de un vistazo lo urgente, sin tener que leer columna por columna
        private void AplicarResaltadoStock()
        {
            foreach (DataGridViewRow fila in dgvCatalogo.Rows)
            {
                Producto486LP p = fila.DataBoundItem as Producto486LP;
                if (p == null) continue;

                if (p.Stock == 0)
                {
                    fila.DefaultCellStyle.BackColor = Color.FromArgb(248, 215, 218);
                    fila.DefaultCellStyle.ForeColor = Color.FromArgb(114, 28, 36);
                    fila.DefaultCellStyle.SelectionBackColor = Color.FromArgb(220, 53, 69);
                    fila.DefaultCellStyle.SelectionForeColor = Color.White;
                }
                else if (p.Stock <= UmbralStockBajo)
                {
                    fila.DefaultCellStyle.BackColor = Color.FromArgb(255, 236, 209);
                    fila.DefaultCellStyle.ForeColor = Color.FromArgb(133, 77, 14);
                    fila.DefaultCellStyle.SelectionBackColor = Color.FromArgb(240, 173, 78);
                    fila.DefaultCellStyle.SelectionForeColor = Color.FromArgb(65, 40, 0);
                }
            }

            dgvCatalogo.Refresh();
        }

        private void dgvCatalogo_SelectionChanged(object sender, EventArgs e)
        {
            _productoSeleccionado = dgvCatalogo.CurrentRow?.DataBoundItem as Producto486LP;
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            var lm = Program.LanguageManager;

            if (_productoSeleccionado == null)
            {
                MessageBox.Show(
                    lm.ObtenerTexto(f, "Frm.GenerarSolicitud.Msg.SeleccionarProducto", "Seleccione un producto del catálogo."),
                    lm.ObtenerTexto(f, "Frm.GenerarSolicitud.Msg.Aviso.Title", "Aviso"),
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int cantidad = (int)numCantidad.Value;

            // 4.1: cantidad invalida (el NumericUpDown ya tiene Minimum=1, esto es una guarda extra)
            if (cantidad <= 0)
            {
                MessageBox.Show(
                    lm.ObtenerTexto(f, "Frm.GenerarSolicitud.Msg.CantidadInvalida", "Cantidad inválida."),
                    lm.ObtenerTexto(f, "Frm.GenerarSolicitud.Msg.Validacion.Title", "Validación"),
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Si el producto ya estaba en la solicitud, se suma la cantidad en vez de duplicar la fila
            DetalleSolicitudCompra486LP existente = _solicitudEnConstruccion.Detalles
                .FirstOrDefault(d => d.IdProducto == _productoSeleccionado.IdProducto);

            if (existente != null)
            {
                existente.CantidadSolicitada += cantidad;
            }
            else
            {
                _solicitudEnConstruccion.Detalles.Add(new DetalleSolicitudCompra486LP
                {
                    IdProducto = _productoSeleccionado.IdProducto,
                    CantidadSolicitada = cantidad,
                    Marca = _productoSeleccionado.Marca,
                    Modelo = _productoSeleccionado.Modelo,
                    Color = _productoSeleccionado.Color,
                    Talle = _productoSeleccionado.Talle
                });
            }

            numCantidad.Value = 1;
            MostrarSolicitud();
        }

        private void btnQuitar_Click(object sender, EventArgs e)
        {
            if (dgvSolicitud.CurrentRow?.DataBoundItem is DetalleSolicitudCompra486LP det)
            {
                _solicitudEnConstruccion.Detalles.Remove(det);
                MostrarSolicitud();
            }
        }

        private void MostrarSolicitud()
        {
            dgvSolicitud.AutoGenerateColumns = false;
            dgvSolicitud.DataSource = null;
            dgvSolicitud.DataSource = new BindingList<DetalleSolicitudCompra486LP>(_solicitudEnConstruccion.Detalles);
        }

        private void btnConfirmar_Click(object sender, EventArgs e)
        {
            var lm = Program.LanguageManager;

            // 7.1: solicitud sin productos
            if (_solicitudEnConstruccion.Detalles.Count == 0)
            {
                MessageBox.Show(
                    lm.ObtenerTexto(f, "Frm.GenerarSolicitud.Msg.SinProductos", "La solicitud no tiene productos."),
                    lm.ObtenerTexto(f, "Frm.GenerarSolicitud.Msg.Aviso.Title", "Aviso"),
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string mensaje;
            SolicitudCompra486LP resultado = _bllSolicitud.RegistrarSolicitud(_solicitudEnConstruccion, out mensaje);

            if (resultado != null)
            {
                MessageBox.Show(mensaje, lm.ObtenerTexto(f, "Frm.GenerarSolicitud.Msg.Informacion.Title", "Información"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
            else
            {
                MessageBox.Show(mensaje, lm.ObtenerTexto(f, "Frm.GenerarSolicitud.Msg.Aviso.Title", "Aviso"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ConfigurarColumnas()
        {
            dgvCatalogo.AutoGenerateColumns = false;
            dgvCatalogo.Columns.Clear();
            dgvCatalogo.ReadOnly = true;
            dgvCatalogo.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Marca", Name = "colCatMarca" });
            dgvCatalogo.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Modelo", Name = "colCatModelo" });
            dgvCatalogo.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Color", Name = "colCatColor" });
            dgvCatalogo.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Talle", Name = "colCatTalle" });
            dgvCatalogo.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Stock", Name = "colCatStock" });

            dgvSolicitud.AutoGenerateColumns = false;
            dgvSolicitud.Columns.Clear();
            dgvSolicitud.ReadOnly = true;
            dgvSolicitud.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Marca", Name = "colSolMarca" });
            dgvSolicitud.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Modelo", Name = "colSolModelo" });
            dgvSolicitud.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Color", Name = "colSolColor" });
            dgvSolicitud.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Talle", Name = "colSolTalle" });
            dgvSolicitud.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "CantidadSolicitada", Name = "colSolCantidad" });

            AplicarEncabezadosColumnas();
        }

        private void AplicarEncabezadosColumnas()
        {
            var lm = Program.LanguageManager;

            if (dgvCatalogo.Columns.Count > 0)
            {
                dgvCatalogo.Columns["colCatMarca"].HeaderText = lm.ObtenerTexto(f, "Frm.GenerarSolicitud.Col.Marca", "Marca");
                dgvCatalogo.Columns["colCatModelo"].HeaderText = lm.ObtenerTexto(f, "Frm.GenerarSolicitud.Col.Modelo", "Modelo");
                dgvCatalogo.Columns["colCatColor"].HeaderText = lm.ObtenerTexto(f, "Frm.GenerarSolicitud.Col.Color", "Color");
                dgvCatalogo.Columns["colCatTalle"].HeaderText = lm.ObtenerTexto(f, "Frm.GenerarSolicitud.Col.Talle", "Talle");
                dgvCatalogo.Columns["colCatStock"].HeaderText = lm.ObtenerTexto(f, "Frm.GenerarSolicitud.Col.Stock", "Stock");
            }

            if (dgvSolicitud.Columns.Count > 0)
            {
                dgvSolicitud.Columns["colSolMarca"].HeaderText = lm.ObtenerTexto(f, "Frm.GenerarSolicitud.Col.Marca", "Marca");
                dgvSolicitud.Columns["colSolModelo"].HeaderText = lm.ObtenerTexto(f, "Frm.GenerarSolicitud.Col.Modelo", "Modelo");
                dgvSolicitud.Columns["colSolColor"].HeaderText = lm.ObtenerTexto(f, "Frm.GenerarSolicitud.Col.Color", "Color");
                dgvSolicitud.Columns["colSolTalle"].HeaderText = lm.ObtenerTexto(f, "Frm.GenerarSolicitud.Col.Talle", "Talle");
                dgvSolicitud.Columns["colSolCantidad"].HeaderText = lm.ObtenerTexto(f, "Frm.GenerarSolicitud.Col.CantidadSolicitada", "Cant. Solicitada");
            }
        }

        public void ActualizarIdioma()
        {
            var lm = Program.LanguageManager;

            this.Text = lm.ObtenerTexto(f, "Frm.GenerarSolicitud.Titulo", "Generar solicitud de compra");
            lblTitulo.Text = lm.ObtenerTexto(f, "Frm.GenerarSolicitud.Titulo", "Generar solicitud de compra");
            grpFiltros.Text = lm.ObtenerTexto(f, "Frm.GenerarSolicitud.Filtros", "Filtros");
            lblMarca.Text = lm.ObtenerTexto(f, "Frm.GenerarSolicitud.Marca", "Marca:");
            lblTalle.Text = lm.ObtenerTexto(f, "Frm.GenerarSolicitud.Talle", "Talle:");
            btnLimpiarFiltros.Text = lm.ObtenerTexto(f, "Frm.GenerarSolicitud.LimpiarFiltros", "Limpiar filtros");
            lblCatalogo.Text = lm.ObtenerTexto(f, "Frm.GenerarSolicitud.Catalogo", "Catálogo (ordenado por stock)");
            grpAgregar.Text = lm.ObtenerTexto(f, "Frm.GenerarSolicitud.GrupoAgregar", "Agregar producto seleccionado a la solicitud");
            lblCantidad.Text = lm.ObtenerTexto(f, "Frm.GenerarSolicitud.Cantidad", "Cantidad solicitada:");
            btnAgregar.Text = lm.ObtenerTexto(f, "Frm.GenerarSolicitud.Agregar", "Agregar a solicitud");
            lblSolicitud.Text = lm.ObtenerTexto(f, "Frm.GenerarSolicitud.Solicitud", "Productos en la solicitud");
            btnQuitar.Text = lm.ObtenerTexto(f, "Frm.GenerarSolicitud.Quitar", "Quitar");
            btnConfirmar.Text = lm.ObtenerTexto(f, "Frm.GenerarSolicitud.Confirmar", "Confirmar solicitud");
            btnCancelar.Text = lm.ObtenerTexto(f, "Frm.GenerarSolicitud.Cancelar", "Cancelar");

            // Los combos de filtro llevan textos traducibles ("(Todas)"/"(Todos)") se rearman.
            CargarCombosFiltro();
            AplicarEncabezadosColumnas();
        }

        private void FrmGenerarSolicitud486LP_FormClosing(object sender, FormClosingEventArgs e)
        {
            Program.LanguageManager.Quitar(this);
        }
    }
}

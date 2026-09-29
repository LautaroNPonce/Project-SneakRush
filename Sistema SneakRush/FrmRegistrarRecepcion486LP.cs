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
    public partial class FrmRegistrarRecepcion486LP : Form, IObserver486LP
    {
        private BLL_OrdenCompra486LP _bllOrden = new BLL_OrdenCompra486LP();
        private BLL_Recepcion486LP _bllRecepcion = new BLL_Recepcion486LP();
        private BLL_Perfil486LP _bllPerfil = new BLL_Perfil486LP();
        private readonly string f = "FrmRegistrarRecepcion486LP";

        private List<OrdenCompra486LP> _ordenesPendientes = new List<OrdenCompra486LP>();
        private OrdenCompra486LP _ordenSeleccionada;
        private Recepcion486LP _recepcionEnConstruccion;
        private bool _puedeConfirmar;
        private bool _puedeCancelar;

        public FrmRegistrarRecepcion486LP()
        {
            InitializeComponent();
            this.Load += FrmRegistrarRecepcion486LP_Load;
            Program.LanguageManager.Agregar(this);
            this.FormClosing += FrmRegistrarRecepcion486LP_FormClosing;

            dgvOrdenes.SelectionChanged += dgvOrdenes_SelectionChanged;
            dgvDetalleRecepcion.CellEndEdit += dgvDetalleRecepcion_CellEndEdit;
            btnConfirmar.Click += btnConfirmar_Click;
            btnCancelar.Click += btnCancelar_Click;
        }

        private void FrmRegistrarRecepcion486LP_Load(object sender, EventArgs e)
        {
            if (!TienePatenteAcceso())
            {
                var lm = Program.LanguageManager;
                MessageBox.Show(
                    lm.ObtenerTexto(f, "Frm.RegistrarRecepcion.Msg.SinPermiso", "No tiene permiso para registrar recepciones de mercadería."),
                    lm.ObtenerTexto(f, "Frm.RegistrarRecepcion.Msg.SinPermiso.Title", "Acceso denegado"),
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
            CargarOrdenesPendientes();
            ActualizarIdioma();
        }

        private bool TienePatenteAcceso()
        {
            var usuario = SessionManager486LP.ObtenerInstancia().UsuarioActual();
            if (usuario == null) return false;

            List<string> permisos = _bllPerfil.ObtenerPermisosPorRol(usuario.Rol);
            return permisos.Contains("COMPRA_REGISTRAR_RECEPCION");
        }

        private void AjustarBotonesSegunPerfil()
        {
            var usuario = SessionManager486LP.ObtenerInstancia().UsuarioActual();
            if (usuario == null) return;

            List<string> permisos = _bllPerfil.ObtenerPermisosPorRol(usuario.Rol);

            _puedeConfirmar = permisos.Contains("RECEPCION_CONFIRMAR");
            _puedeCancelar = permisos.Contains("RECEPCION_CANCELAR");
        }

        private void AplicarPermisosBotones()
        {
            btnConfirmar.Enabled = _puedeConfirmar;
            btnCancelar.Enabled = _puedeCancelar;
        }

        private void CargarOrdenesPendientes()
        {
            _ordenesPendientes = _bllOrden.ListarPendientesDeRecepcion();

            // 2.1: no hay ordenes pendientes de recepcion
            if (_ordenesPendientes.Count == 0)
            {
                var lm = Program.LanguageManager;
                MessageBox.Show(
                    lm.ObtenerTexto(f, "Frm.RegistrarRecepcion.Msg.SinPendientes", "No hay órdenes pendientes de recepción."),
                    lm.ObtenerTexto(f, "Frm.RegistrarRecepcion.Msg.Aviso.Title", "Aviso"),
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                this.BeginInvoke(new Action(() => this.Close()));
                return;
            }

            dgvOrdenes.AutoGenerateColumns = false;
            dgvOrdenes.DataSource = null;
            dgvOrdenes.DataSource = new BindingList<OrdenCompra486LP>(_ordenesPendientes);
        }

        private void dgvOrdenes_SelectionChanged(object sender, EventArgs e)
        {
            OrdenCompra486LP seleccionada = dgvOrdenes.CurrentRow?.DataBoundItem as OrdenCompra486LP;
            if (seleccionada == null) return;
            _ordenSeleccionada = _bllOrden.ObtenerPorId(seleccionada.IdOrden);
            if (_ordenSeleccionada == null) return;

            ArmarRecepcionDesdeDetalleOrden();
        }

        // Arranca asumiendo que llegó todo lo pedido (CantidadRecibida = CantidadPedida)
        // el Encargado de Depósito solo ajusta si algo no llegó completo, sin tipear todo desde cero.
        private void ArmarRecepcionDesdeDetalleOrden()
        {
            _recepcionEnConstruccion = new Recepcion486LP
            {
                IdOrden = _ordenSeleccionada.IdOrden
            };

            foreach (DetalleOrdenCompra486LP detOrden in _ordenSeleccionada.Detalles)
            {
                _recepcionEnConstruccion.Detalles.Add(new DetalleRecepcion486LP
                {
                    IdProducto = detOrden.IdProducto,
                    CantidadPedida = detOrden.Cantidad,
                    CantidadRecibida = detOrden.Cantidad,
                    CantidadFaltante = 0,
                    Marca = detOrden.Marca,
                    Modelo = detOrden.Modelo,
                    Color = detOrden.Color,
                    Talle = detOrden.Talle
                });
            }

            MostrarDetalleRecepcion();
        }

        private void MostrarDetalleRecepcion()
        {
            dgvDetalleRecepcion.AutoGenerateColumns = false;
            dgvDetalleRecepcion.DataSource = null;
            dgvDetalleRecepcion.DataSource = new BindingList<DetalleRecepcion486LP>(_recepcionEnConstruccion.Detalles);
        }

        // 5.1: valida la Cantidad Recibida apenas el Encargado de Deposito termina de editar
        private void dgvDetalleRecepcion_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvDetalleRecepcion.Columns[e.ColumnIndex].Name != "colCantidadRecibida") return;

            if (!(dgvDetalleRecepcion.Rows[e.RowIndex].DataBoundItem is DetalleRecepcion486LP det)) return;

            var lm = Program.LanguageManager;

            if (det.CantidadRecibida < 0 || det.CantidadRecibida > det.CantidadPedida)
            {
                MessageBox.Show(
                    lm.ObtenerTexto(f, "Frm.RegistrarRecepcion.Msg.CantidadInvalida", "Cantidad inválida."),
                    lm.ObtenerTexto(f, "Frm.RegistrarRecepcion.Msg.Validacion.Title", "Validación"),
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                det.CantidadRecibida = det.CantidadPedida;
            }

            det.CantidadFaltante = det.CantidadPedida - det.CantidadRecibida;
            dgvDetalleRecepcion.Refresh();
        }
        private void btnConfirmar_Click(object sender, EventArgs e)
        {
            var lm = Program.LanguageManager;

            if (_recepcionEnConstruccion == null || _recepcionEnConstruccion.Detalles.Count == 0)
            {
                MessageBox.Show(
                    lm.ObtenerTexto(f, "Frm.RegistrarRecepcion.Msg.SeleccionarOrden", "Seleccione una orden de compra."),
                    lm.ObtenerTexto(f, "Frm.RegistrarRecepcion.Msg.Aviso.Title", "Aviso"),
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string mensaje;
            Recepcion486LP resultado = _bllRecepcion.RegistrarRecepcion(_recepcionEnConstruccion, out mensaje);

            if (resultado != null)
            {
                MessageBox.Show(mensaje, lm.ObtenerTexto(f, "Frm.RegistrarRecepcion.Msg.Informacion.Title", "Información"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
            else
            {
                MessageBox.Show(mensaje, lm.ObtenerTexto(f, "Frm.RegistrarRecepcion.Msg.Aviso.Title", "Aviso"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ConfigurarColumnas()
        {
            dgvOrdenes.AutoGenerateColumns = false;
            dgvOrdenes.Columns.Clear();
            dgvOrdenes.ReadOnly = true;
            dgvOrdenes.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "NroOrden", Name = "colNroOrden" });
            dgvOrdenes.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Fecha", Name = "colFechaOrden", DefaultCellStyle = { Format = "dd/MM/yyyy HH:mm" } });

            dgvDetalleRecepcion.AutoGenerateColumns = false;
            dgvDetalleRecepcion.Columns.Clear();
            dgvDetalleRecepcion.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Marca", Name = "colMarca", ReadOnly = true });
            dgvDetalleRecepcion.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Modelo", Name = "colModelo", ReadOnly = true });
            dgvDetalleRecepcion.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Color", Name = "colColor", ReadOnly = true });
            dgvDetalleRecepcion.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Talle", Name = "colTalle", ReadOnly = true });
            dgvDetalleRecepcion.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "CantidadPedida", Name = "colCantidadPedida", ReadOnly = true });
            dgvDetalleRecepcion.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "CantidadRecibida", Name = "colCantidadRecibida", ReadOnly = false });
            dgvDetalleRecepcion.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "CantidadFaltante", Name = "colCantidadFaltante", ReadOnly = true });

            AplicarEncabezadosColumnas();
        }

        private void AplicarEncabezadosColumnas()
        {
            var lm = Program.LanguageManager;

            if (dgvOrdenes.Columns.Count > 0)
            {
                dgvOrdenes.Columns["colNroOrden"].HeaderText = lm.ObtenerTexto(f, "Frm.RegistrarRecepcion.Col.NroOrden", "N° Orden");
                dgvOrdenes.Columns["colFechaOrden"].HeaderText = lm.ObtenerTexto(f, "Frm.RegistrarRecepcion.Col.Fecha", "Fecha");
            }

            if (dgvDetalleRecepcion.Columns.Count > 0)
            {
                dgvDetalleRecepcion.Columns["colMarca"].HeaderText = lm.ObtenerTexto(f, "Frm.RegistrarRecepcion.Col.Marca", "Marca");
                dgvDetalleRecepcion.Columns["colModelo"].HeaderText = lm.ObtenerTexto(f, "Frm.RegistrarRecepcion.Col.Modelo", "Modelo");
                dgvDetalleRecepcion.Columns["colColor"].HeaderText = lm.ObtenerTexto(f, "Frm.RegistrarRecepcion.Col.Color", "Color");
                dgvDetalleRecepcion.Columns["colTalle"].HeaderText = lm.ObtenerTexto(f, "Frm.RegistrarRecepcion.Col.Talle", "Talle");
                dgvDetalleRecepcion.Columns["colCantidadPedida"].HeaderText = lm.ObtenerTexto(f, "Frm.RegistrarRecepcion.Col.CantidadPedida", "Cant. Pedida");
                dgvDetalleRecepcion.Columns["colCantidadRecibida"].HeaderText = lm.ObtenerTexto(f, "Frm.RegistrarRecepcion.Col.CantidadRecibida", "Cant. Recibida");
                dgvDetalleRecepcion.Columns["colCantidadFaltante"].HeaderText = lm.ObtenerTexto(f, "Frm.RegistrarRecepcion.Col.CantidadFaltante", "Cant. Faltante");
            }
        }

        public void ActualizarIdioma()
        {
            var lm = Program.LanguageManager;

            this.Text = lm.ObtenerTexto(f, "Frm.RegistrarRecepcion.Titulo", "Registrar recepción de mercadería");
            lblTitulo.Text = lm.ObtenerTexto(f, "Frm.RegistrarRecepcion.Titulo", "Registrar recepción de mercadería");
            lblOrdenes.Text = lm.ObtenerTexto(f, "Frm.RegistrarRecepcion.Ordenes", "Órdenes pendientes de recepción");
            lblDetalleRecepcion.Text = lm.ObtenerTexto(f, "Frm.RegistrarRecepcion.DetalleRecepcion", "Detalle de la recepción");
            btnConfirmar.Text = lm.ObtenerTexto(f, "Frm.RegistrarRecepcion.Confirmar", "Confirmar recepción");
            btnCancelar.Text = lm.ObtenerTexto(f, "Frm.RegistrarRecepcion.Cancelar", "Cancelar");

            AplicarEncabezadosColumnas();
        }

        private void FrmRegistrarRecepcion486LP_FormClosing(object sender, FormClosingEventArgs e)
        {
            Program.LanguageManager.Quitar(this);
        }
    }
}

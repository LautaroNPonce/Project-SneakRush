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
    public partial class FrmRegistrarOrden486LP : Form, IObserver486LP
    {
        private BLL_SolicitudCompra486LP _bllSolicitud = new BLL_SolicitudCompra486LP();
        private BLL_Proveedor486LP _bllProveedor = new BLL_Proveedor486LP();
        private BLL_OrdenCompra486LP _bllOrden = new BLL_OrdenCompra486LP();
        private BLL_Perfil486LP _bllPerfil = new BLL_Perfil486LP();
        private readonly string f = "FrmRegistrarOrden486LP";

        private List<SolicitudCompra486LP> _solicitudesPendientes = new List<SolicitudCompra486LP>();
        private SolicitudCompra486LP _solicitudSeleccionada;
        private OrdenCompra486LP _ordenEnConstruccion;

        // Patentes granulares por boton (ademas de la de acceso, COMPRA_REGISTRAR_ORDEN).
        private bool _puedeConfirmar;
        private bool _puedeCancelar;

        public FrmRegistrarOrden486LP()
        {
            InitializeComponent();
            this.Load += FrmRegistrarOrden486LP_Load;
            Program.LanguageManager.Agregar(this);
            this.FormClosing += FrmRegistrarOrden486LP_FormClosing;

            dgvSolicitudes.SelectionChanged += dgvSolicitudes_SelectionChanged;
            dgvDetalleOrden.CellEndEdit += dgvDetalleOrden_CellEndEdit;
            btnConfirmar.Click += btnConfirmar_Click;
            btnCancelar.Click += btnCancelar_Click;
        }

        private void FrmRegistrarOrden486LP_Load(object sender, EventArgs e)
        {
            if (!TienePatenteAcceso())
            {
                var lm = Program.LanguageManager;
                MessageBox.Show(
                    lm.ObtenerTexto(f, "Frm.RegistrarOrden.Msg.SinPermiso", "No tiene permiso para registrar órdenes de compra."),
                    lm.ObtenerTexto(f, "Frm.RegistrarOrden.Msg.SinPermiso.Title", "Acceso denegado"),
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
            CargarProveedores();
            CargarSolicitudesPendientes();
            ActualizarIdioma();
        }

        private bool TienePatenteAcceso()
        {
            var usuario = SessionManager486LP.ObtenerInstancia().UsuarioActual();
            if (usuario == null) return false;

            List<string> permisos = _bllPerfil.ObtenerPermisosPorRol(usuario.Rol);
            return permisos.Contains("COMPRA_REGISTRAR_ORDEN");
        }

        private void AjustarBotonesSegunPerfil()
        {
            var usuario = SessionManager486LP.ObtenerInstancia().UsuarioActual();
            if (usuario == null) return;

            List<string> permisos = _bllPerfil.ObtenerPermisosPorRol(usuario.Rol);

            _puedeConfirmar = permisos.Contains("ORDEN_CONFIRMAR");
            _puedeCancelar = permisos.Contains("ORDEN_CANCELAR");
        }

        private void AplicarPermisosBotones()
        {
            btnConfirmar.Enabled = _puedeConfirmar;
            btnCancelar.Enabled = _puedeCancelar;
        }

        // ---------------- Solicitudes pendientes ----------------
        private void CargarSolicitudesPendientes()
        {
            _solicitudesPendientes = _bllSolicitud.ListarPendientes();

            // 2.1: no hay solicitudes pendientes.
            if (_solicitudesPendientes.Count == 0)
            {
                var lm = Program.LanguageManager;
                MessageBox.Show(
                    lm.ObtenerTexto(f, "Frm.RegistrarOrden.Msg.SinPendientes", "No hay solicitudes pendientes."),
                    lm.ObtenerTexto(f, "Frm.RegistrarOrden.Msg.Aviso.Title", "Aviso"),
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                this.BeginInvoke(new Action(() => this.Close()));
                return;
            }

            dgvSolicitudes.AutoGenerateColumns = false;
            dgvSolicitudes.DataSource = null;
            dgvSolicitudes.DataSource = new BindingList<SolicitudCompra486LP>(_solicitudesPendientes);
        }

        private void dgvSolicitudes_SelectionChanged(object sender, EventArgs e)
        {
            SolicitudCompra486LP seleccionada = dgvSolicitudes.CurrentRow?.DataBoundItem as SolicitudCompra486LP;
            if (seleccionada == null) return;

            // ListarPendientes solo trae la cabecera - hace falta reconsultar para el detalle.
            _solicitudSeleccionada = _bllSolicitud.ObtenerPorId(seleccionada.IdSolicitud);
            if (_solicitudSeleccionada == null) return;

            ArmarOrdenDesdeDetalleSolicitud();
        }

        private void ArmarOrdenDesdeDetalleSolicitud()
        {
            _ordenEnConstruccion = new OrdenCompra486LP
            {
                IdSolicitud = _solicitudSeleccionada.IdSolicitud
            };

            foreach (DetalleSolicitudCompra486LP detSol in _solicitudSeleccionada.Detalles)
            {
                _ordenEnConstruccion.Detalles.Add(new DetalleOrdenCompra486LP
                {
                    IdProducto = detSol.IdProducto,
                    Cantidad = detSol.CantidadSolicitada,
                    CostoUnitario = 0,
                    Subtotal = 0,
                    Marca = detSol.Marca,
                    Modelo = detSol.Modelo,
                    Color = detSol.Color,
                    Talle = detSol.Talle,
                    // Dato de referencia: ultimo costo pagado por este producto en una orden
                    // anterior (null si nunca se compro antes) - solo para guiar al Administrador.
                    UltimoCosto = _bllOrden.ObtenerUltimoCosto(detSol.IdProducto)
                });
            }

            MostrarDetalleOrden();
        }

        // ---------------- Proveedor ----------------
        private void CargarProveedores()
        {
            List<Proveedor486LP> proveedores = _bllProveedor.Listar();
            cmbProveedor.DataSource = proveedores;
            cmbProveedor.DisplayMember = "Nombre";
            cmbProveedor.ValueMember = "IdProveedor";
        }

        // ---------------- Detalle de la orden (grilla editable) ----------------
        private void MostrarDetalleOrden()
        {
            dgvDetalleOrden.AutoGenerateColumns = false;
            dgvDetalleOrden.DataSource = null;
            dgvDetalleOrden.DataSource = new BindingList<DetalleOrdenCompra486LP>(_ordenEnConstruccion.Detalles);
            ActualizarCostoTotal();
        }

        // 6.1: valida el Costo Unitario apenas el Administrador termina de editar una celda.
        private void dgvDetalleOrden_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            string nombreColumna = dgvDetalleOrden.Columns[e.ColumnIndex].Name;
            if (nombreColumna != "colCantidad" && nombreColumna != "colCostoUnitario") return;

            if (!(dgvDetalleOrden.Rows[e.RowIndex].DataBoundItem is DetalleOrdenCompra486LP det)) return;

            var lm = Program.LanguageManager;

            if (det.CostoUnitario <= 0)
            {
                MessageBox.Show(
                    lm.ObtenerTexto(f, "Frm.RegistrarOrden.Msg.CostoInvalido", "Costo inválido."),
                    lm.ObtenerTexto(f, "Frm.RegistrarOrden.Msg.Validacion.Title", "Validación"),
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                det.CostoUnitario = 0;
            }

            if (det.Cantidad <= 0)
            {
                det.Cantidad = 1;
            }

            det.Subtotal = det.Cantidad * det.CostoUnitario;
            dgvDetalleOrden.Refresh();
            ActualizarCostoTotal();
        }

        private void ActualizarCostoTotal()
        {
            decimal total = _ordenEnConstruccion?.Detalles?.Sum(d => d.Subtotal) ?? 0;
            lblCostoTotal.Text = total.ToString("C");
        }

        // ---------------- Confirmar / Cancelar ----------------
        private void btnConfirmar_Click(object sender, EventArgs e)
        {
            var lm = Program.LanguageManager;

            if (_ordenEnConstruccion == null || _ordenEnConstruccion.Detalles.Count == 0)
            {
                MessageBox.Show(
                    lm.ObtenerTexto(f, "Frm.RegistrarOrden.Msg.SeleccionarSolicitud", "Seleccione una solicitud de compra."),
                    lm.ObtenerTexto(f, "Frm.RegistrarOrden.Msg.Aviso.Title", "Aviso"),
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (cmbProveedor.SelectedValue == null)
            {
                MessageBox.Show(
                    lm.ObtenerTexto(f, "Frm.RegistrarOrden.Msg.SeleccionarProveedor", "Seleccione un proveedor."),
                    lm.ObtenerTexto(f, "Frm.RegistrarOrden.Msg.Aviso.Title", "Aviso"),
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (_ordenEnConstruccion.Detalles.Any(d => d.CostoUnitario <= 0))
            {
                MessageBox.Show(
                    lm.ObtenerTexto(f, "Frm.RegistrarOrden.Msg.FaltanCostos", "Complete el costo unitario de todos los productos."),
                    lm.ObtenerTexto(f, "Frm.RegistrarOrden.Msg.Validacion.Title", "Validación"),
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            _ordenEnConstruccion.IdProveedor = (int)cmbProveedor.SelectedValue;

            string mensaje;
            OrdenCompra486LP resultado = _bllOrden.RegistrarOrden(_ordenEnConstruccion, out mensaje);

            if (resultado != null)
            {
                MessageBox.Show(mensaje, lm.ObtenerTexto(f, "Frm.RegistrarOrden.Msg.Informacion.Title", "Información"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
            else
            {
                MessageBox.Show(mensaje, lm.ObtenerTexto(f, "Frm.RegistrarOrden.Msg.Aviso.Title", "Aviso"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // ---------------- Columnas ----------------
        private void ConfigurarColumnas()
        {
            dgvSolicitudes.AutoGenerateColumns = false;
            dgvSolicitudes.Columns.Clear();
            dgvSolicitudes.ReadOnly = true;
            dgvSolicitudes.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "IdSolicitud", Name = "colIdSolicitud" });
            dgvSolicitudes.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Fecha", Name = "colFechaSolicitud", DefaultCellStyle = { Format = "dd/MM/yyyy HH:mm" } });

            dgvDetalleOrden.AutoGenerateColumns = false;
            dgvDetalleOrden.Columns.Clear();
            dgvDetalleOrden.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Marca", Name = "colMarca", ReadOnly = true });
            dgvDetalleOrden.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Modelo", Name = "colModelo", ReadOnly = true });
            dgvDetalleOrden.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Color", Name = "colColor", ReadOnly = true });
            dgvDetalleOrden.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Talle", Name = "colTalle", ReadOnly = true });
            dgvDetalleOrden.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Cantidad", Name = "colCantidad", ReadOnly = false });
            dgvDetalleOrden.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "UltimoCosto", Name = "colUltimoCosto", ReadOnly = true, DefaultCellStyle = { Format = "C" } });
            dgvDetalleOrden.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "CostoUnitario", Name = "colCostoUnitario", ReadOnly = false });
            dgvDetalleOrden.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Subtotal", Name = "colSubtotal", ReadOnly = true });

            AplicarEncabezadosColumnas();
        }

        private void AplicarEncabezadosColumnas()
        {
            var lm = Program.LanguageManager;

            if (dgvSolicitudes.Columns.Count > 0)
            {
                dgvSolicitudes.Columns["colIdSolicitud"].HeaderText = lm.ObtenerTexto(f, "Frm.RegistrarOrden.Col.IdSolicitud", "N° Solicitud");
                dgvSolicitudes.Columns["colFechaSolicitud"].HeaderText = lm.ObtenerTexto(f, "Frm.RegistrarOrden.Col.Fecha", "Fecha");
            }

            if (dgvDetalleOrden.Columns.Count > 0)
            {
                dgvDetalleOrden.Columns["colMarca"].HeaderText = lm.ObtenerTexto(f, "Frm.RegistrarOrden.Col.Marca", "Marca");
                dgvDetalleOrden.Columns["colModelo"].HeaderText = lm.ObtenerTexto(f, "Frm.RegistrarOrden.Col.Modelo", "Modelo");
                dgvDetalleOrden.Columns["colColor"].HeaderText = lm.ObtenerTexto(f, "Frm.RegistrarOrden.Col.Color", "Color");
                dgvDetalleOrden.Columns["colTalle"].HeaderText = lm.ObtenerTexto(f, "Frm.RegistrarOrden.Col.Talle", "Talle");
                dgvDetalleOrden.Columns["colCantidad"].HeaderText = lm.ObtenerTexto(f, "Frm.RegistrarOrden.Col.Cantidad", "Cantidad");
                dgvDetalleOrden.Columns["colUltimoCosto"].HeaderText = lm.ObtenerTexto(f, "Frm.RegistrarOrden.Col.UltimoCosto", "Último Costo");
                dgvDetalleOrden.Columns["colCostoUnitario"].HeaderText = lm.ObtenerTexto(f, "Frm.RegistrarOrden.Col.CostoUnitario", "Costo Unitario");
                dgvDetalleOrden.Columns["colSubtotal"].HeaderText = lm.ObtenerTexto(f, "Frm.RegistrarOrden.Col.Subtotal", "Subtotal");
            }
        }

        public void ActualizarIdioma()
        {
            var lm = Program.LanguageManager;

            this.Text = lm.ObtenerTexto(f, "Frm.RegistrarOrden.Titulo", "Registrar orden de compra");
            lblTitulo.Text = lm.ObtenerTexto(f, "Frm.RegistrarOrden.Titulo", "Registrar orden de compra");
            lblSolicitudes.Text = lm.ObtenerTexto(f, "Frm.RegistrarOrden.Solicitudes", "Solicitudes de compra pendientes");
            grpProveedor.Text = lm.ObtenerTexto(f, "Frm.RegistrarOrden.Proveedor", "Proveedor");
            lblProveedor.Text = lm.ObtenerTexto(f, "Frm.RegistrarOrden.ProveedorLabel", "Proveedor:");
            lblDetalleOrden.Text = lm.ObtenerTexto(f, "Frm.RegistrarOrden.DetalleOrden", "Detalle de la orden");
            lblCostoTotalTexto.Text = lm.ObtenerTexto(f, "Frm.RegistrarOrden.CostoTotal", "Costo Total:");
            btnConfirmar.Text = lm.ObtenerTexto(f, "Frm.RegistrarOrden.Confirmar", "Confirmar orden");
            btnCancelar.Text = lm.ObtenerTexto(f, "Frm.RegistrarOrden.Cancelar", "Cancelar");

            AplicarEncabezadosColumnas();
        }

        private void FrmRegistrarOrden486LP_FormClosing(object sender, FormClosingEventArgs e)
        {
            Program.LanguageManager.Quitar(this);
        }
    }
}

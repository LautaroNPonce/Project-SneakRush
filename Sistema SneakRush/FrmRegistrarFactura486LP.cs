using BE;
using BLL;
using Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sistema_SneakRush
{
    public partial class FrmRegistrarFactura486LP : Form, IObserver486LP
    {
        private BLL_OrdenCompra486LP _bllOrden = new BLL_OrdenCompra486LP();
        private BLL_Recepcion486LP _bllRecepcion = new BLL_Recepcion486LP();
        private BLL_Factura486LP _bllFactura = new BLL_Factura486LP();
        private BLL_Proveedor486LP _bllProveedor = new BLL_Proveedor486LP();
        private BLL_Perfil486LP _bllPerfil = new BLL_Perfil486LP();
        private readonly string f = "FrmRegistrarFactura486LP";

        private List<OrdenCompra486LP> _ordenesPendientes = new List<OrdenCompra486LP>();
        private OrdenCompra486LP _ordenSeleccionada;
        private Recepcion486LP _recepcionSeleccionada;
        private string _nombreProveedorSeleccionado;
        private Factura486LP _facturaEnConstruccion;
        private string _rutaComprobante;

        // Patentes granulares por boton (ademas de la de acceso, COMPRA_REGISTRAR_FACTURA).
        private bool _puedeCobrar;
        private bool _puedeVerComprobante;
        private bool _puedeCancelar;

        public FrmRegistrarFactura486LP()
        {
            InitializeComponent();
            this.Load += FrmRegistrarFactura486LP_Load;
            Program.LanguageManager.Agregar(this);
            this.FormClosing += FrmRegistrarFactura486LP_FormClosing;

            dgvOrdenes.SelectionChanged += dgvOrdenes_SelectionChanged;
            rbEfectivo.CheckedChanged += RbMedioPago_CheckedChanged;
            rbTarjeta.CheckedChanged += RbMedioPago_CheckedChanged;
            rbTransferencia.CheckedChanged += RbMedioPago_CheckedChanged;
            btnCobrar.Click += btnCobrar_Click;
            btnVerComprobante.Click += btnVerComprobante_Click;
            btnCancelar.Click += btnCancelar_Click;
        }

        private void FrmRegistrarFactura486LP_Load(object sender, EventArgs e)
        {
            if (!TienePatenteAcceso())
            {
                var lm = Program.LanguageManager;
                MessageBox.Show(
                    lm.ObtenerTexto(f, "Frm.RegistrarFactura.Msg.SinPermiso", "No tiene permiso para registrar facturas de compra."),
                    lm.ObtenerTexto(f, "Frm.RegistrarFactura.Msg.SinPermiso.Title", "Acceso denegado"),
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
            return permisos.Contains("COMPRA_REGISTRAR_FACTURA");
        }

        private void AjustarBotonesSegunPerfil()
        {
            var usuario = SessionManager486LP.ObtenerInstancia().UsuarioActual();
            if (usuario == null) return;

            List<string> permisos = _bllPerfil.ObtenerPermisosPorRol(usuario.Rol);

            _puedeCobrar = permisos.Contains("FACTURA_COBRAR");
            _puedeVerComprobante = permisos.Contains("FACTURA_VERCOMPROBANTE");
            _puedeCancelar = permisos.Contains("FACTURA_CANCELAR");
        }

        private void AplicarPermisosBotones()
        {
            btnCobrar.Enabled = _puedeCobrar;
            // btnVerComprobante ademas necesita que el pago ya se haya confirmado - se resuelve
            // en ActualizarEstadoBotonComprobante(), no solo con la patente.
            btnCancelar.Enabled = _puedeCancelar;
            ActualizarEstadoBotonComprobante();
        }

        private void ActualizarEstadoBotonComprobante()
        {
            btnVerComprobante.Enabled = _puedeVerComprobante && !string.IsNullOrEmpty(_rutaComprobante);
        }

        // ---------------- Ordenes pendientes de facturacion ----------------
        private void CargarOrdenesPendientes()
        {
            _ordenesPendientes = _bllOrden.ListarPendientesDeFacturacion();

            // 2.1: no hay ordenes pendientes de facturacion.
            if (_ordenesPendientes.Count == 0)
            {
                var lm = Program.LanguageManager;
                MessageBox.Show(
                    lm.ObtenerTexto(f, "Frm.RegistrarFactura.Msg.SinPendientes", "No hay órdenes pendientes de facturación."),
                    lm.ObtenerTexto(f, "Frm.RegistrarFactura.Msg.Aviso.Title", "Aviso"),
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
            _recepcionSeleccionada = _bllRecepcion.ObtenerPorIdOrden(seleccionada.IdOrden);
            if (_ordenSeleccionada == null || _recepcionSeleccionada == null) return;

            List<Proveedor486LP> proveedores = _bllProveedor.Listar();
            Proveedor486LP proveedor = proveedores.FirstOrDefault(p => p.IdProveedor == _ordenSeleccionada.IdProveedor);
            _nombreProveedorSeleccionado = proveedor?.Nombre ?? "-";

            ArmarDetalleAFacturar();
        }

        // Combina Costo Unitario (de la Orden, CUN06) con Cantidad Recibida (de la Recepcion,
        // CUN07): el Total a facturar tiene que reflejar lo que REALMENTE entro, no lo pedido
        // originalmente - si hubo faltantes, se paga menos. Se reutilizan los mismos objetos
        // DetalleOrdenCompra486LP sobrescribiendo Cantidad/Subtotal solo para mostrar en
        // pantalla y en el comprobante - no se vuelven a persistir en ningun lado.
        private void ArmarDetalleAFacturar()
        {
            Dictionary<int, int> cantidadRecibidaPorProducto = _recepcionSeleccionada.Detalles
                .ToDictionary(d => d.IdProducto, d => d.CantidadRecibida);

            foreach (DetalleOrdenCompra486LP det in _ordenSeleccionada.Detalles)
            {
                int cantidadRecibida = cantidadRecibidaPorProducto.TryGetValue(det.IdProducto, out int cant) ? cant : 0;
                det.Cantidad = cantidadRecibida;
                det.Subtotal = cantidadRecibida * det.CostoUnitario;
            }

            dgvDetalleFactura.AutoGenerateColumns = false;
            dgvDetalleFactura.DataSource = null;
            dgvDetalleFactura.DataSource = new BindingList<DetalleOrdenCompra486LP>(_ordenSeleccionada.Detalles);

            decimal total = _ordenSeleccionada.Detalles.Sum(d => d.Subtotal);
            lblTotal.Text = total.ToString("C");
        }

        // ---------------- Medio de pago ----------------
        private void RbMedioPago_CheckedChanged(object sender, EventArgs e)
        {
            pnlTarjeta.Visible = rbTarjeta.Checked;
        }

        private string ObtenerMedioPagoSeleccionado()
        {
            if (rbTarjeta.Checked) return "Tarjeta";
            if (rbTransferencia.Checked) return "Transferencia";
            return "Efectivo";
        }

        // 7.1: valida los datos de tarjeta antes de procesar el pago. Solo aplica si el medio
        // de pago elegido es Tarjeta.
        private bool ValidarDatosTarjeta()
        {
            var lm = Program.LanguageManager;

            string numero = txtNumeroTarjeta.Text.Trim();
            string vencimiento = txtVencimiento.Text.Trim();
            string cvv = txtCVV.Text.Trim();

            bool numeroValido = numero.Length >= 13 && numero.Length <= 19 && numero.All(char.IsDigit);
            bool vencimientoValido = System.Text.RegularExpressions.Regex.IsMatch(vencimiento, @"^(0[1-9]|1[0-2])\/\d{2}$");
            bool cvvValido = cvv.Length >= 3 && cvv.Length <= 4 && cvv.All(char.IsDigit);

            if (!numeroValido || !vencimientoValido || !cvvValido)
            {
                MessageBox.Show(
                    lm.ObtenerTexto(f, "Frm.RegistrarFactura.Msg.DatosTarjetaInvalidos", "Datos de tarjeta inválidos."),
                    lm.ObtenerTexto(f, "Frm.RegistrarFactura.Msg.Validacion.Title", "Validación"),
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        // ---------------- Cobrar (registrar factura + procesar pago) ----------------
        private void btnCobrar_Click(object sender, EventArgs e)
        {
            var lm = Program.LanguageManager;

            if (_ordenSeleccionada == null || _recepcionSeleccionada == null)
            {
                MessageBox.Show(
                    lm.ObtenerTexto(f, "Frm.RegistrarFactura.Msg.SeleccionarOrden", "Seleccione una orden de compra."),
                    lm.ObtenerTexto(f, "Frm.RegistrarFactura.Msg.Aviso.Title", "Aviso"),
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string medioPago = ObtenerMedioPagoSeleccionado();

            if (medioPago == "Tarjeta" && !ValidarDatosTarjeta())
            {
                return; // 7.1: se queda en el form para que el Administrador corrija.
            }

            decimal total = _ordenSeleccionada.Detalles.Sum(d => d.Subtotal);

            _facturaEnConstruccion = new Factura486LP { IdOrden = _ordenSeleccionada.IdOrden, Total = total };

            string mensajeFactura;
            Factura486LP facturaRegistrada = _bllFactura.RegistrarFactura(_facturaEnConstruccion, out mensajeFactura);

            if (facturaRegistrada == null)
            {
                MessageBox.Show(mensajeFactura, lm.ObtenerTexto(f, "Frm.RegistrarFactura.Msg.Aviso.Title", "Aviso"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            btnCobrar.Enabled = false;
            dgvOrdenes.Enabled = false;
            pbProcesando.Visible = true;
            lblProcesando.Visible = true;

            IEstrategiaPago486LP estrategia = FabricaEstrategiaPago486LP.Crear(medioPago);

            estrategia.ProcesarPago(total, aprobado =>
            {
                pbProcesando.Visible = false;
                lblProcesando.Visible = false;

                if (!aprobado)
                {
                    // No deberia pasar (el pago siempre aprueba en este proyecto), pero se
                    // contempla igual para no dejar el form colgado si algun dia cambia.
                    btnCobrar.Enabled = true;
                    dgvOrdenes.Enabled = true;
                    return;
                }

                string mensajePago;
                bool ok = _bllFactura.ConfirmarPago(facturaRegistrada, medioPago, _recepcionSeleccionada.Detalles, out mensajePago);

                if (!ok)
                {
                    MessageBox.Show(mensajePago, lm.ObtenerTexto(f, "Frm.RegistrarFactura.Msg.Aviso.Title", "Aviso"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    btnCobrar.Enabled = true;
                    dgvOrdenes.Enabled = true;
                    return;
                }

                _rutaComprobante = GeneradorComprobantePDF486LP.Generar(facturaRegistrada, _nombreProveedorSeleccionado, _ordenSeleccionada.Detalles);
                ActualizarEstadoBotonComprobante();

                MessageBox.Show(
                    lm.ObtenerTexto(f, "Frm.RegistrarFactura.Msg.PagoConfirmado", "Pago confirmado. Ya puede ver el comprobante."),
                    lm.ObtenerTexto(f, "Frm.RegistrarFactura.Msg.Informacion.Title", "Información"),
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            });
        }

        private void btnVerComprobante_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_rutaComprobante)) return;
            Process.Start(_rutaComprobante);
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // ---------------- Columnas ----------------
        private void ConfigurarColumnas()
        {
            dgvOrdenes.AutoGenerateColumns = false;
            dgvOrdenes.Columns.Clear();
            dgvOrdenes.ReadOnly = true;
            dgvOrdenes.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "NroOrden", Name = "colNroOrden" });
            dgvOrdenes.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Fecha", Name = "colFechaOrden", DefaultCellStyle = { Format = "dd/MM/yyyy HH:mm" } });

            dgvDetalleFactura.AutoGenerateColumns = false;
            dgvDetalleFactura.Columns.Clear();
            dgvDetalleFactura.ReadOnly = true;
            dgvDetalleFactura.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Marca", Name = "colMarca" });
            dgvDetalleFactura.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Modelo", Name = "colModelo" });
            dgvDetalleFactura.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Color", Name = "colColor" });
            dgvDetalleFactura.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Talle", Name = "colTalle" });
            dgvDetalleFactura.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Cantidad", Name = "colCantidad" });
            dgvDetalleFactura.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "CostoUnitario", Name = "colCostoUnitario", DefaultCellStyle = { Format = "C" } });
            dgvDetalleFactura.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Subtotal", Name = "colSubtotal", DefaultCellStyle = { Format = "C" } });

            AplicarEncabezadosColumnas();
        }

        private void AplicarEncabezadosColumnas()
        {
            var lm = Program.LanguageManager;

            if (dgvOrdenes.Columns.Count > 0)
            {
                dgvOrdenes.Columns["colNroOrden"].HeaderText = lm.ObtenerTexto(f, "Frm.RegistrarFactura.Col.NroOrden", "N° Orden");
                dgvOrdenes.Columns["colFechaOrden"].HeaderText = lm.ObtenerTexto(f, "Frm.RegistrarFactura.Col.Fecha", "Fecha");
            }

            if (dgvDetalleFactura.Columns.Count > 0)
            {
                dgvDetalleFactura.Columns["colMarca"].HeaderText = lm.ObtenerTexto(f, "Frm.RegistrarFactura.Col.Marca", "Marca");
                dgvDetalleFactura.Columns["colModelo"].HeaderText = lm.ObtenerTexto(f, "Frm.RegistrarFactura.Col.Modelo", "Modelo");
                dgvDetalleFactura.Columns["colColor"].HeaderText = lm.ObtenerTexto(f, "Frm.RegistrarFactura.Col.Color", "Color");
                dgvDetalleFactura.Columns["colTalle"].HeaderText = lm.ObtenerTexto(f, "Frm.RegistrarFactura.Col.Talle", "Talle");
                dgvDetalleFactura.Columns["colCantidad"].HeaderText = lm.ObtenerTexto(f, "Frm.RegistrarFactura.Col.Cantidad", "Cant. Recibida");
                dgvDetalleFactura.Columns["colCostoUnitario"].HeaderText = lm.ObtenerTexto(f, "Frm.RegistrarFactura.Col.CostoUnitario", "Costo Unitario");
                dgvDetalleFactura.Columns["colSubtotal"].HeaderText = lm.ObtenerTexto(f, "Frm.RegistrarFactura.Col.Subtotal", "Subtotal");
            }
        }

        public void ActualizarIdioma()
        {
            var lm = Program.LanguageManager;

            this.Text = lm.ObtenerTexto(f, "Frm.RegistrarFactura.Titulo", "Registrar factura y pago de compra");
            lblTitulo.Text = lm.ObtenerTexto(f, "Frm.RegistrarFactura.Titulo", "Registrar factura y pago de compra");
            lblOrdenes.Text = lm.ObtenerTexto(f, "Frm.RegistrarFactura.Ordenes", "Órdenes pendientes de facturación");
            lblDetalleFactura.Text = lm.ObtenerTexto(f, "Frm.RegistrarFactura.DetalleFactura", "Detalle a facturar");
            lblTotalTexto.Text = lm.ObtenerTexto(f, "Frm.RegistrarFactura.Total", "Total:");
            grpMedioPago.Text = lm.ObtenerTexto(f, "Frm.RegistrarFactura.MedioPago", "Medio de pago");
            lblMedioPago.Text = lm.ObtenerTexto(f, "Frm.RegistrarFactura.MedioPagoLabel", "Medio de pago:");
            rbEfectivo.Text = lm.ObtenerTexto(f, "Frm.RegistrarFactura.Efectivo", "Efectivo");
            rbTarjeta.Text = lm.ObtenerTexto(f, "Frm.RegistrarFactura.Tarjeta", "Tarjeta");
            rbTransferencia.Text = lm.ObtenerTexto(f, "Frm.RegistrarFactura.Transferencia", "Transferencia");
            lblNumeroTarjeta.Text = lm.ObtenerTexto(f, "Frm.RegistrarFactura.Numero", "Número:");
            lblVencimiento.Text = lm.ObtenerTexto(f, "Frm.RegistrarFactura.Vencimiento", "Vencimiento:");
            lblCVV.Text = lm.ObtenerTexto(f, "Frm.RegistrarFactura.CVV", "CVV:");
            lblProcesando.Text = lm.ObtenerTexto(f, "Frm.RegistrarFactura.Procesando", "Procesando el pago...");
            btnCobrar.Text = lm.ObtenerTexto(f, "Frm.RegistrarFactura.Cobrar", "Registrar factura y cobrar");
            btnVerComprobante.Text = lm.ObtenerTexto(f, "Frm.RegistrarFactura.VerComprobante", "Ver Comprobante");
            btnCancelar.Text = lm.ObtenerTexto(f, "Frm.RegistrarFactura.Cancelar", "Cancelar");

            AplicarEncabezadosColumnas();
        }

        private void FrmRegistrarFactura486LP_FormClosing(object sender, FormClosingEventArgs e)
        {
            Program.LanguageManager.Quitar(this);
        }
    }
}

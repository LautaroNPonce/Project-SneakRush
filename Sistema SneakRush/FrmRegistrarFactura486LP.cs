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

        // La orden que se está facturando (llenada al seleccionarla, junto con su Recepción), la
        // factura ya generada, y la ruta del pdf ya generado para esa factura
        private List<OrdenCompra486LP> _ordenesPendientes = new List<OrdenCompra486LP>();
        private OrdenCompra486LP _ordenSeleccionada;
        private Recepcion486LP _recepcionSeleccionada;
        private string _nombreProveedorSeleccionado;
        private Factura486LP _facturaGenerada;
        private string _rutaComprobante;
        private bool _puedeCobrar;
        private bool _puedeVerComprobante;
        private bool _puedeCancelar;
        private bool _formateandoNumero;
        private bool _formateandoVencimiento;

        public FrmRegistrarFactura486LP()
        {
            InitializeComponent();
            Program.LanguageManager.Agregar(this);
            this.FormClosing += FrmRegistrarFactura486LP_FormClosing;
            this.Load += FrmRegistrarFactura486LP_Load;
            dgvOrdenes.SelectionChanged += dgvOrdenes_SelectionChanged;
            btnCobrar.Click += btnCobrar_Click;
            btnVerComprobante.Click += btnVerComprobante_Click;
            btnCancelar.Click += btnCancelar_Click;
            rbEfectivo.CheckedChanged += MedioPago_CheckedChanged;
            rbTarjeta.CheckedChanged += MedioPago_CheckedChanged;
            rbTransferencia.CheckedChanged += MedioPago_CheckedChanged;
            txtNumeroTarjeta.TextChanged += txtNumeroTarjeta_TextChanged;
            txtVencimiento.TextChanged += txtVencimiento_TextChanged;
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
            btnVerComprobante.Enabled = false;
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
            // "Ver comprobante" ademas depende de que ya haya una factura generada.
            btnVerComprobante.Enabled = _puedeVerComprobante && _facturaGenerada != null;
            btnCancelar.Enabled = _puedeCancelar;
        }

        // Habilita/deshabilita el panel de datos de tarjeta segun el medio de pago elegido
        private void MedioPago_CheckedChanged(object sender, EventArgs e)
        {
            grpTarjeta.Enabled = rbTarjeta.Checked;
        }

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

        // Combina costo unitario (orden) con Cantidad Recibida (recepción): el total tiene que reflejar lo que realmente entró, no lo pedido si hubo faltantes, se paga menos
        // Se reutiliza el mismo DetalleOrdenCompra486LP solo para mostrar, sin volver a persistirlo
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

            MostrarDetalle();
        }

        private void MostrarDetalle()
        {
            dgvDetalleFactura.AutoGenerateColumns = false;
            dgvDetalleFactura.DataSource = null;
            dgvDetalleFactura.DataSource = new BindingList<DetalleOrdenCompra486LP>(_ordenSeleccionada.Detalles);
            lblTotal.Text = _ordenSeleccionada.Detalles.Sum(d => d.Subtotal).ToString("C");
        }

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

            if (dgvDetalleFactura.Columns.Count == 0) return;

            dgvDetalleFactura.Columns["colMarca"].HeaderText = lm.ObtenerTexto(f, "Frm.RegistrarFactura.Col.Marca", "Marca");
            dgvDetalleFactura.Columns["colModelo"].HeaderText = lm.ObtenerTexto(f, "Frm.RegistrarFactura.Col.Modelo", "Modelo");
            dgvDetalleFactura.Columns["colColor"].HeaderText = lm.ObtenerTexto(f, "Frm.RegistrarFactura.Col.Color", "Color");
            dgvDetalleFactura.Columns["colTalle"].HeaderText = lm.ObtenerTexto(f, "Frm.RegistrarFactura.Col.Talle", "Talle");
            dgvDetalleFactura.Columns["colCantidad"].HeaderText = lm.ObtenerTexto(f, "Frm.RegistrarFactura.Col.Cantidad", "Cant. Recibida");
            dgvDetalleFactura.Columns["colCostoUnitario"].HeaderText = lm.ObtenerTexto(f, "Frm.RegistrarFactura.Col.CostoUnitario", "Costo Unitario");
            dgvDetalleFactura.Columns["colSubtotal"].HeaderText = lm.ObtenerTexto(f, "Frm.RegistrarFactura.Col.Subtotal", "Subtotal");
        }

        // 7.1: valida los datos de la tarjeta (numero, vencimiento, CVV)
        // Solo se llama si el medio de pago elegido es Tarjeta.
        // Mismo criterio que CUN04: valida formato y que no este vencida
        private bool ValidarDatosTarjeta()
        {
            string numeroSinEspacios = txtNumeroTarjeta.Text.Replace(" ", "").Trim();
            if (!System.Text.RegularExpressions.Regex.IsMatch(numeroSinEspacios, @"^\d{16}$"))
                return false;

            var match = System.Text.RegularExpressions.Regex.Match(txtVencimiento.Text.Trim(), @"^(0[1-9]|1[0-2])/(\d{2})$");
            if (!match.Success)
                return false;

            int mes = int.Parse(match.Groups[1].Value);
            int anio = 2000 + int.Parse(match.Groups[2].Value);
            DateTime finDeMes = new DateTime(anio, mes, DateTime.DaysInMonth(anio, mes));
            if (finDeMes < DateTime.Now.Date)
                return false; // tarjeta vencida

            if (!System.Text.RegularExpressions.Regex.IsMatch(txtCVV.Text.Trim(), @"^\d{3}$"))
                return false;

            return true;
        }

        private void txtNumeroTarjeta_TextChanged(object sender, EventArgs e)
        {
            if (_formateandoNumero) return;
            _formateandoNumero = true;

            string soloDigitos = new string(txtNumeroTarjeta.Text.Where(char.IsDigit).ToArray());
            if (soloDigitos.Length > 16) soloDigitos = soloDigitos.Substring(0, 16);

            StringBuilder formateado = new StringBuilder();
            for (int i = 0; i < soloDigitos.Length; i++)
            {
                if (i > 0 && i % 4 == 0) formateado.Append(' ');
                formateado.Append(soloDigitos[i]);
            }

            txtNumeroTarjeta.Text = formateado.ToString();
            txtNumeroTarjeta.SelectionStart = txtNumeroTarjeta.Text.Length;

            _formateandoNumero = false;
        }

        // Inserta el "/" automaticamente despues de los primeros 2 digitos (mm/aa).
        private void txtVencimiento_TextChanged(object sender, EventArgs e)
        {
            if (_formateandoVencimiento) return;
            _formateandoVencimiento = true;

            string soloDigitos = new string(txtVencimiento.Text.Where(char.IsDigit).ToArray());
            if (soloDigitos.Length > 4) soloDigitos = soloDigitos.Substring(0, 4);

            string formateado = soloDigitos.Length > 2
                ? soloDigitos.Substring(0, 2) + "/" + soloDigitos.Substring(2)
                : soloDigitos;

            txtVencimiento.Text = formateado;
            txtVencimiento.SelectionStart = txtVencimiento.Text.Length;

            _formateandoVencimiento = false;
        }

        private string ObtenerMedioPagoSeleccionado()
        {
            if (rbTarjeta.Checked) return "Tarjeta";
            if (rbTransferencia.Checked) return "Transferencia";
            return "Efectivo";
        }

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
                MessageBox.Show(
                    lm.ObtenerTexto(f, "Frm.RegistrarFactura.Msg.DatosTarjetaInvalidos", "Datos de tarjeta inválidos."),
                    lm.ObtenerTexto(f, "Frm.RegistrarFactura.Msg.Validacion.Title", "Validación"),
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // A diferencia de CUN04 (la venta se registra recién cuando el banco aprueba), acá la factura se registra antes del pago así lo define la especificación de CUN08
            // (paso 5 registra la factura, paso 8 procesa el pago)
            decimal total = _ordenSeleccionada.Detalles.Sum(d => d.Subtotal);
            Factura486LP facturaNueva = new Factura486LP { IdOrden = _ordenSeleccionada.IdOrden, Total = total };

            string mensajeFactura;
            Factura486LP facturaRegistrada = _bllFactura.RegistrarFactura(facturaNueva, out mensajeFactura);

            if (facturaRegistrada == null)
            {
                MessageBox.Show(mensajeFactura, lm.ObtenerTexto(f, "Frm.RegistrarFactura.Msg.Aviso.Title", "Aviso"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // A partir de acá el pago es asincrono (Efectivo resuelve al instante, Tarjeta/Transferencia usan un Hilo de 3s)con el indicador de "Procesando" mientras tanto.
            // La estrategia vuelve sola al hilo de UI (SynchronizationContext), por eso el callback puede tocar el form sin problema.
            MostrarProcesando(true);

            IEstrategiaPago486LP estrategia = FabricaEstrategiaPago486LP.Crear(medioPago);
            estrategia.ProcesarPago(total, (aprobado) =>
            {
                MostrarProcesando(false);

                if (aprobado)
                {
                    string mensajePago;
                    bool ok = _bllFactura.ConfirmarPago(facturaRegistrada, medioPago, _recepcionSeleccionada.Detalles, out mensajePago);

                    if (ok)
                    {
                        _facturaGenerada = facturaRegistrada;
                        _rutaComprobante = GeneradorComprobantePDF486LP.Generar(facturaRegistrada, _nombreProveedorSeleccionado, _ordenSeleccionada.Detalles);

                        MessageBox.Show(mensajePago, lm.ObtenerTexto(f, "Frm.RegistrarFactura.Msg.Informacion.Title", "Información"), MessageBoxButtons.OK, MessageBoxIcon.Information);

                        // Una vez cobrada, no se puede volver a operar sobre la misma factura
                        dgvOrdenes.Enabled = false;
                        grpMedioPago.Enabled = false;
                        grpTarjeta.Enabled = false;
                        btnCobrar.Enabled = false;
                        btnVerComprobante.Enabled = _puedeVerComprobante;
                    }
                    else
                    {
                        MessageBox.Show(mensajePago, lm.ObtenerTexto(f, "Frm.RegistrarFactura.Msg.Aviso.Title", "Aviso"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            });
        }

        // Muestra/oculta el indicador visual de procesando el pago
        private void MostrarProcesando(bool mostrando)
        {
            lblProcesando.Visible = mostrando;
            pbProcesando.Visible = mostrando;

            btnCobrar.Enabled = !mostrando && _puedeCobrar;
            dgvOrdenes.Enabled = !mostrando;
            grpMedioPago.Enabled = !mostrando;
            grpTarjeta.Enabled = !mostrando && rbTarjeta.Checked;
        }

        private void btnVerComprobante_Click(object sender, EventArgs e)
        {
            var lm = Program.LanguageManager;

            if (string.IsNullOrEmpty(_rutaComprobante) || !System.IO.File.Exists(_rutaComprobante))
            {
                MessageBox.Show(
                    lm.ObtenerTexto(f, "Frm.RegistrarFactura.Msg.SinComprobante", "Todavía no se generó ningún comprobante."),
                    lm.ObtenerTexto(f, "Frm.RegistrarFactura.Msg.Aviso.Title", "Aviso"),
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                Process.Start(new ProcessStartInfo(_rutaComprobante) { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    string.Format(lm.ObtenerTexto(f, "Frm.RegistrarFactura.Msg.ErrorAbrirComprobante", "No se pudo abrir el comprobante: {0}"), ex.Message),
                    lm.ObtenerTexto(f, "Frm.RegistrarFactura.Msg.Aviso.Title", "Aviso"),
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        public void ActualizarIdioma()
        {
            var lm = Program.LanguageManager;

            this.Text = lm.ObtenerTexto(f, "Frm.RegistrarFactura.Titulo", "Registrar factura y pago de compra");
            lblTitulo.Text = lm.ObtenerTexto(f, "Frm.RegistrarFactura.Titulo", "Registrar factura y pago de compra");
            grpOrden.Text = lm.ObtenerTexto(f, "Frm.RegistrarFactura.Ordenes", "Órdenes pendientes de facturación");
            lblDetalleFactura.Text = lm.ObtenerTexto(f, "Frm.RegistrarFactura.DetalleFactura", "Detalle a facturar");
            grpMedioPago.Text = lm.ObtenerTexto(f, "Frm.RegistrarFactura.MedioPago", "Medio de pago");
            rbEfectivo.Text = lm.ObtenerTexto(f, "Frm.RegistrarFactura.Efectivo", "Efectivo");
            rbTarjeta.Text = lm.ObtenerTexto(f, "Frm.RegistrarFactura.Tarjeta", "Tarjeta");
            rbTransferencia.Text = lm.ObtenerTexto(f, "Frm.RegistrarFactura.Transferencia", "Transferencia");
            grpTarjeta.Text = lm.ObtenerTexto(f, "Frm.RegistrarFactura.DatosTarjeta", "Datos de la tarjeta");
            lblNumeroTarjeta.Text = lm.ObtenerTexto(f, "Frm.RegistrarFactura.Numero", "Número:");
            lblVencimiento.Text = lm.ObtenerTexto(f, "Frm.RegistrarFactura.Vencimiento", "Vencimiento (MM/AA):");
            lblCVV.Text = lm.ObtenerTexto(f, "Frm.RegistrarFactura.CVV", "CVV:");
            lblProcesando.Text = lm.ObtenerTexto(f, "Frm.RegistrarFactura.Procesando", "Procesando el pago...");
            lblTotalTexto.Text = lm.ObtenerTexto(f, "Frm.RegistrarFactura.Total", "Total:");
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

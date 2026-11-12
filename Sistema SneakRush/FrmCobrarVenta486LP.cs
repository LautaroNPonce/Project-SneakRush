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
    public partial class FrmCobrarVenta486LP : Form, IObserver486LP, IRefrescable486LP
    {
        private BLL_Venta486LP _bllVenta = new BLL_Venta486LP();
        private BLL_Carrito486LP _bllCarrito = new BLL_Carrito486LP();
        private BLL_Cliente486LP _bllCliente = new BLL_Cliente486LP();
        private BLL_Producto486LP _bllProducto = new BLL_Producto486LP();
        private BLL_Perfil486LP _bllPerfil = new BLL_Perfil486LP();
        private readonly string f = "FrmCobrarVenta486LP";
        private Carrito486LP _carritoActual;
        private Venta486LP _ventaGenerada;
        private string _rutaComprobante;
        private bool _puedeSeleccionar;
        private bool _puedeCobrar;
        private bool _puedeVerComprobante;
        private bool _puedeCancelar;
        private bool _clienteRegistrado; // el cliente del carrito seleccionado ya esta registrado (se verifica antes del medio de pago)
        private bool _stockSuficiente; // hay stock para todos los productos del carrito seleccionado (se verifica al seleccionar y al cobrar)
        private bool _formateandoNumero;
        private bool _formateandoVencimiento;

        public FrmCobrarVenta486LP()
        {
            InitializeComponent();
            Ayuda486LP.Vincular(this, "cobrar-venta");
            EstiloBotonHabilitacion(btnSeleccionar);
            EstiloBotonHabilitacion(btnCobrar);
            EstiloBotonHabilitacion(btnVerComprobante);
            Program.LanguageManager.Agregar(this);
            this.FormClosing += FrmCobrarVenta486LP_FormClosing;
            this.Load += FrmCobrarVenta486LP_Load;
            btnSeleccionar.Click += btnSeleccionar_Click;
            btnCobrar.Click += btnCobrar_Click;
            btnVerComprobante.Click += btnVerComprobante_Click;
            btnCancelar.Click += btnCancelar_Click;
            rbEfectivo.CheckedChanged += MedioPago_CheckedChanged;
            rbTarjeta.CheckedChanged += MedioPago_CheckedChanged;
            rbTransferencia.CheckedChanged += MedioPago_CheckedChanged;
            txtNumero.TextChanged += txtNumero_TextChanged;
            txtVencimiento.TextChanged += txtVencimiento_TextChanged;
        }

        // Los botones de colores propios se ven claramente deshabilitados (fondo gris y texto legible) cuando no se pueden usar
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

        private void FrmCobrarVenta486LP_Load(object sender, EventArgs e)
        {
            if (!TienePatenteAcceso())
            {
                var lm = Program.LanguageManager;
                MessageBox.Show(
                    lm.ObtenerTexto(f, "Frm.CobrarVenta.Msg.SinPermiso", "No tiene permiso para cobrar ventas."),
                    lm.ObtenerTexto(f, "Frm.CobrarVenta.Msg.SinPermiso.Title", "Acceso denegado"),
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);

                this.BeginInvoke(new Action(() =>
                {
                    this.DialogResult = DialogResult.Cancel;
                    this.Close();
                }));
                return;
            }

            txtDNI.MaxLength = 8;
            txtDNI.KeyPress += FiltrosTeclado486LP.SoloDigitos; // el DNI solo admite numeros
            ConfigurarColumnas();
            btnVerComprobante.Enabled = false;
            AjustarBotonesSegunPerfil();
            AplicarPermisosBotones();
            ActualizarHabilitacionCobro(); // sin carrito seleccionado no se puede cobrar
            ActualizarIdioma();
        }

        private bool TienePatenteAcceso()
        {
            var usuario = SessionManager486LP.ObtenerInstancia().UsuarioActual();
            if (usuario == null) return false;

            List<string> permisos = _bllPerfil.ObtenerPermisosPorRol(usuario.Rol);
            return permisos.Contains("VENTA_REGISTRAR_OPERACION");
        }

        private void AjustarBotonesSegunPerfil()
        {
            var usuario = SessionManager486LP.ObtenerInstancia().UsuarioActual();
            if (usuario == null) return;

            List<string> permisos = _bllPerfil.ObtenerPermisosPorRol(usuario.Rol);

            _puedeSeleccionar = permisos.Contains("COBRO_SELECCIONAR");
            _puedeCobrar = permisos.Contains("COBRO_COBRAR");
            _puedeVerComprobante = permisos.Contains("COBRO_VERCOMPROBANTE");
            _puedeCancelar = permisos.Contains("COBRO_CANCELAR");
        }

        private void AplicarPermisosBotones()
        {
            btnSeleccionar.Enabled = _puedeSeleccionar;
            btnCobrar.Enabled = _puedeCobrar;
            btnVerComprobante.Enabled = _puedeVerComprobante && _ventaGenerada != null; 
            btnCancelar.Enabled = _puedeCancelar;
        }

        private void MedioPago_CheckedChanged(object sender, EventArgs e)
        {
            if (!rbTarjeta.Checked) LimpiarDatosTarjeta();
            ActualizarHabilitacionCobro();
        }

        private void LimpiarDatosTarjeta()
        {
            txtNumero.Clear();
            txtVencimiento.Clear();
            txtCVV.Clear();
        }

        public void Refrescar()
        {
            if (_ventaGenerada == null || lblProcesando.Visible) return;

            _carritoActual = null;
            _ventaGenerada = null;
            _rutaComprobante = null;
            _clienteRegistrado = false;
            _stockSuficiente = false;

            txtDNI.Clear();
            txtDNI.Enabled = true;
            dgvDetalle.DataSource = null;
            lblTotal.Text = 0m.ToString("C");
            rbEfectivo.Checked = true;
            LimpiarDatosTarjeta();

            AplicarPermisosBotones();
            ActualizarHabilitacionCobro();
            txtDNI.Focus();
        }

        private void ActualizarHabilitacionCobro()
        {
            bool listoParaCobrar = _carritoActual != null && _clienteRegistrado && _stockSuficiente && _ventaGenerada == null;
            btnCobrar.Enabled = listoParaCobrar && _puedeCobrar;
            grpMedioPago.Enabled = listoParaCobrar;
            grpTarjeta.Enabled = listoParaCobrar && rbTarjeta.Checked;
        }

        private void btnSeleccionar_Click(object sender, EventArgs e)
        {
            var lm = Program.LanguageManager;
            string dni = txtDNI.Text.Trim();

            if (!System.Text.RegularExpressions.Regex.IsMatch(dni, @"^\d{7,8}$"))
            {
                MessageBox.Show(
                    lm.ObtenerTexto(f, "Frm.CobrarVenta.Msg.DNIIncorrecto", "DNI incorrecto. Debe tener 7 u 8 dígitos numéricos (sin puntos ni espacios)."),
                    lm.ObtenerTexto(f, "Frm.CobrarVenta.Msg.Validacion.Title", "Validación"),
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            _carritoActual = null;
            _clienteRegistrado = false;
            _stockSuficiente = false;
            dgvDetalle.DataSource = null;
            ActualizarHabilitacionCobro();

            Carrito486LP carrito = _bllCarrito.ObtenerActivoPorDNI(dni);
            if (carrito == null)
            {
                MessageBox.Show(
                    lm.ObtenerTexto(f, "Frm.CobrarVenta.Msg.SinCarritoActivo", "Ese cliente no tiene un carrito activo para cobrar."),
                    lm.ObtenerTexto(f, "Frm.CobrarVenta.Msg.Aviso.Title", "Aviso"),
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            foreach (DetalleCarrito486LP det in carrito.Detalles)
            {
                if (det.Producto == null)
                {
                    det.Producto = _bllProducto.ObtenerPorId(det.IdProducto);
                }
            }

            _carritoActual = carrito;
            MostrarDetalle();
            _clienteRegistrado = VerificarClienteRegistrado(carrito.DNICliente);
            if (_clienteRegistrado)
                _stockSuficiente = VerificarStockCarrito(true);

            ActualizarHabilitacionCobro();
        }

        private bool VerificarStockCarrito(bool mostrarMensaje)
        {
            string mensaje;
            bool alcanza = _bllVenta.VerificarStockCarrito(_carritoActual, out mensaje);

            if (!alcanza && mostrarMensaje)
            {
                var lm = Program.LanguageManager;
                MessageBox.Show(
                    mensaje,
                    lm.ObtenerTexto(f, "Frm.CobrarVenta.Msg.StockInsuficiente.Title", "Stock insuficiente"),
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            return alcanza;
        }

        private bool VerificarClienteRegistrado(string dni)
        {
            var lm = Program.LanguageManager;

            if (_bllVenta.ClienteRegistrado(dni)) return true;

            MessageBox.Show(
                string.Format(lm.ObtenerTexto(f, "Frm.CobrarVenta.Msg.ClienteNoRegistrado", "El cliente con DNI {0} no está registrado. Debe registrarlo para continuar con el cobro."), dni),
                lm.ObtenerTexto(f, "Frm.CobrarVenta.Msg.ClienteNoRegistrado.Title", "Cliente no registrado"),
                MessageBoxButtons.OK, MessageBoxIcon.Information);

            using (FrmGestionClientes486LP frmClientes = new FrmGestionClientes486LP(dni))
            {
                frmClientes.ShowDialog(this);
            }

            if (_bllVenta.ClienteRegistrado(dni)) return true;

            MessageBox.Show(
                string.Format(lm.ObtenerTexto(f, "Frm.CobrarVenta.Msg.ClienteSinRegistrar", "El cliente con DNI {0} sigue sin estar registrado, por lo que no se puede cobrar. Si el cliente no desea registrarse, use Cancelar para cancelar el carrito."), dni),
                lm.ObtenerTexto(f, "Frm.CobrarVenta.Msg.Aviso.Title", "Aviso"),
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return false;
        }

        private void MostrarDetalle()
        {
            dgvDetalle.AutoGenerateColumns = false;
            dgvDetalle.DataSource = null;
            dgvDetalle.DataSource = new BindingList<DetalleCarrito486LP>(_carritoActual.Detalles);
            lblTotal.Text = _carritoActual.Total.ToString("C");
        }

        private void ConfigurarColumnas()
        {
            dgvDetalle.AutoGenerateColumns = false;
            dgvDetalle.Columns.Clear();
            dgvDetalle.ReadOnly = true;

            dgvDetalle.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Marca", Name = "colMarca" });
            dgvDetalle.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Modelo", Name = "colModelo" });
            dgvDetalle.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Color", Name = "colColor" });
            dgvDetalle.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Talle", Name = "colTalle" });
            dgvDetalle.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Cantidad", Name = "colCantidad" });
            dgvDetalle.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Precio", Name = "colPrecio", DefaultCellStyle = new DataGridViewCellStyle { Format = "C2", Alignment = DataGridViewContentAlignment.MiddleRight } });
            dgvDetalle.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Subtotal", Name = "colSubtotal", DefaultCellStyle = new DataGridViewCellStyle { Format = "C2", Alignment = DataGridViewContentAlignment.MiddleRight } });

            AplicarEncabezadosColumnas();
        }

        private void AplicarEncabezadosColumnas()
        {
            if (dgvDetalle.Columns.Count == 0) return;

            var lm = Program.LanguageManager;
            dgvDetalle.Columns["colMarca"].HeaderText = lm.ObtenerTexto(f, "Frm.CobrarVenta.Col.Marca", "Marca");
            dgvDetalle.Columns["colModelo"].HeaderText = lm.ObtenerTexto(f, "Frm.CobrarVenta.Col.Modelo", "Modelo");
            dgvDetalle.Columns["colColor"].HeaderText = lm.ObtenerTexto(f, "Frm.CobrarVenta.Col.Color", "Color");
            dgvDetalle.Columns["colTalle"].HeaderText = lm.ObtenerTexto(f, "Frm.CobrarVenta.Col.Talle", "Talle");
            dgvDetalle.Columns["colCantidad"].HeaderText = lm.ObtenerTexto(f, "Frm.CobrarVenta.Col.Cantidad", "Cantidad");
            dgvDetalle.Columns["colPrecio"].HeaderText = lm.ObtenerTexto(f, "Frm.CobrarVenta.Col.Precio", "Precio");
            dgvDetalle.Columns["colSubtotal"].HeaderText = lm.ObtenerTexto(f, "Frm.CobrarVenta.Col.Subtotal", "Subtotal");
        }


        private string ValidarDatosTarjeta()
        {
            string numeroSinEspacios = txtNumero.Text.Replace(" ", "").Trim();
            if (!System.Text.RegularExpressions.Regex.IsMatch(numeroSinEspacios, @"^\d{16}$"))
                return Mensajes486LP.Obtener("Tarjeta.Numero");

            var match = System.Text.RegularExpressions.Regex.Match(txtVencimiento.Text.Trim(), @"^(0[1-9]|1[0-2])/(\d{2})$");
            if (!match.Success)
                return Mensajes486LP.Obtener("Tarjeta.Vencimiento");

            int mes = int.Parse(match.Groups[1].Value);
            int anio = 2000 + int.Parse(match.Groups[2].Value);
            DateTime finDeMes = new DateTime(anio, mes, DateTime.DaysInMonth(anio, mes));
            if (finDeMes < DateTime.Now.Date)
                return Mensajes486LP.Obtener("Tarjeta.Vencida"); // tarjeta vencida

            if (!System.Text.RegularExpressions.Regex.IsMatch(txtCVV.Text.Trim(), @"^\d{3}$"))
                return Mensajes486LP.Obtener("Tarjeta.CVV");

            return null;
        }

        private void txtNumero_TextChanged(object sender, EventArgs e)
        {
            if (_formateandoNumero) return;
            _formateandoNumero = true;

            string soloDigitos = new string(txtNumero.Text.Where(char.IsDigit).ToArray());
            if (soloDigitos.Length > 16) soloDigitos = soloDigitos.Substring(0, 16);

            StringBuilder formateado = new StringBuilder();
            for (int i = 0; i < soloDigitos.Length; i++)
            {
                if (i > 0 && i % 4 == 0) formateado.Append(' ');
                formateado.Append(soloDigitos[i]);
            }

            txtNumero.Text = formateado.ToString();
            txtNumero.SelectionStart = txtNumero.Text.Length;

            _formateandoNumero = false;
        }

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

            if (_carritoActual == null)
            {
                MessageBox.Show(
                    lm.ObtenerTexto(f, "Frm.CobrarVenta.Msg.SeleccionarPrimero", "Primero seleccione un cliente con carrito activo."),
                    lm.ObtenerTexto(f, "Frm.CobrarVenta.Msg.Aviso.Title", "Aviso"),
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!VerificarStockCarrito(true))
            {
                _stockSuficiente = false;
                ActualizarHabilitacionCobro();
                return;
            }

            string medioPago = ObtenerMedioPagoSeleccionado();

            if (medioPago == "Tarjeta")
            {
                string errorTarjeta = ValidarDatosTarjeta();
                if (errorTarjeta != null)
                {
                    MessageBox.Show(
                        errorTarjeta,
                        lm.ObtenerTexto(f, "Frm.CobrarVenta.Msg.Validacion.Title", "Validación"),
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            MostrarProcesando(true);

            IEstrategiaPago486LP estrategia = FabricaEstrategiaPago486LP.Crear(medioPago);
            estrategia.ProcesarPago(_carritoActual.Total, (aprobado) =>
            {
                MostrarProcesando(false);

                if (aprobado)
                {
                    string mensaje;
                    Venta486LP venta = _bllVenta.RegistrarVenta(_carritoActual, medioPago, out mensaje);

                    if (venta != null)
                    {
                        _ventaGenerada = venta;
                        bool comprobanteGenerado = GenerarComprobante();

                        MessageBox.Show(mensaje, lm.ObtenerTexto(f, "Frm.CobrarVenta.Msg.Informacion.Title", "Información"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                        if (!comprobanteGenerado)
                        {
                            MessageBox.Show(
                                Mensajes486LP.Obtener("Comprobante.ErrorGenerar"),
                                lm.ObtenerTexto(f, "Frm.CobrarVenta.Msg.Aviso.Title", "Aviso"),
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }

                        txtDNI.Enabled = false;
                        btnSeleccionar.Enabled = false;
                        grpMedioPago.Enabled = false;
                        grpTarjeta.Enabled = false;
                        btnCobrar.Enabled = false;
                        btnVerComprobante.Enabled = _puedeVerComprobante;
                    }
                    else
                    {
                        MessageBox.Show(mensaje, lm.ObtenerTexto(f, "Frm.CobrarVenta.Msg.Aviso.Title", "Aviso"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        string detalleStock;
                        _stockSuficiente = _bllVenta.VerificarStockCarrito(_carritoActual, out detalleStock);
                        ActualizarHabilitacionCobro();
                    }
                }
            });
        }

        private bool GenerarComprobante()
        {
            try
            {
                Cliente486LP cliente = _bllCliente.Obtener(_ventaGenerada.DNICliente);
                _rutaComprobante = GeneradorComprobantePDF486LP.Generar(_ventaGenerada, cliente);
                return !string.IsNullOrEmpty(_rutaComprobante);
            }
            catch (Exception ex)
            {
                _rutaComprobante = null;
                new BLL_Bitacora486LP().Registrar(new BitacoraEvento486LP("Ventas",
                    $"Error al generar el comprobante PDF de la venta {_ventaGenerada.NroComprobante}: {ex.Message}",
                    Criticidad486LP.MuyAlta, "Sistema", "Sistema"));
                return false;
            }
        }

        private void MostrarProcesando(bool mostrando)
        {
            lblProcesando.Visible = mostrando;
            pbProcesando.Visible = mostrando;
            btnSeleccionar.Enabled = !mostrando && _puedeSeleccionar;
            btnCancelar.Enabled = !mostrando && _puedeCancelar; // no se puede cancelar el carrito mientras se procesa el pago
            if (mostrando)
            {
                btnCobrar.Enabled = false;
                grpMedioPago.Enabled = false;
                grpTarjeta.Enabled = false;
            }
            else
            {
                ActualizarHabilitacionCobro();
            }
        }

        private void btnVerComprobante_Click(object sender, EventArgs e)
        {
            var lm = Program.LanguageManager;

            if ((string.IsNullOrEmpty(_rutaComprobante) || !System.IO.File.Exists(_rutaComprobante)) && _ventaGenerada != null)
            {
                if (!GenerarComprobante())
                {
                    MessageBox.Show(
                        Mensajes486LP.Obtener("Comprobante.ErrorGenerar"),
                        lm.ObtenerTexto(f, "Frm.CobrarVenta.Msg.Aviso.Title", "Aviso"),
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            if (string.IsNullOrEmpty(_rutaComprobante) || !System.IO.File.Exists(_rutaComprobante))
            {
                MessageBox.Show(
                    lm.ObtenerTexto(f, "Frm.CobrarVenta.Msg.SinComprobante", "Todavía no se generó ningún comprobante."),
                    lm.ObtenerTexto(f, "Frm.CobrarVenta.Msg.Aviso.Title", "Aviso"),
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                Process.Start(new ProcessStartInfo(_rutaComprobante) { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                new BLL_Bitacora486LP().Registrar(new BitacoraEvento486LP("Ventas",
                    $"Error al abrir el comprobante PDF {_rutaComprobante}: {ex.Message}",
                    Criticidad486LP.Media, "Sistema", "Sistema"));
                MessageBox.Show(
                    Mensajes486LP.Obtener("Comprobante.ErrorAbrir"),
                    lm.ObtenerTexto(f, "Frm.CobrarVenta.Msg.Aviso.Title", "Aviso"),
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            if (_carritoActual == null || _ventaGenerada != null)
            {
                this.Close();
                return;
            }

            var lm = Program.LanguageManager;

            DialogResult r = MessageBox.Show(
                string.Format(lm.ObtenerTexto(f, "Frm.CobrarVenta.Msg.ConfirmarCancelarCarrito", "¿Confirma la cancelación del carrito del cliente con DNI {0}? El carrito quedará en estado Cancelado."), _carritoActual.DNICliente),
                lm.ObtenerTexto(f, "Frm.CobrarVenta.Msg.ConfirmarCancelarCarrito.Title", "Cancelar carrito"),
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (r != DialogResult.Yes) return;

            string mensaje;
            bool ok = _bllVenta.CancelarCarrito(_carritoActual.IdCarrito, out mensaje);

            MessageBox.Show(
                mensaje,
                lm.ObtenerTexto(f, ok ? "Frm.CobrarVenta.Msg.Informacion.Title" : "Frm.CobrarVenta.Msg.Aviso.Title", ok ? "Información" : "Aviso"),
                MessageBoxButtons.OK, ok ? MessageBoxIcon.Information : MessageBoxIcon.Warning);

            if (ok) this.Close();
        }

        public void ActualizarIdioma()
        {
            var lm = Program.LanguageManager;

            this.Text = lm.ObtenerTexto(f, "Frm.CobrarVenta.Titulo", "Cobrar venta");
            lblTitulo.Text = lm.ObtenerTexto(f, "Frm.CobrarVenta.Titulo", "Cobrar venta");
            grpCliente.Text = lm.ObtenerTexto(f, "Frm.CobrarVenta.Cliente", "Cliente");
            lblDNI.Text = lm.ObtenerTexto(f, "Frm.CobrarVenta.DNI", "DNI:");
            btnSeleccionar.Text = lm.ObtenerTexto(f, "Frm.CobrarVenta.Seleccionar", "Seleccionar");
            lblDetalle.Text = lm.ObtenerTexto(f, "Frm.CobrarVenta.Detalle", "Detalle de la venta");
            grpMedioPago.Text = lm.ObtenerTexto(f, "Frm.CobrarVenta.MetodoPago", "Método de pago");
            rbEfectivo.Text = lm.ObtenerTexto(f, "Frm.CobrarVenta.Efectivo", "Efectivo");
            rbTarjeta.Text = lm.ObtenerTexto(f, "Frm.CobrarVenta.Tarjeta", "Tarjeta");
            rbTransferencia.Text = lm.ObtenerTexto(f, "Frm.CobrarVenta.Transferencia", "Transferencia");
            grpTarjeta.Text = lm.ObtenerTexto(f, "Frm.CobrarVenta.DatosTarjeta", "Datos de la tarjeta");
            lblNumero.Text = lm.ObtenerTexto(f, "Frm.CobrarVenta.Numero", "Número:");
            lblVencimiento.Text = lm.ObtenerTexto(f, "Frm.CobrarVenta.Vencimiento", "Vencimiento (MM/AA):");
            lblCVV.Text = lm.ObtenerTexto(f, "Frm.CobrarVenta.CVV", "CVV:");
            lblProcesando.Text = lm.ObtenerTexto(f, "Frm.CobrarVenta.Procesando", "Procesando pago...");
            lblTotalTexto.Text = lm.ObtenerTexto(f, "Frm.CobrarVenta.Total", "Total:");
            btnCobrar.Text = lm.ObtenerTexto(f, "Frm.CobrarVenta.Cobrar", "Cobrar");
            btnVerComprobante.Text = lm.ObtenerTexto(f, "Frm.CobrarVenta.VerComprobante", "Ver comprobante");
            btnCancelar.Text = lm.ObtenerTexto(f, "Frm.CobrarVenta.Cancelar", "Cancelar");

            AplicarEncabezadosColumnas();
        }

        private void FrmCobrarVenta486LP_FormClosing(object sender, FormClosingEventArgs e)
        {
            Program.LanguageManager.Quitar(this);
        }

        private void lblVencimiento_Click(object sender, EventArgs e)
        {

        }
    }
}

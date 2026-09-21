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
    public partial class FrmCobrarVenta486LP : Form, IObserver486LP
    {
        private BLL_Venta486LP _bllVenta = new BLL_Venta486LP();
        private BLL_Carrito486LP _bllCarrito = new BLL_Carrito486LP();
        private BLL_Cliente486LP _bllCliente = new BLL_Cliente486LP();
        private BLL_Producto486LP _bllProducto = new BLL_Producto486LP();
        private BLL_Perfil486LP _bllPerfil = new BLL_Perfil486LP();
        private readonly string f = "FrmCobrarVenta486LP";

        // El carrito que se esta cobrando (llenado en btnSeleccionar_Click), la venta ya generada
        // (llenada recien despues de un cobro exitoso) y la ruta del PDF ya generado para esa venta.
        private Carrito486LP _carritoActual;
        private Venta486LP _ventaGenerada;
        private string _rutaComprobante;

        // Patentes granulares por boton (ademas de la patente de acceso al form, VENTA_REGISTRAR_OPERACION).
        private bool _puedeSeleccionar;
        private bool _puedeCobrar;
        private bool _puedeVerComprobante;
        private bool _puedeCancelar;
        private bool _formateandoNumero;
        private bool _formateandoVencimiento;

        public FrmCobrarVenta486LP()
        {
            InitializeComponent();
            Program.LanguageManager.Agregar(this);
            this.FormClosing += FrmCobrarVenta486LP_FormClosing;
            this.Load += FrmCobrarVenta486LP_Load;

            // Los eventos se conectan aca (no en el Designer, que se armo sin logica todavia).
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

        private void FrmCobrarVenta486LP_Load(object sender, EventArgs e)
        {
            // Verificacion de acceso al form completo (mismo patron que los otros 3 CUN).
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

            ConfigurarColumnas();
            btnVerComprobante.Enabled = false;
            AjustarBotonesSegunPerfil();
            AplicarPermisosBotones();
            ActualizarIdioma();
        }

        // Verifica que el usuario en sesion tenga la patente de acceso al form.
        private bool TienePatenteAcceso()
        {
            var usuario = SessionManager486LP.ObtenerInstancia().UsuarioActual();
            if (usuario == null) return false;

            List<string> permisos = _bllPerfil.ObtenerPermisosPorRol(usuario.Rol);
            return permisos.Contains("VENTA_REGISTRAR_OPERACION");
        }

        // Patentes granulares por boton.
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
            // "Ver comprobante" ademas depende de que ya haya una venta generada.
            btnVerComprobante.Enabled = _puedeVerComprobante && _ventaGenerada != null;
            btnCancelar.Enabled = _puedeCancelar;
        }

        // Habilita/deshabilita el panel de datos de tarjeta segun el medio de pago elegido.
        private void MedioPago_CheckedChanged(object sender, EventArgs e)
        {
            grpTarjeta.Enabled = rbTarjeta.Checked;
        }

        // ---------------- Seleccionar (buscar carrito por DNI) ----------------
        private void btnSeleccionar_Click(object sender, EventArgs e)
        {
            var lm = Program.LanguageManager;
            string dni = txtDNI.Text.Trim();

            // 3.1: formato de DNI invalido
            if (!System.Text.RegularExpressions.Regex.IsMatch(dni, @"^\d{7,8}$"))
            {
                MessageBox.Show(
                    lm.ObtenerTexto(f, "Frm.CobrarVenta.Msg.DNIIncorrecto", "DNI incorrecto."),
                    lm.ObtenerTexto(f, "Frm.CobrarVenta.Msg.Validacion.Title", "Validación"),
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Paso 4: recuperar el carrito "Activo" de ese DNI. El cliente ya quedo asignado
            // al carrito en CUN02 (el include a CUN03 vive ahi, ya no en este form) - aca no
            // hace falta verificar/registrar cliente, solo buscar el carrito.
            Carrito486LP carrito = _bllCarrito.ObtenerActivoPorDNI(dni);
            if (carrito == null)
            {
                // 3.2: sin carrito activo para ese DNI (o el DNI no corresponde a ningun cliente).
                MessageBox.Show(
                    lm.ObtenerTexto(f, "Frm.CobrarVenta.Msg.SinCarritoActivo", "Ese cliente no tiene un carrito activo para cobrar."),
                    lm.ObtenerTexto(f, "Frm.CobrarVenta.Msg.Aviso.Title", "Aviso"),
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // El Producto de cada detalle llega en null desde el Mapper de Carrito - se resuelve aca
            // para poder mostrar Marca/Modelo/Color/Talle en la grilla (solo lectura).
            foreach (DetalleCarrito486LP det in carrito.Detalles)
            {
                if (det.Producto == null)
                {
                    det.Producto = _bllProducto.ObtenerPorId(det.IdProducto);
                }
            }

            _carritoActual = carrito;
            MostrarDetalle();
        }

        private void MostrarDetalle()
        {
            dgvDetalle.AutoGenerateColumns = false;
            dgvDetalle.DataSource = null;
            dgvDetalle.DataSource = new BindingList<DetalleCarrito486LP>(_carritoActual.Detalles);
            lblTotal.Text = _carritoActual.Total.ToString("C");
        }

        // Define las columnas de la grilla de detalle (solo lectura - el carrito ya viene armado de CUN02).
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
            dgvDetalle.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Precio", Name = "colPrecio" });
            dgvDetalle.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Subtotal", Name = "colSubtotal" });

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

        // 8.1: valida los datos de la tarjeta (numero, vencimiento, CVV). Solo se llama si el medio
        // de pago elegido es Tarjeta.
        private bool ValidarDatosTarjeta()
        {
            string numeroSinEspacios = txtNumero.Text.Replace(" ", "").Trim();
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

        // Formatea el numero de tarjeta en grupos de 4 mientras se escribe (ej. "1234 5678 9012 3456").
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

        // Inserta el "/" automaticamente despues de los primeros 2 digitos (MM/AA).
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

        // ---------------- Cobrar ----------------
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

            string medioPago = ObtenerMedioPagoSeleccionado();

            if (medioPago == "Tarjeta" && !ValidarDatosTarjeta())
            {
                MessageBox.Show(
                    lm.ObtenerTexto(f, "Frm.CobrarVenta.Msg.DatosTarjetaInvalidos", "Datos de tarjeta inválidos."),
                    lm.ObtenerTexto(f, "Frm.CobrarVenta.Msg.Validacion.Title", "Validación"),
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // A partir de aca el pago es ASINCRONO (Efectivo resuelve al instante, Tarjeta/Transferencia
            // usan un Hilo de ~3s) - se muestra el indicador de "Procesando" mientras tanto. La estrategia
            // vuelve sola al hilo de UI (via SynchronizationContext), por eso el callback puede tocar
            // controles del form sin problema, sin que la BLL necesite conocer el Form en absoluto.
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

                        // Paso 6/12: se genera el comprobante (PDF, formato ticket) apenas se
                        // confirma la venta - "Ver comprobante" despues solo lo vuelve a abrir.
                        Cliente486LP cliente = _bllCliente.Obtener(venta.DNICliente);
                        _rutaComprobante = GeneradorComprobantePDF486LP.Generar(venta, cliente);

                        MessageBox.Show(mensaje, lm.ObtenerTexto(f, "Frm.CobrarVenta.Msg.Informacion.Title", "Información"), MessageBoxButtons.OK, MessageBoxIcon.Information);

                        // Una vez cobrada, no se puede volver a operar sobre la misma venta.
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
                    }
                }
            });
        }

        // Muestra/oculta el indicador visual de "Procesando pago..." (barra + texto) y bloquea los
        // controles mientras dura, para que no se pueda tocar nada a mitad del procesamiento.
        private void MostrarProcesando(bool mostrando)
        {
            lblProcesando.Visible = mostrando;
            pbProcesando.Visible = mostrando;

            btnCobrar.Enabled = !mostrando && _puedeCobrar;
            btnSeleccionar.Enabled = !mostrando && _puedeSeleccionar;
            grpMedioPago.Enabled = !mostrando;
            grpTarjeta.Enabled = !mostrando && rbTarjeta.Checked;
        }

        // ---------------- Ver comprobante ----------------
        // Abre el PDF ya generado (en btnCobrar_Click) con el visor predeterminado del sistema.
        private void btnVerComprobante_Click(object sender, EventArgs e)
        {
            var lm = Program.LanguageManager;

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
                MessageBox.Show(
                    string.Format(lm.ObtenerTexto(f, "Frm.CobrarVenta.Msg.ErrorAbrirComprobante", "No se pudo abrir el comprobante: {0}"), ex.Message),
                    lm.ObtenerTexto(f, "Frm.CobrarVenta.Msg.Aviso.Title", "Aviso"),
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ---------------- Cancelar ----------------
        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
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

using BLL;
using Services;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Sistema_SneakRush
{
    public partial class FrmMenuPrincipal486LP : Form, IObserver486LP
    {
        private bool _hayInconsistencia;

        public FrmMenuPrincipal486LP(bool hayInconsistencia = false)
        {
            InitializeComponent();
            this.IsMdiContainer = true;
            this.MdiChildActivate += FrmMenuPrincipal486LP_MdiChildActivate;
            this.Shown += FrmMenuPrincipal486LP_Shown;
            _hayInconsistencia = hayInconsistencia;
            Program.LanguageManager.Agregar(this);
        }

        private void FrmMenuPrincipal486LP_Load(object sender, System.EventArgs e)
        {
            foreach (Control c in this.Controls)
            {
                if (c is MdiClient mdiCliente)
                {
                    mdiCliente.BackColor = this.BackColor;
                    break;
                }
            }

            ActualizarEstado();
            AjustarMenuSegunPerfil();
            ActualizarIdioma();
            AgregarAccentNaranja();
        }

        public void ActualizarEstado()
        {
            var usuario = SessionManager486LP.ObtenerInstancia().UsuarioActual();
            if (usuario != null)
            {
                var lm = Program.LanguageManager;
                string f = "FrmMenuPrincipal486LP";
                string nombreCompleto = $"{usuario.Nombre} {usuario.Apellido}";
                lblEstado.Text = string.Format(lm.ObtenerTexto(f, "lbl.BarraEstado", "Usuario: {0}  |  Rol: {1}"),nombreCompleto, usuario.Rol);
            }
        }

        private void AjustarMenuSegunPerfil()
        {
            var usuario = SessionManager486LP.ObtenerInstancia().UsuarioActual();
            if (usuario == null)
            {
                return;
            }

            if (_hayInconsistencia)
            {
                // Solo Cambiar contraseña y Cerrar sesión hasta recalcular DV o restaurar la BD
                usuarioToolStripMenuItem.Visible = true;
                cambiarContraseñaToolStripMenuItem.Visible = true;
                cerrarSesiónToolStripMenuItem.Visible = true;
                cambiarIdiomaToolStripMenuItem.Visible = true;
                iniciarSesionToolStripMenuItem.Visible = false;

                ayudaToolStripMenuItem.Visible = false;
                adminToolStripMenuItem.Visible = false;
                gestionToolStripMenuItem.Visible = false;
                compraToolStripMenuItem.Visible = false;
                ventaToolStripMenuItem.Visible = false;
                reporteToolStripMenuItem.Visible = false;
                return;
            }

            BLL_Perfil486LP bllPerfil = new BLL_Perfil486LP();
            List<string> permisos = bllPerfil.ObtenerPermisosPorRol(usuario.Rol);

            usuarioToolStripMenuItem.Visible = true;
            ayudaToolStripMenuItem.Visible = true;
            iniciarSesionToolStripMenuItem.Visible = permisos.Contains("USUARIO_INICIAR_SESION");

            bool verAdmin = permisos.Contains("GESTION_USUARIOS")
                         || permisos.Contains("GESTION_FAMILIAS")
                         || permisos.Contains("GESTION_ROLES")
                         || permisos.Contains("BITACORA_EVENTOS")
                         || permisos.Contains("GESTION_RESPALDOS");

            adminToolStripMenuItem.Visible = verAdmin;

            if (verAdmin)
            {
                gestiónDeUsuariosToolStripMenuItem.Visible = permisos.Contains("GESTION_USUARIOS");
                gestiónDePerfilesToolStripMenuItem.Visible = permisos.Contains("GESTION_FAMILIAS") || permisos.Contains("GESTION_ROLES");
                gestionDeFamiliasToolStripMenuItem.Visible = permisos.Contains("GESTION_FAMILIAS");
                gestionDeRolesToolStripMenuItem.Visible = permisos.Contains("GESTION_ROLES");
                bitácoraDeEventosToolStripMenuItem.Visible = permisos.Contains("BITACORA_EVENTOS");
                gestiónDeRespaldosToolStripMenuItem.Visible = permisos.Contains("GESTION_RESPALDOS");
            }

            bool verMaestro = permisos.Contains("MAESTRO_CLIENTES")
                           || permisos.Contains("MAESTRO_PRODUCTOS")
                           || permisos.Contains("MAESTRO_PROVEEDORES")
                           || permisos.Contains("MAESTRO_CATEGORIAS");

            gestionToolStripMenuItem.Visible = verMaestro;

            if (verMaestro)
            {
                clientesToolStripMenuItem.Visible = permisos.Contains("MAESTRO_CLIENTES");
                productosToolStripMenuItem.Visible = permisos.Contains("MAESTRO_PRODUCTOS");
                proveedoresToolStripMenuItem.Visible = permisos.Contains("MAESTRO_PROVEEDORES");
                categoríasMarcasToolStripMenuItem.Visible = permisos.Contains("MAESTRO_CATEGORIAS");
            }

            bool verCompra = permisos.Contains("COMPRA_GENERAR_SOLICITUD")
              || permisos.Contains("COMPRA_REGISTRAR_ORDEN")
              || permisos.Contains("COMPRA_REGISTRAR_RECEPCION")
              || permisos.Contains("COMPRA_REGISTRAR_FACTURA");

            compraToolStripMenuItem.Visible = verCompra;

            if (verCompra)
            {
                generarSolicitudDeCompraToolStripMenuItem.Visible = permisos.Contains("COMPRA_GENERAR_SOLICITUD");
                registrarOrdenDeCompraToolStripMenuItem.Visible = permisos.Contains("COMPRA_REGISTRAR_ORDEN");
                registrarRecepciónDeMercaderíaToolStripMenuItem.Visible = permisos.Contains("COMPRA_REGISTRAR_RECEPCION");
                registrarFacturaYPagoDeCompraToolStripMenuItem.Visible = permisos.Contains("COMPRA_REGISTRAR_FACTURA");
            }

            bool verVenta = permisos.Contains("VENTA_GESTIONAR_CARRITO")
                         || permisos.Contains("VENTA_REGISTRAR_OPERACION")
                         || permisos.Contains("VENTA_CONSULTAR_PRODUCTOS");

            ventaToolStripMenuItem.Visible = verVenta;

            if (verVenta)
            {
                gestionarCarritoToolStripMenuItem.Visible = permisos.Contains("VENTA_GESTIONAR_CARRITO");
                registrarOperaciónDeVentaToolStripMenuItem.Visible = permisos.Contains("VENTA_REGISTRAR_OPERACION");
                consultarProductosToolStripMenuItem.Visible = permisos.Contains("VENTA_CONSULTAR_PRODUCTOS");
            }

            bool verReporte = permisos.Contains("REPORTE_VENTAS")
                           || permisos.Contains("REPORTE_COMPRAS")
                           || permisos.Contains("REPORTE_INTELIGENTE");

            reporteToolStripMenuItem.Visible = verReporte;

            if (verReporte)
            {
                reporteDeVentasToolStripMenuItem.Visible = permisos.Contains("REPORTE_VENTAS");
                reporteDeComprasToolStripMenuItem.Visible = permisos.Contains("REPORTE_COMPRAS");
                reporteInteligenteToolStripMenuItem.Visible = permisos.Contains("REPORTE_INTELIGENTE");
            }

            ActualizarAvisoStock();
        }

        private System.Drawing.Bitmap _iconoAdvertencia;

        // Icono rojo de advertencia dibujado en codigo (triangulo con un signo de exclamacion)
        private System.Drawing.Bitmap ObtenerIconoAdvertencia()
        {
            if (_iconoAdvertencia != null) return _iconoAdvertencia;

            System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(16, 16);
            using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bmp))
            {
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                g.Clear(System.Drawing.Color.Transparent);

                System.Drawing.Point[] triangulo =
                {
                    new System.Drawing.Point(8, 1),
                    new System.Drawing.Point(15, 14),
                    new System.Drawing.Point(1, 14)
                };
                using (System.Drawing.SolidBrush relleno = new System.Drawing.SolidBrush(System.Drawing.Color.FromArgb(200, 30, 30)))
                    g.FillPolygon(relleno, triangulo);
                using (System.Drawing.Pen borde = new System.Drawing.Pen(System.Drawing.Color.FromArgb(140, 0, 0), 1f))
                    g.DrawPolygon(borde, triangulo);

                using (System.Drawing.Font fuente = new System.Drawing.Font("Segoe UI", 9f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel))
                using (System.Drawing.StringFormat centro = new System.Drawing.StringFormat())
                {
                    centro.Alignment = System.Drawing.StringAlignment.Center;
                    centro.LineAlignment = System.Drawing.StringAlignment.Center;
                    g.DrawString("!", fuente, System.Drawing.Brushes.White, new System.Drawing.RectangleF(0, 4, 16, 11), centro);
                }
            }

            _iconoAdvertencia = bmp;
            return _iconoAdvertencia;
        }

        // Muestra (o saca) el icono rojo en Reporte y Estado de stock segun haya productos para reponer.
        // Solo aplica a quien puede ver el reporte de estado de stock.
        private void ActualizarAvisoStock()
        {
            try
            {
                bool puedeVer = !_hayInconsistencia
                    && SessionManager486LP.ObtenerInstancia().UsuarioActual() != null
                    && reporteToolStripMenuItem.Available
                    && reporteDeComprasToolStripMenuItem.Available;

                int sinStock = 0, stockBajo = 0;
                if (puedeVer)
                {
                    int[] cantidades = new BLL_Reporte486LP().ContarProductosAReponer();
                    sinStock = cantidades[0];
                    stockBajo = cantidades[1];
                }

                int total = sinStock + stockBajo;
                if (total > 0)
                {
                    var lm = Program.LanguageManager;
                    string tooltip = string.Format(
                        lm.ObtenerTexto("FrmMenuPrincipal486LP", "Aviso.Stock.Tooltip", "Hay {0} producto(s) para reponer: revise el reporte de estado de stock."),
                        total);

                    reporteToolStripMenuItem.Image = ObtenerIconoAdvertencia();
                    reporteToolStripMenuItem.ToolTipText = tooltip;
                    reporteDeComprasToolStripMenuItem.Image = ObtenerIconoAdvertencia();
                    reporteDeComprasToolStripMenuItem.ToolTipText = tooltip;
                }
                else
                {
                    reporteToolStripMenuItem.Image = null;
                    reporteToolStripMenuItem.ToolTipText = null;
                    reporteDeComprasToolStripMenuItem.Image = null;
                    reporteDeComprasToolStripMenuItem.ToolTipText = null;
                }
            }
            catch
            {
                // El icono es solo un aviso: si algo falla no tiene que impedir el uso del menu
            }
        }

        // Al iniciar sesion avisa si hay productos sin stock o con stock bajo y ofrece abrir el reporte
        private void FrmMenuPrincipal486LP_Shown(object sender, System.EventArgs e)
        {
            if (_hayInconsistencia || !reporteDeComprasToolStripMenuItem.Available || !reporteToolStripMenuItem.Available)
                return;

            int[] cantidades = new BLL_Reporte486LP().ContarProductosAReponer();
            if (cantidades[0] + cantidades[1] == 0)
                return;

            var lm = Program.LanguageManager;
            string f = "FrmMenuPrincipal486LP";
            string mensaje = string.Format(
                lm.ObtenerTexto(f, "Aviso.Stock.Mensaje", "Productos sin stock: {0}\nProductos con stock bajo ({2} unidades o menos): {1}\n\n¿Desea abrir el reporte de estado de stock para ver qué reponer?"),
                cantidades[0], cantidades[1], BLL_Reporte486LP.UMBRAL_STOCK_BAJO);

            DialogResult respuesta = MessageBox.Show(
                mensaje,
                lm.ObtenerTexto(f, "Aviso.Stock.Titulo", "Aviso de stock"),
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (respuesta != DialogResult.Yes)
                return;

            AbrirFormulario(new FrmReporteCompras486LP());
            foreach (Form hijo in this.MdiChildren)
            {
                FrmReporteCompras486LP reporte = hijo as FrmReporteCompras486LP;
                if (reporte != null)
                {
                    reporte.MostrarSoloAReponer();
                    break;
                }
            }
        }

        private void FrmMenuPrincipal486LP_FormClosing(object sender, System.Windows.Forms.FormClosingEventArgs e)
        {
            Program.LanguageManager.Quitar(this);
        }

        // Menu Usuario
        private void cambiarContraseñaToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            AbrirFormulario(new FrmCambiarContraseña486LP());
        }
        private void cerrarSesiónToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            AbrirFormulario(new FrmCerrarSesion486LP());
        }

        // Menu Administrador
        private void gestiónDeUsuariosToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            AbrirFormulario(new FrmGestionUsuarios486LP());
        }
        private void gestionDeFamiliasToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            AbrirFormulario(new FrmGestionFamilias486LP());
        }
        private void gestionDeRolesToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            AbrirFormulario(new FrmGestionPerfiles486LP());
        }
        private void bitácoraDeEventosToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            AbrirFormulario(new FrmBitacoraEvento486LP());
        }
        private void gestiónDeRespaldosToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            AbrirFormulario(new FrmGestionRespaldo486LP());
        }

        // Menu Maestro
        private void clientesToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            AbrirFormulario(new FrmGestionClientes486LP());
        }
        private void productosToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            AbrirFormulario(new FrmGestionProductos486LP());
        }
        private void categoríasMarcasToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            AbrirFormulario(new FrmGestionCategoriasMarcas486LP());
        }
        private void proveedoresToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            AbrirFormulario(new FrmGestionProveedores486LP());
        }

        // Menu Compra
        private void generarSolicitudDeCompraToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            AbrirFormulario(new FrmGenerarSolicitud486LP());
        }
        private void registrarOrdenDeCompraToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            // Igual que en la recepción: si no hay solicitudes pendientes se avisa desde el menú y no se abre el formulario.
            if (new BLL_SolicitudCompra486LP().ListarPendientes().Count == 0)
            {
                var lm = Program.LanguageManager;
                string fOrd = "FrmRegistrarOrden486LP";
                MessageBox.Show(
                    lm.ObtenerTexto(fOrd, "Frm.RegistrarOrden.Msg.SinPendientes", "No hay solicitudes pendientes."),
                    lm.ObtenerTexto(fOrd, "Frm.RegistrarOrden.Msg.Aviso.Title", "Aviso"),
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            AbrirFormulario(new FrmRegistrarOrden486LP());
        }
        private void registrarRecepciónDeMercaderíaToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            // Si no hay órdenes pendientes de recepción no se abre el formulario: se avisa directamente desde el menú
            // (antes el formulario aparecía, mostraba el aviso y se cerraba solo).
            if (new BLL_OrdenCompra486LP().ListarPendientesDeRecepcion().Count == 0)
            {
                var lm = Program.LanguageManager;
                string fRec = "FrmRegistrarRecepcion486LP";
                MessageBox.Show(
                    lm.ObtenerTexto(fRec, "Frm.RegistrarRecepcion.Msg.SinPendientes", "No hay órdenes pendientes de recepción."),
                    lm.ObtenerTexto(fRec, "Frm.RegistrarRecepcion.Msg.Aviso.Title", "Aviso"),
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            AbrirFormulario(new FrmRegistrarRecepcion486LP());
        }

        private void registrarFacturaYPagoDeCompraToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            // Si no hay órdenes pendientes de facturación se avisa desde el menú y no se abre el formulario.
            if (new BLL_OrdenCompra486LP().ListarPendientesDeFacturacion().Count == 0)
            {
                var lm = Program.LanguageManager;
                string fFac = "FrmRegistrarFactura486LP";
                MessageBox.Show(
                    lm.ObtenerTexto(fFac, "Frm.RegistrarFactura.Msg.SinPendientes", "No hay órdenes pendientes de facturación."),
                    lm.ObtenerTexto(fFac, "Frm.RegistrarFactura.Msg.Aviso.Title", "Aviso"),
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            AbrirFormulario(new FrmRegistrarFactura486LP());
        }

        // Menu Venta
        private void gestionarCarritoToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            AbrirFormulario(new FrmGestionCarrito486LP());
        }
        private void registrarOperaciónDeVentaToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            AbrirFormulario(new FrmCobrarVenta486LP());
        }
        private void consultarProductosToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            AbrirFormulario(new FrmConsultarProducto486LP());
        }

        // Menu Reporte
        private void reporteDeVentasToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            AbrirFormulario(new FrmReporteVenta486LP());
        }
        private void reporteDeComprasToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            AbrirFormulario(new FrmReporteCompras486LP());
        }
        private void reporteInteligenteToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            AbrirFormulario(new FrmReporteInteligente486LP());
        }

        // Menu Ayuda
        private void verDocumentaciónToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            MensajeEnDesarrollo("Ver Documentación");
        }
        private void guíaDeUsuarioToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            MensajeEnDesarrollo("Guía de Usuario");
        }

        private void AbrirFormulario(Form formulario)
        {
            foreach (Form f in this.MdiChildren)
            {
                if (f.GetType() == formulario.GetType())
                {
                    // Si estaba minimizado se restaura y se maximiza. No se pasa por "Normal" con los demas abiertos:
                    // en un MDI eso restaura a todos los hijos y quedan chicos al volver a abrirlos.
                    if (f.WindowState == FormWindowState.Minimized)
                    {
                        f.WindowState = FormWindowState.Normal;
                    }
                    if (f.WindowState != FormWindowState.Maximized)
                    {
                        f.WindowState = FormWindowState.Maximized;
                    }
                    f.BringToFront();
                    f.Activate();
                    (f as IRefrescable486LP)?.Refrescar();
                    formulario.Dispose();
                    return;
                }
            }

            formulario.MdiParent = this;
            formulario.WindowState = FormWindowState.Maximized;
            formulario.Show();
        }

        // Cada vez que se activa una ventana hija se asegura que quede maximizada, asi ninguna aparece mas chica
        private bool _ajustandoMdi;

        private void FrmMenuPrincipal486LP_MdiChildActivate(object sender, System.EventArgs e)
        {
            // Al cambiar de ventana se recalcula el icono: si se cargo stock, desaparece solo
            ActualizarAvisoStock();

            if (_ajustandoMdi)
            {
                return;
            }

            Form hijo = this.ActiveMdiChild;
            if (hijo == null || hijo is FrmIniciarSesionLP486 || hijo.WindowState == FormWindowState.Maximized)
            {
                return;
            }

            try
            {
                _ajustandoMdi = true;
                hijo.WindowState = FormWindowState.Maximized;
            }
            finally
            {
                _ajustandoMdi = false;
            }
        }

        private void MensajeEnDesarrollo(string modulo)
        {
            MessageBox.Show(
                $"El módulo \"{modulo}\" está en desarrollo.",
                "SneakRush — En desarrollo",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void iniciarSesionToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            FrmIniciarSesionLP486 frm = new FrmIniciarSesionLP486();
            frm.MdiParent = this;
            frm.Show();
        }

        private void AplicarCambioIdioma(string codigo, string nombre)
        {
            Program.LanguageManager.CambiarIdioma(codigo);

            var usuario = SessionManager486LP.ObtenerInstancia().UsuarioActual();
            if (usuario != null)
            {
                string msg;
                new BLL_Idioma486LP().GuardarIdioma(usuario.IdUsuario, nombre, out msg);
            }
        }

        private void españolToolStripMenuItem_Click_1(object sender, System.EventArgs e)
        {
            AplicarCambioIdioma("es", "Español");
        }

        private void inglesToolStripMenuItem_Click_1(object sender, System.EventArgs e)
        {
            AplicarCambioIdioma("en", "Inglés");
        }

        private void portuguesToolStripMenuItem_Click_1(object sender, System.EventArgs e)
        {
            AplicarCambioIdioma("pt", "Portugués");
        }

        public void ActualizarIdioma()
        {
            var lm = Program.LanguageManager;
            string f = "FrmMenuPrincipal486LP";

            this.Text = lm.ObtenerTexto(f, "Title");

            // Menú Usuario
            usuarioToolStripMenuItem.Text = lm.ObtenerTexto(f, "usuarioToolStripMenuItem");
            cambiarContraseñaToolStripMenuItem.Text = lm.ObtenerTexto(f, "cambiarContraseñaToolStripMenuItem");
            cambiarIdiomaToolStripMenuItem.Text = lm.ObtenerTexto(f, "cambiarIdiomaToolStripMenuItem");
            iniciarSesionToolStripMenuItem.Text = lm.ObtenerTexto(f, "iniciarSesionToolStripMenuItem");
            cerrarSesiónToolStripMenuItem.Text = lm.ObtenerTexto(f, "cerrarSesiónToolStripMenuItem");

            // Los idiomas se muestran siempre con su propio nombre, sin importar el idioma activo
            españolToolStripMenuItem.Text = "Español";
            inglesToolStripMenuItem.Text = "English";
            portuguesToolStripMenuItem.Text = "Português";

            // Menú Administrador
            adminToolStripMenuItem.Text = lm.ObtenerTexto(f, "adminToolStripMenuItem");
            gestiónDeUsuariosToolStripMenuItem.Text = lm.ObtenerTexto(f, "gestiónDeUsuariosToolStripMenuItem");
            gestiónDePerfilesToolStripMenuItem.Text = lm.ObtenerTexto(f, "gestiónDePerfilesToolStripMenuItem");
            gestionDeFamiliasToolStripMenuItem.Text = lm.ObtenerTexto(f, "gestionDeFamiliasToolStripMenuItem");
            gestionDeRolesToolStripMenuItem.Text = lm.ObtenerTexto(f, "gestionDeRolesToolStripMenuItem");
            bitácoraDeEventosToolStripMenuItem.Text = lm.ObtenerTexto(f, "bitácoraDeEventosToolStripMenuItem");
            gestiónDeRespaldosToolStripMenuItem.Text = lm.ObtenerTexto(f, "gestiónDeRespaldosToolStripMenuItem");

            // Menú Maestro
            gestionToolStripMenuItem.Text = lm.ObtenerTexto(f, "gestionToolStripMenuItem");
            clientesToolStripMenuItem.Text = lm.ObtenerTexto(f, "clientesToolStripMenuItem");
            productosToolStripMenuItem.Text = lm.ObtenerTexto(f, "productosToolStripMenuItem");
            categoríasMarcasToolStripMenuItem.Text = lm.ObtenerTexto(f, "categoríasMarcasToolStripMenuItem");
            proveedoresToolStripMenuItem.Text = lm.ObtenerTexto(f, "proveedoresToolStripMenuItem");

            // Menú Compra
            compraToolStripMenuItem.Text = lm.ObtenerTexto(f, "compraToolStripMenuItem");
            generarSolicitudDeCompraToolStripMenuItem.Text = lm.ObtenerTexto(f, "generarSolicitudDeCompraToolStripMenuItem");
            registrarOrdenDeCompraToolStripMenuItem.Text = lm.ObtenerTexto(f, "registrarOrdenDeCompraToolStripMenuItem");
            registrarRecepciónDeMercaderíaToolStripMenuItem.Text = lm.ObtenerTexto(f, "registrarRecepciónDeMercaderíaToolStripMenuItem");
            registrarFacturaYPagoDeCompraToolStripMenuItem.Text = lm.ObtenerTexto(f, "registrarFacturaYPagoDeCompraToolStripMenuItem");

            // Menú Venta
            ventaToolStripMenuItem.Text = lm.ObtenerTexto(f, "ventaToolStripMenuItem");
            gestionarCarritoToolStripMenuItem.Text = lm.ObtenerTexto(f, "gestionarCarritoToolStripMenuItem");
            registrarOperaciónDeVentaToolStripMenuItem.Text = lm.ObtenerTexto(f, "registrarOperaciónDeVentaToolStripMenuItem");
            consultarProductosToolStripMenuItem.Text = lm.ObtenerTexto(f, "consultarProductosToolStripMenuItem");

            // Menú Reporte
            reporteToolStripMenuItem.Text = lm.ObtenerTexto(f, "reporteToolStripMenuItem");
            reporteDeVentasToolStripMenuItem.Text = lm.ObtenerTexto(f, "reporteDeVentasToolStripMenuItem");
            reporteDeComprasToolStripMenuItem.Text = lm.ObtenerTexto(f, "reporteDeComprasToolStripMenuItem");
            reporteInteligenteToolStripMenuItem.Text = lm.ObtenerTexto(f, "reporteInteligenteToolStripMenuItem");

            // Menú Ayuda
            ayudaToolStripMenuItem.Text = lm.ObtenerTexto(f, "ayudaToolStripMenuItem");
            verDocumentaciónToolStripMenuItem.Text = lm.ObtenerTexto(f, "verDocumentaciónToolStripMenuItem");
            guíaDeUsuarioToolStripMenuItem.Text = lm.ObtenerTexto(f, "guíaDeUsuarioToolStripMenuItem");

            ActualizarEstado();
            ActualizarAvisoStock();
        }

        private void AgregarAccentNaranja()
        {
            Menu.Paint += (s, ev) =>
            {
                int y = Menu.ClientSize.Height - 2;
                using (var pluma = new System.Drawing.Pen(System.Drawing.Color.FromArgb(0xF0, 0x5A, 0x28), 3f))
                    ev.Graphics.DrawLine(pluma, 0, y, Menu.ClientSize.Width, y);
            };
            lblEstado.Paint += (s, ev) =>
            {
                using (var pluma = new System.Drawing.Pen(System.Drawing.Color.FromArgb(0xF0, 0x5A, 0x28), 2f))
                    ev.Graphics.DrawLine(pluma, 0, 0, lblEstado.ClientSize.Width, 0);
            };
            lblEstado.Invalidate();
            Menu.Invalidate();
        }
    }

    // Forms hijos del MDI que, al volver a elegirse desde el menu estando ya abiertos, vuelven a cargar sus datos
    // (si no, quedaban con la informacion de la primera vez que se abrieron)
    public interface IRefrescable486LP
    {
        void Refrescar();
    }

    // Filtros de teclado compartidos por los formularios (se asignan al evento KeyPress del cuadro de texto)
    public static class FiltrosTeclado486LP
    {
        // Solo digitos (DNI)
        public static void SoloDigitos(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                e.Handled = true;
        }

        // Digitos, espacio y los signos + - ( ) (telefono)
        public static void Telefono(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && " +-()".IndexOf(e.KeyChar) < 0)
                e.Handled = true;
        }
    }
}

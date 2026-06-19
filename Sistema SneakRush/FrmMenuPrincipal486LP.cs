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
                usuarioToolStripMenuItem.Visible = true;
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
                          || permisos.Contains("COMPRA_REGISTRAR_RECEPCION");

            compraToolStripMenuItem.Visible = verCompra;

            if (verCompra)
            {
                generarSolicitudDeCompraToolStripMenuItem.Visible = permisos.Contains("COMPRA_GENERAR_SOLICITUD");
                registrarOrdenDeCompraToolStripMenuItem.Visible = permisos.Contains("COMPRA_REGISTRAR_ORDEN");
                registrarRecepciónDeMercaderíaToolStripMenuItem.Visible = permisos.Contains("COMPRA_REGISTRAR_RECEPCION");
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
                           || permisos.Contains("REPORTE_STOCK")
                           || permisos.Contains("REPORTE_COMPRAS")
                           || permisos.Contains("REPORTE_ANALITICAS");

            reporteToolStripMenuItem.Visible = verReporte;

            if (verReporte)
            {
                reporteDeVentasToolStripMenuItem.Visible = permisos.Contains("REPORTE_VENTAS");
                reporteDeStockToolStripMenuItem.Visible = permisos.Contains("REPORTE_STOCK");
                reporteDeComprasToolStripMenuItem.Visible = permisos.Contains("REPORTE_COMPRAS");
                consultasAnalíticasToolStripMenuItem.Visible = permisos.Contains("REPORTE_ANALITICAS");
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
            MensajeEnDesarrollo("Gestión de Respaldos");
        }

        // Menu Maestro
        private void clientesToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            MensajeEnDesarrollo("Clientes");
        }
        private void productosToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            MensajeEnDesarrollo("Productos");
        }
        private void categoríasMarcasToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            MensajeEnDesarrollo("Categorías / Marcas");
        }
        private void proveedoresToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            MensajeEnDesarrollo("Proveedores");
        }

        // Menu Compra
        private void generarSolicitudDeCompraToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            MensajeEnDesarrollo("Generar Solicitud");
        }
        private void registrarOrdenDeCompraToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            MensajeEnDesarrollo("Registrar Orden");
        }
        private void registrarRecepciónDeMercaderíaToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            MensajeEnDesarrollo("Registrar Recepción de Mercadería");
        }

        // Menu Venta
        private void gestionarCarritoToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            MensajeEnDesarrollo("Gestionar Carrito");
        }
        private void registrarOperaciónDeVentaToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            MensajeEnDesarrollo("Registrar Operación");
        }
        private void consultarProductosToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            MensajeEnDesarrollo("Consultar Productos");
        }

        // Menu Reporte
        private void reporteDeVentasToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            MensajeEnDesarrollo("Reporte de Ventas");
        }
        private void reporteDeStockToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            MensajeEnDesarrollo("Reporte de Stock");
        }
        private void reporteDeComprasToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            MensajeEnDesarrollo("Reporte de Compras");
        }
        private void consultasAnalíticasToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            MensajeEnDesarrollo("Consultas Analíticas");
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
                    f.BringToFront();
                    formulario.Dispose();
                    return;
                }
            }

            formulario.MdiParent = this;
            formulario.WindowState = FormWindowState.Maximized;
            formulario.Show();
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

            // Menú Venta
            ventaToolStripMenuItem.Text = lm.ObtenerTexto(f, "ventaToolStripMenuItem");
            gestionarCarritoToolStripMenuItem.Text = lm.ObtenerTexto(f, "gestionarCarritoToolStripMenuItem");
            registrarOperaciónDeVentaToolStripMenuItem.Text = lm.ObtenerTexto(f, "registrarOperaciónDeVentaToolStripMenuItem");
            consultarProductosToolStripMenuItem.Text = lm.ObtenerTexto(f, "consultarProductosToolStripMenuItem");

            // Menú Reporte
            reporteToolStripMenuItem.Text = lm.ObtenerTexto(f, "reporteToolStripMenuItem");
            reporteDeVentasToolStripMenuItem.Text = lm.ObtenerTexto(f, "reporteDeVentasToolStripMenuItem");
            reporteDeStockToolStripMenuItem.Text = lm.ObtenerTexto(f, "reporteDeStockToolStripMenuItem");
            reporteDeComprasToolStripMenuItem.Text = lm.ObtenerTexto(f, "reporteDeComprasToolStripMenuItem");
            consultasAnalíticasToolStripMenuItem.Text = lm.ObtenerTexto(f, "consultasAnalíticasToolStripMenuItem");

            // Menú Ayuda
            ayudaToolStripMenuItem.Text = lm.ObtenerTexto(f, "ayudaToolStripMenuItem");
            verDocumentaciónToolStripMenuItem.Text = lm.ObtenerTexto(f, "verDocumentaciónToolStripMenuItem");
            guíaDeUsuarioToolStripMenuItem.Text = lm.ObtenerTexto(f, "guíaDeUsuarioToolStripMenuItem");

            ActualizarEstado();
        }
    }
}

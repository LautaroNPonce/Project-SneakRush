using Services;
using System.Windows.Forms;

namespace Sistema_SneakRush
{
    public partial class FrmMenuPrincipal486LP : Form
    {
        public FrmMenuPrincipal486LP()
        {
            InitializeComponent();
            this.IsMdiContainer = true;
        }

        private void FrmMenuPrincipal486LP_Load(object sender, System.EventArgs e)
        {
            foreach (Control c in this.Controls)
            {
                if (c is MdiClient mdiCliente)
                {
                    mdiCliente.BackColor = this.BackColor; // toma el color de mi  form
                    break;
                }
            }

            ActualizarEstado();
            AjustarMenuSegunPerfil();

            cmbIdioma.Items.AddRange(new object[] { "Español", "Inglés", "Português" });
            cmbIdioma.SelectedIndex = 0;
            cmbIdioma.DropDownStyle = ComboBoxStyle.DropDownList;
        }

        public void ActualizarEstado()
        {
            var usuario = SessionManager486LP.ObtenerInstancia().UsuarioActual();
            if (usuario != null) 
            { 
                lblEstado.Text = $"Usuario: {usuario.Nombre} {usuario.Apellido}  |  Rol: {usuario.Rol}"; 
            }
        }

        // Acceso a los menus según el rol del usuario.Solo el Administrador y el Analista ven el menú Administrador.
        private void AjustarMenuSegunPerfil()
        {
            // Solo el Administrador y analista ve el menú Administrador
            var usuario = SessionManager486LP.ObtenerInstancia().UsuarioActual();
            if (usuario == null) return;
            {
                adminToolStripMenuItem.Visible = usuario.Rol == "Administrador" || usuario.Rol == "Analista";
            }
        }

        //  Menu Usuario      
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
        private void gestiónDePerfilesToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            MensajeEnDesarrollo("Gestión de Perfiles");
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

        // ---Menu Reporte---
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

        // ---Menu Ayuda---
        private void verDocumentaciónToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            MensajeEnDesarrollo("Ver Documentación");
        }
        private void guíaDeUsuarioToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            MensajeEnDesarrollo("Guía de Usuario");
        }

        // Abro un formulario hijo en MDI. Si ya está abierto, lo trae al frente (instancia única).
        private void AbrirFormulario(Form formulario)
        {
            foreach (Form f in this.MdiChildren)
            {
                if (f.GetType() == formulario.GetType())
                {
                    f.BringToFront();
                    formulario.Dispose(); // descarto la instancia duplicada
                    return;
                }
            }

            formulario.MdiParent = this;
            formulario.WindowState = FormWindowState.Maximized;
            formulario.Show();
        }

        // Muestro un mensaje estándar para módulos aún no implementados.
        private void MensajeEnDesarrollo(string modulo)
        {
            MessageBox.Show(
                $"El módulo \"{modulo}\" está en desarrollo.",
                "SneakRush — En desarrollo",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
        }

        private void iniciarSesionToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            FrmIniciarSesionLP486 frm = new FrmIniciarSesionLP486();
            frm.MdiParent = this;
            frm.Show();
        }

        //private void cmbIdioma_SelectedIndexChanged(object sender, System.EventArgs e)
        //{
        //    switch (cmbIdioma.SelectedIndex)
        //    {
        //        case 0: SessionManager486LP.ObtenerInstancia().IdiomaActual = "es"; 
        //            break;
        //        case 1: SessionManager486LP.ObtenerInstancia().IdiomaActual = "en"; 
        //            break;
        //        case 2: SessionManager486LP.ObtenerInstancia().IdiomaActual = "pt"; 
        //            break;
        //    }
        //}
    }
}

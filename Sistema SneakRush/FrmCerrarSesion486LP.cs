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
    public partial class FrmCerrarSesion486LP : Form, IObserver486LP
    {
        private BLL_Usuarios486LP _bll = new BLL_Usuarios486LP();
        public FrmCerrarSesion486LP()
        {
            InitializeComponent();
            AjustarBotonesSegunPerfil();
            Program.LanguageManager.Agregar(this);
            this.FormClosing += FrmCerrarSesion486LP_FormClosing;
            ActualizarIdioma();
        }

        private void btnAceptar_Click(object sender, EventArgs e)
        {
            Usuario486LP usuarioActual = SessionManager486LP.ObtenerInstancia().UsuarioActual();

            string mensaje;
            _bll.Logout(usuarioActual.DNI, out mensaje);

            if (!string.IsNullOrEmpty(mensaje))
                MessageBox.Show(mensaje, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

            Form menu = this.MdiParent;
            FrmIniciarSesionLP486 login = new FrmIniciarSesionLP486();
            login.Show();
            menu.Close();
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void AjustarBotonesSegunPerfil()
        {
            var usuario = SessionManager486LP.ObtenerInstancia().UsuarioActual();
            if (usuario == null) return;

            List<string> permisos = new BLL_Perfil486LP().ObtenerPermisosPorRol(usuario.Rol);

            btnAceptar.Enabled = permisos.Contains("CERRAR_SESION_ACEPTAR");
            btnCancelar.Enabled = permisos.Contains("CERRAR_SESION_CANCELAR");
        }

        public void ActualizarIdioma()
        {
            var lm = Program.LanguageManager;
            string f = "FrmCerrarSesion486LP";

            this.Text = lm.ObtenerTexto(f, "Title");
            label1.Text = lm.ObtenerTexto(f, "lblPregunta");
            btnAceptar.Text = lm.ObtenerTexto(f, "btnAceptar");
            btnCancelar.Text = lm.ObtenerTexto(f, "btnCancelar");
        }

        private void FrmCerrarSesion486LP_FormClosing(object sender, FormClosingEventArgs e)
        {
            Program.LanguageManager.Quitar(this);
        }
    }
}

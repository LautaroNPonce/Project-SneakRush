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
    public partial class FrmIniciarSesionLP486 : Form
    {
        public FrmIniciarSesionLP486()
        {
            InitializeComponent();
        }

        private void FrmIniciarSesionLP486_Load(object sender, EventArgs e)
        {
            txtUsuario.Focus();
        }

        private void btnIngresar_Click(object sender, EventArgs e)
        {
            string nombreUsuario = txtUsuario.Text.Trim();
            string contraseña = txtContraseña.Text;

            if (string.IsNullOrEmpty(nombreUsuario) || string.IsNullOrEmpty(contraseña))
            {
                MessageBox.Show("Por favor completá todos los campos.","SneakRush — Campos vacíos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Usuario486LP usuario = null;
            string mensaje = string.Empty;

            BLL_Usuarios486LP bll = new BLL_Usuarios486LP();
            int resultado = bll.Login(nombreUsuario, contraseña, out usuario, out mensaje);

            switch (resultado)
            {
                case 1:

                    if (this.MdiParent is FrmMenuPrincipal486LP menu)
                    {
                        // Re-Login — validar si es el mismo usuario
                        var usuarioActual = SessionManager486LP.ObtenerInstancia().UsuarioActual();
                        if (usuarioActual != null && usuarioActual.NombreUsuario == usuario.NombreUsuario)
                        {
                            MessageBox.Show(
                                $"Ya tenés una sesión activa como {usuario.Nombre} {usuario.Apellido}.",
                                "SneakRush — Sesión activa",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
                            LimpiarCampos();
                            return;
                        }

                        SessionManager486LP.ObtenerInstancia().LogOut();
                        SessionManager486LP.ObtenerInstancia().LogIN(usuario);

                        if (usuario.DebeCambiarContraseña)
                        {
                            MessageBox.Show("Por seguridad, debe cambiar su contraseña antes de continuar.", "SneakRush — Cambio requerido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            FrmCambiarContraseña486LP frmCambiar = new FrmCambiarContraseña486LP();
                            frmCambiar.ShowDialog();
                        }

                        menu.ActualizarEstado();
                        MessageBox.Show("Re-Login exitoso. Sesión verificada correctamente.", "SneakRush — Re-Login", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.Close();
                    }
                    else
                    {
                        // Login normal
                        SessionManager486LP.ObtenerInstancia().LogOut();
                        SessionManager486LP.ObtenerInstancia().LogIN(usuario);

                        if (usuario.DebeCambiarContraseña)
                        {
                            MessageBox.Show("Por seguridad, debe cambiar su contraseña antes de continuar.", "SneakRush — Cambio requerido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            FrmCambiarContraseña486LP frmCambiar = new FrmCambiarContraseña486LP();
                            frmCambiar.ShowDialog();
                        }

                        FrmMenuPrincipal486LP menuPrincipal = new FrmMenuPrincipal486LP();
                        menuPrincipal.Show();
                        this.Hide();
                    }
                    break;

                case 0:
                    MessageBox.Show("Usuario no encontrado.","SneakRush — Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    LimpiarCampos();
                    break;

                case -1:
                    MessageBox.Show("Contraseña incorrecta.", "SneakRush — Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtContraseña.Clear();
                    txtContraseña.Focus();
                    break;

                case -2:
                    MessageBox.Show("El usuario está inactivo. Contacte al Administrador.", "SneakRush — Usuario inactivo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    LimpiarCampos();
                    break;

                case -3:
                    MessageBox.Show("El usuario está bloqueado. Contacte al Administrador.", "SneakRush — Usuario bloqueado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    LimpiarCampos();
                    break;

                default:
                    MessageBox.Show($"Error inesperado: {mensaje}", "SneakRush — Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;

            }
        }

        private void txtUsuario_KeyDown(object sender, KeyEventArgs e) //
        {
            if (e.KeyCode == Keys.Enter) txtContraseña.Focus();
        }

        private void txtContraseña_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) btnIngresar_Click(sender, e);
        }

        private void FrmIniciarSesionLP486_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                if (this.MdiParent == null)
                    Application.Exit(); // login normal → cierra la app
                else
                {
                    e.Cancel = true;
                    this.Hide(); // re-login → se oculta nomás
                }
            }
        }

        private void LimpiarCampos()
        {
            txtUsuario.Clear();
            txtContraseña.Clear();
            txtUsuario.Focus();
        }

        private void btnVerContraseña_Click(object sender, EventArgs e)
        {
            txtContraseña.PasswordChar = txtContraseña.PasswordChar == '*' ? '\0' : '*';
        }

    }
}

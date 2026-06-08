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
                        // Re-Login — SessionManager no permite LogIN si hay sesión activa
                        try
                        {
                            SessionManager486LP.ObtenerInstancia().LogIN(usuario);
                        }
                        catch (InvalidOperationException ex)
                        {
                            MessageBox.Show(ex.Message, "SneakRush — Sesión activa", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            LimpiarCampos();
                            return;
                        }

                        if (usuario.DebeCambiarContraseña) { /* ... */ }

                        menu.ActualizarEstado();
                        MessageBox.Show("Re-Login exitoso. Sesión verificada correctamente.", "SneakRush — Re-Login", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.Close();
                    }
                    else
                    {
                        // Login normal
                        SessionManager486LP.ObtenerInstancia().LogOut();
                        SessionManager486LP.ObtenerInstancia().LogIN(usuario);

                        if (usuario.DebeCambiarContraseña) { /* ... */ }

                        // Verificar integridad de DV antes de abrir el menú
                        BLL_DV486LP bllDV = new BLL_DV486LP();
                        string tablaAfectada;

                        if (!bllDV.VerificarIntegridad("Usuarios", out tablaAfectada))
                        {
                            this.Hide();
                            DV486LP dv = new DV486LP(tablaAfectada, "", "");
                            FrmReparacionBD486LP frmReparacion = new FrmReparacionBD486LP(dv);
                            frmReparacion.ShowDialog();
                            this.Show();
                            return;
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

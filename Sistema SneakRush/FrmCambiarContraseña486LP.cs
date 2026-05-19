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
    public partial class FrmCambiarContraseña486LP : Form
    {
        private BLL_Usuarios486LP _bll = new BLL_Usuarios486LP();
        public FrmCambiarContraseña486LP()
        {
            InitializeComponent();
        }

        private void FrmCambiarContraseña486LP_Load(object sender, EventArgs e)
        {
            var usuario = SessionManager486LP.ObtenerInstancia().UsuarioActual();
            if (usuario != null)
            {
                txtNombreUsuario.Text = usuario.NombreUsuario;
            }
        }

        private void btnAceptar_Click(object sender, EventArgs e)
        {
            var usuario = SessionManager486LP.ObtenerInstancia().UsuarioActual();

            if (usuario == null)
            {
                MessageBox.Show("No hay sesión activa.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string mensaje;
            bool resultado = _bll.CambiarContraseña(usuario.IdUsuario, txtContraseña.Text, txtNuevaContraseña.Text, txtConfirmarContraseña.Text, usuario.DNI, out mensaje);

            if (resultado)
            {
                MessageBox.Show("Contraseña cambiada exitosamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LimpiarCampos();
                this.Close();
            }
            else
            {
                MessageBox.Show(mensaje, "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        private void LimpiarCampos()
        {
            txtContraseña.Text = string.Empty;
            txtNuevaContraseña.Text = string.Empty;
            txtConfirmarContraseña.Text = string.Empty;
        }

        private void txtContraseña_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) 
            {
                txtNuevaContraseña.Focus();
            }
        }

        private void txtNuevaContraseña_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) 
            {
                txtConfirmarContraseña.Focus();
            }
        }

        private void txtConfirmarContraseña_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) 
            { 
                btnAceptar_Click(sender, e); 
            }
        }

        private void btnVerContraseña_Click(object sender, EventArgs e)
        {
            txtContraseña.PasswordChar = txtContraseña.PasswordChar == '*' ? '\0' : '*';
        }

        private void btnVerNueva_Click(object sender, EventArgs e)
        {
            txtNuevaContraseña.PasswordChar = txtNuevaContraseña.PasswordChar == '*' ? '\0' : '*';
        }

        private void btnVerConfirmar_Click(object sender, EventArgs e)
        {
            txtConfirmarContraseña.PasswordChar = txtConfirmarContraseña.PasswordChar == '*' ? '\0' : '*';
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}

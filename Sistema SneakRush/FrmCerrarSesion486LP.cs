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
    public partial class FrmCerrarSesion486LP : Form
    {
        private BLL_Usuarios486LP _bll = new BLL_Usuarios486LP();
        public FrmCerrarSesion486LP()
        {
            InitializeComponent();
        }

        private void btnAceptar_Click(object sender, EventArgs e)
        {
            Usuario486LP usuarioActual = SessionManager486LP.ObtenerInstancia().UsuarioActual();

            // Bitácora PRIMERO, después LogOut() — el orden lo garantiza la BLL internamente
            _bll.Logout(usuarioActual.NombreUsuario);

            Form menu = this.MdiParent;
            FrmIniciarSesionLP486 login = new FrmIniciarSesionLP486();
            login.Show();
            menu.Close();
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}

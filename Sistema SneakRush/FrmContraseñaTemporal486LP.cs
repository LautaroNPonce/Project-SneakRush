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
    public partial class FrmContraseñaTemporal486LP : Form
    {
        public FrmContraseñaTemporal486LP(string contraseña)
        {
            InitializeComponent();
            txtContraseña.Text = contraseña;
        }

        private void btnCopiar_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(txtContraseña.Text);
            btnCopiar.Text = "Copiado";
        }

        private void btnAceptar_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}

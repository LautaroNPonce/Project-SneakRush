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
    public partial class FrmReparacionBD486LP : Form
    {
        private BLL_DV486LP _bllDV = new BLL_DV486LP();
        private DV486LP _dv;

        public FrmReparacionBD486LP(DV486LP dv)
        {
            InitializeComponent();
            _dv = dv;
        }

        private void FrmReparacionBD486LP_Load(object sender, EventArgs e)
        {
            lblTabla.Text = _dv.TablaAfectada;
        }

        private void btnRecalcular_Click(object sender, EventArgs e)
        {
            _bllDV.RecalcularDV(_dv.TablaAfectada);
            MessageBox.Show("Dígitos verificadores recalculados correctamente.", "Éxito",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnRestaurar_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Funcionalidad de restauración pendiente de implementar.", "Aviso",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}

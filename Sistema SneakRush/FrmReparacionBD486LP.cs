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
using static Services.DV486LP;

namespace Sistema_SneakRush
{
    public partial class FrmReparacionBD486LP : Form, IObserver486LP
    {
        private BLL_DV486LP _bllDV = new BLL_DV486LP();
        private List<string> _tablas;
        private List<InconsistenciaDV486LP> _inconsistencias = new List<InconsistenciaDV486LP>();

        // recibe todas las tablas con inconsistencia.
        public FrmReparacionBD486LP(List<string> tablas)
        {
            InitializeComponent();
            _tablas = tablas ?? new List<string>();
            Program.LanguageManager.Agregar(this);
            this.FormClosing += FrmReparacionBD486LP_FormClosing;
        }

        // si en algún lado todavía se llama con una sola tabla.
        public FrmReparacionBD486LP(DV486LP dv) : this(new List<string> { dv.TablaAfectada }) { }

        private void FrmReparacionBD486LP_Load(object sender, EventArgs e)
        {
            CargarInconsistencias();
            ActualizarIdioma();
        }

        private void CargarInconsistencias()
        {
            _inconsistencias = _bllDV.ObtenerInconsistenciasDeTablas(_tablas);
            RenderGrilla();
        }

        private void RenderGrilla()
        {
            var lm = Program.LanguageManager;
            string f = "FrmReparacionBD486LP";

            dgvInconsistencias.Rows.Clear();

            foreach (InconsistenciaDV486LP inc in _inconsistencias)
            {
                string textoInc = lm.ObtenerTexto(f, inc.Inconsistencia, inc.Inconsistencia);
                dgvInconsistencias.Rows.Add(inc.ID, inc.Tabla, textoInc);
            }
        }

        private void btnRecalcular_Click(object sender, EventArgs e)
        {
            var lm = Program.LanguageManager;
            string f = "FrmReparacionBD486LP";

            if (!_bllDV.RecalcularTablas(_tablas, out string mensaje))
            {
                MessageBox.Show(string.Format(lm.ObtenerTexto(f, "Msg.ErrorRecalcular", "Error al recalcular: {0}"), mensaje),
                    lm.ObtenerTexto(f, "Msg.ErrorRecalcular.Title", "Error"),MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            MessageBox.Show(lm.ObtenerTexto(f, "Msg.Exito", "Dígitos verificadores recalculados correctamente."),
                lm.ObtenerTexto(f, "Msg.Exito.Title", "Éxito"),MessageBoxButtons.OK, MessageBoxIcon.Information);

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnRestaurar_Click(object sender, EventArgs e)
        {
            var lm = Program.LanguageManager;
            string f = "FrmReparacionBD486LP";

            MessageBox.Show(lm.ObtenerTexto(f, "Msg.RestaurarPendiente", "Funcionalidad de restauración pendiente de implementar."),
                lm.ObtenerTexto(f, "Msg.RestaurarPendiente.Title", "Aviso"),MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        public void ActualizarIdioma()
        {
            var lm = Program.LanguageManager;
            string f = "FrmReparacionBD486LP";

            this.Text = lm.ObtenerTexto(f, "Title", "Reparación de Base de Datos");

            btnRecalcular.Text = lm.ObtenerTexto(f, "btnRecalcular", "Recalcular");
            btnRestaurar.Text = lm.ObtenerTexto(f, "btnRestaurar", "Restaurar");
            btnSalir.Text = lm.ObtenerTexto(f, "btnSalir", "Salir");

            if (dgvInconsistencias.Columns.Count >= 3)
            {
                dgvInconsistencias.Columns[0].HeaderText = lm.ObtenerTexto(f, "col.ID", "ID");
                dgvInconsistencias.Columns[1].HeaderText = lm.ObtenerTexto(f, "col.Tabla", "Tabla");
                dgvInconsistencias.Columns[2].HeaderText = lm.ObtenerTexto(f, "col.Inconsistencia", "Inconsistencia");
            }

            RenderGrilla();
        }

        private void FrmReparacionBD486LP_FormClosing(object sender, FormClosingEventArgs e)
        {
            Program.LanguageManager.Quitar(this);
        }
    }
}

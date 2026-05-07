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
    public partial class FrmBitacoraEvento486LP : Form
    {

        private BLL_Bitacora486LP _bll = new BLL_Bitacora486LP();
        public FrmBitacoraEvento486LP()
        {
            InitializeComponent();
            //this.WindowState = FormWindowState.Maximized;
        }

        private void FrmBitacoraEvento486LP_Load(object sender, EventArgs e)
        {
            ConfigurarDgv();
            CargarCombos();
            CargarGrilla();
        }

        private void ConfigurarDgv()
        {
            dgtBitacoraEvento.AutoGenerateColumns = false;
            dgtBitacoraEvento.ReadOnly = true;
            dgtBitacoraEvento.AllowUserToAddRows = false;
            dgtBitacoraEvento.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgtBitacoraEvento.MultiSelect = false;

            dgtBitacoraEvento.Columns.Clear();

            dgtBitacoraEvento.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Numero", HeaderText = "N°", Width = 60 });
            dgtBitacoraEvento.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Fecha", HeaderText = "Fecha y Hora", Width = 160, DefaultCellStyle = { Format = "dd/MM/yyyy HH:mm:ss" } });
            dgtBitacoraEvento.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "NombreUsuario", HeaderText = "Usuario", Width = 120 });
            dgtBitacoraEvento.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "DNI", HeaderText = "DNI", Width = 100 });
            dgtBitacoraEvento.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Modulo", HeaderText = "Módulo", Width = 150 });
            dgtBitacoraEvento.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Descripcion", HeaderText = "Descripción", Width = 200 });
            dgtBitacoraEvento.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Criticidad", HeaderText = "Criticidad", Width = 110 });

            dgtBitacoraEvento.ShowCellToolTips = true;
        }

        private void CargarCombos()
        {
            cmbModulo.Items.Clear();
            cmbModulo.Items.Add("");
            cmbModulo.Items.Add("Login");
            cmbModulo.Items.Add("Logout");
            cmbModulo.Items.Add("Cambiar Contraseña");
            cmbModulo.Items.Add("Gestión de Usuarios");
            cmbModulo.SelectedIndex = 0;

            cmbCriticidad.Items.Clear();
            cmbCriticidad.Items.Add("");
            cmbCriticidad.Items.Add("INFO");
            cmbCriticidad.Items.Add("ADVERTENCIA");
            cmbCriticidad.Items.Add("ALTO");
            cmbCriticidad.Items.Add("MUY ALTO");
            cmbCriticidad.SelectedIndex = 0;
        }

        private void CargarGrilla()
        {
            List<BitacoraEvento486LP> lista = _bll.Listar();
            ActualizarGrilla(lista);
        }

        private void ActualizarGrilla(List<BitacoraEvento486LP> lista)
        {
            dgtBitacoraEvento.DataSource = null;
            dgtBitacoraEvento.DataSource = lista;
            lblTotal.Text = $"Registros: {lista.Count}";
        }

        private void btnAplicar_Click(object sender, EventArgs e)
        {
            string dni = txtDNI.Text.Trim();
            string modulo = cmbModulo.SelectedItem?.ToString() ?? "";
            string criticidad = cmbCriticidad.SelectedItem?.ToString() ?? "";
            string fechaInicio = dtpFechaDesde.Checked ? dtpFechaDesde.Value.ToString("yyyy-MM-dd") : "";

            string fechaFin = dtpFechaHasta.Checked ? dtpFechaHasta.Value.ToString("yyyy-MM-dd") + " 23:59:59" : "";

            if (dtpFechaDesde.Checked && dtpFechaHasta.Checked && dtpFechaDesde.Value.Date > dtpFechaHasta.Value.Date)
            {
                MessageBox.Show("La fecha desde no puede ser mayor a la fecha hasta.", "Fechas inválidas", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            List<BitacoraEvento486LP> lista = _bll.Filtrar(dni, "", modulo, criticidad, fechaInicio, fechaFin);

            ActualizarGrilla(lista);
            btnCancelar.Text = "Cancelar";

            if (lista.Count == 0)
            {
                MessageBox.Show("No se encontraron registros con los filtros aplicados.","Sin resultados", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            if (btnCancelar.Text == "Cancelar")
            {
                Resetear();
            }
            else
            {
                this.Close();
            }
        }

        private void Resetear()
        {
            txtDNI.Clear();
            cmbModulo.SelectedIndex = 0;
            cmbCriticidad.SelectedIndex = 0;
            dtpFechaDesde.Value = DateTime.Today;
            dtpFechaHasta.Value = DateTime.Today;
            btnCancelar.Text = "Salir";
            CargarGrilla();
        }
    }
}

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
    public partial class FrmGestionFamilias486LP : Form
    {
        private BLL_Familia486LP _bll = new BLL_Familia486LP();
        private int _idFamiliaSeleccionada = -1;
        private int _idPermisoSeleccionado = -1;

        public FrmGestionFamilias486LP()
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;
        }

        private void FrmGestionFamilias486LP_Load(object sender, EventArgs e)
        {
            ConfigurarDGV();
            CargarFamilias();
            CargarPatentes();
        }

        private void CargarFamilias()
        {
            dgvFamilias.Rows.Clear();
            List<Familia486LP> familias = _bll.ObtenerFamilias();
            foreach (Familia486LP f in familias)
            {
                dgvFamilias.Rows.Add(f.Id, f.Nombre);
            }
        }

        private void CargarPatentes()
        {
            dgvPatentes.Rows.Clear();
            List<Permiso486LP> patentes = _bll.ObtenerPatentes();
            foreach (Permiso486LP p in patentes)
            {
                dgvPatentes.Rows.Add(p.Id, p.Nombre);
            }
        }

        private void CargarPermisosAsignados(int idFamilia)
        {
            dgvAsignados.Rows.Clear();
            List<Permiso486LP> asignados = _bll.ObtenerPermisosDeFamilia(idFamilia);
            foreach (Permiso486LP p in asignados)
            {
                dgvAsignados.Rows.Add(p.Id, p.Nombre);
            }
        }

        private void dgvFamilias_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvFamilias.CurrentRow == null) 
            { 
                return; 
            }

            Familia486LP f = dgvFamilias.CurrentRow.DataBoundItem as Familia486LP;

            if (f != null)
            {
                _idFamiliaSeleccionada = f.Id;
                txtNombre.Text = f.Nombre;
                CargarPermisosAsignados(_idFamiliaSeleccionada);
                lblFamiliaSeleccionada.Text = "Permisos de: " + f.Nombre;
            }
            else
            {
                if (dgvFamilias.CurrentRow.Cells[0].Value != null)
                {
                    _idFamiliaSeleccionada = Convert.ToInt32(dgvFamilias.CurrentRow.Cells[0].Value);
                    txtNombre.Text = dgvFamilias.CurrentRow.Cells[1].Value.ToString();
                    CargarPermisosAsignados(_idFamiliaSeleccionada);
                    lblFamiliaSeleccionada.Text = "Permisos de: " + dgvFamilias.CurrentRow.Cells[1].Value.ToString();
                }
            }
        }

        private void dgvPatentes_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvPatentes.CurrentRow == null) 
            {
                return;
            }
            if (dgvPatentes.CurrentRow.Cells[0].Value != null)
            { 
                _idPermisoSeleccionado = Convert.ToInt32(dgvPatentes.CurrentRow.Cells[0].Value); 
            }
        }

        private void btnCrear_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            {
                MessageBox.Show("Debe ingresar un nombre.", "Validación",MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string mensaje;
            bool resultado = _bll.CrearFamilia(txtNombre.Text.Trim(), out mensaje);

            if (resultado)
            {
                MessageBox.Show(mensaje, "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Resetear();
            }
            else
            {
                MessageBox.Show(mensaje, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnModificar_Click(object sender, EventArgs e)
        {
            if (_idFamiliaSeleccionada <= 0)
            {
                MessageBox.Show("Debe seleccionar una Familia.", "Validación",MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            {
                MessageBox.Show("Debe ingresar un nombre válido.", "Validación",MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string mensaje;
            bool resultado = _bll.ModificarFamilia(_idFamiliaSeleccionada, txtNombre.Text.Trim(), out mensaje);

            if (resultado)
            {
                MessageBox.Show(mensaje, "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Resetear();
            }
            else
            {
                MessageBox.Show(mensaje, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (_idFamiliaSeleccionada <= 0)
            {
                MessageBox.Show("Debe seleccionar una Familia.", "Validación",MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Advertencia si está asignada a algún perfil
            if (_bll.EstaAsignadaAPerfil(_idFamiliaSeleccionada))
            {
                DialogResult confirm = MessageBox.Show("La familia está asignada a uno o más perfiles. Si la elimina, se quitará de esos perfiles. ¿Desea continuar?","Advertencia", 
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (confirm == DialogResult.No) return;
            }

            // Advertencia si tiene permisos asignados
            if (_bll.TienePermisosAsignados(_idFamiliaSeleccionada))
            {
                DialogResult confirm = MessageBox.Show("La familia tiene permisos asignados. Si la elimina, se quitarán todos los permisos asociados. ¿Desea continuar?","Advertencia", 
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (confirm == DialogResult.No) 
                { 
                    return; 
                }
            }
            else
            {
                DialogResult confirm = MessageBox.Show("¿Está seguro que desea eliminar la familia seleccionada?","Confirmar eliminación", 
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (confirm == DialogResult.No) 
                { 
                    return; 
                }
            }

            string mensaje;
            bool resultado = _bll.EliminarFamilia(_idFamiliaSeleccionada, out mensaje);

            if (resultado)
            {
                MessageBox.Show(mensaje, "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Resetear();
            }
            else
            {
                MessageBox.Show(mensaje, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            if (_idFamiliaSeleccionada <= 0)
            {
                MessageBox.Show("Debe seleccionar una Familia.", "Validación",MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (_idPermisoSeleccionado <= 0)
            {
                MessageBox.Show("Debe seleccionar un Permiso.", "Validación",MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string mensaje;
            bool resultado = _bll.AsignarPermiso(_idFamiliaSeleccionada, _idPermisoSeleccionado, out mensaje);

            if (resultado)
            {
                MessageBox.Show(mensaje, "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                CargarPermisosAsignados(_idFamiliaSeleccionada);
            }
            else
            {
                MessageBox.Show(mensaje, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnQuitar_Click(object sender, EventArgs e)
        {
            if (_idFamiliaSeleccionada <= 0)
            {
                MessageBox.Show("Debe seleccionar una Familia.", "Validación",MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (dgvAsignados.CurrentRow == null || dgvAsignados.CurrentRow.Cells[0].Value == null)
            {
                MessageBox.Show("Debe seleccionar un Permiso para quitar.", "Validación",MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int idPermiso = Convert.ToInt32(dgvAsignados.CurrentRow.Cells[0].Value);

            string mensaje;
            bool resultado = _bll.QuitarPermiso(_idFamiliaSeleccionada, idPermiso, out mensaje);

            if (resultado)
            {
                MessageBox.Show(mensaje, "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                CargarPermisosAsignados(_idFamiliaSeleccionada);
            }
            else
            {
                MessageBox.Show(mensaje, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            Resetear();
        }

        private void Resetear()
        {
            txtNombre.Text = "";
            _idFamiliaSeleccionada = -1;
            _idPermisoSeleccionado = -1;
            dgvAsignados.Rows.Clear();
            lblFamiliaSeleccionada.Text = "Permisos de: (ninguna seleccionada)";
            CargarFamilias();
            CargarPatentes();
        }

        private void ConfigurarDGV()
        {
            // dgvFamilias
            dgvFamilias.ReadOnly = true;
            dgvFamilias.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvFamilias.MultiSelect = false;
            dgvFamilias.AllowUserToAddRows = false;
            dgvFamilias.Columns.Clear();
            dgvFamilias.Columns.Add("colIdFamilia", "ID");
            dgvFamilias.Columns.Add("colNombreFamilia", "Familia");
            dgvFamilias.Columns["colIdFamilia"].Visible = false;
            dgvFamilias.Columns["colNombreFamilia"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            // dgvPatentes
            dgvPatentes.ReadOnly = true;
            dgvPatentes.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvPatentes.MultiSelect = false;
            dgvPatentes.AllowUserToAddRows = false;
            dgvPatentes.Columns.Clear();
            dgvPatentes.Columns.Add("colIdPatente", "ID");
            dgvPatentes.Columns.Add("colNombrePatente", "Permiso disponible");
            dgvPatentes.Columns["colIdPatente"].Visible = false;
            dgvPatentes.Columns["colNombrePatente"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            // dgvAsignados
            dgvAsignados.ReadOnly = true;
            dgvAsignados.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvAsignados.MultiSelect = false;
            dgvAsignados.AllowUserToAddRows = false;
            dgvAsignados.Columns.Clear();
            dgvAsignados.Columns.Add("colIdAsignado", "ID");
            dgvAsignados.Columns.Add("colNombreAsignado", "Permiso asignado");
            dgvAsignados.Columns["colIdAsignado"].Visible = false;
            dgvAsignados.Columns["colNombreAsignado"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}

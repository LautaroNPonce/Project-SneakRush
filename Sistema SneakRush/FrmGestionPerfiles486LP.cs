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
    public partial class FrmGestionPerfiles486LP : Form
    {
        private BLL_Perfil486LP _bll = new BLL_Perfil486LP();
        private int _idPerfilSeleccionado = -1;
        private int _idFamiliaSeleccionada = -1;
        private int _idPermisoSeleccionado = -1;

        public FrmGestionPerfiles486LP()
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;
        }

        private void FrmGestionPerfiles486LP_Load(object sender, EventArgs e)
        {
            ConfigurarDGV();
            CargarPerfiles();
            CargarFamilias();
            CargarPatentes();
        }
        private void CargarPerfiles()
        {
            dgvPerfiles.Rows.Clear();
            List<Perfil486LP> perfiles = _bll.ObtenerPerfiles();
            foreach (Perfil486LP p in perfiles)
            {
                dgvPerfiles.Rows.Add(p.IdPerfil, p.Nombre);
            }
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

        private void CargarFamiliasAsignadas(int idPerfil)
        {
            dgvFamiliasAsignadas.Rows.Clear();
            List<Familia486LP> asignadas = _bll.ObtenerFamiliasDePerfil(idPerfil);
            foreach (Familia486LP f in asignadas)
            {
                dgvFamiliasAsignadas.Rows.Add(f.Id, f.Nombre);
            }
        }

        private void CargarPermisosAsignados(int idPerfil)
        {
            dgvPermisosAsignados.Rows.Clear();
            List<Permiso486LP> asignados = _bll.ObtenerPermisosDePerfil(idPerfil);
            foreach (Permiso486LP p in asignados)
            {
                dgvPermisosAsignados.Rows.Add(p.Id, p.Nombre);
            }
        }

        private void dgvPerfiles_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvPerfiles.CurrentRow == null) return;
            if (dgvPerfiles.CurrentRow.Cells[0].Value == null) return;

            _idPerfilSeleccionado = Convert.ToInt32(dgvPerfiles.CurrentRow.Cells[0].Value);
            txtNombre.Text = dgvPerfiles.CurrentRow.Cells[1].Value.ToString();
            lblPerfilSeleccionado.Text = "Perfil: " + dgvPerfiles.CurrentRow.Cells[1].Value.ToString();

            CargarFamiliasAsignadas(_idPerfilSeleccionado);
            CargarPermisosAsignados(_idPerfilSeleccionado);
        }

        private void dgvFamilias_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvFamilias.CurrentRow == null) 
            { 
                return; 
            }
            if (dgvFamilias.CurrentRow.Cells[0].Value == null) 
            { 
                return; 
            }

            _idFamiliaSeleccionada = Convert.ToInt32(dgvFamilias.CurrentRow.Cells[0].Value);
        }

        private void dgvPatentes_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvPatentes.CurrentRow == null) 
            { 
                return; 
            }
            if (dgvPatentes.CurrentRow.Cells[0].Value == null) 
            { 
                return; 
            }

            _idPermisoSeleccionado = Convert.ToInt32(dgvPatentes.CurrentRow.Cells[0].Value);
        }

        private void btnCrear_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            {
                MessageBox.Show("Debe ingresar un nombre.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string mensaje;
            bool resultado = _bll.CrearPerfil(txtNombre.Text.Trim(), out mensaje);

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
            if (_idPerfilSeleccionado <= 0)
            {
                MessageBox.Show("Debe seleccionar un Perfil.", "Validación",MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            {
                MessageBox.Show("Debe ingresar un nombre válido.", "Validación",MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string mensaje;
            bool resultado = _bll.ModificarPerfil(_idPerfilSeleccionado, txtNombre.Text.Trim(), out mensaje);

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
            if (_idPerfilSeleccionado <= 0)
            {
                MessageBox.Show("Debe seleccionar un Perfil.", "Validación",MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Usuario486LP usuarioActual = SessionManager486LP.ObtenerInstancia().UsuarioActual();
            if (usuarioActual != null && usuarioActual.IdPerfil.HasValue && usuarioActual.IdPerfil.Value == _idPerfilSeleccionado)
            {
                MessageBox.Show("No puede eliminar el perfil que está usando actualmente.", "Error",MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (_bll.TieneUsuariosAsignados(_idPerfilSeleccionado))
            {
                MessageBox.Show("No se puede eliminar un Perfil con usuarios asignados.", "Error",MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (_bll.TieneFamiliasAsignadas(_idPerfilSeleccionado) || _bll.TienePermisosAsignados(_idPerfilSeleccionado))
            {
                DialogResult confirm = MessageBox.Show("El perfil tiene familias o permisos asignados. Si lo elimina, se quitarán todas las asociaciones. ¿Desea continuar?",
                    "Advertencia",MessageBoxButtons.YesNo,MessageBoxIcon.Warning);

                if (confirm == DialogResult.No)
                    return;
            }
            else
            {
                DialogResult confirm = MessageBox.Show("¿Está seguro que desea eliminar el perfil seleccionado?","Confirmar eliminación",
                    MessageBoxButtons.YesNo,MessageBoxIcon.Question);

                if (confirm == DialogResult.No)
                { 
                    return; 
                }
            }

            string mensaje;
            bool resultado = _bll.EliminarPerfil(_idPerfilSeleccionado, out mensaje);

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
        private void btnAsignarFamilia_Click(object sender, EventArgs e)
        {
            if (_idPerfilSeleccionado <= 0)
            {
                MessageBox.Show("Debe seleccionar un Perfil.", "Validación",MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (_idFamiliaSeleccionada <= 0)
            {
                MessageBox.Show("Debe seleccionar una Familia.", "Validación",MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string mensaje;
            bool resultado = _bll.AsignarFamilia(_idPerfilSeleccionado, _idFamiliaSeleccionada, out mensaje);

            if (resultado)
            {
                MessageBox.Show(mensaje, "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                CargarFamiliasAsignadas(_idPerfilSeleccionado);
            }
            else
            {
                MessageBox.Show(mensaje, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnQuitarFamilia_Click(object sender, EventArgs e)
        {
            if (_idPerfilSeleccionado <= 0)
            {
                MessageBox.Show("Debe seleccionar un Perfil.", "Validación",MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (dgvFamiliasAsignadas.CurrentRow == null || dgvFamiliasAsignadas.CurrentRow.Cells[0].Value == null)
            {
                MessageBox.Show("Debe seleccionar una Familia para quitar.", "Validación",MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int idFamilia = Convert.ToInt32(dgvFamiliasAsignadas.CurrentRow.Cells[0].Value);

            string mensaje;
            bool resultado = _bll.QuitarFamilia(_idPerfilSeleccionado, idFamilia, out mensaje);

            if (resultado)
            {
                MessageBox.Show(mensaje, "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                CargarFamiliasAsignadas(_idPerfilSeleccionado);
            }
            else
            {
                MessageBox.Show(mensaje, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnAsignarPermiso_Click(object sender, EventArgs e)
        {
            if (_idPerfilSeleccionado <= 0)
            {
                MessageBox.Show("Debe seleccionar un Perfil.", "Validación",MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (_idPermisoSeleccionado <= 0)
            {
                MessageBox.Show("Debe seleccionar un Permiso.", "Validación",MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string mensaje;
            bool resultado = _bll.AsignarPermiso(_idPerfilSeleccionado, _idPermisoSeleccionado, out mensaje);

            if (resultado)
            {
                MessageBox.Show(mensaje, "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                CargarPermisosAsignados(_idPerfilSeleccionado);
            }
            else
            {
                MessageBox.Show(mensaje, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnQuitarPermiso_Click(object sender, EventArgs e)
        {
            if (_idPerfilSeleccionado <= 0)
            {
                MessageBox.Show("Debe seleccionar un Perfil.", "Validación",MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (dgvPermisosAsignados.CurrentRow == null || dgvPermisosAsignados.CurrentRow.Cells[0].Value == null)
            {
                MessageBox.Show("Debe seleccionar un Permiso para quitar.", "Validación",MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int idPermiso = Convert.ToInt32(dgvPermisosAsignados.CurrentRow.Cells[0].Value);

            string mensaje;
            bool resultado = _bll.QuitarPermiso(_idPerfilSeleccionado, idPermiso, out mensaje);

            if (resultado)
            {
                MessageBox.Show(mensaje, "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                CargarPermisosAsignados(_idPerfilSeleccionado);
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
            _idPerfilSeleccionado = -1;
            _idFamiliaSeleccionada = -1;
            _idPermisoSeleccionado = -1;
            dgvFamiliasAsignadas.Rows.Clear();
            dgvPermisosAsignados.Rows.Clear();
            lblPerfilSeleccionado.Text = "Perfil: (ninguno seleccionado)";
            CargarPerfiles();
            CargarFamilias();
            CargarPatentes();
        }

        private void ConfigurarDGV()
        {
            // dgvPerfiles
            dgvPerfiles.ReadOnly = true;
            dgvPerfiles.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvPerfiles.MultiSelect = false;
            dgvPerfiles.AllowUserToAddRows = false;
            dgvPerfiles.Columns.Clear();
            dgvPerfiles.Columns.Add("colIdPerfil", "ID");
            dgvPerfiles.Columns.Add("colNombrePerfil", "Perfil");
            dgvPerfiles.Columns["colIdPerfil"].Visible = false;
            dgvPerfiles.Columns["colNombrePerfil"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            // dgvFamilias
            dgvFamilias.ReadOnly = true;
            dgvFamilias.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvFamilias.MultiSelect = false;
            dgvFamilias.AllowUserToAddRows = false;
            dgvFamilias.Columns.Clear();
            dgvFamilias.Columns.Add("colIdFamilia", "ID");
            dgvFamilias.Columns.Add("colNombreFamilia", "Familia disponible");
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

            // dgvFamiliasAsignadas
            dgvFamiliasAsignadas.ReadOnly = true;
            dgvFamiliasAsignadas.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvFamiliasAsignadas.MultiSelect = false;
            dgvFamiliasAsignadas.AllowUserToAddRows = false;
            dgvFamiliasAsignadas.Columns.Clear();
            dgvFamiliasAsignadas.Columns.Add("colIdFamiliaAsig", "ID");
            dgvFamiliasAsignadas.Columns.Add("colNombreFamiliaAsig", "Familia asignada");
            dgvFamiliasAsignadas.Columns["colIdFamiliaAsig"].Visible = false;
            dgvFamiliasAsignadas.Columns["colNombreFamiliaAsig"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            // dgvPermisosAsignados
            dgvPermisosAsignados.ReadOnly = true;
            dgvPermisosAsignados.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvPermisosAsignados.MultiSelect = false;
            dgvPermisosAsignados.AllowUserToAddRows = false;
            dgvPermisosAsignados.Columns.Clear();
            dgvPermisosAsignados.Columns.Add("colIdPermisoAsig", "ID");
            dgvPermisosAsignados.Columns.Add("colNombrePermisoAsig", "Permiso asignado");
            dgvPermisosAsignados.Columns["colIdPermisoAsig"].Visible = false;
            dgvPermisosAsignados.Columns["colNombrePermisoAsig"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        }
    }
}

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
        private List<Permiso486LP> _todasLasPatentes = new List<Permiso486LP>();
        private int _idFamiliaSeleccionada = -1;
        private int _idPermisoSeleccionado = -1;
        private bool _puedeCrear;
        private bool _puedeModificar;
        private bool _puedeEliminar;
        private bool _puedeAsignarPatente;
        private bool _puedeQuitarPatente;
        private bool _puedeCancelar;
        private bool _puedeSalir;

        public FrmGestionFamilias486LP()
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;
        }

        private void FrmGestionFamilias486LP_Load(object sender, EventArgs e)
        {
            ConfigurarDGV();
            CargarComboModulos();
            CargarFamilias();
            CargarPatentes();
            AjustarBotonesSegunPerfil();
            AplicarPermisosBotones();
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
            _todasLasPatentes = _bll.ObtenerPatentes();
            MostrarPatentes();
        }

        private void CargarComboModulos()
        {
            cmbFiltroModulo.Items.Clear();
            cmbFiltroModulo.Items.AddRange(new object[]
            {
                "Todos", "Usuarios", "Familias", "Perfiles", "Bitácora",
                "Respaldos", "Maestro", "Compra", "Venta", "Reporte",
                "Cierre de sesión", "Cambiar contraseña"
            });
            cmbFiltroModulo.SelectedIndex = 0;   // "Todos"
        }

        private void MostrarPatentes()
        {
            dgvPatentes.Rows.Clear();
            string filtro = cmbFiltroModulo.SelectedItem?.ToString() ?? "Todos";

            foreach (Permiso486LP p in _todasLasPatentes)
            {
                if (filtro == "Todos" || ObtenerModulo(p.Nombre) == filtro)
                {
                    dgvPatentes.Rows.Add(p.Id, p.Nombre);
                }
            }
        }

        private void cmbFiltroModulo_SelectedIndexChanged(object sender, EventArgs e)
        {
            MostrarPatentes();
        }

        // Deriva el módulo a partir del nombre de la patente (solo para el filtro visual).
        private string ObtenerModulo(string patente)
        {
            if (patente == "GESTION_USUARIOS" || patente.StartsWith("USUARIOS_")) return "Usuarios";
            if (patente == "GESTION_FAMILIAS" || patente.StartsWith("FAMILIAS_")) return "Familias";
            if (patente == "GESTION_ROLES" || patente.StartsWith("PERFILES_")) return "Perfiles";
            if (patente.StartsWith("BITACORA_")) return "Bitácora";
            if (patente == "GESTION_RESPALDOS") return "Respaldos";
            if (patente.StartsWith("MAESTRO_")) return "Maestro";
            if (patente.StartsWith("COMPRA_")) return "Compra";
            if (patente.StartsWith("VENTA_")) return "Venta";
            if (patente.StartsWith("REPORTE_")) return "Reporte";
            if (patente.StartsWith("CERRAR_SESION_")) return "Cierre de sesión";
            if (patente.StartsWith("CAMBIAR_CONTRASENA_")) return "Cambiar contraseña";
            return "Otros";
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

        private void AjustarBotonesSegunPerfil()
        {
            var usuario = SessionManager486LP.ObtenerInstancia().UsuarioActual();
            if (usuario == null) return;

            BLL_Perfil486LP bllPerfil = new BLL_Perfil486LP();
            List<string> permisos = bllPerfil.ObtenerPermisosPorRol(usuario.Rol);

            _puedeCrear = permisos.Contains("FAMILIAS_CREAR");
            _puedeModificar = permisos.Contains("FAMILIAS_MODIFICAR");
            _puedeEliminar = permisos.Contains("FAMILIAS_ELIMINAR");
            _puedeAsignarPatente = permisos.Contains("FAMILIAS_ASIGNAR_PATENTE");
            _puedeQuitarPatente = permisos.Contains("FAMILIAS_QUITAR_PATENTE");
            _puedeCancelar = permisos.Contains("FAMILIAS_CANCELAR");
            _puedeSalir = permisos.Contains("FAMILIAS_SALIR");
        }

        private void AplicarPermisosBotones()
        {
            btnCrear.Enabled = _puedeCrear;
            btnModificar.Enabled = _puedeModificar;
            btnEliminar.Enabled = _puedeEliminar;
            btnAgregar.Enabled = _puedeAsignarPatente;   // "Agregar a familia"
            btnQuitar.Enabled = _puedeQuitarPatente;    // "Quitar permiso seleccionado"
            btnCancelar.Enabled = _puedeCancelar;
            btnSalir.Enabled = _puedeSalir;
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

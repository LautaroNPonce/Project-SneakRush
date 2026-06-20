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
    public partial class FrmGestionFamilias486LP : Form, IObserver486LP
    {
        private BLL_Familia486LP _bll = new BLL_Familia486LP();
        private List<Permiso486LP> _todasLasPatentes = new List<Permiso486LP>();
        // Códigos internos de los módulos (en español, los que usa ObtenerModulo).
        // El orden DEBE coincidir con el orden en que se agregan al combo en CargarComboModulos.
        private readonly string[] _codigosModulo =
        {
            "Todos", "Usuarios", "Familias", "Perfiles", "Bitácora", "Respaldos",
            "Maestro", "Compra", "Venta", "Reporte", "Cierre de sesión", "Cambiar contraseña"
        };
        private List<int> _idsAsignados = new List<int>(); // Para no mostrar en disponibles los ya asignados
        private string _nombreFamiliaSeleccionada = string.Empty;
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
            Program.LanguageManager.Agregar(this);
            this.FormClosing += FrmGestionFamilias486LP_FormClosing;
        }

        private void FrmGestionFamilias486LP_Load(object sender, EventArgs e)
        {
            ConfigurarDGV();
            CargarComboModulos();
            CargarFamilias();
            CargarPatentes();
            AjustarBotonesSegunPerfil();
            AplicarPermisosBotones();
            ActualizarIdioma();
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
            var lm = Program.LanguageManager;
            string f = "FrmGestionFamilias486LP";

            int indiceActual = cmbFiltroModulo.SelectedIndex;   // recordar la selección por índice

            cmbFiltroModulo.SelectedIndexChanged -= cmbFiltroModulo_SelectedIndexChanged;

            cmbFiltroModulo.Items.Clear();
            cmbFiltroModulo.Items.AddRange(new object[]
            {
                lm.ObtenerTexto(f, "mod.Todos"),
                lm.ObtenerTexto(f, "mod.Usuarios"),
                lm.ObtenerTexto(f, "mod.Familias"),
                lm.ObtenerTexto(f, "mod.Perfiles"),
                lm.ObtenerTexto(f, "mod.Bitacora"),
                lm.ObtenerTexto(f, "mod.Respaldos"),
                lm.ObtenerTexto(f, "mod.Maestro"),
                lm.ObtenerTexto(f, "mod.Compra"),
                lm.ObtenerTexto(f, "mod.Venta"),
                lm.ObtenerTexto(f, "mod.Reporte"),
                lm.ObtenerTexto(f, "mod.CierreSesion"),
                lm.ObtenerTexto(f, "mod.CambiarContrasena")
            });

            cmbFiltroModulo.SelectedIndexChanged += cmbFiltroModulo_SelectedIndexChanged;
            cmbFiltroModulo.SelectedIndex = (indiceActual >= 0) ? indiceActual : 0;
        }

        private void MostrarPatentes()
        {
            _idPermisoSeleccionado = -1;
            dgvPatentes.Rows.Clear();
            string filtro = (cmbFiltroModulo.SelectedIndex >= 0) ? _codigosModulo[cmbFiltroModulo.SelectedIndex] : "Todos";

            foreach (Permiso486LP p in _todasLasPatentes)
            {
                // Excluir los permisos que ya están asignados a la familia seleccionada
                if (_idsAsignados.Contains(p.Id))
                    continue;

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
            _idsAsignados.Clear();

            List<Permiso486LP> asignados = _bll.ObtenerPermisosDeFamilia(idFamilia);
            foreach (Permiso486LP p in asignados)
            {
                dgvAsignados.Rows.Add(p.Id, p.Nombre);
                _idsAsignados.Add(p.Id);
            }

            // Refrescar "disponibles" para que no muestre lo que ya está asignado
            MostrarPatentes();
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
                _nombreFamiliaSeleccionada = f.Nombre;
                txtNombre.Text = f.Nombre;
                CargarPermisosAsignados(_idFamiliaSeleccionada);
                ActualizarLabelFamilia();
            }
            else
            {
                if (dgvFamilias.CurrentRow.Cells[0].Value != null)
                {
                    _idFamiliaSeleccionada = Convert.ToInt32(dgvFamilias.CurrentRow.Cells[0].Value);
                    _nombreFamiliaSeleccionada = dgvFamilias.CurrentRow.Cells[1].Value.ToString();
                    txtNombre.Text = dgvFamilias.CurrentRow.Cells[1].Value.ToString();
                    CargarPermisosAsignados(_idFamiliaSeleccionada);
                    ActualizarLabelFamilia();
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
            var lm = Program.LanguageManager;
            string f = "FrmGestionFamilias486LP";

            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            {
                MessageBox.Show(
                    lm.ObtenerTexto(f, "Msg.IngresarNombre", "Debe ingresar un nombre."),
                    lm.ObtenerTexto(f, "Msg.IngresarNombre.Title", "Validación"),
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (txtNombre.Text.Trim().Length > 50)
            {
                MessageBox.Show(
                    lm.ObtenerTexto(f, "Msg.NombreMuyLargo", "El nombre no puede superar los 50 caracteres."),
                    lm.ObtenerTexto(f, "Msg.NombreMuyLargo.Title", "Validación"),
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string mensaje;
            bool resultado = _bll.CrearFamilia(txtNombre.Text.Trim(), out mensaje);

            if (resultado)
            {
                MessageBox.Show(mensaje, lm.ObtenerTexto(f, "Msg.Exito.Title", "Éxito"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                Resetear();
            }
            else
            {
                MessageBox.Show(mensaje, lm.ObtenerTexto(f, "Msg.Error.Title", "Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnModificar_Click(object sender, EventArgs e)
        {
            var lm = Program.LanguageManager;
            string f = "FrmGestionFamilias486LP";

            if (_idFamiliaSeleccionada <= 0)
            {
                MessageBox.Show(lm.ObtenerTexto(f, "Msg.SeleccionarFamilia", "Debe seleccionar una Familia."),
                    lm.ObtenerTexto(f, "Msg.SeleccionarFamilia.Title", "Validación"),MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            {
                MessageBox.Show(lm.ObtenerTexto(f, "Msg.IngresarNombreValido", "Debe ingresar un nombre válido."),
                    lm.ObtenerTexto(f, "Msg.IngresarNombreValido.Title", "Validación"),MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (txtNombre.Text.Trim().Length > 50)
            {
                MessageBox.Show(lm.ObtenerTexto(f, "Msg.NombreMuyLargo", "El nombre no puede superar los 50 caracteres."),
                    lm.ObtenerTexto(f, "Msg.NombreMuyLargo.Title", "Validación"),MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string mensaje;
            bool resultado = _bll.ModificarFamilia(_idFamiliaSeleccionada, txtNombre.Text.Trim(), out mensaje);

            if (resultado)
            {
                MessageBox.Show(mensaje, lm.ObtenerTexto(f, "Msg.Exito.Title", "Éxito"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                Resetear();
            }
            else
            {
                MessageBox.Show(mensaje, lm.ObtenerTexto(f, "Msg.Error.Title", "Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            var lm = Program.LanguageManager;
            string f = "FrmGestionFamilias486LP";

            if (_idFamiliaSeleccionada <= 0)
            {
                MessageBox.Show(lm.ObtenerTexto(f, "Msg.SeleccionarFamilia", "Debe seleccionar una Familia."),
                    lm.ObtenerTexto(f, "Msg.SeleccionarFamilia.Title", "Validación"),MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Auto-impacto (prioridad): la familia es de mi propio perfil
            if (EsFamiliaDeMiPerfil(_idFamiliaSeleccionada))
            {
                DialogResult confirm = MessageBox.Show(lm.ObtenerTexto(f, "Msg.AutoImpactoEliminar", "Esta familia forma parte de su propio perfil. Si la elimina, perderá ese acceso usted mismo. ¿Desea continuar?"),
                    lm.ObtenerTexto(f, "Msg.AutoImpactoEliminar.Title", "Advertencia"),MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (confirm == DialogResult.No) return;
            }
            else if (_bll.EstaAsignadaAPerfil(_idFamiliaSeleccionada))
            {
                DialogResult confirm = MessageBox.Show(lm.ObtenerTexto(f, "Msg.AdvertenciaPerfiles", "La familia está asignada a uno o más perfiles. Si la elimina, se quitará de esos perfiles. ¿Desea continuar?"),
                    lm.ObtenerTexto(f, "Msg.AdvertenciaPerfiles.Title", "Advertencia"),MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (confirm == DialogResult.No) return;
            }

            if (_bll.TienePermisosAsignados(_idFamiliaSeleccionada))
            {
                DialogResult confirm = MessageBox.Show(lm.ObtenerTexto(f, "Msg.AdvertenciaPermisos", "La familia tiene permisos asignados. Si la elimina, se quitarán todos los permisos asociados. ¿Desea continuar?"),
                    lm.ObtenerTexto(f, "Msg.AdvertenciaPermisos.Title", "Advertencia"),MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (confirm == DialogResult.No) return;
            }
            else
            {
                DialogResult confirm = MessageBox.Show(lm.ObtenerTexto(f, "Msg.ConfirmarEliminar", "¿Está seguro que desea eliminar la familia seleccionada?"),
                    lm.ObtenerTexto(f, "Msg.ConfirmarEliminar.Title", "Confirmar eliminación"),MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (confirm == DialogResult.No) return;
            }

            string mensaje;
            bool resultado = _bll.EliminarFamilia(_idFamiliaSeleccionada, out mensaje);

            if (resultado)
            {
                MessageBox.Show(mensaje, lm.ObtenerTexto(f, "Msg.Exito.Title", "Éxito"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                Resetear();
            }
            else
            {
                MessageBox.Show(mensaje, lm.ObtenerTexto(f, "Msg.Error.Title", "Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            var lm = Program.LanguageManager;
            string f = "FrmGestionFamilias486LP";

            if (_idFamiliaSeleccionada <= 0)
            {
                MessageBox.Show(lm.ObtenerTexto(f, "Msg.SeleccionarFamilia", "Debe seleccionar una Familia."),
                    lm.ObtenerTexto(f, "Msg.SeleccionarFamilia.Title", "Validación"),MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (_idPermisoSeleccionado <= 0)
            {
                MessageBox.Show(lm.ObtenerTexto(f, "Msg.SeleccionarPermiso", "Debe seleccionar un Permiso."),
                    lm.ObtenerTexto(f, "Msg.SeleccionarPermiso.Title", "Validación"),MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string mensaje;
            bool resultado = _bll.AsignarPermiso(_idFamiliaSeleccionada, _idPermisoSeleccionado, out mensaje);

            if (resultado)
            {
                MessageBox.Show(mensaje, lm.ObtenerTexto(f, "Msg.Exito.Title", "Éxito"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                CargarPermisosAsignados(_idFamiliaSeleccionada);
            }
            else
            {
                MessageBox.Show(mensaje, lm.ObtenerTexto(f, "Msg.Error.Title", "Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnQuitar_Click(object sender, EventArgs e)
        {
            var lm = Program.LanguageManager;
            string f = "FrmGestionFamilias486LP";

            if (_idFamiliaSeleccionada <= 0)
            {
                MessageBox.Show(lm.ObtenerTexto(f, "Msg.SeleccionarFamilia", "Debe seleccionar una Familia."),
                    lm.ObtenerTexto(f, "Msg.SeleccionarFamilia.Title", "Validación"),MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (dgvAsignados.CurrentRow == null || dgvAsignados.CurrentRow.Cells[0].Value == null)
            {
                MessageBox.Show(lm.ObtenerTexto(f, "Msg.SeleccionarPermisoQuitar", "Debe seleccionar un Permiso para quitar."),
                    lm.ObtenerTexto(f, "Msg.SeleccionarPermisoQuitar.Title", "Validación"),MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (EsFamiliaDeMiPerfil(_idFamiliaSeleccionada))
            {
                DialogResult confirm = MessageBox.Show(lm.ObtenerTexto(f, "Msg.AutoImpactoQuitar", "Esta familia forma parte de su propio perfil. Si le quita este permiso, perderá ese acceso usted mismo. ¿Desea continuar?"),
                    lm.ObtenerTexto(f, "Msg.AutoImpactoQuitar.Title", "Advertencia"),MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (confirm == DialogResult.No)
                { 
                    return; 
                }
            }
            else if (_bll.EstaAsignadaAPerfil(_idFamiliaSeleccionada))
            {
                DialogResult confirm = MessageBox.Show(lm.ObtenerTexto(f, "Msg.QuitarPermisoEnUso", "Esta familia está asignada a uno o más perfiles. Si le quita este permiso, esos perfiles dejarán de tenerlo. ¿Desea continuar?"),
                    lm.ObtenerTexto(f, "Msg.QuitarPermisoEnUso.Title", "Advertencia"),MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (confirm == DialogResult.No)
                {
                    return;
                }
            }

            int idPermiso = Convert.ToInt32(dgvAsignados.CurrentRow.Cells[0].Value);
            string mensaje;
            bool resultado = _bll.QuitarPermiso(_idFamiliaSeleccionada, idPermiso, out mensaje);

            if (resultado)
            {
                MessageBox.Show(mensaje, lm.ObtenerTexto(f, "Msg.Exito.Title", "Éxito"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                CargarPermisosAsignados(_idFamiliaSeleccionada);
            }
            else
            {
                MessageBox.Show(mensaje, lm.ObtenerTexto(f, "Msg.Error.Title", "Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            _nombreFamiliaSeleccionada = string.Empty;
            dgvAsignados.Rows.Clear();
            _idsAsignados.Clear();   // sin familia seleccionada → todos los permisos vuelven a disponibles
            ActualizarLabelFamilia();
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
            btnAgregar.Enabled = _puedeAsignarPatente;   
            btnQuitar.Enabled = _puedeQuitarPatente;    
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

            TraducirColumnas();
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        public void ActualizarIdioma()
        {
            var lm = Program.LanguageManager;
            string f = "FrmGestionFamilias486LP";

            this.Text = lm.ObtenerTexto(f, "Title");
            lblTitulo.Text = lm.ObtenerTexto(f, "Title");

            pnlFamilias.Text = lm.ObtenerTexto(f, "pnlFamilias");
            pnlPatentes.Text = lm.ObtenerTexto(f, "pnlPatentes");
            pnlAsignados.Text = lm.ObtenerTexto(f, "pnlAsignados");

            btnCrear.Text = lm.ObtenerTexto(f, "btnCrear");
            btnModificar.Text = lm.ObtenerTexto(f, "btnModificar");
            btnEliminar.Text = lm.ObtenerTexto(f, "btnEliminar");
            btnCancelar.Text = lm.ObtenerTexto(f, "btnCancelar");
            btnAgregar.Text = lm.ObtenerTexto(f, "btnAgregar");
            btnQuitar.Text = lm.ObtenerTexto(f, "btnQuitar");
            btnSalir.Text = lm.ObtenerTexto(f, "btnSalir");

            TraducirColumnas();
            ActualizarLabelFamilia();
            CargarComboModulos();
        }

        private void TraducirColumnas()
        {
            var lm = Program.LanguageManager;
            string f = "FrmGestionFamilias486LP";

            if (dgvFamilias.Columns.Contains("colNombreFamilia"))
                dgvFamilias.Columns["colNombreFamilia"].HeaderText = lm.ObtenerTexto(f, "col.Familia");
            if (dgvPatentes.Columns.Contains("colNombrePatente"))
                dgvPatentes.Columns["colNombrePatente"].HeaderText = lm.ObtenerTexto(f, "col.PermisoDisponible");
            if (dgvAsignados.Columns.Contains("colNombreAsignado"))
                dgvAsignados.Columns["colNombreAsignado"].HeaderText = lm.ObtenerTexto(f, "col.PermisoAsignado");
        }

        private void ActualizarLabelFamilia()
        {
            var lm = Program.LanguageManager;
            string f = "FrmGestionFamilias486LP";

            if (_idFamiliaSeleccionada > 0 && !string.IsNullOrEmpty(_nombreFamiliaSeleccionada))
                lblFamiliaSeleccionada.Text = string.Format(lm.ObtenerTexto(f, "lbl.PermisosDe"), _nombreFamiliaSeleccionada);
            else
                lblFamiliaSeleccionada.Text = lm.ObtenerTexto(f, "lbl.PermisosDeNinguna");
        }

        private void FrmGestionFamilias486LP_FormClosing(object sender, FormClosingEventArgs e)
        {
            Program.LanguageManager.Quitar(this);
        }

        // ¿La familia pertenece al perfil del usuario logueado actualmente?
        private bool EsFamiliaDeMiPerfil(int idFamilia)
        {
            Usuario486LP usuario = SessionManager486LP.ObtenerInstancia().UsuarioActual();
            if (usuario == null || !usuario.IdPerfil.HasValue)
            { 
                return false; 
            }

            BLL_Perfil486LP bllPerfil = new BLL_Perfil486LP();
            List<Familia486LP> misFamilias = bllPerfil.ObtenerFamiliasDePerfil(usuario.IdPerfil.Value);
            return misFamilias.Any(f => f.Id == idFamilia);
        }
    }
}

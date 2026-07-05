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
    public partial class FrmGestionPerfiles486LP : Form, IObserver486LP
    {
        private BLL_Perfil486LP _bll = new BLL_Perfil486LP();
        private BLL_Familia486LP _bllFamilia = new BLL_Familia486LP();
        private List<Permiso486LP> _todasLasPatentes = new List<Permiso486LP>();
        private List<int> _idsFamiliasAsignadas = new List<int>();
        private List<int> _idsPermisosAsignados = new List<int>();
        private readonly string[] _codigosModulo =
        {
            "Todos", "Usuarios", "Familias", "Perfiles", "Bitácora", "Respaldos",
            "Maestro", "Compra", "Venta", "Reporte", "Cierre de sesión", "Cambiar contraseña"
        };

        // Permisos que dan acceso a administrar el sistema y no se pueden quitar del Administrador.
        private readonly string[] _permisosCriticos =
        {
            "GESTION_ROLES", "GESTION_FAMILIAS",
            "PERFILES_ASIGNAR_FAMILIA", "PERFILES_QUITAR_FAMILIA",
            "PERFILES_ASIGNAR_PATENTE", "PERFILES_QUITAR_PATENTE",
            "FAMILIAS_ASIGNAR_PATENTE", "FAMILIAS_QUITAR_PATENTE"
        };

        private string _nombrePerfilSeleccionado = string.Empty;
        private int _idPerfilSeleccionado = -1;
        private int _idFamiliaSeleccionada = -1;
        private int _idPermisoSeleccionado = -1;
        private bool _puedeCrear;
        private bool _puedeModificar;
        private bool _puedeEliminar;
        private bool _puedeAsignarFamilia;
        private bool _puedeQuitarFamilia;
        private bool _puedeAsignarPatente;
        private bool _puedeQuitarPatente;
        private bool _puedeCancelar;
        private bool _puedeSalir;

        public FrmGestionPerfiles486LP()
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;
            Program.LanguageManager.Agregar(this);
            this.FormClosing += FrmGestionPerfiles486LP_FormClosing;
        }

        private void FrmGestionPerfiles486LP_Load(object sender, EventArgs e)
        {
            ConfigurarDGV();
            CargarComboModulos();
            CargarPerfiles();
            CargarFamilias();
            CargarPatentes();
            AjustarBotonesSegunPerfil();
            AplicarPermisosBotones();
            ActualizarIdioma();
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
            _idFamiliaSeleccionada = -1;
            dgvFamilias.Rows.Clear();
            List<Familia486LP> familias = _bll.ObtenerFamilias();
            foreach (Familia486LP f in familias)
            {
                // Excluyo las familias que ya están asignadas al perfil seleccionado
                if (_idsFamiliasAsignadas.Contains(f.Id))
                    continue;

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
            string f = "FrmGestionPerfiles486LP";

            int indiceActual = cmbFiltroModulo.SelectedIndex;

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
                // Excluyo los permisos que ya están asignados al perfil seleccionado
                if (_idsPermisosAsignados.Contains(p.Id))
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

        // Derivo el módulo a partir del nombre de la patente (solo para el filtro visual).
        private string ObtenerModulo(string patente)
        {
            if (patente == "GESTION_USUARIOS" || patente.StartsWith("USUARIOS_")) return "Usuarios";
            if (patente == "GESTION_FAMILIAS" || patente.StartsWith("FAMILIAS_")) return "Familias";
            if (patente == "GESTION_ROLES" || patente.StartsWith("PERFILES_")) return "Perfiles";
            if (patente.StartsWith("BITACORA_")) return "Bitácora";
            if (patente == "GESTION_RESPALDOS" || patente.StartsWith("RESPALDOS_")) return "Respaldos";
            if (patente.StartsWith("MAESTRO_")) return "Maestro";
            if (patente.StartsWith("COMPRA_")) return "Compra";
            if (patente.StartsWith("VENTA_")) return "Venta";
            if (patente.StartsWith("REPORTE_")) return "Reporte";
            if (patente.StartsWith("CERRAR_SESION_")) return "Cierre de sesión";
            if (patente.StartsWith("CAMBIAR_CONTRASENA_")) return "Cambiar contraseña";
            return "Otros";
        }

        private void CargarFamiliasAsignadas(int idPerfil)
        {
            dgvFamiliasAsignadas.Rows.Clear();
            dgvPermisosDeFamilia.Rows.Clear();
            _idsFamiliasAsignadas.Clear();

            List<Familia486LP> asignadas = _bll.ObtenerFamiliasDePerfil(idPerfil);
            foreach (Familia486LP f in asignadas)
            {
                dgvFamiliasAsignadas.Rows.Add(f.Id, f.Nombre);
                _idsFamiliasAsignadas.Add(f.Id);
            }

            // Refrescar "familias disponibles" para que no muestre las ya asignadas
            CargarFamilias();
        }

        private void CargarPermisosAsignados(int idPerfil)
        {
            dgvPermisosAsignados.Rows.Clear();
            _idsPermisosAsignados.Clear();

            List<Permiso486LP> asignados = _bll.ObtenerPermisosDePerfil(idPerfil);
            foreach (Permiso486LP p in asignados)
            {
                dgvPermisosAsignados.Rows.Add(p.Id, p.Nombre);
                _idsPermisosAsignados.Add(p.Id);
            }

            // Refrescar "permisos disponibles" para que no muestre los ya asignados
            MostrarPatentes();
        }

        private void dgvPerfiles_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvPerfiles.CurrentRow == null) return;
            if (dgvPerfiles.CurrentRow.Cells[0].Value == null) return;

            _idPerfilSeleccionado = Convert.ToInt32(dgvPerfiles.CurrentRow.Cells[0].Value);
            _nombrePerfilSeleccionado = dgvPerfiles.CurrentRow.Cells[1].Value.ToString();
            txtNombre.Text = dgvPerfiles.CurrentRow.Cells[1].Value.ToString();
            ActualizarLabelPerfil();

            CargarFamiliasAsignadas(_idPerfilSeleccionado);
            CargarPermisosAsignados(_idPerfilSeleccionado);
        }

        private void dgvFamiliasAsignadas_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvFamiliasAsignadas.CurrentRow == null || dgvFamiliasAsignadas.CurrentRow.Cells[0].Value == null)
            {
                dgvPermisosDeFamilia.Rows.Clear();
                return;
            }

            int idFamilia = Convert.ToInt32(dgvFamiliasAsignadas.CurrentRow.Cells[0].Value);
            CargarPermisosDeFamilia(idFamilia);
        }

        // Muestra el contenido (patentes) de la familia asignada seleccionada.
        private void CargarPermisosDeFamilia(int idFamilia)
        {
            dgvPermisosDeFamilia.Rows.Clear();
            List<Permiso486LP> permisos = _bllFamilia.ObtenerPermisosDeFamilia(idFamilia);
            foreach (Permiso486LP p in permisos)
            {
                dgvPermisosDeFamilia.Rows.Add(p.Id, p.Nombre);
            }
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
            var lm = Program.LanguageManager;
            string f = "FrmGestionPerfiles486LP";

            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            {
                MessageBox.Show(lm.ObtenerTexto(f, "Msg.IngresarNombre", "Debe ingresar un nombre."),
                    lm.ObtenerTexto(f, "Msg.IngresarNombre.Title", "Validación"),MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (txtNombre.Text.Trim().Length > 50)
            {
                MessageBox.Show(lm.ObtenerTexto(f, "Msg.NombreMuyLargo", "El nombre no puede superar los 50 caracteres."),
                    lm.ObtenerTexto(f, "Msg.NombreMuyLargo.Title", "Validación"),MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string mensaje;
            bool resultado = _bll.CrearPerfil(txtNombre.Text.Trim(), out mensaje);

            if (resultado)
            {
                MessageBox.Show(lm.ObtenerTexto(f, "Msg.PerfilCreado", "Perfil creado correctamente."),lm.ObtenerTexto(f, "Msg.Exito.Title", "Éxito"), 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                Resetear();
            }
            else
            {
                MessageBox.Show(lm.ObtenerTexto(f, mensaje, mensaje),lm.ObtenerTexto(f, "Msg.Error.Title", "Error"), 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnModificar_Click(object sender, EventArgs e)
        {
            var lm = Program.LanguageManager;
            string f = "FrmGestionPerfiles486LP";

            if (_idPerfilSeleccionado <= 0)
            {
                MessageBox.Show(lm.ObtenerTexto(f, "Msg.SeleccionarPerfil", "Debe seleccionar un Perfil."),
                    lm.ObtenerTexto(f, "Msg.SeleccionarPerfil.Title", "Validación"),MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
            bool resultado = _bll.ModificarPerfil(_idPerfilSeleccionado, txtNombre.Text.Trim(), out mensaje);

            if (resultado)
            {
                Usuario486LP usuarioActual = SessionManager486LP.ObtenerInstancia().UsuarioActual();
                if (usuarioActual != null && usuarioActual.IdPerfil.HasValue
                    && usuarioActual.IdPerfil.Value == _idPerfilSeleccionado)
                {
                    MessageBox.Show(lm.ObtenerTexto(f, "Msg.ModificarPerfilPropio", "Modificó el nombre de su propio perfil. Debe cerrar sesión e iniciar nuevamente para que el cambio tome efecto."),
                        lm.ObtenerTexto(f, "Msg.ModificarPerfilPropio.Title", "Aviso"),MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                MessageBox.Show(lm.ObtenerTexto(f, "Msg.PerfilModificado", "Perfil modificado correctamente."),
                    lm.ObtenerTexto(f, "Msg.Exito.Title", "Éxito"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                Resetear();
            }
            else
            {
                MessageBox.Show(lm.ObtenerTexto(f, mensaje, mensaje),lm.ObtenerTexto(f, "Msg.Error.Title", "Error"), 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void btnEliminar_Click(object sender, EventArgs e)
        {
            var lm = Program.LanguageManager;
            string f = "FrmGestionPerfiles486LP";

            if (_idPerfilSeleccionado <= 0)
            {
                MessageBox.Show(lm.ObtenerTexto(f, "Msg.SeleccionarPerfil", "Debe seleccionar un Perfil."),
                    lm.ObtenerTexto(f, "Msg.SeleccionarPerfil.Title", "Validación"),MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Usuario486LP usuarioActual = SessionManager486LP.ObtenerInstancia().UsuarioActual();
            if (usuarioActual != null && usuarioActual.IdPerfil.HasValue
                && usuarioActual.IdPerfil.Value == _idPerfilSeleccionado)
            {
                MessageBox.Show(lm.ObtenerTexto(f, "Msg.NoEliminarPerfilActivo", "No puede eliminar el perfil que está usando actualmente."),
                    lm.ObtenerTexto(f, "Msg.NoEliminarPerfilActivo.Title", "Error"),MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (_bll.TieneUsuariosAsignados(_idPerfilSeleccionado))
            {
                DialogResult confirm = MessageBox.Show(lm.ObtenerTexto(f, "Msg.AdvertenciaUsuarios", "Hay usuarios asignados a este perfil. Al eliminarlo quedarán sin perfil asignado. ¿Desea continuar?"),
                    lm.ObtenerTexto(f, "Msg.AdvertenciaUsuarios.Title", "Advertencia"),MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (confirm == DialogResult.No) 
                { 
                    return; 
                }
            }

            if (_bll.TieneFamiliasAsignadas(_idPerfilSeleccionado) || _bll.TienePermisosAsignados(_idPerfilSeleccionado))
            {
                DialogResult confirm = MessageBox.Show(lm.ObtenerTexto(f, "Msg.AdvertenciaAsociaciones", "El perfil tiene familias o permisos asignados. Si lo elimina, se quitarán todas las asociaciones. ¿Desea continuar?"),
                    lm.ObtenerTexto(f, "Msg.AdvertenciaAsociaciones.Title", "Advertencia"),MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (confirm == DialogResult.No) 
                { 
                    return; 
                }
            }
            else
            {
                DialogResult confirm = MessageBox.Show(lm.ObtenerTexto(f, "Msg.ConfirmarEliminar", "¿Está seguro que desea eliminar el perfil seleccionado?"),
                    lm.ObtenerTexto(f, "Msg.ConfirmarEliminar.Title", "Confirmar eliminación"),MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (confirm == DialogResult.No) 
                { 
                    return; 
                }
            }

            string mensaje;
            bool resultado = _bll.EliminarPerfil(_idPerfilSeleccionado, out mensaje);

            if (resultado)
            {
                MessageBox.Show(lm.ObtenerTexto(f, "Msg.PerfilEliminado", "Perfil eliminado correctamente."),
                    lm.ObtenerTexto(f, "Msg.Exito.Title", "Éxito"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                Resetear();
            }
            else
            {
                MessageBox.Show(lm.ObtenerTexto(f, mensaje, mensaje),
                    lm.ObtenerTexto(f, "Msg.Error.Title", "Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnAsignarFamilia_Click(object sender, EventArgs e)
        {
            var lm = Program.LanguageManager;
            string f = "FrmGestionPerfiles486LP";

            if (_idPerfilSeleccionado <= 0)
            {
                MessageBox.Show(lm.ObtenerTexto(f, "Msg.SeleccionarPerfil", "Debe seleccionar un Perfil."),
                    lm.ObtenerTexto(f, "Msg.SeleccionarPerfil.Title", "Validación"),MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (_idFamiliaSeleccionada <= 0)
            {
                MessageBox.Show(lm.ObtenerTexto(f, "Msg.SeleccionarFamilia", "Debe seleccionar una Familia."),
                    lm.ObtenerTexto(f, "Msg.SeleccionarFamilia.Title", "Validación"),MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // La familia comparte permisos con otra ya asignada: avisar, no bloquear
            if (_bll.FamiliaSolapaConAsignadas(_idPerfilSeleccionado, _idFamiliaSeleccionada))
            {
                DialogResult confirm = MessageBox.Show(lm.ObtenerTexto(f, "Msg.PermisosCompartidos", "Esta familia comparte permisos con otra ya asignada a este perfil. ¿Desea asignársela igual?\n\nNo cambia nada: si el permiso está en las dos familias, el perfil lo tiene una sola vez."),
                    lm.ObtenerTexto(f, "Msg.PermisosCompartidos.Title", "Permisos compartidos"),MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (confirm == DialogResult.No) 
                { 
                    return; 
                }
            }

            string mensaje;
            bool resultado = _bll.AsignarFamilia(_idPerfilSeleccionado, _idFamiliaSeleccionada, out mensaje);

            if (resultado)
            {
                // al reves quitar los permisos sueltos que esta familia ya cubre
                List<string> quitados = _bll.QuitarSueltosCubiertosPorFamilia(_idPerfilSeleccionado, _idFamiliaSeleccionada);
                if (quitados.Count > 0)
                {
                    MessageBox.Show(lm.ObtenerTexto(f, "Msg.SueltosQuitados", "Esta familia ya incluye permisos que tenías asignados sueltos. Se quitaron de los permisos directos porque ahora quedan cubiertos por la familia:") + "\n\n- " + string.Join("\n- ", quitados),
                        lm.ObtenerTexto(f, "Msg.SueltosQuitados.Title", "Permisos sueltos quitados"),MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                MessageBox.Show(lm.ObtenerTexto(f, "Msg.FamiliaAsignada", "Familia asignada correctamente."),
                    lm.ObtenerTexto(f, "Msg.Exito.Title", "Éxito"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                CargarFamiliasAsignadas(_idPerfilSeleccionado);
                CargarPermisosAsignados(_idPerfilSeleccionado);
            }
            else
            {
                MessageBox.Show(lm.ObtenerTexto(f, mensaje, mensaje),
                    lm.ObtenerTexto(f, "Msg.Error.Title", "Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnQuitarFamilia_Click(object sender, EventArgs e)
        {
            var lm = Program.LanguageManager;
            string f = "FrmGestionPerfiles486LP";

            if (_idPerfilSeleccionado <= 0)
            {
                MessageBox.Show(lm.ObtenerTexto(f, "Msg.SeleccionarPerfil", "Debe seleccionar un Perfil."),
                    lm.ObtenerTexto(f, "Msg.SeleccionarPerfil.Title", "Validación"),MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (dgvFamiliasAsignadas.CurrentRow == null || dgvFamiliasAsignadas.CurrentRow.Cells[0].Value == null)
            {
                MessageBox.Show(lm.ObtenerTexto(f, "Msg.SeleccionarFamiliaQuitar", "Debe seleccionar una Familia para quitar."),
                    lm.ObtenerTexto(f, "Msg.SeleccionarFamiliaQuitar.Title", "Validación"),MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Usuario486LP usuarioActual = SessionManager486LP.ObtenerInstancia().UsuarioActual();
            if (usuarioActual != null && usuarioActual.IdPerfil.HasValue && usuarioActual.IdPerfil.Value == _idPerfilSeleccionado)
            {
                DialogResult confirm = MessageBox.Show(lm.ObtenerTexto(f, "Msg.AdvertenciaQuitarFamilia", "Está quitando una familia de su propio perfil. Perderá acceso a sus funciones la próxima vez que inicie sesión. ¿Desea continuar?"),
                    lm.ObtenerTexto(f, "Msg.AdvertenciaQuitarFamilia.Title", "Advertencia"),MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (confirm == DialogResult.No) 
                { 
                    return; 
                }
            }

            int idFamilia = Convert.ToInt32(dgvFamiliasAsignadas.CurrentRow.Cells[0].Value);

            string mensaje;
            bool resultado = _bll.QuitarFamilia(_idPerfilSeleccionado, idFamilia, out mensaje);

            if (resultado)
            {
                MessageBox.Show(lm.ObtenerTexto(f, "Msg.FamiliaQuitada", "Familia quitada correctamente."),
                    lm.ObtenerTexto(f, "Msg.Exito.Title", "Éxito"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                CargarFamiliasAsignadas(_idPerfilSeleccionado);
            }
            else
            {
                MessageBox.Show(lm.ObtenerTexto(f, mensaje, mensaje),
                    lm.ObtenerTexto(f, "Msg.Error.Title", "Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnAsignarPermiso_Click(object sender, EventArgs e)
        {
            var lm = Program.LanguageManager;
            string f = "FrmGestionPerfiles486LP";

            if (_idPerfilSeleccionado <= 0)
            {
                MessageBox.Show(lm.ObtenerTexto(f, "Msg.SeleccionarPerfil", "Debe seleccionar un Perfil."),
                    lm.ObtenerTexto(f, "Msg.SeleccionarPerfil.Title", "Validación"),MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (_idPermisoSeleccionado <= 0)
            {
                MessageBox.Show(lm.ObtenerTexto(f, "Msg.SeleccionarPermiso", "Debe seleccionar un Permiso."),
                    lm.ObtenerTexto(f, "Msg.SeleccionarPermiso.Title", "Validación"),MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string mensaje;
            bool resultado = _bll.AsignarPermiso(_idPerfilSeleccionado, _idPermisoSeleccionado, out mensaje);

            if (resultado)
            {
                MessageBox.Show(lm.ObtenerTexto(f, "Msg.PermisoAsignado", "Permiso asignado correctamente."),
                    lm.ObtenerTexto(f, "Msg.Exito.Title", "Éxito"),MessageBoxButtons.OK, MessageBoxIcon.Information);
                CargarPermisosAsignados(_idPerfilSeleccionado);
            }
            else
            {
                string texto = lm.ObtenerTexto(f, mensaje, mensaje);

                if (mensaje == "Msg.PermisoYaEnFamilia")
                {
                    texto = string.Format(texto, _bll.FamiliaQueContienePermiso(_idPerfilSeleccionado, _idPermisoSeleccionado));
                }
                MessageBox.Show(texto, lm.ObtenerTexto(f, "Msg.Error.Title", "Error"),MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnQuitarPermiso_Click(object sender, EventArgs e)
        {
            var lm = Program.LanguageManager;
            string f = "FrmGestionPerfiles486LP";

            if (_idPerfilSeleccionado <= 0)
            {
                MessageBox.Show(lm.ObtenerTexto(f, "Msg.SeleccionarPerfil", "Debe seleccionar un Perfil."),
                    lm.ObtenerTexto(f, "Msg.SeleccionarPerfil.Title", "Validación"),MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (dgvPermisosAsignados.CurrentRow == null || dgvPermisosAsignados.CurrentRow.Cells[0].Value == null)
            {
                MessageBox.Show(lm.ObtenerTexto(f, "Msg.SeleccionarPermisoQuitar", "Debe seleccionar un Permiso para quitar."),
                    lm.ObtenerTexto(f, "Msg.SeleccionarPermisoQuitar.Title", "Validación"),MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int idPermiso = Convert.ToInt32(dgvPermisosAsignados.CurrentRow.Cells[0].Value);
            string nombrePermiso = dgvPermisosAsignados.CurrentRow.Cells[1].Value.ToString();

            // Auto-bloqueo: no se pueden quitar permisos críticos de gestión del Administrador
            if (_nombrePerfilSeleccionado == "Administrador" && _permisosCriticos.Contains(nombrePermiso))
            {
                MessageBox.Show(lm.ObtenerTexto(f, "Msg.BloqueoAdmin", "No puede quitar este permiso del perfil Administrador: dejaría al sistema sin forma de administrar permisos."),
                    lm.ObtenerTexto(f, "Msg.BloqueoAdmin.Title", "Acción no permitida"),MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Advertir si está quitando un permiso de su propio perfil
            Usuario486LP usuarioActual = SessionManager486LP.ObtenerInstancia().UsuarioActual();
            if (usuarioActual != null && usuarioActual.IdPerfil.HasValue && usuarioActual.IdPerfil.Value == _idPerfilSeleccionado)
            {
                DialogResult confirm = MessageBox.Show(lm.ObtenerTexto(f, "Msg.AdvertenciaQuitarPermiso", "Está quitando un permiso de su propio perfil. Perderá acceso a esa función la próxima vez que inicie sesión. ¿Desea continuar?"),
                    lm.ObtenerTexto(f, "Msg.AdvertenciaQuitarPermiso.Title", "Advertencia"),MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (confirm == DialogResult.No) return;
            }

            string mensaje;
            bool resultado = _bll.QuitarPermiso(_idPerfilSeleccionado, idPermiso, out mensaje);

            if (resultado)
            {
                MessageBox.Show(lm.ObtenerTexto(f, "Msg.PermisoQuitado", "Permiso quitado correctamente."),
                    lm.ObtenerTexto(f, "Msg.Exito.Title", "Éxito"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                CargarPermisosAsignados(_idPerfilSeleccionado);
            }
            else
            {
                MessageBox.Show(lm.ObtenerTexto(f, mensaje, mensaje),
                    lm.ObtenerTexto(f, "Msg.Error.Title", "Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            _nombrePerfilSeleccionado = string.Empty;
            dgvFamiliasAsignadas.Rows.Clear();
            dgvPermisosAsignados.Rows.Clear();
            dgvPermisosDeFamilia.Rows.Clear();
            _idsFamiliasAsignadas.Clear();   // sin perfil seleccionado todo vuelve a disponibles
            _idsPermisosAsignados.Clear();
            ActualizarLabelPerfil();
            CargarPerfiles();
            CargarFamilias();
            CargarPatentes();
        }

        private void AjustarBotonesSegunPerfil()
        {
            var usuario = SessionManager486LP.ObtenerInstancia().UsuarioActual();
            if (usuario == null) return;

            List<string> permisos = _bll.ObtenerPermisosPorRol(usuario.Rol);

            _puedeCrear = permisos.Contains("PERFILES_CREAR");
            _puedeModificar = permisos.Contains("PERFILES_MODIFICAR");
            _puedeEliminar = permisos.Contains("PERFILES_ELIMINAR");
            _puedeAsignarFamilia = permisos.Contains("PERFILES_ASIGNAR_FAMILIA");
            _puedeQuitarFamilia = permisos.Contains("PERFILES_QUITAR_FAMILIA");
            _puedeAsignarPatente = permisos.Contains("PERFILES_ASIGNAR_PATENTE");
            _puedeQuitarPatente = permisos.Contains("PERFILES_QUITAR_PATENTE");
            _puedeCancelar = permisos.Contains("PERFILES_CANCELAR");
            _puedeSalir = permisos.Contains("PERFILES_SALIR");
        }

        private void AplicarPermisosBotones()
        {
            btnCrear.Enabled = _puedeCrear;
            btnModificar.Enabled = _puedeModificar;
            btnEliminar.Enabled = _puedeEliminar;
            btnAsignarFamilia.Enabled = _puedeAsignarFamilia;
            btnQuitarFamilia.Enabled = _puedeQuitarFamilia;
            btnAsignarPermiso.Enabled = _puedeAsignarPatente;   // "Asignar permiso al perfil"
            btnQuitarPermiso.Enabled = _puedeQuitarPatente;    // "Quitar permiso seleccionado"
            btnCancelar.Enabled = _puedeCancelar;
            btnSalir.Enabled = _puedeSalir;
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

            // dgvPermisosDeFamilia (muestra los permisos de la familia asignada seleccionada)
            dgvPermisosDeFamilia.ReadOnly = true;
            dgvPermisosDeFamilia.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvPermisosDeFamilia.MultiSelect = false;
            dgvPermisosDeFamilia.AllowUserToAddRows = false;
            dgvPermisosDeFamilia.Columns.Clear();
            dgvPermisosDeFamilia.Columns.Add("colIdPermisoFam", "ID");
            dgvPermisosDeFamilia.Columns.Add("colNombrePermisoFam", "Permiso de la familia");
            dgvPermisosDeFamilia.Columns["colIdPermisoFam"].Visible = false;
            dgvPermisosDeFamilia.Columns["colNombrePermisoFam"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            TraducirColumnas();
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        public void ActualizarIdioma()
        {
            var lm = Program.LanguageManager;
            string f = "FrmGestionPerfiles486LP";

            this.Text = lm.ObtenerTexto(f, "Title");
            lblTitulo.Text = lm.ObtenerTexto(f, "Title");

            pnlPerfiles.Text = lm.ObtenerTexto(f, "pnlPerfiles");
            pnlFamilias.Text = lm.ObtenerTexto(f, "pnlFamilias");
            pnlPatentes.Text = lm.ObtenerTexto(f, "pnlPatentes");
            pnlComposicion.Text = lm.ObtenerTexto(f, "pnlComposicion");

            lblFamiliasAsignadas.Text = lm.ObtenerTexto(f, "lblFamiliasAsignadas");
            lblPermisosAsignados.Text = lm.ObtenerTexto(f, "lblPermisosAsignados");
            lblPermisosDeFamilia.Text = lm.ObtenerTexto(f, "lblPermisosDeFamilia");

            btnCrear.Text = lm.ObtenerTexto(f, "btnCrear");
            btnModificar.Text = lm.ObtenerTexto(f, "btnModificar");
            btnEliminar.Text = lm.ObtenerTexto(f, "btnEliminar");
            btnCancelar.Text = lm.ObtenerTexto(f, "btnCancelar");
            btnAsignarFamilia.Text = lm.ObtenerTexto(f, "btnAsignarFamilia");
            btnQuitarFamilia.Text = lm.ObtenerTexto(f, "btnQuitarFamilia");
            btnAsignarPermiso.Text = lm.ObtenerTexto(f, "btnAsignarPermiso");
            btnQuitarPermiso.Text = lm.ObtenerTexto(f, "btnQuitarPermiso");
            btnSalir.Text = lm.ObtenerTexto(f, "btnSalir");

            TraducirColumnas();
            ActualizarLabelPerfil();
            CargarComboModulos();
        }

        private void TraducirColumnas()
        {
            var lm = Program.LanguageManager;
            string f = "FrmGestionPerfiles486LP";

            if (dgvPerfiles.Columns.Contains("colNombrePerfil"))
            { 
                dgvPerfiles.Columns["colNombrePerfil"].HeaderText = lm.ObtenerTexto(f, "col.Perfil"); 
            }
            if (dgvFamilias.Columns.Contains("colNombreFamilia"))
            { 
                dgvFamilias.Columns["colNombreFamilia"].HeaderText = lm.ObtenerTexto(f, "col.FamiliaDisponible"); 
            }
            if (dgvPatentes.Columns.Contains("colNombrePatente"))
            { 
                dgvPatentes.Columns["colNombrePatente"].HeaderText = lm.ObtenerTexto(f, "col.PermisoDisponible"); 
            }
            if (dgvFamiliasAsignadas.Columns.Contains("colNombreFamiliaAsig"))
            { 
                dgvFamiliasAsignadas.Columns["colNombreFamiliaAsig"].HeaderText = lm.ObtenerTexto(f, "col.FamiliaAsignada"); 
            }
            if (dgvPermisosAsignados.Columns.Contains("colNombrePermisoAsig"))
            { 
                dgvPermisosAsignados.Columns["colNombrePermisoAsig"].HeaderText = lm.ObtenerTexto(f, "col.PermisoAsignado"); 
            }
            if (dgvPermisosDeFamilia.Columns.Contains("colNombrePermisoFam"))
            {
                dgvPermisosDeFamilia.Columns["colNombrePermisoFam"].HeaderText = lm.ObtenerTexto(f, "col.PermisoDeFamilia");
            }

        }

        private void ActualizarLabelPerfil()
        {
            var lm = Program.LanguageManager;
            string f = "FrmGestionPerfiles486LP";

            if (_idPerfilSeleccionado > 0 && !string.IsNullOrEmpty(_nombrePerfilSeleccionado))
            {
                lblPerfilSeleccionado.Text = string.Format(lm.ObtenerTexto(f, "lbl.Perfil"), _nombrePerfilSeleccionado);
            }
            else
            { 
                lblPerfilSeleccionado.Text = lm.ObtenerTexto(f, "lbl.PerfilNinguno"); 
            }
        }

        private void FrmGestionPerfiles486LP_FormClosing(object sender, FormClosingEventArgs e)
        {
            Program.LanguageManager.Quitar(this);
        }

    }
}

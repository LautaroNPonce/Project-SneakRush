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
    public partial class FrmGestionUsuarios486LP : Form, IObserver486LP
    {
        private BLL_Usuarios486LP _bll = new BLL_Usuarios486LP();
        private string _modo = "Ninguno";
        private int _idUsuarioSeleccionado = -1; 
        private string _dniUsuarioSeleccionado = string.Empty;
        private bool _puedeAgregar;
        private bool _puedeModificar;
        private bool _puedeEliminar;
        private bool _puedeDesbloquear;
        private bool _puedeAplicar;
        private bool _puedeCancelar;
        private bool _puedeSalir;

        public FrmGestionUsuarios486LP()
        {
            InitializeComponent();
            Program.LanguageManager.Agregar(this);
            this.FormClosing += FrmGestionUsuarios486LP_FormClosing;

        }

        private void FrmGestionUsuarios486LP_Load(object sender, EventArgs e)
        {
            CargarComboRol();
            rbtnActivos.Checked = true;
            CargarDgv();
            HabilitarCampos();
            lblModo.Text = string.Empty;
            SetModo("Consulta");
            ActualizarIdioma();
            AjustarBotonesSegunPerfil();
            AplicarPermisosBotones();
        }

        private void CargarComboRol()
        {
            cmbRol.Items.Clear();
            var perfiles = new BLL_Perfil486LP().ObtenerPerfiles();
            foreach (var perfil in perfiles)
            {
                cmbRol.Items.Add(perfil.Nombre);
            }
        }

        private void CargarDgv()
        {
            List<Usuario486LP> lista = rbtnActivos.Checked ? _bll.ListarActivos() : _bll.Listar();

            dgvUsuarios.DataSource = null;
            dgvUsuarios.DataSource = lista;

            ConfigurarColumnas();
            ColorearFilas();
        }

        private void MostrarColumna(string nombrePropiedad, string headerText)
        {
            if (dgvUsuarios.Columns.Contains(nombrePropiedad))
            {
                dgvUsuarios.Columns[nombrePropiedad].Visible = true;
                dgvUsuarios.Columns[nombrePropiedad].HeaderText = headerText;
            }
        }

        private void HabilitarCampos()
        {
            txtDNI.Enabled = true;
            txtNombre.Enabled = true;
            txtApellido.Enabled = true;
            txtNombreUsuario.Enabled = true;
            txtCorreo.Enabled = true;
            cmbRol.Enabled = true;
            rbtnActivoSi.Enabled = true;
            rbtnActivoNo.Enabled = true;
        }

        private void DeshabilitarCampos(bool limpiar = true)
        {
            txtDNI.Enabled = false;
            txtNombre.Enabled = false;
            txtApellido.Enabled = false;
            txtNombreUsuario.Enabled = false;
            txtCorreo.Enabled = false;
            cmbRol.Enabled = false;
            rbtnActivoSi.Enabled = false;
            rbtnActivoNo.Enabled = false;
            if (limpiar) LimpiarCampos();
        }

        private void LimpiarCampos()
        {
            txtDNI.Text = string.Empty;
            txtNombre.Text = string.Empty;
            txtApellido.Text = string.Empty;
            txtNombreUsuario.Text = string.Empty;
            txtCorreo.Text = string.Empty;
            cmbRol.SelectedIndex = -1;
            rbtnActivoSi.Checked = false;
            rbtnActivoNo.Checked = false;
        }

        private void CargarCamposDesdeUsuario(Usuario486LP u)
        {
            txtDNI.Text = u.DNI;
            txtNombre.Text = u.Nombre;
            txtApellido.Text = u.Apellido;
            txtNombreUsuario.Text = u.NombreUsuario;
            txtCorreo.Text = u.Email;
            if (cmbRol.Items.Contains(u.Rol)) 
            { 
                cmbRol.SelectedItem = u.Rol; 
            }
            else
            { 
                cmbRol.SelectedIndex = -1; 
            }
            rbtnActivoSi.Checked = u.Activo;
            rbtnActivoNo.Checked = !u.Activo;
        }

        private void SetModo(string modo)
        {
            _modo = modo;
            ActualizarLabelModo();
            if (modo == "Consulta")
            { 
                DeshabilitarCampos(limpiar: false); 
            }
        }

        private void ActualizarLabelModo()
        {
            var lm = Program.LanguageManager;
            string f = "FrmGestionUsuarios486LP";
            switch (_modo)
            {
                case "Consulta": lblModo.Text = lm.ObtenerTexto(f, "lblModoConsulta"); 
                    break;
                case "Añadir": lblModo.Text = lm.ObtenerTexto(f, "lblModoAñadir"); 
                    break;
                case "Modificar": lblModo.Text = lm.ObtenerTexto(f, "lblModoModificar"); 
                    break;
                case "Eliminar": lblModo.Text = lm.ObtenerTexto(f, "lblModoEliminar"); 
                    break;
                case "Desbloquear": lblModo.Text = lm.ObtenerTexto(f, "lblModoDesbloquear"); 
                    break;
                case "Bloquear": lblModo.Text = lm.ObtenerTexto(f, "lblModoBloquear"); 
                    break;
                default: lblModo.Text = string.Empty; 
                    break;
            }
        }

        private void dgvUsuarios_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvUsuarios.CurrentRow == null)
            { 
                return; 
            }

            Usuario486LP u = dgvUsuarios.CurrentRow.DataBoundItem as Usuario486LP;

            if (u == null)
            { 
                return; 
            }

            _idUsuarioSeleccionado = u.IdUsuario;
            _dniUsuarioSeleccionado = u.DNI;

            if (_modo == "Modificar" || _modo == "Eliminar" || _modo == "Desbloquear" || _modo == "Bloquear")
            {
                CargarCamposDesdeUsuario(u);
            }

            var lm = Program.LanguageManager;
            string f = "FrmGestionUsuarios486LP";
            btnDesbloquear.Text = u.Bloqueado ? lm.ObtenerTexto(f, "btnDesbloquear"): lm.ObtenerTexto(f, "btnBloquear");
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            SetModo("Añadir");
            LimpiarCampos();
            HabilitarCampos();
            txtNombreUsuario.Enabled = false;
            rbtnActivoSi.Checked = true;
            btnCancelar.Enabled = _puedeCancelar;

        }

        private void btnModificar_Click(object sender, EventArgs e)
        {
            var lm = Program.LanguageManager;
            string f = "FrmGestionUsuarios486LP";

            if (_idUsuarioSeleccionado == -1)
            {
                MessageBox.Show(lm.ObtenerTexto(f, "Msg.SeleccionarModificar", "Seleccione un usuario para modificar."),
                    lm.ObtenerTexto(f, "Msg.SeleccionarModificar.Title", "Aviso"),MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            SetModo("Modificar");

            Usuario486LP u = dgvUsuarios.CurrentRow?.DataBoundItem as Usuario486LP;
            if (u != null) CargarCamposDesdeUsuario(u);
            HabilitarCampos();
            txtDNI.Enabled = false;
            txtNombreUsuario.Enabled = false;
            btnCancelar.Enabled = _puedeCancelar;
        }

        private void BtnEliminar_Click(object sender, EventArgs e)
        {
            var lm = Program.LanguageManager;
            string f = "FrmGestionUsuarios486LP";

            if (_idUsuarioSeleccionado == -1)
            {
                MessageBox.Show(lm.ObtenerTexto(f, "Msg.SeleccionarEliminar", "Seleccione un usuario para eliminar."),
                    lm.ObtenerTexto(f, "Msg.SeleccionarEliminar.Title", "Aviso"),MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            SetModo("Eliminar");

            Usuario486LP u = dgvUsuarios.CurrentRow?.DataBoundItem as Usuario486LP;
            if (u != null)
            {
                DeshabilitarCampos();
                txtDNI.Text = u.DNI;
                txtNombre.Text = u.Nombre;
                txtApellido.Text = u.Apellido;
                txtNombreUsuario.Text = u.NombreUsuario;
                txtCorreo.Text = u.Email;
                cmbRol.SelectedItem = u.Rol;
                rbtnActivoSi.Checked = u.Activo;
                rbtnActivoNo.Checked = !u.Activo;
            }

            btnCancelar.Enabled = _puedeCancelar;
        }

        private void btnDesbloquear_Click(object sender, EventArgs e)
        {
            var lm = Program.LanguageManager;
            string f = "FrmGestionUsuarios486LP";

            if (_idUsuarioSeleccionado == -1)
            {
                MessageBox.Show(lm.ObtenerTexto(f, "Msg.SeleccionarDesbloquear", "Seleccione un usuario de la grilla."),
                    lm.ObtenerTexto(f, "Msg.SeleccionarDesbloquear.Title", "Aviso"),MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Usuario486LP u = dgvUsuarios.CurrentRow?.DataBoundItem as Usuario486LP;
            if (u == null) return;

            SetModo(u.Bloqueado ? "Desbloquear" : "Bloquear");
            DeshabilitarCampos();
            txtDNI.Text = u.DNI;
            txtNombre.Text = u.Nombre;
            txtApellido.Text = u.Apellido;
            txtNombreUsuario.Text = u.NombreUsuario;
            txtCorreo.Text = u.Email;
            cmbRol.SelectedItem = u.Rol;
            rbtnActivoSi.Checked = u.Activo;
            rbtnActivoNo.Checked = !u.Activo;
            btnAgregar.Enabled = false;
            btnModificar.Enabled = false;
            btnEliminar.Enabled = false;
            rbtnActivos.Enabled = false;
            rbtnTodos.Enabled = false;
            btnCancelar.Enabled = _puedeCancelar;
        }

        private void btnAplicar_Click(object sender, EventArgs e)
        {
            var lm = Program.LanguageManager;
            string f = "FrmGestionUsuarios486LP";
            string msg;

            if (_modo == "Añadir")
            {
                if (!ValidarCampos()) 
                { 
                    return; 
                }

                var perfiles = new BLL_Perfil486LP().ObtenerPerfiles();
                var perfilEncontrado = perfiles.FirstOrDefault(p => p.Nombre == cmbRol.SelectedItem?.ToString());

                Usuario486LP nuevo = new Usuario486LP
                {
                    DNI = txtDNI.Text.Trim(),
                    Nombre = txtNombre.Text.Trim(),
                    Apellido = txtApellido.Text.Trim(),
                    NombreUsuario = txtNombreUsuario.Text.Trim(),
                    Email = txtCorreo.Text.Trim(),
                    Rol = cmbRol.SelectedItem?.ToString() ?? string.Empty,
                    IdPerfil = perfilEncontrado?.IdPerfil
                };

                string contraseñaTemporal;
                bool resultado = _bll.Agregar(nuevo, out msg, out contraseñaTemporal);

                if (resultado)
                {
                    MessageBox.Show(lm.ObtenerTexto(f, "Msg.UsuarioCreado", "Usuario creado correctamente.\nLa contraseña temporal es: Apellido + DNI"),
                        lm.ObtenerTexto(f, "Msg.UsuarioCreado.Title", "Usuario creado"),MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Resetear();
                    CargarDgv();
                }
                else
                {
                    MessageBox.Show(msg, lm.ObtenerTexto(f, "Msg.ErrorAgregar.Title", "Error al agregar"),
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else if (_modo == "Modificar")
            {
                if (!ValidarCampos())
                {
                    return;
                }
                Usuario486LP usuarioActual = SessionManager486LP.ObtenerInstancia().UsuarioActual();
                if (usuarioActual != null && usuarioActual.IdUsuario == _idUsuarioSeleccionado && !rbtnActivoSi.Checked)
                {
                    MessageBox.Show(lm.ObtenerTexto(f, "Msg.NoDesactivarPropia", "No puede desactivar su propia cuenta mientras está en uso."),
                        lm.ObtenerTexto(f, "Msg.NoDesactivarPropia.Title", "Acción no permitida"),MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                var perfiles = new BLL_Perfil486LP().ObtenerPerfiles();
                var perfilEncontrado = perfiles.FirstOrDefault(p => p.Nombre == cmbRol.SelectedItem?.ToString());

                Usuario486LP mod = new Usuario486LP
                {
                    IdUsuario = _idUsuarioSeleccionado,
                    DNI = txtDNI.Text.Trim(),
                    NombreUsuario = txtNombreUsuario.Text.Trim(),
                    Nombre = txtNombre.Text.Trim(),
                    Apellido = txtApellido.Text.Trim(),
                    Email = txtCorreo.Text.Trim(),
                    Rol = cmbRol.SelectedItem?.ToString() ?? string.Empty,
                    Activo = rbtnActivoSi.Checked,
                    IdPerfil = perfilEncontrado?.IdPerfil
                };

                bool resultado = _bll.Modificar(mod, out msg);

                if (resultado)
                {
                    MessageBox.Show(lm.ObtenerTexto(f, "Msg.UsuarioModificado", "Usuario modificado correctamente."),
                        lm.ObtenerTexto(f, "Msg.UsuarioModificado.Title", "Éxito"),MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Resetear();
                    CargarDgv();
                }
                else
                {
                    MessageBox.Show(msg, lm.ObtenerTexto(f, "Msg.ErrorModificar.Title", "Error al modificar"),
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else if (_modo == "Eliminar")
            {
                Usuario486LP usuarioActual = SessionManager486LP.ObtenerInstancia().UsuarioActual();
                if (usuarioActual != null && usuarioActual.IdUsuario == _idUsuarioSeleccionado)
                {
                    MessageBox.Show(lm.ObtenerTexto(f, "Msg.NoEliminarPropia", "No puede eliminar su propia cuenta mientras está en uso."),
                        lm.ObtenerTexto(f, "Msg.NoEliminarPropia.Title", "Acción no permitida"),MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                DialogResult confirm = MessageBox.Show(string.Format(lm.ObtenerTexto(f, "Msg.ConfirmarEliminar", "¿Está seguro que desea eliminar al usuario '{0}'?"), txtNombreUsuario.Text),
                    lm.ObtenerTexto(f, "Msg.ConfirmarEliminar.Title", "Confirmar eliminación"),MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (confirm == DialogResult.Yes)
                {
                    bool resultado = _bll.Eliminar(_idUsuarioSeleccionado, out msg);

                    if (resultado)
                    {
                        MessageBox.Show(lm.ObtenerTexto(f, "Msg.UsuarioEliminado", "Usuario eliminado correctamente."),
                            lm.ObtenerTexto(f, "Msg.UsuarioEliminado.Title", "Éxito"),MessageBoxButtons.OK, MessageBoxIcon.Information);
                        Resetear();
                        CargarDgv();
                    }
                    else
                    {
                        MessageBox.Show(msg, lm.ObtenerTexto(f, "Msg.ErrorEliminar.Title", "Error al eliminar"),
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                if (_modo == "Consulta")
                {
                    FiltrarDgv();
                }
                else if (_modo == "Desbloquear" || _modo == "Bloquear")
                {
                    Usuario486LP u = dgvUsuarios.CurrentRow?.DataBoundItem as Usuario486LP;
                    if (u == null) 
                    { 
                        return; 
                    }

                    // Esto lo utilizo para el Anti-autobloqueo
                    Usuario486LP usuarioSesion = SessionManager486LP.ObtenerInstancia().UsuarioActual();
                    if (usuarioSesion != null && usuarioSesion.IdUsuario == u.IdUsuario && !u.Bloqueado)
                    {
                        MessageBox.Show(lm.ObtenerTexto(f, "Msg.NoBloquearPropia", "No puede bloquear su propia cuenta mientras está en uso."),
                            lm.ObtenerTexto(f, "Msg.NoBloquearPropia.Title", "Acción no permitida"),MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    string accion = u.Bloqueado ? lm.ObtenerTexto(f, "Msg.Accion.Desbloquear", "desbloquear") : lm.ObtenerTexto(f, "Msg.Accion.Bloquear", "bloquear");
                    DialogResult confirm = MessageBox.Show(string.Format(lm.ObtenerTexto(f, "Msg.ConfirmarAccion", "¿Desea {0} al usuario '{1}'?"), accion, u.NombreUsuario),
                        lm.ObtenerTexto(f, "Msg.ConfirmarAccion.Title", "Confirmar acción"),MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (confirm != DialogResult.Yes) 
                    { 
                        return; 
                    }

                    bool resultado = u.Bloqueado ? _bll.Desbloquear(_dniUsuarioSeleccionado, out msg) : _bll.BloquearPorDNI(_dniUsuarioSeleccionado, out msg);

                    if (resultado)
                    {
                        string textoExito = u.Bloqueado ? lm.ObtenerTexto(f, "Msg.UsuarioDesbloqueado", "Usuario desbloqueado correctamente.") : lm.ObtenerTexto(f, "Msg.UsuarioBloqueado", "Usuario bloqueado correctamente.");
                        string tituloExito = u.Bloqueado ? lm.ObtenerTexto(f, "Msg.UsuarioDesbloqueado.Title", "Éxito") : lm.ObtenerTexto(f, "Msg.UsuarioBloqueado.Title", "Éxito");
                        MessageBox.Show(textoExito, tituloExito, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        Resetear();
                        CargarDgv();
                    }
                    else
                    {
                        MessageBox.Show(msg, lm.ObtenerTexto(f, "Msg.Error.Title", "Error"),MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void rbtnActivos_CheckedChanged(object sender, EventArgs e)
        {
            if (rbtnActivos.Checked) 
            {
                CargarDgv();
            }
        }

        private void rbtnTodos_CheckedChanged(object sender, EventArgs e)
        {
            if (rbtnTodos.Checked) 
            {
                CargarDgv();
            }
        }

        private void Resetear()
        {
            SetModo("Consulta");
            LimpiarCampos();
            AplicarPermisosBotones();
            rbtnActivos.Enabled = true;
            rbtnTodos.Enabled = true;
            _idUsuarioSeleccionado = -1;
            _dniUsuarioSeleccionado = string.Empty;
        }

        private void AjustarBotonesSegunPerfil()
        {
            var usuario = SessionManager486LP.ObtenerInstancia().UsuarioActual();
            if (usuario == null) return;

            BLL_Perfil486LP bllPerfil = new BLL_Perfil486LP();
            List<string> permisos = bllPerfil.ObtenerPermisosPorRol(usuario.Rol);

            _puedeAgregar = permisos.Contains("USUARIOS_AGREGAR");
            _puedeModificar = permisos.Contains("USUARIOS_MODIFICAR");
            _puedeEliminar = permisos.Contains("USUARIOS_ELIMINAR");
            _puedeDesbloquear = permisos.Contains("USUARIOS_DESBLOQUEAR");
            _puedeAplicar = permisos.Contains("USUARIOS_APLICAR");
            _puedeCancelar = permisos.Contains("USUARIOS_CANCELAR");
            _puedeSalir = permisos.Contains("USUARIOS_SALIR");
        }

        private void AplicarPermisosBotones()
        {
            btnAgregar.Enabled = _puedeAgregar;
            btnModificar.Enabled = _puedeModificar;
            btnEliminar.Enabled = _puedeEliminar;
            btnDesbloquear.Enabled = _puedeDesbloquear;
            btnAplicar.Enabled = _puedeAplicar;
            btnCancelar.Enabled = false;
            btnSalir.Enabled = _puedeSalir;
        }

        private void ColorearFilas()
        {
            foreach (DataGridViewRow row in dgvUsuarios.Rows)
            {
                if (row.DataBoundItem is Usuario486LP u && !u.Activo)
                {
                    row.DefaultCellStyle.BackColor = Color.LightCoral;
                    row.DefaultCellStyle.ForeColor = Color.DarkRed;
                }
                else
                {
                    row.DefaultCellStyle.BackColor = Color.Empty;
                    row.DefaultCellStyle.ForeColor = Color.Empty;
                }
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            Resetear();
            CargarDgv();
        }

        private void FiltrarDgv()
        {
            List<Usuario486LP> lista = rbtnActivos.Checked ? _bll.ListarActivos() : _bll.Listar();

            string dni = txtDNI.Text.Trim();
            string nombre = txtNombre.Text.Trim();
            string apellido = txtApellido.Text.Trim();
            string email = txtCorreo.Text.Trim();
            string rol = cmbRol.SelectedItem?.ToString() ?? string.Empty;
            string login = txtNombreUsuario.Text.Trim();

            if (!string.IsNullOrEmpty(dni)) 
            {
                lista = lista.Where(u => u.DNI.Contains(dni)).ToList();
            }  
            if (!string.IsNullOrEmpty(nombre)) 
            {
                lista = lista.Where(u => u.Nombre.ToLower().Contains(nombre.ToLower())).ToList();
            }
            if (!string.IsNullOrEmpty(apellido)) 
            {
                lista = lista.Where(u => u.Apellido.ToLower().Contains(apellido.ToLower())).ToList();
            }
            if (!string.IsNullOrEmpty(email)) 
            {
                lista = lista.Where(u => u.Email.ToLower().Contains(email.ToLower())).ToList();
            }
            if (!string.IsNullOrEmpty(rol)) 
            {
                lista = lista.Where(u => u.Rol == rol).ToList();
            }
            if (!string.IsNullOrEmpty(login)) 
            {
                lista = lista.Where(u => u.NombreUsuario.ToLower().Contains(login.ToLower())).ToList();
            }

            dgvUsuarios.DataSource = null;
            dgvUsuarios.DataSource = lista;
            ConfigurarColumnas();
            ColorearFilas();
        }

        private void ConfigurarColumnas()
        {
            if (dgvUsuarios.Columns.Count == 0) 
            {
                return;
            }

            var lm = Program.LanguageManager;
            string f = "FrmGestionUsuarios486LP";

            foreach (DataGridViewColumn col in dgvUsuarios.Columns)
                col.Visible = false;

            MostrarColumna("NombreUsuario", lm.ObtenerTexto(f, "col.NombreUsuario"));
            MostrarColumna("Nombre", lm.ObtenerTexto(f, "col.Nombre"));
            MostrarColumna("Apellido", lm.ObtenerTexto(f, "col.Apellido"));
            MostrarColumna("DNI", lm.ObtenerTexto(f, "col.DNI"));
            MostrarColumna("Rol", lm.ObtenerTexto(f, "col.Rol"));
            MostrarColumna("Email", lm.ObtenerTexto(f, "col.Email"));
            MostrarColumna("Bloqueado", lm.ObtenerTexto(f, "col.Bloqueado"));
        }

        private bool ValidarCampos()
        {
            var lm = Program.LanguageManager;
            string f = "FrmGestionUsuarios486LP";

            if (string.IsNullOrWhiteSpace(txtDNI.Text))
            {
                MessageBox.Show(lm.ObtenerTexto(f, "Msg.Val.DNIObligatorio", "El DNI es obligatorio."),
                    lm.ObtenerTexto(f, "Msg.Val.Title", "Validación"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (!System.Text.RegularExpressions.Regex.IsMatch(txtDNI.Text.Trim(), @"^\d{7,8}$"))
            {
                MessageBox.Show(lm.ObtenerTexto(f, "Msg.Val.DNIFormato", "El DNI debe tener 7 u 8 dígitos numéricos."),
                    lm.ObtenerTexto(f, "Msg.Val.Title", "Validación"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            {
                MessageBox.Show(lm.ObtenerTexto(f, "Msg.Val.NombreObligatorio", "El Nombre es obligatorio."),
                    lm.ObtenerTexto(f, "Msg.Val.Title", "Validación"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtApellido.Text))
            {
                MessageBox.Show(lm.ObtenerTexto(f, "Msg.Val.ApellidoObligatorio", "El Apellido es obligatorio."),
                    lm.ObtenerTexto(f, "Msg.Val.Title", "Validación"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtNombreUsuario.Text))
            {
                MessageBox.Show(lm.ObtenerTexto(f, "Msg.Val.LoginObligatorio", "El Login es obligatorio."),
                    lm.ObtenerTexto(f, "Msg.Val.Title", "Validación"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtCorreo.Text) || !System.Text.RegularExpressions.Regex.IsMatch(txtCorreo.Text.Trim(), @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                MessageBox.Show(lm.ObtenerTexto(f, "Msg.Val.EmailFormato", "El Email no tiene un formato válido."),
                    lm.ObtenerTexto(f, "Msg.Val.Title", "Validación"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (cmbRol.SelectedIndex == -1)
            {
                MessageBox.Show(lm.ObtenerTexto(f, "Msg.Val.RolObligatorio", "Debe seleccionar un Rol."),
                    lm.ObtenerTexto(f, "Msg.Val.Title", "Validación"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        private void AutoGenerarLogin()
        {
            string nombre = txtNombre.Text.Trim();
            string apellido = txtApellido.Text.Trim();

            if (!string.IsNullOrEmpty(nombre) && !string.IsNullOrEmpty(apellido))
            {
                txtNombreUsuario.Text = nombre + "." + apellido;
            }
            else if (!string.IsNullOrEmpty(nombre))
            {
                txtNombreUsuario.Text = nombre;
            }
            else 
            {
                txtNombreUsuario.Text = string.Empty;
            }
        }

        private void txtNombre_TextChanged(object sender, EventArgs e)
        {
            if (_modo == "Añadir") 
            {
                AutoGenerarLogin();
            } 
        }

        private void txtApellido_TextChanged(object sender, EventArgs e)
        {
            if (_modo == "Añadir")
            {
                AutoGenerarLogin();
            }
        }

        public void ActualizarIdioma()
        {
            var lm = Program.LanguageManager;
            string f = "FrmGestionUsuarios486LP";

            this.Text = lm.ObtenerTexto(f, "Title");
            label1.Text = lm.ObtenerTexto(f, "Title"); 

            label2.Text = lm.ObtenerTexto(f, "lblDNI");
            label3.Text = lm.ObtenerTexto(f, "lblNombre");
            label4.Text = lm.ObtenerTexto(f, "lblApellido");
            label5.Text = lm.ObtenerTexto(f, "lblRol");
            label6.Text = lm.ObtenerTexto(f, "lblNombreUsuario");
            label7.Text = lm.ObtenerTexto(f, "lblCorreo");
            label8.Text = lm.ObtenerTexto(f, "lblActivo");

            btnAgregar.Text = lm.ObtenerTexto(f, "btnAgregar");
            btnEliminar.Text = lm.ObtenerTexto(f, "btnEliminar");
            btnModificar.Text = lm.ObtenerTexto(f, "btnModificar");
            btnDesbloquear.Text = lm.ObtenerTexto(f, "btnDesbloquear");
            btnAplicar.Text = lm.ObtenerTexto(f, "btnAplicar");
            btnCancelar.Text = lm.ObtenerTexto(f, "btnCancelar");
            btnSalir.Text = lm.ObtenerTexto(f, "btnSalir");

            rbtnActivos.Text = lm.ObtenerTexto(f, "rbtnActivos");
            rbtnTodos.Text = lm.ObtenerTexto(f, "rbtnTodos");
            rbtnActivoSi.Text = lm.ObtenerTexto(f, "rbtnActivoSi");
            rbtnActivoNo.Text = lm.ObtenerTexto(f, "rbtnActivoNo");

            ConfigurarColumnas();
            ActualizarLabelModo();
        }

        private void FrmGestionUsuarios486LP_FormClosing(object sender, FormClosingEventArgs e)
        {
            Program.LanguageManager.Quitar(this);
        }

    }
}

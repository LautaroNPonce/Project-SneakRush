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
    public partial class FrmGestionUsuarios486LP : Form
    {
        private BLL_Usuarios486LP _bll = new BLL_Usuarios486LP();
        private string _modo = "Ninguno";
        private int _idUsuarioSeleccionado = -1; // Variable para almacenar el ID del usuario seleccionado
        private string _dniUsuarioSeleccionado = string.Empty; // Variable para almacenar el DNI del usuario seleccionado

        public FrmGestionUsuarios486LP()
        {
            InitializeComponent();
        }

        private void FrmGestionUsuarios486LP_Load(object sender, EventArgs e)
        {
            CargarComboRol();
            rbtnActivos.Checked = true;
            CargarDgv();
            HabilitarCampos();
            lblModo.Text = string.Empty;
            SetModo("Consulta");
        }

        private void CargarComboRol()
        {
            cmbRol.Items.Clear();
            cmbRol.Items.Add("Administrador");
            cmbRol.Items.Add("Vendedor");
            cmbRol.Items.Add("Cajero");
            cmbRol.Items.Add("Encargado de Deposito");
            cmbRol.Items.Add("Analista");
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

        private void DeshabilitarCampos()
        {
            txtDNI.Enabled = false;
            txtNombre.Enabled = false;
            txtApellido.Enabled = false;
            txtNombreUsuario.Enabled = false;
            txtCorreo.Enabled = false;
            cmbRol.Enabled = false;
            rbtnActivoSi.Enabled = false;
            rbtnActivoNo.Enabled = false;
            LimpiarCampos();
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
            cmbRol.SelectedItem = u.Rol;
            rbtnActivoSi.Checked = u.Activo;
            rbtnActivoNo.Checked = !u.Activo;
        }

        private void SetModo(string modo)
        {
            _modo = modo;
            switch (modo)
            {
                case "Consulta": lblModo.Text = "Modo Consulta"; break;
                case "Añadir": lblModo.Text = "Modo Añadir"; break;
                case "Modificar": lblModo.Text = "Modo Modificar"; break;
                case "Eliminar": lblModo.Text = "Modo Eliminar"; break;
                case "Desbloquear": lblModo.Text = "Modo Desbloquear"; break;
                case "Bloquear": lblModo.Text = "Modo Bloquear"; break;
                default: lblModo.Text = string.Empty; break;
            }
        }

        private void dgvUsuarios_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvUsuarios.CurrentRow == null) return;

            Usuario486LP u = dgvUsuarios.CurrentRow.DataBoundItem as Usuario486LP;

            if (u == null) return;

            _idUsuarioSeleccionado = u.IdUsuario;
            _dniUsuarioSeleccionado = u.DNI;
            btnDesbloquear.Text = u.Bloqueado ? "Desbloquear" : "Bloquear";

            if (_modo == "Modificar" || _modo == "Eliminar" || _modo == "Desbloquear" || _modo == "Bloquear")
            {
                CargarCamposDesdeUsuario(u);
            }
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            SetModo("Añadir");
            LimpiarCampos();
            HabilitarCampos();
            txtNombreUsuario.Enabled = false;
            rbtnActivoSi.Checked = true;
            btnCancelar.Enabled = true;

        }

        private void btnModificar_Click(object sender, EventArgs e)
        {
            if (_idUsuarioSeleccionado == -1)
            {
                MessageBox.Show("Seleccione un usuario para modificar.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            SetModo("Modificar");

            // Cargar datos del usuario seleccionado en los campos
            Usuario486LP u = dgvUsuarios.CurrentRow?.DataBoundItem as Usuario486LP;
            if (u != null) CargarCamposDesdeUsuario(u);

            HabilitarCampos();
            txtDNI.Enabled = false; // DNI no se modifica
            txtNombreUsuario.Enabled = false; // NombreUsuario no se modifica
            btnCancelar.Enabled = true;
        }

        private void BtnEliminar_Click(object sender, EventArgs e)
        {
            if (_idUsuarioSeleccionado == -1)
            {
                MessageBox.Show("Seleccione un usuario para eliminar.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            SetModo("Eliminar");

            // Mostrar datos del usuario en los campos (solo lectura)
            Usuario486LP u = dgvUsuarios.CurrentRow?.DataBoundItem as Usuario486LP;
            if (u != null)
            {
                DeshabilitarCampos(); // limpia y deshabilita
                // Repoblar manualmente en modo solo lectura
                txtDNI.Text = u.DNI;
                txtNombre.Text = u.Nombre;
                txtApellido.Text = u.Apellido;
                txtNombreUsuario.Text = u.NombreUsuario;
                txtCorreo.Text = u.Email;
                cmbRol.SelectedItem = u.Rol;
                rbtnActivoSi.Checked = u.Activo;
                rbtnActivoNo.Checked = !u.Activo;
            }

            btnCancelar.Enabled = true;
        }

        private void btnDesbloquear_Click(object sender, EventArgs e)
        {
            if (_idUsuarioSeleccionado == -1)
            {
                MessageBox.Show("Seleccione un usuario de la grilla.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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

            // Deshabilitar botones de acción y marcadores
            btnAgregar.Enabled = false;
            btnModificar.Enabled = false;
            btnEliminar.Enabled = false;
            rbtnActivos.Enabled = false;
            rbtnTodos.Enabled = false;
            btnCancelar.Enabled = true;
        }

        private void btnAplicar_Click(object sender, EventArgs e)
        {
            string msg;

            if (_modo == "Añadir") 
            {
                if (!ValidarCampos()) return;
                Usuario486LP nuevo = new Usuario486LP
                {
                    DNI = txtDNI.Text.Trim(),
                    Nombre = txtNombre.Text.Trim(),
                    Apellido = txtApellido.Text.Trim(),
                    NombreUsuario = txtNombreUsuario.Text.Trim(),
                    Email = txtCorreo.Text.Trim(),
                    Rol = cmbRol.SelectedItem?.ToString() ?? string.Empty
                };

                string contraseñaTemporal;
                bool resultado = _bll.Agregar(nuevo, out msg, out contraseñaTemporal);

                if (resultado)
                {
                    FrmContraseñaUsuarioNuevo486LP frmTemp = new FrmContraseñaUsuarioNuevo486LP(contraseñaTemporal);
                    frmTemp.ShowDialog(this);

                    Resetear();
                    CargarDgv();
                }
                else
                {
                    MessageBox.Show(msg, "Error al agregar", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else if (_modo == "Modificar")
            {
                if (!ValidarCampos()) return;
                Usuario486LP usuarioActual = SessionManager486LP.ObtenerInstancia().UsuarioActual();
                if (usuarioActual != null && usuarioActual.IdUsuario == _idUsuarioSeleccionado && !rbtnActivoSi.Checked)
                {
                    MessageBox.Show("No puede desactivar su propia cuenta mientras está en uso.", "Acción no permitida", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                Usuario486LP mod = new Usuario486LP
                {
                    IdUsuario = _idUsuarioSeleccionado,
                    DNI = txtDNI.Text.Trim(),
                    NombreUsuario = txtNombreUsuario.Text.Trim(),
                    Nombre = txtNombre.Text.Trim(),
                    Apellido = txtApellido.Text.Trim(),
                    Email = txtCorreo.Text.Trim(),
                    Rol = cmbRol.SelectedItem?.ToString() ?? string.Empty,
                    Activo = rbtnActivoSi.Checked
                };

                bool resultado = _bll.Modificar(mod, out msg);

                if (resultado)
                {
                    MessageBox.Show("Usuario modificado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Resetear();
                    CargarDgv();
                }
                else
                {
                    MessageBox.Show(msg, "Error al modificar", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else if (_modo == "Eliminar")
            {
                Usuario486LP usuarioActual = SessionManager486LP.ObtenerInstancia().UsuarioActual();
                if (usuarioActual != null && usuarioActual.IdUsuario == _idUsuarioSeleccionado)
                {
                    MessageBox.Show(
                        "No puede eliminar su propia cuenta mientras está en uso.",
                        "Acción no permitida",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return;
                }

                DialogResult confirm = MessageBox.Show($"¿Está seguro que desea eliminar al usuario '{txtNombreUsuario.Text}'?", "Confirmar eliminación", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (confirm == DialogResult.Yes)
                {
                    bool resultado = _bll.Eliminar(_idUsuarioSeleccionado, out msg);

                    if (resultado)
                    {
                        MessageBox.Show("Usuario eliminado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        Resetear();
                        CargarDgv();
                    }
                    else
                    {
                        MessageBox.Show(msg, "Error al eliminar", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                    if (u == null) return;

                    // Anti-autobloqueo
                    Usuario486LP usuarioSesion = SessionManager486LP.ObtenerInstancia().UsuarioActual();
                    if (usuarioSesion != null && usuarioSesion.IdUsuario == u.IdUsuario && !u.Bloqueado)
                    {
                        MessageBox.Show("No puede bloquear su propia cuenta mientras está en uso.", "Acción no permitida", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    string accion = u.Bloqueado ? "desbloquear" : "bloquear";
                    DialogResult confirm = MessageBox.Show($"¿Desea {accion} al usuario '{u.NombreUsuario}'?", "Confirmar acción", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (confirm != DialogResult.Yes) return;

                    bool resultado = u.Bloqueado
                        ? _bll.Desbloquear(_dniUsuarioSeleccionado, out msg)
                        : _bll.BloquearPorDNI(_dniUsuarioSeleccionado, out msg);

                    if (resultado)
                    {
                        string textoExito = u.Bloqueado ? "Usuario desbloqueado correctamente." : "Usuario bloqueado correctamente.";
                        MessageBox.Show(textoExito, "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        Resetear();
                        CargarDgv();
                    }
                    else
                    {
                        MessageBox.Show(msg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            btnCancelar.Enabled = false;
            btnAgregar.Enabled = true;
            btnModificar.Enabled = true;
            btnEliminar.Enabled = true;
            btnDesbloquear.Enabled = true;
            rbtnActivos.Enabled = true;
            rbtnTodos.Enabled = true;
            _idUsuarioSeleccionado = -1;
            _dniUsuarioSeleccionado = string.Empty;
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
            if (dgvUsuarios.Columns.Count == 0) return;

            foreach (DataGridViewColumn col in dgvUsuarios.Columns)
                col.Visible = false;

            MostrarColumna("NombreUsuario", "NombreUsuario");
            MostrarColumna("Nombre", "Nombre");
            MostrarColumna("Apellido", "Apellido");
            MostrarColumna("DNI", "DNI");
            MostrarColumna("Rol", "Rol");
            MostrarColumna("Email", "Email");
            MostrarColumna("Bloqueado", "Bloqueado");
        }

        private bool ValidarCampos()
        {
            if (string.IsNullOrWhiteSpace(txtDNI.Text)) 
            { 
                { MessageBox.Show("El DNI es obligatorio.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning); return false; } 
            }

            if (!System.Text.RegularExpressions.Regex.IsMatch(txtDNI.Text.Trim(), @"^\d{7,8}$")) 
            {
                { MessageBox.Show("El DNI debe tener 7 u 8 dígitos numéricos.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning); return false; }
            }

            if (string.IsNullOrWhiteSpace(txtNombre.Text)) 
            { 
                { MessageBox.Show("El Nombre es obligatorio.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning); return false; } 
            }

            if (string.IsNullOrWhiteSpace(txtApellido.Text)) 
            { 
                { MessageBox.Show("El Apellido es obligatorio.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning); return false; } 
            }

            if (string.IsNullOrWhiteSpace(txtNombreUsuario.Text)) 
            { 
                { MessageBox.Show("El Login es obligatorio.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning); return false; } 
            }
         
            if (string.IsNullOrWhiteSpace(txtCorreo.Text) || !txtCorreo.Text.Contains("@") || !txtCorreo.Text.Contains(".")) 
            { 
                { MessageBox.Show("El Email no tiene un formato válido.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning); return false; } 
            }
            
            if (cmbRol.SelectedIndex == -1) 
            { 
                { MessageBox.Show("Debe seleccionar un Rol.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning); return false; } 
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
    }
}

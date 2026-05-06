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
            DeshabilitarCampos();
            lblModo.Text = string.Empty;
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

            // Ocultar todas y mostrar solo las que queremos
            if (dgvUsuarios.Columns.Count > 0)
            {
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
                case "Agregar": lblModo.Text = "Modo Agregar"; break;
                case "Modificar": lblModo.Text = "Modo Modificar"; break;
                case "Eliminar": lblModo.Text = "Modo Eliminar"; break;
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

            // Si ya estamos en Modificar o Eliminar, sincronizar campos con la nueva fila
            if (_modo == "Modificar" || _modo == "Eliminar")
            {
                CargarCamposDesdeUsuario(u);
            }
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            SetModo("Agregar");
            LimpiarCampos();
            HabilitarCampos();
            rbtnActivoSi.Checked = true; // nuevo usuario arranca activo
            btnSalir.Text = "Cancelar";
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
            btnSalir.Text = "Cancelar";
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

            btnSalir.Text = "Cancelar";
        }

        private void btnDesbloquear_Click(object sender, EventArgs e)
        {
            Usuario486LP u = dgvUsuarios.CurrentRow?.DataBoundItem as Usuario486LP;
            if (u == null)
            {
                MessageBox.Show("Seleccione un usuario.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            _dniUsuarioSeleccionado = u.DNI;

            string accion = u.Bloqueado ? "desbloquear" : "bloquear";
            DialogResult confirm = MessageBox.Show($"¿Está seguro que desea {accion} al usuario '{u.NombreUsuario}'?","Confirmar acción",MessageBoxButtons.YesNo,MessageBoxIcon.Question);

            if (confirm != DialogResult.Yes) return;

            string msg;

            if (u.Bloqueado)
            {
                bool resultado = _bll.Desbloquear(_dniUsuarioSeleccionado, out msg);
                if (resultado)
                {
                    MessageBox.Show("Usuario desbloqueado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else 
                {
                    MessageBox.Show(msg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                bool resultado = _bll.BloquearPorDNI(_dniUsuarioSeleccionado, out msg);
                if (resultado)
                {
                    MessageBox.Show("Usuario bloqueado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show(msg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            CargarDgv();
        }

        private void btnAplicar_Click(object sender, EventArgs e)
        {
            string msg;

            if (_modo == "Agregar")
            {
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
                Usuario486LP usuarioActual = SessionManager486LP.ObtenerInstancia().UsuarioActual();
                if (usuarioActual != null && usuarioActual.IdUsuario == _idUsuarioSeleccionado && !rbtnActivoSi.Checked)
                {
                    MessageBox.Show("No puede desactivar su propia cuenta mientras está en uso.","Acción no permitida",MessageBoxButtons.OK,MessageBoxIcon.Error);
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
                MessageBox.Show("Seleccione una operación primero (Agregar, Modificar o Eliminar).", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            if (btnSalir.Text == "Cancelar")
            {
                Resetear();
            }
            else
            {
                this.Close();
            }
        }

        private void rbtnActivos_CheckedChanged(object sender, EventArgs e)
        {
            if (rbtnActivos.Checked) CargarDgv();
        }

        private void rbtnTodos_CheckedChanged(object sender, EventArgs e)
        {
            if (rbtnTodos.Checked) CargarDgv();
        }

        private void Resetear()
        {
            SetModo("Ninguno");
            DeshabilitarCampos();
            btnSalir.Text = "Salir";
            _idUsuarioSeleccionado = -1;
            _dniUsuarioSeleccionado = string.Empty;
        }
    }
}

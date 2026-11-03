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
    public partial class FrmGestionClientes486LP : Form, IObserver486LP
    {
        private BLL_Cliente486LP _bllCliente = new BLL_Cliente486LP();
        private readonly string f = "FrmGestionClientes486LP";
        private bool _puedeAgregar;
        private bool _puedeModificar;
        private bool _puedeEliminar;
        private bool _puedeLimpiar;
        private bool _puedeCerrar;
        private readonly string _dniPrecargado;

        public FrmGestionClientes486LP()
        {
            InitializeComponent();
            Program.LanguageManager.Agregar(this);
            this.FormClosing += FrmGestionClientes486LP_FormClosing;
        }

        // Include el DNI del carrito llega precargado y no se puede modificar
        public FrmGestionClientes486LP(string dniPrecargado) : this()
        {
            _dniPrecargado = dniPrecargado;
        }

        private void FrmGestionClientes486LP_Load(object sender, EventArgs e)
        {
            if (!TienePatenteAcceso())
            {
                var lm = Program.LanguageManager;
                MessageBox.Show(
                    lm.ObtenerTexto(f, "Frm.GestionClientes.Msg.SinPermiso", "No tiene permiso para gestionar clientes."),
                    lm.ObtenerTexto(f, "Frm.GestionClientes.Msg.SinPermiso.Title", "Acceso denegado"),
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);

                this.BeginInvoke(new Action(() =>
                {
                    this.DialogResult = DialogResult.Cancel;
                    this.Close();
                }));
                return;
            }

            txtDNI.MaxLength = 8;
            txtNombre.MaxLength = 50;
            txtApellido.MaxLength = 50;
            txtCorreo.MaxLength = 100;
            txtTelefono.MaxLength = 20;
            txtDNI.KeyPress += FiltrosTeclado486LP.SoloDigitos;
            txtTelefono.KeyPress += FiltrosTeclado486LP.Telefono;

            ConfigurarColumnas();
            RefrescarGrilla();
            AjustarBotonesSegunPerfil();
            AplicarPermisosBotones();
            ActualizarIdioma();

            if (!string.IsNullOrEmpty(_dniPrecargado))
            {
                txtDNI.Text = _dniPrecargado;
                txtDNI.ReadOnly = true;
                txtNombre.Focus();
            }
        }
        private bool TienePatenteAcceso()
        {
            var usuario = SessionManager486LP.ObtenerInstancia().UsuarioActual();
            if (usuario == null) return false;

            List<string> permisos = new BLL_Perfil486LP().ObtenerPermisosPorRol(usuario.Rol);
            return permisos.Contains("MAESTRO_CLIENTES");
        }

        private void AjustarBotonesSegunPerfil()
        {
            var usuario = SessionManager486LP.ObtenerInstancia().UsuarioActual();
            if (usuario == null) return;

            BLL_Perfil486LP bllPerfil = new BLL_Perfil486LP();
            List<string> permisos = bllPerfil.ObtenerPermisosPorRol(usuario.Rol);

            _puedeAgregar = permisos.Contains("CLIENTES_AGREGAR");
            _puedeModificar = permisos.Contains("CLIENTES_MODIFICAR");
            _puedeEliminar = permisos.Contains("CLIENTES_ELIMINAR");
            _puedeLimpiar = permisos.Contains("CLIENTES_LIMPIAR");
            _puedeCerrar = permisos.Contains("CLIENTES_CERRAR");
        }

        private void AplicarPermisosBotones()
        {
            btnAgregar.Enabled = _puedeAgregar;
            btnModificar.Enabled = _puedeModificar;
            btnEliminar.Enabled = _puedeEliminar;
            btnLimpiar.Enabled = _puedeLimpiar;
            btnCerrar.Enabled = _puedeCerrar;
        }

        private void ConfigurarColumnas()
        {
            dgvClientes.AutoGenerateColumns = false;
            dgvClientes.Columns.Clear();

            dgvClientes.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "DNI", Name = "colDNI", Width = 110 });
            dgvClientes.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Nombre", Name = "colNombre", Width = 130 });
            dgvClientes.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Apellido", Name = "colApellido", Width = 130 });
            dgvClientes.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Telefono", Name = "colTelefono", Width = 120 });

            AplicarEncabezadosColumnas();
        }

        private void AplicarEncabezadosColumnas()
        {
            if (dgvClientes.Columns.Count == 0) return;

            var lm = Program.LanguageManager;
            dgvClientes.Columns["colDNI"].HeaderText = lm.ObtenerTexto(f, "Frm.GestionClientes.Col.DNI", "DNI");
            dgvClientes.Columns["colNombre"].HeaderText = lm.ObtenerTexto(f, "Frm.GestionClientes.Col.Nombre", "Nombre");
            dgvClientes.Columns["colApellido"].HeaderText = lm.ObtenerTexto(f, "Frm.GestionClientes.Col.Apellido", "Apellido");
            dgvClientes.Columns["colTelefono"].HeaderText = lm.ObtenerTexto(f, "Frm.GestionClientes.Col.Telefono", "Teléfono");
        }

        private void RefrescarGrilla()
        {
            dgvClientes.DataSource = null;
            dgvClientes.DataSource = _bllCliente.Listar();
        }

        private void dgvClientes_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (!string.IsNullOrEmpty(_dniPrecargado)) return; // con el DNI del carrito precargado no se cambia de cliente

            Cliente486LP cli = dgvClientes.Rows[e.RowIndex].DataBoundItem as Cliente486LP;
            if (cli == null) return;

            txtDNI.Text = cli.DNI;
            txtDNI.ReadOnly = true; // el DNI es la clave: con un cliente elegido no se puede cambiar (Limpiar lo habilita de nuevo)
            txtNombre.Text = cli.Nombre;
            txtApellido.Text = cli.Apellido;
            txtTelefono.Text = cli.Telefono;
            txtCorreo.Text = cli.Correo;
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            var lm = Program.LanguageManager;
            if (!ValidarCampos()) return;

            Cliente486LP cli = TomarDatosDelFormulario();
            string mensaje;

            if (_bllCliente.Agregar(cli, out mensaje))
            {
                MessageBox.Show(mensaje, lm.ObtenerTexto(f, "Frm.GestionClientes.Msg.Title", "Gestión de clientes"), MessageBoxButtons.OK, MessageBoxIcon.Information);

                if (!string.IsNullOrEmpty(_dniPrecargado))
                {
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                    return;
                }

                LimpiarCampos();
                RefrescarGrilla();
            }
            else
            {
                MessageBox.Show(mensaje, lm.ObtenerTexto(f, "Frm.GestionClientes.Msg.Title", "Gestión de clientes"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnModificar_Click(object sender, EventArgs e)
        {
            var lm = Program.LanguageManager;
            if (!ValidarCampos()) return;

            Cliente486LP cli = TomarDatosDelFormulario();
            string mensaje;

            if (_bllCliente.Modificar(cli, out mensaje))
            {
                MessageBox.Show(mensaje, lm.ObtenerTexto(f, "Frm.GestionClientes.Msg.Title", "Gestión de clientes"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                LimpiarCampos();
                RefrescarGrilla();
            }
            else
            {
                MessageBox.Show(mensaje, lm.ObtenerTexto(f, "Frm.GestionClientes.Msg.Title", "Gestión de clientes"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            var lm = Program.LanguageManager;

            if (string.IsNullOrWhiteSpace(txtDNI.Text))
            {
                MessageBox.Show(
                    lm.ObtenerTexto(f, "Frm.GestionClientes.Msg.SeleccionarParaEliminar", "Seleccione un cliente de la grilla para eliminar."),
                    lm.ObtenerTexto(f, "Frm.GestionClientes.Msg.Title", "Gestión de clientes"),
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult r = MessageBox.Show(
                string.Format(lm.ObtenerTexto(f, "Frm.GestionClientes.Msg.ConfirmarEliminacion", "¿Confirma la eliminación del cliente con DNI {0}?"), txtDNI.Text.Trim()),
                lm.ObtenerTexto(f, "Frm.GestionClientes.Msg.Title", "Gestión de clientes"),
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (r != DialogResult.Yes) return;

            string mensaje;
            if (_bllCliente.Eliminar(txtDNI.Text.Trim(), out mensaje))
            {
                MessageBox.Show(mensaje, lm.ObtenerTexto(f, "Frm.GestionClientes.Msg.Title", "Gestión de clientes"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                LimpiarCampos();
                RefrescarGrilla();
            }
            else
            {
                MessageBox.Show(mensaje, lm.ObtenerTexto(f, "Frm.GestionClientes.Msg.Title", "Gestión de clientes"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            LimpiarCampos();
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private bool ValidarCampos()
        {
            var lm = Program.LanguageManager;

            if (string.IsNullOrWhiteSpace(txtDNI.Text) ||
                string.IsNullOrWhiteSpace(txtNombre.Text) ||
                string.IsNullOrWhiteSpace(txtApellido.Text) ||
                string.IsNullOrWhiteSpace(txtCorreo.Text))
            {
                MessageBox.Show(
                    lm.ObtenerTexto(f, "Frm.GestionClientes.Msg.CamposObligatorios", "Complete los campos obligatorios (DNI, Nombre, Apellido y Correo)."),
                    lm.ObtenerTexto(f, "Frm.GestionClientes.Msg.Title", "Gestión de clientes"),
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            return true;
        }

        private Cliente486LP TomarDatosDelFormulario()
        {
            return new Cliente486LP(
                txtDNI.Text.Trim(),
                txtNombre.Text.Trim(),
                txtApellido.Text.Trim(),
                txtCorreo.Text.Trim(),
                txtTelefono.Text.Trim());
        }

        private void LimpiarCampos()
        {
            if (string.IsNullOrEmpty(_dniPrecargado))
            {
                txtDNI.ReadOnly = false;
                txtDNI.Clear(); // con el DNI del carrito precargado, Limpiar no lo borra
            }
            txtNombre.Clear();
            txtApellido.Clear();
            txtCorreo.Clear();
            txtTelefono.Clear();
            if (string.IsNullOrEmpty(_dniPrecargado))
                txtDNI.Focus();
            else
                txtNombre.Focus();
        }

        public void ActualizarIdioma()
        {
            var lm = Program.LanguageManager;

            this.Text = lm.ObtenerTexto(f, "Frm.GestionClientes.Titulo", "Gestión de clientes");
            lblTitulo.Text = lm.ObtenerTexto(f, "Frm.GestionClientes.Titulo", "Gestión de clientes");
            grpDatos.Text = lm.ObtenerTexto(f, "Frm.GestionClientes.Datos", "Datos del cliente");
            lblDNI.Text = lm.ObtenerTexto(f, "Frm.GestionClientes.DNI", "DNI:") + " *";
            lblNombre.Text = lm.ObtenerTexto(f, "Frm.GestionClientes.Nombre", "Nombre:") + " *";
            lblApellido.Text = lm.ObtenerTexto(f, "Frm.GestionClientes.Apellido", "Apellido:") + " *";
            lblCorreo.Text = lm.ObtenerTexto(f, "Frm.GestionClientes.Correo", "Correo:") + " *";
            lblTelefono.Text = lm.ObtenerTexto(f, "Frm.GestionClientes.Telefono", "Teléfono:");
            lblNota.Text = lm.ObtenerTexto(f, "Frm.GestionClientes.Nota", "* Campos obligatorios. El teléfono es opcional.");
            lblClientes.Text = lm.ObtenerTexto(f, "Frm.GestionClientes.Lista", "Clientes registrados:");
            btnAgregar.Text = lm.ObtenerTexto(f, "Frm.GestionClientes.Agregar", "Agregar");
            btnModificar.Text = lm.ObtenerTexto(f, "Frm.GestionClientes.Modificar", "Modificar");
            btnEliminar.Text = lm.ObtenerTexto(f, "Frm.GestionClientes.Eliminar", "Eliminar");
            btnLimpiar.Text = lm.ObtenerTexto(f, "Frm.GestionClientes.Limpiar", "Limpiar");
            btnCerrar.Text = lm.ObtenerTexto(f, "Frm.GestionClientes.Cerrar", "Cerrar");

            AplicarEncabezadosColumnas();
        }

        private void FrmGestionClientes486LP_FormClosing(object sender, FormClosingEventArgs e)
        {
            Program.LanguageManager.Quitar(this);
        }
    }
}

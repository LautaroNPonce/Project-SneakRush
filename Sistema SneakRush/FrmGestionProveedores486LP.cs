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
    public partial class FrmGestionProveedores486LP : Form, IObserver486LP
    {
        private BLL_Proveedor486LP _bllProveedor = new BLL_Proveedor486LP();
        private readonly string f = "FrmGestionProveedores486LP";

        // Patentes granulares por boton (ademas de la patente de acceso al form, MAESTRO_PROVEEDORES).
        private bool _puedeAgregar;
        private bool _puedeModificar;
        private bool _puedeEliminar;
        private bool _puedeLimpiar;
        private bool _puedeCerrar;

        // Id del proveedor elegido en la grilla (0 = ninguno)
        private int _idSeleccionado;

        public FrmGestionProveedores486LP()
        {
            InitializeComponent();
            Program.LanguageManager.Agregar(this);
            this.FormClosing += FrmGestionProveedores486LP_FormClosing;
        }

        private void FrmGestionProveedores486LP_Load(object sender, EventArgs e)
        {
            // Verificacion de acceso al form completo (mismo patron que los otros CUN de Maestro).
            if (!TienePatenteAcceso())
            {
                var lm = Program.LanguageManager;
                MessageBox.Show(
                    lm.ObtenerTexto(f, "Frm.GestionProveedores.Msg.SinPermiso", "No tiene permiso para procesar proveedores."),
                    lm.ObtenerTexto(f, "Frm.GestionProveedores.Msg.SinPermiso.Title", "Acceso denegado"),
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);

                this.BeginInvoke(new Action(() =>
                {
                    this.DialogResult = DialogResult.Cancel;
                    this.Close();
                }));
                return;
            }

            // Largos maximos acordes a las reglas de validacion de la BLL (nombre 100, CUIT 11 digitos con guiones, telefono 20, correo 100)
            txtNombre.MaxLength = 100;
            txtCUIT.MaxLength = 13;
            txtTelefono.MaxLength = 20;
            txtCorreo.MaxLength = 100;

            // El CUIT solo admite numeros y guion
            txtCUIT.KeyPress += CUIT_KeyPress;

            ConfigurarColumnas();
            RefrescarGrilla();
            AjustarBotonesSegunPerfil();
            AplicarPermisosBotones();
            ActualizarIdioma();
        }

        // Verifica que el usuario en sesion tenga la patente de acceso al form.
        private bool TienePatenteAcceso()
        {
            var usuario = SessionManager486LP.ObtenerInstancia().UsuarioActual();
            if (usuario == null) return false;

            List<string> permisos = new BLL_Perfil486LP().ObtenerPermisosPorRol(usuario.Rol);
            return permisos.Contains("MAESTRO_PROVEEDORES");
        }

        // Patentes granulares por boton.
        private void AjustarBotonesSegunPerfil()
        {
            var usuario = SessionManager486LP.ObtenerInstancia().UsuarioActual();
            if (usuario == null) return;

            BLL_Perfil486LP bllPerfil = new BLL_Perfil486LP();
            List<string> permisos = bllPerfil.ObtenerPermisosPorRol(usuario.Rol);

            _puedeAgregar = permisos.Contains("MAESTRO_PROVEEDORES_AGREGAR");
            _puedeModificar = permisos.Contains("MAESTRO_PROVEEDORES_MODIFICAR");
            _puedeEliminar = permisos.Contains("MAESTRO_PROVEEDORES_ELIMINAR");
            _puedeLimpiar = permisos.Contains("MAESTRO_PROVEEDORES_LIMPIAR");
            _puedeCerrar = permisos.Contains("MAESTRO_PROVEEDORES_CERRAR");
        }

        private void AplicarPermisosBotones()
        {
            btnAgregar.Enabled = _puedeAgregar;
            btnModificar.Enabled = _puedeModificar;
            btnEliminar.Enabled = _puedeEliminar;
            btnLimpiar.Enabled = _puedeLimpiar;
            btnCerrar.Enabled = _puedeCerrar;
        }

        // Define las columnas de la grilla ligadas a las propiedades del Proveedor486LP.
        private void ConfigurarColumnas()
        {
            dgvProveedores.AutoGenerateColumns = false;
            dgvProveedores.Columns.Clear();

            dgvProveedores.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Nombre", Name = "colNombre", Width = 170 });
            dgvProveedores.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "CUIT", Name = "colCUIT", Width = 110 });
            dgvProveedores.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Telefono", Name = "colTelefono", Width = 110 });
            dgvProveedores.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Correo", Name = "colCorreo", Width = 170 });

            // En la grilla el CUIT se ve con guiones (30-70123456-8), aunque se guarde solo con digitos
            dgvProveedores.CellFormatting += dgvProveedores_CellFormatting;

            AplicarEncabezadosColumnas();
        }

        // Traduce los encabezados de la grilla - separado de ConfigurarColumnas() para poder
        // llamarlo tambien desde ActualizarIdioma() cuando el idioma cambia en caliente.
        private void AplicarEncabezadosColumnas()
        {
            if (dgvProveedores.Columns.Count == 0) return;

            var lm = Program.LanguageManager;
            dgvProveedores.Columns["colNombre"].HeaderText = lm.ObtenerTexto(f, "Frm.GestionProveedores.Col.Nombre", "Nombre");
            dgvProveedores.Columns["colCUIT"].HeaderText = lm.ObtenerTexto(f, "Frm.GestionProveedores.Col.CUIT", "CUIT");
            dgvProveedores.Columns["colTelefono"].HeaderText = lm.ObtenerTexto(f, "Frm.GestionProveedores.Col.Telefono", "Teléfono");
            dgvProveedores.Columns["colCorreo"].HeaderText = lm.ObtenerTexto(f, "Frm.GestionProveedores.Col.Correo", "Correo");
        }

        private void dgvProveedores_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || e.Value == null) return;
            if (dgvProveedores.Columns[e.ColumnIndex].Name != "colCUIT") return;

            e.Value = FormatearCUIT(e.Value.ToString());
            e.FormattingApplied = true;
        }

        // 30701234568 -> 30-70123456-8
        private string FormatearCUIT(string cuit)
        {
            if (cuit != null && cuit.Length == 11 && cuit.All(char.IsDigit))
                return cuit.Substring(0, 2) + "-" + cuit.Substring(2, 8) + "-" + cuit.Substring(10, 1);
            return cuit;
        }

        // Trae la lista de proveedores y la bindea a la grilla.
        private void RefrescarGrilla()
        {
            dgvProveedores.DataSource = null;
            dgvProveedores.DataSource = _bllProveedor.Listar();
            dgvProveedores.ClearSelection();
        }

        // Al hacer clic en una fila, carga los datos del proveedor en los cuadros de texto para poder Modificar o Eliminar.
        private void dgvProveedores_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            Proveedor486LP p = dgvProveedores.Rows[e.RowIndex].DataBoundItem as Proveedor486LP;
            if (p == null) return;

            _idSeleccionado = p.IdProveedor;
            txtNombre.Text = p.Nombre;
            txtCUIT.Text = FormatearCUIT(p.CUIT);
            txtTelefono.Text = p.Telefono;
            txtCorreo.Text = p.Correo;
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            var lm = Program.LanguageManager;

            // Con un proveedor elegido en la grilla hay que limpiar antes de cargar uno nuevo
            if (_idSeleccionado != 0)
            {
                MostrarAviso(lm.ObtenerTexto(f, "Frm.GestionProveedores.Msg.LimpiarAntesDeAgregar", "Hay un proveedor elegido en la grilla. Presione Limpiar antes de agregar uno nuevo."));
                return;
            }

            if (!ValidarCampos()) return;

            Proveedor486LP proveedor = TomarDatosDelFormulario();
            string mensaje;

            if (_bllProveedor.Agregar(proveedor, out mensaje))
            {
                MessageBox.Show(mensaje, lm.ObtenerTexto(f, "Frm.GestionProveedores.Msg.Title", "Procesar proveedores"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                LimpiarCampos();
                RefrescarGrilla();
            }
            else
            {
                MostrarAviso(mensaje);
            }
        }

        private void btnModificar_Click(object sender, EventArgs e)
        {
            var lm = Program.LanguageManager;

            if (_idSeleccionado == 0)
            {
                MostrarAviso(lm.ObtenerTexto(f, "Frm.GestionProveedores.Msg.SeleccionarParaModificar", "Seleccione un proveedor de la grilla para modificar."));
                return;
            }

            if (!ValidarCampos()) return;

            Proveedor486LP proveedor = TomarDatosDelFormulario();
            proveedor.IdProveedor = _idSeleccionado;
            string mensaje;

            if (_bllProveedor.Modificar(proveedor, out mensaje))
            {
                MessageBox.Show(mensaje, lm.ObtenerTexto(f, "Frm.GestionProveedores.Msg.Title", "Procesar proveedores"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                LimpiarCampos();
                RefrescarGrilla();
            }
            else
            {
                MostrarAviso(mensaje);
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            var lm = Program.LanguageManager;

            if (_idSeleccionado == 0)
            {
                MostrarAviso(lm.ObtenerTexto(f, "Frm.GestionProveedores.Msg.SeleccionarParaEliminar", "Seleccione un proveedor de la grilla para eliminar."));
                return;
            }

            DialogResult r = MessageBox.Show(
                string.Format(lm.ObtenerTexto(f, "Frm.GestionProveedores.Msg.ConfirmarEliminacion", "¿Confirma la eliminación del proveedor {0}?"), txtNombre.Text.Trim()),
                lm.ObtenerTexto(f, "Frm.GestionProveedores.Msg.Title", "Procesar proveedores"),
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (r != DialogResult.Yes) return;

            string mensaje;
            if (_bllProveedor.Eliminar(_idSeleccionado, out mensaje))
            {
                MessageBox.Show(mensaje, lm.ObtenerTexto(f, "Frm.GestionProveedores.Msg.Title", "Procesar proveedores"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                LimpiarCampos();
                RefrescarGrilla();
            }
            else
            {
                MostrarAviso(mensaje);
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

        // Valida solo lo que corresponde a la GUI (campos obligatorios no vacios).
        // Las validaciones de negocio (formato de nombre, CUIT, correo y telefono, proveedor repetido, etc.) las hace la BLL y devuelve el mensaje ya traducido.
        private bool ValidarCampos()
        {
            var lm = Program.LanguageManager;

            if (string.IsNullOrWhiteSpace(txtNombre.Text) ||
                string.IsNullOrWhiteSpace(txtCUIT.Text) ||
                string.IsNullOrWhiteSpace(txtCorreo.Text))
            {
                MostrarAviso(lm.ObtenerTexto(f, "Frm.GestionProveedores.Msg.CamposObligatorios", "Complete los campos obligatorios (Nombre, CUIT y Correo)."));
                return false;
            }
            return true;
        }

        private Proveedor486LP TomarDatosDelFormulario()
        {
            return new Proveedor486LP(
                0,
                txtNombre.Text.Trim(),
                txtCUIT.Text.Trim(),
                txtTelefono.Text.Trim(),
                txtCorreo.Text.Trim());
        }

        private void LimpiarCampos()
        {
            _idSeleccionado = 0;
            txtNombre.Clear();
            txtCUIT.Clear();
            txtTelefono.Clear();
            txtCorreo.Clear();
            dgvProveedores.ClearSelection();
            txtNombre.Focus();
        }

        private void MostrarAviso(string texto)
        {
            var lm = Program.LanguageManager;
            MessageBox.Show(texto, lm.ObtenerTexto(f, "Frm.GestionProveedores.Msg.Title", "Procesar proveedores"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        // Solo digitos y guion
        private void CUIT_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '-')
                e.Handled = true;
        }

        public void ActualizarIdioma()
        {
            var lm = Program.LanguageManager;

            this.Text = lm.ObtenerTexto(f, "Frm.GestionProveedores.Titulo", "Procesar proveedores");
            lblTitulo.Text = lm.ObtenerTexto(f, "Frm.GestionProveedores.Titulo", "Procesar proveedores");
            grpDatos.Text = lm.ObtenerTexto(f, "Frm.GestionProveedores.Datos", "Datos del proveedor");
            lblNombre.Text = lm.ObtenerTexto(f, "Frm.GestionProveedores.Nombre", "Nombre:");
            lblCUIT.Text = lm.ObtenerTexto(f, "Frm.GestionProveedores.CUIT", "CUIT:");
            lblTelefono.Text = lm.ObtenerTexto(f, "Frm.GestionProveedores.Telefono", "Teléfono:");
            lblCorreo.Text = lm.ObtenerTexto(f, "Frm.GestionProveedores.Correo", "Correo:");
            lblProveedores.Text = lm.ObtenerTexto(f, "Frm.GestionProveedores.Lista", "Proveedores registrados:");
            btnAgregar.Text = lm.ObtenerTexto(f, "Frm.GestionProveedores.Agregar", "Agregar");
            btnModificar.Text = lm.ObtenerTexto(f, "Frm.GestionProveedores.Modificar", "Modificar");
            btnEliminar.Text = lm.ObtenerTexto(f, "Frm.GestionProveedores.Eliminar", "Eliminar");
            btnLimpiar.Text = lm.ObtenerTexto(f, "Frm.GestionProveedores.Limpiar", "Limpiar");
            btnCerrar.Text = lm.ObtenerTexto(f, "Frm.GestionProveedores.Cerrar", "Cerrar");

            AplicarEncabezadosColumnas();
        }

        private void FrmGestionProveedores486LP_FormClosing(object sender, FormClosingEventArgs e)
        {
            Program.LanguageManager.Quitar(this);
        }
    }
}

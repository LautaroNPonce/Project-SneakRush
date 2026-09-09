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

        public FrmGestionClientes486LP()
        {
            InitializeComponent();
            Program.LanguageManager.Agregar(this);
            this.FormClosing += FrmGestionClientes486LP_FormClosing;
        }

        private void FrmGestionClientes486LP_Load(object sender, EventArgs e)
        {
            ConfigurarColumnas();
            RefrescarGrilla();
            ActualizarIdioma();
        }

        // Define las columnas de la grilla ligadas a las propiedades del Cliente486LP.
        // El Correo NO se muestra (esta cifrado en AES); se maneja solo en los cuadros de texto.
        private void ConfigurarColumnas()
        {
            dgvClientes.AutoGenerateColumns = false;
            dgvClientes.Columns.Clear();

            dgvClientes.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "DNI", HeaderText = "DNI", Width = 110 });
            dgvClientes.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Nombre", HeaderText = "Nombre", Width = 130 });
            dgvClientes.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Apellido", HeaderText = "Apellido", Width = 130 });
            dgvClientes.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Telefono", HeaderText = "Teléfono", Width = 120 });
        }

        // Trae la lista de clientes y la bindea a la grilla.
        private void RefrescarGrilla()
        {
            dgvClientes.DataSource = null;
            dgvClientes.DataSource = _bllCliente.Listar();
        }

        // Al hacer clic en una fila, carga los datos del cliente en los cuadros de texto para poder Modificar o Eliminar. El Correo se muestra desencriptado.
        private void dgvClientes_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            Cliente486LP cli = dgvClientes.Rows[e.RowIndex].DataBoundItem as Cliente486LP;
            if (cli == null) return;

            txtDNI.Text = cli.DNI;
            txtNombre.Text = cli.Nombre;
            txtApellido.Text = cli.Apellido;
            txtTelefono.Text = cli.Telefono;
            txtCorreo.Text = cli.Correo;
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            if (!ValidarCampos()) return;

            Cliente486LP cli = TomarDatosDelFormulario();
            string mensaje;

            if (_bllCliente.Agregar(cli, out mensaje))
            {
                MessageBox.Show(mensaje, "Gestión de clientes", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LimpiarCampos();
                RefrescarGrilla();
            }
            else
            {
                MessageBox.Show(mensaje, "Gestión de clientes", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnModificar_Click(object sender, EventArgs e)
        {
            if (!ValidarCampos()) return;

            Cliente486LP cli = TomarDatosDelFormulario();
            string mensaje;

            if (_bllCliente.Modificar(cli, out mensaje))
            {
                MessageBox.Show(mensaje, "Gestión de clientes", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LimpiarCampos();
                RefrescarGrilla();
            }
            else
            {
                MessageBox.Show(mensaje, "Gestión de clientes", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtDNI.Text))
            {
                MessageBox.Show("Seleccione un cliente de la grilla para eliminar.", "Gestión de clientes", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult r = MessageBox.Show(
                "¿Confirma la eliminación del cliente con DNI " + txtDNI.Text.Trim() + "?",
                "Gestión de clientes", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (r != DialogResult.Yes) return;

            string mensaje;
            if (_bllCliente.Eliminar(txtDNI.Text.Trim(), out mensaje))
            {
                MessageBox.Show(mensaje, "Gestión de clientes", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LimpiarCampos();
                RefrescarGrilla();
            }
            else
            {
                MessageBox.Show(mensaje, "Gestión de clientes", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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

        // Valida solo lo que corresponde a la GUI (campos no vacios y DNI numerico).
        // Las validaciones de negocio (DNI duplicado, etc.) las hace la BLL.
        private bool ValidarCampos()
        {
            if (string.IsNullOrWhiteSpace(txtDNI.Text) ||
                string.IsNullOrWhiteSpace(txtNombre.Text) ||
                string.IsNullOrWhiteSpace(txtApellido.Text) ||
                string.IsNullOrWhiteSpace(txtCorreo.Text))
            {
                MessageBox.Show("Complete los campos obligatorios (DNI, Nombre, Apellido y Correo).",
                    "Gestión de clientes", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
            txtDNI.Clear();
            txtNombre.Clear();
            txtApellido.Clear();
            txtCorreo.Clear();
            txtTelefono.Clear();
            txtDNI.Focus();
        }

        public void ActualizarIdioma()
        {
            var lm = Program.LanguageManager;
            string f = "FrmGestionClientes486LP";

            this.Text = lm.ObtenerTexto(f, "Frm.GestionClientes.Titulo", "Gestión de clientes");
            lblTitulo.Text = lm.ObtenerTexto(f, "Frm.GestionClientes.Titulo", "Gestión de clientes");
            grpDatos.Text = lm.ObtenerTexto(f, "Frm.GestionClientes.Datos", "Datos del cliente");
            lblDNI.Text = lm.ObtenerTexto(f, "Frm.GestionClientes.DNI", "DNI:");
            lblNombre.Text = lm.ObtenerTexto(f, "Frm.GestionClientes.Nombre", "Nombre:");
            lblApellido.Text = lm.ObtenerTexto(f, "Frm.GestionClientes.Apellido", "Apellido:");
            lblCorreo.Text = lm.ObtenerTexto(f, "Frm.GestionClientes.Correo", "Correo:");
            lblTelefono.Text = lm.ObtenerTexto(f, "Frm.GestionClientes.Telefono", "Teléfono:");
            lblClientes.Text = lm.ObtenerTexto(f, "Frm.GestionClientes.Lista", "Clientes registrados:");
            btnAgregar.Text = lm.ObtenerTexto(f, "Frm.GestionClientes.Agregar", "Agregar");
            btnModificar.Text = lm.ObtenerTexto(f, "Frm.GestionClientes.Modificar", "Modificar");
            btnEliminar.Text = lm.ObtenerTexto(f, "Frm.GestionClientes.Eliminar", "Eliminar");
            btnLimpiar.Text = lm.ObtenerTexto(f, "Frm.GestionClientes.Limpiar", "Limpiar");
            btnCerrar.Text = lm.ObtenerTexto(f, "Frm.GestionClientes.Cerrar", "Cerrar");
        }

        private void FrmGestionClientes486LP_FormClosing(object sender, FormClosingEventArgs e)
        {
            Program.LanguageManager.Quitar(this);
        }
    }
}

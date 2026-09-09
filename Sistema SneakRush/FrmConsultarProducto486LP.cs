using BE;
using BLL;
using iTextSharp.text.pdf;
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
    public partial class FrmConsultarProducto486LP : Form, IObserver486LP
    {
        private BLL_Producto486LP _bllProducto = new BLL_Producto486LP();
        private readonly string f = "FrmConsultarProducto486LP";

        // Producto elegido por el usuario. El carrito lo lee tras cerrar con DialogResult.OK.
        public Producto486LP ProductoSeleccionado { get; private set; }

        public FrmConsultarProducto486LP()
        {
            InitializeComponent();
            Program.LanguageManager.Agregar(this);
            this.FormClosing += FrmConsultarProducto486LP_FormClosing;
        }

        private void FrmConsultarProducto486LP_Load(object sender, EventArgs e)
        {
            // Flujo alternativo 2.1: usuario sin patente VENTA_CONSULTAR_PRODUCTOS
            if (!TienePatenteConsulta())
            {
                var lm = Program.LanguageManager;
                MessageBox.Show(
                    lm.ObtenerTexto(f, "Msg.SinPermiso", "No tiene permiso para consultar productos."),
                    lm.ObtenerTexto(f, "Msg.SinPermiso.Title", "Acceso denegado"),
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);

                // Se cierra despues del Load para evitar problemas al cerrar durante la carga.
                this.BeginInvoke(new Action(() =>
                {
                    this.DialogResult = DialogResult.Cancel;
                    this.Close();
                }));
                return;
            }

            CargarGrilla();
            ActualizarIdioma();
        }

        // Verifica que el usuario en sesion tenga la patente para consultar productos.
        private bool TienePatenteConsulta()
        {
            var usuario = SessionManager486LP.ObtenerInstancia().UsuarioActual();
            if (usuario == null) return false;

            List<string> permisos = new BLL_Perfil486LP().ObtenerPermisosPorRol(usuario.Rol);
            return permisos.Contains("VENTA_CONSULTAR_PRODUCTOS");
        }

        // Carga la grilla con el catalogo completo (paso 3 del escenario principal)
        private void CargarGrilla()
        {
            MostrarEnGrilla(_bllProducto.Listar());
        }

        // Vuelca la lista en la grilla y aplica formato de columnas.
        private void MostrarEnGrilla(List<Producto486LP> lista)
        {
            dgvProductos.DataSource = null;
            dgvProductos.DataSource = lista;
            AplicarFormatoColumnas();
        }

        private void AplicarFormatoColumnas()
        {
            if (dgvProductos.Columns.Count == 0) return;

            var lm = Program.LanguageManager;

            if (dgvProductos.Columns.Contains("IdProducto"))
                dgvProductos.Columns["IdProducto"].Visible = false;
            if (dgvProductos.Columns.Contains("Marca"))
                dgvProductos.Columns["Marca"].HeaderText = lm.ObtenerTexto(f, "col.Marca", "Marca");
            if (dgvProductos.Columns.Contains("Modelo"))
                dgvProductos.Columns["Modelo"].HeaderText = lm.ObtenerTexto(f, "col.Modelo", "Modelo");
            if (dgvProductos.Columns.Contains("Color"))
                dgvProductos.Columns["Color"].HeaderText = lm.ObtenerTexto(f, "col.Color", "Color");
            if (dgvProductos.Columns.Contains("Talle"))
                dgvProductos.Columns["Talle"].HeaderText = lm.ObtenerTexto(f, "col.Talle", "Talle");
            if (dgvProductos.Columns.Contains("Precio"))
            {
                dgvProductos.Columns["Precio"].HeaderText = lm.ObtenerTexto(f, "col.Precio", "Precio");
                dgvProductos.Columns["Precio"].DefaultCellStyle.Format = "C2";
            }
            if (dgvProductos.Columns.Contains("Stock"))
                dgvProductos.Columns["Stock"].HeaderText = lm.ObtenerTexto(f, "col.Stock", "Stock");

            dgvProductos.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        // Boton "Buscar": aplica los filtros ingresados (pasos 4 a 7 + alternativos 6.1 y 7.1)
        private void Buscar()
        {
            List<Producto486LP> lista = _bllProducto.Buscar(
                txtMarca.Text, txtModelo.Text, txtColor.Text, txtTalle.Text, chkSoloStock.Checked);

            MostrarEnGrilla(lista);

            // Flujo alternativo 7.1: sin resultados
            if (lista.Count == 0)
            {
                var lm = Program.LanguageManager;
                MessageBox.Show(
                    lm.ObtenerTexto(f, "Msg.SinResultados", "No se encontraron productos con esos criterios."),
                    lm.ObtenerTexto(f, "Msg.SinResultados.Title", "Sin resultados"),
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            Buscar();
        }

        // Boton "Limpiar": borra los filtros y vuelve a mostrar el catalogo completo
        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            txtMarca.Clear();
            txtModelo.Clear();
            txtColor.Clear();
            txtTalle.Clear();
            chkSoloStock.Checked = false;
            CargarGrilla();
        }

        // Boton "Seleccionar": toma el producto de la fila actual y cierra el form.
        private void btnSeleccionar_Click(object sender, EventArgs e)
        {
            if (dgvProductos.CurrentRow == null)
            {
                var lm = Program.LanguageManager;
                MessageBox.Show(
                    lm.ObtenerTexto(f, "Msg.SeleccionarProducto", "Seleccione un producto de la grilla."),
                    lm.ObtenerTexto(f, "Msg.SeleccionarProducto.Title", "Aviso"),
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            ProductoSeleccionado = dgvProductos.CurrentRow.DataBoundItem as Producto486LP;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        // Boton "Cancelar": cierra sin elegir producto.
        private void btnCancelar_Click(object sender, EventArgs e)
        {
            ProductoSeleccionado = null;
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        // Observer de idioma: actualiza todas las leyendas en caliente.
        public void ActualizarIdioma()
        {
            var lm = Program.LanguageManager;

            this.Text = lm.ObtenerTexto(f, "Titulo", "Consultar productos");
            lblTitulo.Text = lm.ObtenerTexto(f, "lblTitulo", "Consultar productos");
            lblMarca.Text = lm.ObtenerTexto(f, "lblMarca", "Marca:");
            lblModelo.Text = lm.ObtenerTexto(f, "lblModelo", "Modelo:");
            lblColor.Text = lm.ObtenerTexto(f, "lblColor", "Color:");
            lblTalle.Text = lm.ObtenerTexto(f, "lblTalle", "Talle:");
            chkSoloStock.Text = lm.ObtenerTexto(f, "chkSoloStock", "Solo con stock");
            btnBuscar.Text = lm.ObtenerTexto(f, "btnBuscar", "Buscar");
            btnLimpiar.Text = lm.ObtenerTexto(f, "btnLimpiar", "Limpiar");
            btnSeleccionar.Text = lm.ObtenerTexto(f, "btnSeleccionar", "Seleccionar");
            btnCancelar.Text = lm.ObtenerTexto(f, "btnCancelar", "Cancelar");

            AplicarFormatoColumnas();
        }

        private void FrmConsultarProducto486LP_FormClosing(object sender, FormClosingEventArgs e)
        {
            Program.LanguageManager.Quitar(this);
        }
    }
}
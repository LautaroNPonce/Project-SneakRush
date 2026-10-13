using BE;
using BLL;
using Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sistema_SneakRush
{
    public partial class FrmGestionProductos486LP : Form, IObserver486LP
    {
        private BLL_Producto486LP _bllProducto = new BLL_Producto486LP();
        private BLL_Marca486LP _bllMarca = new BLL_Marca486LP();
        private BLL_Categoria486LP _bllCategoria = new BLL_Categoria486LP();
        private readonly string f = "FrmGestionProductos486LP";

        // Patentes granulares por boton (ademas de la patente de acceso al form, MAESTRO_PRODUCTOS).
        private bool _puedeAgregar;
        private bool _puedeModificar;
        private bool _puedeEliminar;
        private bool _puedeLimpiar;
        private bool _puedeCerrar;
        private bool _puedeImportar;

        // Id del producto elegido en la grilla (0 = ninguno). Con uno elegido el stock no se edita: solo cambia con ventas y compras.
        private int _idSeleccionado;

        public FrmGestionProductos486LP()
        {
            InitializeComponent();
            Program.LanguageManager.Agregar(this);
            this.FormClosing += FrmGestionProductos486LP_FormClosing;
        }

        private void FrmGestionProductos486LP_Load(object sender, EventArgs e)
        {
            // Verificacion de acceso al form completo (mismo patron que los otros CUN de Maestro).
            if (!TienePatenteAcceso())
            {
                var lm = Program.LanguageManager;
                MessageBox.Show(
                    lm.ObtenerTexto(f, "Frm.GestionProductos.Msg.SinPermiso", "No tiene permiso para procesar productos."),
                    lm.ObtenerTexto(f, "Frm.GestionProductos.Msg.SinPermiso.Title", "Acceso denegado"),
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);

                this.BeginInvoke(new Action(() =>
                {
                    this.DialogResult = DialogResult.Cancel;
                    this.Close();
                }));
                return;
            }

            // Largos maximos acordes a las reglas de validacion de la BLL (modelo 50, color 30, talle 4, precio y stock numericos)
            txtModelo.MaxLength = 50;
            txtColor.MaxLength = 30;
            txtTalle.MaxLength = 4;
            txtPrecio.MaxLength = 12;
            txtStock.MaxLength = 9;

            // Teclas permitidas: talle y precio admiten numeros y separador decimal; stock solo numeros
            txtTalle.KeyPress += SoloNumeroConDecimal_KeyPress;
            txtPrecio.KeyPress += SoloNumeroConDecimal_KeyPress;
            txtStock.KeyPress += SoloNumero_KeyPress;

            CargarCombos();
            ConfigurarColumnas();
            RefrescarGrilla();
            AjustarBotonesSegunPerfil();
            AplicarPermisosBotones();
            LimpiarCampos();
            ActualizarIdioma();
        }

        // Verifica que el usuario en sesion tenga la patente de acceso al form.
        private bool TienePatenteAcceso()
        {
            var usuario = SessionManager486LP.ObtenerInstancia().UsuarioActual();
            if (usuario == null) return false;

            List<string> permisos = new BLL_Perfil486LP().ObtenerPermisosPorRol(usuario.Rol);
            return permisos.Contains("MAESTRO_PRODUCTOS");
        }

        // Patentes granulares por boton.
        private void AjustarBotonesSegunPerfil()
        {
            var usuario = SessionManager486LP.ObtenerInstancia().UsuarioActual();
            if (usuario == null) return;

            BLL_Perfil486LP bllPerfil = new BLL_Perfil486LP();
            List<string> permisos = bllPerfil.ObtenerPermisosPorRol(usuario.Rol);

            _puedeAgregar = permisos.Contains("MAESTRO_PRODUCTOS_AGREGAR");
            _puedeModificar = permisos.Contains("MAESTRO_PRODUCTOS_MODIFICAR");
            _puedeEliminar = permisos.Contains("MAESTRO_PRODUCTOS_ELIMINAR");
            _puedeLimpiar = permisos.Contains("MAESTRO_PRODUCTOS_LIMPIAR");
            _puedeCerrar = permisos.Contains("MAESTRO_PRODUCTOS_CERRAR");
            _puedeImportar = permisos.Contains("MAESTRO_PRODUCTOS_IMPORTAR");
        }

        private void AplicarPermisosBotones()
        {
            btnAgregar.Enabled = _puedeAgregar;
            btnModificar.Enabled = _puedeModificar;
            btnEliminar.Enabled = _puedeEliminar;
            btnLimpiar.Enabled = _puedeLimpiar;
            btnCerrar.Enabled = _puedeCerrar;
            btnImportar.Enabled = _puedeImportar;
        }

        // Carga los combos de marca y categoria con lo que hay en las tablas Marca y Categoria
        private void CargarCombos()
        {
            cmbMarca.DataSource = null;
            cmbMarca.DisplayMember = "Nombre";
            cmbMarca.ValueMember = "IdMarca";
            cmbMarca.DataSource = _bllMarca.Listar();
            cmbMarca.SelectedIndex = -1;

            cmbCategoria.DataSource = null;
            cmbCategoria.DisplayMember = "Nombre";
            cmbCategoria.ValueMember = "IdCategoria";
            cmbCategoria.DataSource = _bllCategoria.Listar();
            cmbCategoria.SelectedIndex = -1;
        }

        // Define las columnas de la grilla ligadas a las propiedades del Producto486LP.
        private void ConfigurarColumnas()
        {
            dgvProductos.AutoGenerateColumns = false;
            dgvProductos.Columns.Clear();

            dgvProductos.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Marca", Name = "colMarca", Width = 90 });
            dgvProductos.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Categoria", Name = "colCategoria", Width = 90 });
            dgvProductos.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Modelo", Name = "colModelo", Width = 120 });
            dgvProductos.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Color", Name = "colColor", Width = 70 });
            dgvProductos.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Talle", Name = "colTalle", Width = 50 });
            dgvProductos.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Precio", Name = "colPrecio", Width = 80, DefaultCellStyle = new DataGridViewCellStyle { Format = "C2" } });
            dgvProductos.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Stock", Name = "colStock", Width = 50 });

            AplicarEncabezadosColumnas();
        }

        // Traduce los encabezados de la grilla - separado de ConfigurarColumnas() para poder
        // llamarlo tambien desde ActualizarIdioma() cuando el idioma cambia en caliente.
        private void AplicarEncabezadosColumnas()
        {
            if (dgvProductos.Columns.Count == 0) return;

            var lm = Program.LanguageManager;
            dgvProductos.Columns["colMarca"].HeaderText = lm.ObtenerTexto(f, "Frm.GestionProductos.Col.Marca", "Marca");
            dgvProductos.Columns["colCategoria"].HeaderText = lm.ObtenerTexto(f, "Frm.GestionProductos.Col.Categoria", "Categoría");
            dgvProductos.Columns["colModelo"].HeaderText = lm.ObtenerTexto(f, "Frm.GestionProductos.Col.Modelo", "Modelo");
            dgvProductos.Columns["colColor"].HeaderText = lm.ObtenerTexto(f, "Frm.GestionProductos.Col.Color", "Color");
            dgvProductos.Columns["colTalle"].HeaderText = lm.ObtenerTexto(f, "Frm.GestionProductos.Col.Talle", "Talle");
            dgvProductos.Columns["colPrecio"].HeaderText = lm.ObtenerTexto(f, "Frm.GestionProductos.Col.Precio", "Precio");
            dgvProductos.Columns["colStock"].HeaderText = lm.ObtenerTexto(f, "Frm.GestionProductos.Col.Stock", "Stock");
        }

        // Trae la lista de productos y la bindea a la grilla.
        private void RefrescarGrilla()
        {
            dgvProductos.DataSource = null;
            dgvProductos.DataSource = _bllProducto.Listar();
            dgvProductos.ClearSelection();
        }

        // Al hacer clic en una fila, carga los datos del producto en los controles para poder Modificar o Eliminar.
        // El stock se muestra pero no se puede editar.
        private void dgvProductos_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            Producto486LP p = dgvProductos.Rows[e.RowIndex].DataBoundItem as Producto486LP;
            if (p == null) return;

            _idSeleccionado = p.IdProducto;
            cmbMarca.SelectedValue = p.IdMarca;
            cmbCategoria.SelectedValue = p.IdCategoria;
            txtModelo.Text = p.Modelo;
            txtColor.Text = p.Color;
            txtTalle.Text = p.Talle;
            txtPrecio.Text = p.Precio.ToString("0.00");
            txtStock.Text = p.Stock.ToString();
            txtStock.ReadOnly = true;
            ActualizarEtiquetaStock();
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            var lm = Program.LanguageManager;

            // Con un producto elegido en la grilla hay que limpiar antes de cargar uno nuevo
            if (_idSeleccionado != 0)
            {
                MostrarAviso(lm.ObtenerTexto(f, "Frm.GestionProductos.Msg.LimpiarAntesDeAgregar", "Hay un producto elegido en la grilla. Presione Limpiar antes de agregar uno nuevo."));
                return;
            }

            Producto486LP producto;
            if (!TomarDatosDelFormulario(true, out producto)) return;

            string mensaje;
            if (_bllProducto.Agregar(producto, out mensaje))
            {
                MessageBox.Show(mensaje, lm.ObtenerTexto(f, "Frm.GestionProductos.Msg.Title", "Procesar productos"), MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                MostrarAviso(lm.ObtenerTexto(f, "Frm.GestionProductos.Msg.SeleccionarParaModificar", "Seleccione un producto de la grilla para modificar."));
                return;
            }

            Producto486LP producto;
            if (!TomarDatosDelFormulario(false, out producto)) return;
            producto.IdProducto = _idSeleccionado;

            string mensaje;
            if (_bllProducto.Modificar(producto, out mensaje))
            {
                MessageBox.Show(mensaje, lm.ObtenerTexto(f, "Frm.GestionProductos.Msg.Title", "Procesar productos"), MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                MostrarAviso(lm.ObtenerTexto(f, "Frm.GestionProductos.Msg.SeleccionarParaEliminar", "Seleccione un producto de la grilla para eliminar."));
                return;
            }

            DialogResult r = MessageBox.Show(
                string.Format(lm.ObtenerTexto(f, "Frm.GestionProductos.Msg.ConfirmarEliminacion", "¿Confirma la eliminación del producto {0}?"), DescribirSeleccionado()),
                lm.ObtenerTexto(f, "Frm.GestionProductos.Msg.Title", "Procesar productos"),
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (r != DialogResult.Yes) return;

            string mensaje;
            if (_bllProducto.Eliminar(_idSeleccionado, out mensaje))
            {
                MessageBox.Show(mensaje, lm.ObtenerTexto(f, "Frm.GestionProductos.Msg.Title", "Procesar productos"), MessageBoxButtons.OK, MessageBoxIcon.Information);
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

        // Abre el dialogo de importacion masiva desde el XML del proveedor. Al volver, la grilla se refresca porque el stock y los productos pudieron cambiar
        private void btnImportar_Click(object sender, EventArgs e)
        {
            using (FrmImportarProductos486LP dialogo = new FrmImportarProductos486LP())
            {
                dialogo.ShowDialog(this);
            }

            RefrescarGrilla();
            LimpiarCampos();
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // Valida solo lo que corresponde a la GUI (campos obligatorios no vacios y numeros bien escritos).
        // Las reglas de negocio (formato de modelo, color y talle, rangos, producto repetido, etc.) las hace la BLL y devuelve el mensaje ya traducido.
        private bool TomarDatosDelFormulario(bool esAlta, out Producto486LP producto)
        {
            var lm = Program.LanguageManager;
            producto = null;

            if (cmbMarca.SelectedIndex < 0 ||
                cmbCategoria.SelectedIndex < 0 ||
                string.IsNullOrWhiteSpace(txtModelo.Text) ||
                string.IsNullOrWhiteSpace(txtColor.Text) ||
                string.IsNullOrWhiteSpace(txtTalle.Text) ||
                string.IsNullOrWhiteSpace(txtPrecio.Text))
            {
                MostrarAviso(lm.ObtenerTexto(f, "Frm.GestionProductos.Msg.CamposObligatorios", "Complete los campos obligatorios (marca, categoría, modelo, color, talle y precio)."));
                return false;
            }

            decimal precio;
            if (!decimal.TryParse(txtPrecio.Text.Trim().Replace(',', '.'), NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out precio))
            {
                MostrarAviso(Mensajes486LP.Obtener("Producto.PrecioInvalido"));
                return false;
            }

            // El stock solo se carga al dar de alta; vacio cuenta como cero
            int stock = 0;
            if (esAlta && !string.IsNullOrWhiteSpace(txtStock.Text) && !int.TryParse(txtStock.Text.Trim(), out stock))
            {
                MostrarAviso(Mensajes486LP.Obtener("Producto.StockInvalido"));
                return false;
            }

            producto = new Producto486LP
            {
                IdMarca = (int)cmbMarca.SelectedValue,
                IdCategoria = (int)cmbCategoria.SelectedValue,
                Modelo = txtModelo.Text.Trim(),
                Color = txtColor.Text.Trim(),
                Talle = txtTalle.Text.Trim(),
                Precio = precio,
                Stock = stock
            };
            return true;
        }

        private void LimpiarCampos()
        {
            _idSeleccionado = 0;
            cmbMarca.SelectedIndex = -1;
            cmbCategoria.SelectedIndex = -1;
            txtModelo.Clear();
            txtColor.Clear();
            txtTalle.Clear();
            txtPrecio.Clear();
            txtStock.ReadOnly = false;
            txtStock.Text = "0";
            dgvProductos.ClearSelection();
            ActualizarEtiquetaStock();
            cmbMarca.Focus();
        }

        // "Stock inicial" al dar de alta, "Stock actual" cuando hay un producto elegido
        private void ActualizarEtiquetaStock()
        {
            var lm = Program.LanguageManager;
            lblStock.Text = _idSeleccionado == 0
                ? lm.ObtenerTexto(f, "Frm.GestionProductos.StockInicial", "Stock inicial:")
                : lm.ObtenerTexto(f, "Frm.GestionProductos.StockActual", "Stock actual:");
        }

        private string DescribirSeleccionado()
        {
            return string.Format("{0} {1} - {2} - {3}", cmbMarca.Text, txtModelo.Text.Trim(), txtColor.Text.Trim(), txtTalle.Text.Trim());
        }

        private void MostrarAviso(string texto)
        {
            var lm = Program.LanguageManager;
            MessageBox.Show(texto, lm.ObtenerTexto(f, "Frm.GestionProductos.Msg.Title", "Procesar productos"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        // Solo digitos
        private void SoloNumero_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                e.Handled = true;
        }

        // Digitos y un separador decimal (coma o punto)
        private void SoloNumeroConDecimal_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != ',' && e.KeyChar != '.')
                e.Handled = true;
        }

        public void ActualizarIdioma()
        {
            var lm = Program.LanguageManager;

            this.Text = lm.ObtenerTexto(f, "Frm.GestionProductos.Titulo", "Procesar productos");
            lblTitulo.Text = lm.ObtenerTexto(f, "Frm.GestionProductos.Titulo", "Procesar productos");
            grpDatos.Text = lm.ObtenerTexto(f, "Frm.GestionProductos.Datos", "Datos del producto");
            lblMarca.Text = lm.ObtenerTexto(f, "Frm.GestionProductos.Marca", "Marca:");
            lblCategoria.Text = lm.ObtenerTexto(f, "Frm.GestionProductos.Categoria", "Categoría:");
            lblModelo.Text = lm.ObtenerTexto(f, "Frm.GestionProductos.Modelo", "Modelo:");
            lblColor.Text = lm.ObtenerTexto(f, "Frm.GestionProductos.Color", "Color:");
            lblTalle.Text = lm.ObtenerTexto(f, "Frm.GestionProductos.Talle", "Talle:");
            lblPrecio.Text = lm.ObtenerTexto(f, "Frm.GestionProductos.Precio", "Precio:");
            lblProductos.Text = lm.ObtenerTexto(f, "Frm.GestionProductos.Lista", "Productos registrados:");
            btnAgregar.Text = lm.ObtenerTexto(f, "Frm.GestionProductos.Agregar", "Agregar");
            btnModificar.Text = lm.ObtenerTexto(f, "Frm.GestionProductos.Modificar", "Modificar");
            btnEliminar.Text = lm.ObtenerTexto(f, "Frm.GestionProductos.Eliminar", "Eliminar");
            btnLimpiar.Text = lm.ObtenerTexto(f, "Frm.GestionProductos.Limpiar", "Limpiar");
            btnImportar.Text = lm.ObtenerTexto(f, "Frm.GestionProductos.Importar", "Importar XML");
            btnCerrar.Text = lm.ObtenerTexto(f, "Frm.GestionProductos.Cerrar", "Cerrar");

            ActualizarEtiquetaStock();
            AplicarEncabezadosColumnas();
        }

        private void FrmGestionProductos486LP_FormClosing(object sender, FormClosingEventArgs e)
        {
            Program.LanguageManager.Quitar(this);
        }
    }
}

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
    public partial class FrmGestionCategoriasMarcas486LP : Form, IObserver486LP
    {
        private BLL_Categoria486LP _bllCategoria = new BLL_Categoria486LP();
        private BLL_Marca486LP _bllMarca = new BLL_Marca486LP();
        private readonly string f = "FrmGestionCategoriasMarcas486LP";

        // Patentes granulares por boton (ademas de la patente de acceso al form, MAESTRO_CATEGORIAS).
        private bool _puedeAgregarCategoria;
        private bool _puedeModificarCategoria;
        private bool _puedeEliminarCategoria;
        private bool _puedeLimpiarCategoria;
        private bool _puedeAgregarMarca;
        private bool _puedeModificarMarca;
        private bool _puedeEliminarMarca;
        private bool _puedeLimpiarMarca;
        private bool _puedeCerrar;

        public FrmGestionCategoriasMarcas486LP()
        {
            InitializeComponent();
            Program.LanguageManager.Agregar(this);
            this.FormClosing += FrmGestionCategoriasMarcas486LP_FormClosing;
        }

        private void FrmGestionCategoriasMarcas486LP_Load(object sender, EventArgs e)
        {
            // Verificacion de acceso al form completo (mismo patron que los otros CUN de Maestro).
            if (!TienePatenteAcceso())
            {
                var lm = Program.LanguageManager;
                MessageBox.Show(
                    lm.ObtenerTexto(f, "Frm.GestionCategoriasMarcas.Msg.SinPermiso", "No tiene permiso para procesar categorías y marcas."),
                    lm.ObtenerTexto(f, "Frm.GestionCategoriasMarcas.Msg.SinPermiso.Title", "Acceso denegado"),
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);

                this.BeginInvoke(new Action(() =>
                {
                    this.DialogResult = DialogResult.Cancel;
                    this.Close();
                }));
                return;
            }

            // Largo maximo acorde a la regla de validacion de la BLL (nombre 50)
            txtCategoria.MaxLength = 50;
            txtMarca.MaxLength = 50;

            ConfigurarCategoriaColumnas();
            ConfigurarMarcaColumnas();
            RefrescarCategorias();
            RefrescarMarcas();
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
            return permisos.Contains("MAESTRO_CATEGORIAS");
        }

        // Patentes granulares por boton.
        private void AjustarBotonesSegunPerfil()
        {
            var usuario = SessionManager486LP.ObtenerInstancia().UsuarioActual();
            if (usuario == null) return;

            BLL_Perfil486LP bllPerfil = new BLL_Perfil486LP();
            List<string> permisos = bllPerfil.ObtenerPermisosPorRol(usuario.Rol);

            _puedeAgregarCategoria = permisos.Contains("MAESTRO_CATEGORIAS_AGREGAR");
            _puedeModificarCategoria = permisos.Contains("MAESTRO_CATEGORIAS_MODIFICAR");
            _puedeEliminarCategoria = permisos.Contains("MAESTRO_CATEGORIAS_ELIMINAR");
            _puedeLimpiarCategoria = permisos.Contains("MAESTRO_CATEGORIAS_LIMPIAR");
            _puedeAgregarMarca = permisos.Contains("MAESTRO_MARCAS_AGREGAR");
            _puedeModificarMarca = permisos.Contains("MAESTRO_MARCAS_MODIFICAR");
            _puedeEliminarMarca = permisos.Contains("MAESTRO_MARCAS_ELIMINAR");
            _puedeLimpiarMarca = permisos.Contains("MAESTRO_MARCAS_LIMPIAR");
            _puedeCerrar = permisos.Contains("MAESTRO_CATEGORIAS_CERRAR");
        }

        private void AplicarPermisosBotones()
        {
            btnAgregarCategoria.Enabled = _puedeAgregarCategoria;
            btnModificarCategoria.Enabled = _puedeModificarCategoria;
            btnEliminarCategoria.Enabled = _puedeEliminarCategoria;
            btnLimpiarCategoria.Enabled = _puedeLimpiarCategoria;
            btnAgregarMarca.Enabled = _puedeAgregarMarca;
            btnModificarMarca.Enabled = _puedeModificarMarca;
            btnEliminarMarca.Enabled = _puedeEliminarMarca;
            btnLimpiarMarca.Enabled = _puedeLimpiarMarca;
            btnCerrar.Enabled = _puedeCerrar;
        }

        // ===================== CATEGORIAS =====================

        // Id de la categoria elegida en la grilla (0 = ninguna)
        private int _idCategoriaSeleccionada;

        private void ConfigurarCategoriaColumnas()
        {
            dgvCategorias.AutoGenerateColumns = false;
            dgvCategorias.Columns.Clear();
            dgvCategorias.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Nombre", Name = "colCategoriaNombre", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
            AplicarCategoriaEncabezado();
        }

        // Traduce el encabezado de la grilla - separado para poder llamarlo tambien desde ActualizarIdioma()
        private void AplicarCategoriaEncabezado()
        {
            if (dgvCategorias.Columns.Count == 0) return;

            var lm = Program.LanguageManager;
            dgvCategorias.Columns["colCategoriaNombre"].HeaderText = lm.ObtenerTexto(f, "Frm.GestionCategoriasMarcas.Col.Nombre", "Nombre");
        }

        private void RefrescarCategorias()
        {
            dgvCategorias.DataSource = null;
            dgvCategorias.DataSource = _bllCategoria.Listar();
            dgvCategorias.ClearSelection();
        }

        // Al hacer clic en una fila, carga el nombre en el cuadro de texto para poder Modificar o Eliminar.
        private void dgvCategorias_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            Categoria486LP categoria = dgvCategorias.Rows[e.RowIndex].DataBoundItem as Categoria486LP;
            if (categoria == null) return;

            _idCategoriaSeleccionada = categoria.IdCategoria;
            txtCategoria.Text = categoria.Nombre;
        }

        private void btnAgregarCategoria_Click(object sender, EventArgs e)
        {
            var lm = Program.LanguageManager;

            // Con una categoria elegida en la grilla hay que limpiar antes de cargar una nueva
            if (_idCategoriaSeleccionada != 0)
            {
                MostrarAviso(lm.ObtenerTexto(f, "Frm.GestionCategoriasMarcas.Msg.LimpiarAntesDeAgregarCategoria", "Hay una categoría elegida en la grilla. Presione Limpiar antes de agregar una nueva."));
                return;
            }

            if (!ValidarCategoria()) return;

            string mensaje;
            if (_bllCategoria.Agregar(new Categoria486LP { Nombre = txtCategoria.Text.Trim() }, out mensaje))
            {
                MessageBox.Show(mensaje, lm.ObtenerTexto(f, "Frm.GestionCategoriasMarcas.Msg.Title", "Procesar categorías y marcas"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                LimpiarCategoria();
                RefrescarCategorias();
            }
            else
            {
                MostrarAviso(mensaje);
            }
        }

        private void btnModificarCategoria_Click(object sender, EventArgs e)
        {
            var lm = Program.LanguageManager;

            if (_idCategoriaSeleccionada == 0)
            {
                MostrarAviso(lm.ObtenerTexto(f, "Frm.GestionCategoriasMarcas.Msg.SeleccionarParaModificarCategoria", "Seleccione una categoría de la grilla para modificar."));
                return;
            }

            if (!ValidarCategoria()) return;

            string mensaje;
            if (_bllCategoria.Modificar(new Categoria486LP { IdCategoria = _idCategoriaSeleccionada, Nombre = txtCategoria.Text.Trim() }, out mensaje))
            {
                MessageBox.Show(mensaje, lm.ObtenerTexto(f, "Frm.GestionCategoriasMarcas.Msg.Title", "Procesar categorías y marcas"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                LimpiarCategoria();
                RefrescarCategorias();
            }
            else
            {
                MostrarAviso(mensaje);
            }
        }

        private void btnEliminarCategoria_Click(object sender, EventArgs e)
        {
            var lm = Program.LanguageManager;

            if (_idCategoriaSeleccionada == 0)
            {
                MostrarAviso(lm.ObtenerTexto(f, "Frm.GestionCategoriasMarcas.Msg.SeleccionarParaEliminarCategoria", "Seleccione una categoría de la grilla para eliminar."));
                return;
            }

            DialogResult r = MessageBox.Show(
                string.Format(lm.ObtenerTexto(f, "Frm.GestionCategoriasMarcas.Msg.ConfirmarEliminacionCategoria", "¿Confirma la eliminación de la categoría {0}?"), txtCategoria.Text.Trim()),
                lm.ObtenerTexto(f, "Frm.GestionCategoriasMarcas.Msg.Title", "Procesar categorías y marcas"),
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (r != DialogResult.Yes) return;

            string mensaje;
            if (_bllCategoria.Eliminar(_idCategoriaSeleccionada, out mensaje))
            {
                MessageBox.Show(mensaje, lm.ObtenerTexto(f, "Frm.GestionCategoriasMarcas.Msg.Title", "Procesar categorías y marcas"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                LimpiarCategoria();
                RefrescarCategorias();
            }
            else
            {
                MostrarAviso(mensaje);
            }
        }

        private void btnLimpiarCategoria_Click(object sender, EventArgs e)
        {
            LimpiarCategoria();
        }

        // Valida solo lo que corresponde a la GUI (nombre no vacio). Las reglas de negocio (formato, repetida) las hace la BLL
        private bool ValidarCategoria()
        {
            var lm = Program.LanguageManager;

            if (string.IsNullOrWhiteSpace(txtCategoria.Text))
            {
                MostrarAviso(lm.ObtenerTexto(f, "Frm.GestionCategoriasMarcas.Msg.NombreObligatorioCategoria", "Ingrese el nombre de la categoría."));
                return false;
            }
            return true;
        }

        private void LimpiarCategoria()
        {
            _idCategoriaSeleccionada = 0;
            txtCategoria.Clear();
            dgvCategorias.ClearSelection();
            txtCategoria.Focus();
        }

        // ===================== MARCAS =====================

        // Id de la marca elegida en la grilla (0 = ninguna)
        private int _idMarcaSeleccionada;

        private void ConfigurarMarcaColumnas()
        {
            dgvMarcas.AutoGenerateColumns = false;
            dgvMarcas.Columns.Clear();
            dgvMarcas.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Nombre", Name = "colMarcaNombre", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
            AplicarMarcaEncabezado();
        }

        // Traduce el encabezado de la grilla - separado para poder llamarlo tambien desde ActualizarIdioma()
        private void AplicarMarcaEncabezado()
        {
            if (dgvMarcas.Columns.Count == 0) return;

            var lm = Program.LanguageManager;
            dgvMarcas.Columns["colMarcaNombre"].HeaderText = lm.ObtenerTexto(f, "Frm.GestionCategoriasMarcas.Col.Nombre", "Nombre");
        }

        private void RefrescarMarcas()
        {
            dgvMarcas.DataSource = null;
            dgvMarcas.DataSource = _bllMarca.Listar();
            dgvMarcas.ClearSelection();
        }

        // Al hacer clic en una fila, carga el nombre en el cuadro de texto para poder Modificar o Eliminar.
        private void dgvMarcas_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            Marca486LP marca = dgvMarcas.Rows[e.RowIndex].DataBoundItem as Marca486LP;
            if (marca == null) return;

            _idMarcaSeleccionada = marca.IdMarca;
            txtMarca.Text = marca.Nombre;
        }

        private void btnAgregarMarca_Click(object sender, EventArgs e)
        {
            var lm = Program.LanguageManager;

            // Con una marca elegida en la grilla hay que limpiar antes de cargar una nueva
            if (_idMarcaSeleccionada != 0)
            {
                MostrarAviso(lm.ObtenerTexto(f, "Frm.GestionCategoriasMarcas.Msg.LimpiarAntesDeAgregarMarca", "Hay una marca elegida en la grilla. Presione Limpiar antes de agregar una nueva."));
                return;
            }

            if (!ValidarMarca()) return;

            string mensaje;
            if (_bllMarca.Agregar(new Marca486LP { Nombre = txtMarca.Text.Trim() }, out mensaje))
            {
                MessageBox.Show(mensaje, lm.ObtenerTexto(f, "Frm.GestionCategoriasMarcas.Msg.Title", "Procesar categorías y marcas"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                LimpiarMarca();
                RefrescarMarcas();
            }
            else
            {
                MostrarAviso(mensaje);
            }
        }

        private void btnModificarMarca_Click(object sender, EventArgs e)
        {
            var lm = Program.LanguageManager;

            if (_idMarcaSeleccionada == 0)
            {
                MostrarAviso(lm.ObtenerTexto(f, "Frm.GestionCategoriasMarcas.Msg.SeleccionarParaModificarMarca", "Seleccione una marca de la grilla para modificar."));
                return;
            }

            if (!ValidarMarca()) return;

            string mensaje;
            if (_bllMarca.Modificar(new Marca486LP { IdMarca = _idMarcaSeleccionada, Nombre = txtMarca.Text.Trim() }, out mensaje))
            {
                MessageBox.Show(mensaje, lm.ObtenerTexto(f, "Frm.GestionCategoriasMarcas.Msg.Title", "Procesar categorías y marcas"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                LimpiarMarca();
                RefrescarMarcas();
            }
            else
            {
                MostrarAviso(mensaje);
            }
        }

        private void btnEliminarMarca_Click(object sender, EventArgs e)
        {
            var lm = Program.LanguageManager;

            if (_idMarcaSeleccionada == 0)
            {
                MostrarAviso(lm.ObtenerTexto(f, "Frm.GestionCategoriasMarcas.Msg.SeleccionarParaEliminarMarca", "Seleccione una marca de la grilla para eliminar."));
                return;
            }

            DialogResult r = MessageBox.Show(
                string.Format(lm.ObtenerTexto(f, "Frm.GestionCategoriasMarcas.Msg.ConfirmarEliminacionMarca", "¿Confirma la eliminación de la marca {0}?"), txtMarca.Text.Trim()),
                lm.ObtenerTexto(f, "Frm.GestionCategoriasMarcas.Msg.Title", "Procesar categorías y marcas"),
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (r != DialogResult.Yes) return;

            string mensaje;
            if (_bllMarca.Eliminar(_idMarcaSeleccionada, out mensaje))
            {
                MessageBox.Show(mensaje, lm.ObtenerTexto(f, "Frm.GestionCategoriasMarcas.Msg.Title", "Procesar categorías y marcas"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                LimpiarMarca();
                RefrescarMarcas();
            }
            else
            {
                MostrarAviso(mensaje);
            }
        }

        private void btnLimpiarMarca_Click(object sender, EventArgs e)
        {
            LimpiarMarca();
        }

        // Valida solo lo que corresponde a la GUI (nombre no vacio). Las reglas de negocio (formato, repetida) las hace la BLL
        private bool ValidarMarca()
        {
            var lm = Program.LanguageManager;

            if (string.IsNullOrWhiteSpace(txtMarca.Text))
            {
                MostrarAviso(lm.ObtenerTexto(f, "Frm.GestionCategoriasMarcas.Msg.NombreObligatorioMarca", "Ingrese el nombre de la marca."));
                return false;
            }
            return true;
        }

        private void LimpiarMarca()
        {
            _idMarcaSeleccionada = 0;
            txtMarca.Clear();
            dgvMarcas.ClearSelection();
            txtMarca.Focus();
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void MostrarAviso(string texto)
        {
            var lm = Program.LanguageManager;
            MessageBox.Show(texto, lm.ObtenerTexto(f, "Frm.GestionCategoriasMarcas.Msg.Title", "Procesar categorías y marcas"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        public void ActualizarIdioma()
        {
            var lm = Program.LanguageManager;

            this.Text = lm.ObtenerTexto(f, "Frm.GestionCategoriasMarcas.Titulo", "Procesar categorías y marcas");
            lblTitulo.Text = lm.ObtenerTexto(f, "Frm.GestionCategoriasMarcas.Titulo", "Procesar categorías y marcas");

            grpCategoria.Text = lm.ObtenerTexto(f, "Frm.GestionCategoriasMarcas.DatosCategoria", "Datos de la categoría");
            lblNombreCategoria.Text = lm.ObtenerTexto(f, "Frm.GestionCategoriasMarcas.Nombre", "Nombre:");
            lblCategorias.Text = lm.ObtenerTexto(f, "Frm.GestionCategoriasMarcas.ListaCategorias", "Categorías registradas:");
            btnAgregarCategoria.Text = lm.ObtenerTexto(f, "Frm.GestionCategoriasMarcas.Agregar", "Agregar");
            btnModificarCategoria.Text = lm.ObtenerTexto(f, "Frm.GestionCategoriasMarcas.Modificar", "Modificar");
            btnEliminarCategoria.Text = lm.ObtenerTexto(f, "Frm.GestionCategoriasMarcas.Eliminar", "Eliminar");
            btnLimpiarCategoria.Text = lm.ObtenerTexto(f, "Frm.GestionCategoriasMarcas.Limpiar", "Limpiar");

            grpMarca.Text = lm.ObtenerTexto(f, "Frm.GestionCategoriasMarcas.DatosMarca", "Datos de la marca");
            lblNombreMarca.Text = lm.ObtenerTexto(f, "Frm.GestionCategoriasMarcas.Nombre", "Nombre:");
            lblMarcas.Text = lm.ObtenerTexto(f, "Frm.GestionCategoriasMarcas.ListaMarcas", "Marcas registradas:");
            btnAgregarMarca.Text = lm.ObtenerTexto(f, "Frm.GestionCategoriasMarcas.Agregar", "Agregar");
            btnModificarMarca.Text = lm.ObtenerTexto(f, "Frm.GestionCategoriasMarcas.Modificar", "Modificar");
            btnEliminarMarca.Text = lm.ObtenerTexto(f, "Frm.GestionCategoriasMarcas.Eliminar", "Eliminar");
            btnLimpiarMarca.Text = lm.ObtenerTexto(f, "Frm.GestionCategoriasMarcas.Limpiar", "Limpiar");

            btnCerrar.Text = lm.ObtenerTexto(f, "Frm.GestionCategoriasMarcas.Cerrar", "Cerrar");

            AplicarCategoriaEncabezado();
            AplicarMarcaEncabezado();
        }

        private void FrmGestionCategoriasMarcas486LP_FormClosing(object sender, FormClosingEventArgs e)
        {
            Program.LanguageManager.Quitar(this);
        }
    }
}

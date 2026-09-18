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
    public partial class FrmGestionCarrito486LP : Form, IObserver486LP
    {
        private BLL_Carrito486LP _bllCarrito = new BLL_Carrito486LP();
        private BLL_Producto486LP _bllProducto = new BLL_Producto486LP();
        private BLL_Cliente486LP _bllCliente = new BLL_Cliente486LP();
        private readonly string f = "FrmGestionCarrito486LP";
        private Carrito486LP _carrito = new Carrito486LP();
        private bool _clienteAsignado = false;

        // Patentes granulares por boton (ademas de la patente de acceso al form, VENTA_GESTIONAR_CARRITO).
        private bool _puedeAsignar;
        private bool _puedeAgregar;
        private bool _puedeEliminar;
        private bool _puedeModificar;
        private bool _puedeFinalizar;
        private bool _puedeCancelar;

        public FrmGestionCarrito486LP()
        {
            InitializeComponent();
            Program.LanguageManager.Agregar(this);
            this.FormClosing += FrmGestionCarrito486LP_FormClosing;
        }

        private void FrmGestionCarrito486LP_Load(object sender, EventArgs e)
        {
            // Verificacion de acceso al form completo (mismo patron que FrmConsultarProducto486LP).
            if (!TienePatenteAcceso())
            {
                var lm = Program.LanguageManager;
                MessageBox.Show(
                    lm.ObtenerTexto(f, "Frm.GestionCarrito.Msg.SinPermiso", "No tiene permiso para gestionar el carrito."),
                    lm.ObtenerTexto(f, "Frm.GestionCarrito.Msg.SinPermiso.Title", "Acceso denegado"),
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);

                this.BeginInvoke(new Action(() =>
                {
                    this.DialogResult = DialogResult.Cancel;
                    this.Close();
                }));
                return;
            }

            _carrito = new Carrito486LP();
            _clienteAsignado = false;
            ConfigurarColumnas();
            RefrescarGrilla();
            ActualizarTotal();
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
            return permisos.Contains("VENTA_GESTIONAR_CARRITO");
        }

        // Patentes granulares por boton.
        private void AjustarBotonesSegunPerfil()
        {
            var usuario = SessionManager486LP.ObtenerInstancia().UsuarioActual();
            if (usuario == null) return;

            BLL_Perfil486LP bllPerfil = new BLL_Perfil486LP();
            List<string> permisos = bllPerfil.ObtenerPermisosPorRol(usuario.Rol);

            _puedeAsignar = permisos.Contains("CARRITO_ASIGNAR");
            _puedeAgregar = permisos.Contains("CARRITO_AGREGAR");
            _puedeEliminar = permisos.Contains("CARRITO_ELIMINAR");
            _puedeModificar = permisos.Contains("CARRITO_MODIFICAR");
            _puedeFinalizar = permisos.Contains("CARRITO_FINALIZAR");
            _puedeCancelar = permisos.Contains("CARRITO_CANCELAR");
        }

        private void AplicarPermisosBotones()
        {
            btnAsignar.Enabled = _puedeAsignar;
            btnAgregar.Enabled = _puedeAgregar;
            btnEliminar.Enabled = _puedeEliminar;
            btnModificar.Enabled = _puedeModificar;
            btnFinalizar.Enabled = _puedeFinalizar;
            btnCancelar.Enabled = _puedeCancelar;
        }

        private void btnAsignar_Click(object sender, EventArgs e)
        {
            var lm = Program.LanguageManager;
            string dni = txtDNI.Text.Trim();

            // Validar formato del DNI (solo numeros, 7 u 8 digitos)
            if (!System.Text.RegularExpressions.Regex.IsMatch(dni, @"^\d{7,8}$"))
            {
                MessageBox.Show(
                    lm.ObtenerTexto(f, "Frm.GestionCarrito.Msg.DNIIncorrecto", "DNI incorrecto."),
                    lm.ObtenerTexto(f, "Frm.GestionCarrito.Msg.Validacion.Title", "Validación"),
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // El cliente tiene que estar registrado antes de asignarlo al carrito.
            if (!_bllCliente.Existe(dni))
            {
                DialogResult r = MessageBox.Show(
                    string.Format(lm.ObtenerTexto(f, "Frm.GestionCarrito.Msg.ClienteNoRegistrado", "El cliente con DNI {0} no está registrado. ¿Desea registrarlo ahora?"), dni),
                    lm.ObtenerTexto(f, "Frm.GestionCarrito.Msg.ClienteNoRegistrado.Title", "Cliente no registrado"),
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (r == DialogResult.Yes)
                {
                    using (FrmGestionClientes486LP frmClientes = new FrmGestionClientes486LP())
                    {
                        frmClientes.ShowDialog();
                    }
                }

                // Se vuelve a verificar: si se registro en la pantalla anterior, ya existe.
                if (!_bllCliente.Existe(dni))
                {
                    return; // el cajero cancelo el registro o cerro sin registrar; no se asigna
                }
            }

            _carrito.DNICliente = dni;
            _clienteAsignado = true;
            MessageBox.Show(
                lm.ObtenerTexto(f, "Frm.GestionCarrito.Msg.ClienteAsignado", "Cliente asignado al carrito."),
                lm.ObtenerTexto(f, "Frm.GestionCarrito.Msg.Informacion.Title", "Información"),
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            var lm = Program.LanguageManager;

            // Abre el CUN01 "Consultar productos" para elegir el producto.
            using (FrmConsultarProducto486LP frm = new FrmConsultarProducto486LP())
            {
                if (frm.ShowDialog() == DialogResult.OK && frm.ProductoSeleccionado != null)
                {
                    int cantidad = (int)nudCantidad.Value;
                    string mensaje;

                    bool ok = _bllCarrito.Agregar(_carrito, frm.ProductoSeleccionado, cantidad, out mensaje);
                    if (!ok)
                    {
                        MessageBox.Show(mensaje, lm.ObtenerTexto(f, "Frm.GestionCarrito.Msg.Aviso.Title", "Aviso"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    RefrescarGrilla();
                    ActualizarTotal();
                }
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            var lm = Program.LanguageManager;

            if (dgvCarrito.CurrentRow == null)
            {
                MessageBox.Show(
                    lm.ObtenerTexto(f, "Frm.GestionCarrito.Msg.SeleccionarParaEliminar", "Seleccione un producto del carrito para eliminar."),
                    lm.ObtenerTexto(f, "Frm.GestionCarrito.Msg.Aviso.Title", "Aviso"),
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DetalleCarrito486LP det = dgvCarrito.CurrentRow.DataBoundItem as DetalleCarrito486LP;
            if (det != null)
            {
                _bllCarrito.QuitarProducto(_carrito, det);
                RefrescarGrilla();
                ActualizarTotal();
            }
        }

        private void btnModificar_Click(object sender, EventArgs e)
        {
            var lm = Program.LanguageManager;

            if (dgvCarrito.CurrentRow == null)
            {
                MessageBox.Show(
                    lm.ObtenerTexto(f, "Frm.GestionCarrito.Msg.SeleccionarParaModificar", "Seleccione un producto del carrito para modificar."),
                    lm.ObtenerTexto(f, "Frm.GestionCarrito.Msg.Aviso.Title", "Aviso"),
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DetalleCarrito486LP det = dgvCarrito.CurrentRow.DataBoundItem as DetalleCarrito486LP;
            if (det != null)
            {
                int nuevaCantidad = (int)nudCantidad.Value;

                if (!_bllProducto.VerificarStock(det.IdProducto, nuevaCantidad))
                {
                    MessageBox.Show(
                        lm.ObtenerTexto(f, "Frm.GestionCarrito.Msg.SinStock", "Sin stock disponible para la cantidad solicitada."),
                        lm.ObtenerTexto(f, "Frm.GestionCarrito.Msg.Aviso.Title", "Aviso"),
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                det.Cantidad = nuevaCantidad;
                det.Subtotal = det.Precio * nuevaCantidad;
                _carrito.Total = _carrito.Detalles.Sum(d => d.Subtotal);
                RefrescarGrilla();
                ActualizarTotal();
            }
        }

        private void btnFinalizar_Click(object sender, EventArgs e)
        {
            var lm = Program.LanguageManager;

            if (!_clienteAsignado)
            {
                MessageBox.Show(
                    lm.ObtenerTexto(f, "Frm.GestionCarrito.Msg.DebeAsignarCliente", "Debe asignar un cliente antes de finalizar."),
                    lm.ObtenerTexto(f, "Frm.GestionCarrito.Msg.Aviso.Title", "Aviso"),
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string mensaje;
            bool ok = _bllCarrito.GuardarCarrito(_carrito, out mensaje);

            if (ok)
            {
                MessageBox.Show(
                    lm.ObtenerTexto(f, "Frm.GestionCarrito.Msg.CarritoGuardado", "Carrito guardado con éxito."),
                    lm.ObtenerTexto(f, "Frm.GestionCarrito.Msg.Informacion.Title", "Información"),
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
            else
            {
                MessageBox.Show(mensaje, lm.ObtenerTexto(f, "Frm.GestionCarrito.Msg.Aviso.Title", "Aviso"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void RefrescarGrilla()
        {
            // Se bindea la lista REAL de DetalleCarrito (no una proyeccion anonima), para que dgvCarrito.CurrentRow.DataBoundItem devuelva un DetalleCarrito486LP
            dgvCarrito.AutoGenerateColumns = false;
            dgvCarrito.DataSource = null;
            dgvCarrito.DataSource = new BindingList<DetalleCarrito486LP>(_carrito.Detalles);
        }

        private void ActualizarTotal()
        {
            lblTotal.Text = _carrito.Total.ToString("C");
        }

        public void ActualizarIdioma()
        {
            var lm = Program.LanguageManager;
            this.Text = lm.ObtenerTexto(f, "Frm.GestionCarrito.Titulo", "Gestionar carrito");
            lblTitulo.Text = lm.ObtenerTexto(f, "Frm.GestionCarrito.Titulo", "Gestionar carrito");
            lblCarrito.Text = lm.ObtenerTexto(f, "Frm.GestionCarrito.Carrito", "Carrito");
            grpPersona.Text = lm.ObtenerTexto(f, "Frm.GestionCarrito.Persona", "Persona");
            grpProducto.Text = lm.ObtenerTexto(f, "Frm.GestionCarrito.Producto", "Producto");
            lblDNI.Text = lm.ObtenerTexto(f, "Frm.GestionCarrito.DNI", "DNI:");
            lblCantidad.Text = lm.ObtenerTexto(f, "Frm.GestionCarrito.Cantidad", "Cantidad:");
            lblTotalTexto.Text = lm.ObtenerTexto(f, "Frm.GestionCarrito.Total", "Total:");
            btnAsignar.Text = lm.ObtenerTexto(f, "Frm.GestionCarrito.Asignar", "Asignar");
            btnAgregar.Text = lm.ObtenerTexto(f, "Frm.GestionCarrito.Agregar", "Agregar");
            btnEliminar.Text = lm.ObtenerTexto(f, "Frm.GestionCarrito.Eliminar", "Eliminar");
            btnModificar.Text = lm.ObtenerTexto(f, "Frm.GestionCarrito.Modificar", "Modificar");
            btnFinalizar.Text = lm.ObtenerTexto(f, "Frm.GestionCarrito.Finalizar", "Finalizar carga");
            btnCancelar.Text = lm.ObtenerTexto(f, "Frm.GestionCarrito.Cancelar", "Cancelar");

            AplicarEncabezadosColumnas();
        }

        private void FrmGestionCarrito486LP_FormClosing(object sender, FormClosingEventArgs e)
        {
            Program.LanguageManager.Quitar(this);
        }

        // Define las columnas de la grilla ligadas a las propiedades reales del DetalleCarrito486LP (Marca/Modelo/Color/Talle son de solo lectura en el BE).
        private void ConfigurarColumnas()
        {
            dgvCarrito.AutoGenerateColumns = false;
            dgvCarrito.Columns.Clear();

            dgvCarrito.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Marca", Name = "colMarca" });
            dgvCarrito.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Modelo", Name = "colModelo" });
            dgvCarrito.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Color", Name = "colColor" });
            dgvCarrito.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Talle", Name = "colTalle" });
            dgvCarrito.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Cantidad", Name = "colCantidad" });
            dgvCarrito.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Precio", Name = "colPrecio" });
            dgvCarrito.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Subtotal", Name = "colSubtotal" });

            AplicarEncabezadosColumnas();
        }

        // Traduce los encabezados de la grilla - separado de ConfigurarColumnas() para poder llamarlo tambien desde ActualizarIdioma() cuando el idioma cambia en caliente.
        private void AplicarEncabezadosColumnas()
        {
            if (dgvCarrito.Columns.Count == 0) return;

            var lm = Program.LanguageManager;
            dgvCarrito.Columns["colMarca"].HeaderText = lm.ObtenerTexto(f, "Frm.GestionCarrito.Col.Marca", "Marca");
            dgvCarrito.Columns["colModelo"].HeaderText = lm.ObtenerTexto(f, "Frm.GestionCarrito.Col.Modelo", "Modelo");
            dgvCarrito.Columns["colColor"].HeaderText = lm.ObtenerTexto(f, "Frm.GestionCarrito.Col.Color", "Color");
            dgvCarrito.Columns["colTalle"].HeaderText = lm.ObtenerTexto(f, "Frm.GestionCarrito.Col.Talle", "Talle");
            dgvCarrito.Columns["colCantidad"].HeaderText = lm.ObtenerTexto(f, "Frm.GestionCarrito.Col.Cantidad", "Cantidad");
            dgvCarrito.Columns["colPrecio"].HeaderText = lm.ObtenerTexto(f, "Frm.GestionCarrito.Col.Precio", "Precio");
            dgvCarrito.Columns["colSubtotal"].HeaderText = lm.ObtenerTexto(f, "Frm.GestionCarrito.Col.Subtotal", "Subtotal");
        }
    }
}
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

        // Carrito en construccion (en memoria hasta finalizar la carga)
        private Carrito486LP _carrito = new Carrito486LP();
        private bool _clienteAsignado = false;

        public FrmGestionCarrito486LP()
        {
            InitializeComponent();
            Program.LanguageManager.Agregar(this);
            this.FormClosing += FrmGestionCarrito486LP_FormClosing;
        }

        private void FrmGestionCarrito486LP_Load(object sender, EventArgs e)
        {
            _carrito = new Carrito486LP();
            _clienteAsignado = false;
            ConfigurarColumnas();
            RefrescarGrilla();
            ActualizarTotal();
            ActualizarIdioma();
        }

        // ---------------- Asignar cliente ----------------
        private void btnAsignar_Click(object sender, EventArgs e)
        {
            string dni = txtDNI.Text.Trim();

            // Validar formato del DNI (solo numeros, 7 u 8 digitos)
            if (!System.Text.RegularExpressions.Regex.IsMatch(dni, @"^\d{7,8}$"))
            {
                MessageBox.Show("DNI incorrecto.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // El cliente tiene que estar registrado antes de asignarlo al carrito.
            if (!_bllCliente.Existe(dni))
            {
                DialogResult r = MessageBox.Show(
                    "El cliente con DNI " + dni + " no está registrado. ¿Desea registrarlo ahora?",
                    "Cliente no registrado", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

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
            MessageBox.Show("Cliente asignado al carrito.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // ---------------- Agregar producto ----------------
        private void btnAgregar_Click(object sender, EventArgs e)
        {
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
                        MessageBox.Show(mensaje, "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    RefrescarGrilla();
                    ActualizarTotal();
                }
            }
        }

        // ---------------- Eliminar renglon ----------------
        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (dgvCarrito.CurrentRow == null)
            {
                MessageBox.Show("Seleccione un producto del carrito para eliminar.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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

        // ---------------- Modificar cantidad ----------------
        private void btnModificar_Click(object sender, EventArgs e)
        {
            if (dgvCarrito.CurrentRow == null)
            {
                MessageBox.Show("Seleccione un producto del carrito para modificar.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DetalleCarrito486LP det = dgvCarrito.CurrentRow.DataBoundItem as DetalleCarrito486LP;
            if (det != null)
            {
                int nuevaCantidad = (int)nudCantidad.Value;

                if (!_bllProducto.VerificarStock(det.IdProducto, nuevaCantidad))
                {
                    MessageBox.Show("Sin stock disponible para la cantidad solicitada.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                det.Cantidad = nuevaCantidad;
                det.Subtotal = det.Precio * nuevaCantidad;
                _carrito.Total = _carrito.Detalles.Sum(d => d.Subtotal);
                RefrescarGrilla();
                ActualizarTotal();
            }
        }

        // ---------------- Finalizar carga ----------------
        private void btnFinalizar_Click(object sender, EventArgs e)
        {
            if (!_clienteAsignado)
            {
                MessageBox.Show("Debe asignar un cliente antes de finalizar.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string mensaje;
            bool ok = _bllCarrito.GuardarCarrito(_carrito, out mensaje);

            if (ok)
            {
                MessageBox.Show("Carrito guardado con éxito.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
            else
            {
                MessageBox.Show(mensaje, "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // ---------------- Cancelar ----------------
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

        // ---------------- Observer de idioma ----------------
        public void ActualizarIdioma()
        {
            var lm = Program.LanguageManager;
            string f = "FrmGestionCarrito486LP";
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

            dgvCarrito.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Marca", HeaderText = "Marca" });
            dgvCarrito.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Modelo", HeaderText = "Modelo" });
            dgvCarrito.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Color", HeaderText = "Color" });
            dgvCarrito.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Talle", HeaderText = "Talle" });
            dgvCarrito.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Cantidad", HeaderText = "Cantidad" });
            dgvCarrito.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Precio", HeaderText = "Precio" });
            dgvCarrito.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Subtotal", HeaderText = "Subtotal" });
        }
    }
}
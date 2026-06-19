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
    public partial class FrmIniciarSesionLP486 : Form, IObserver486LP
    {
        public FrmIniciarSesionLP486()
        {
            InitializeComponent();
            Program.LanguageManager.Agregar(this);
            this.FormClosing += FrmIniciarSesionLP486_FormClosing;
        }

        private void FrmIniciarSesionLP486_Load(object sender, EventArgs e)
        {
            txtUsuario.Focus();
            ActualizarIdioma();
        }

        private void btnIngresar_Click(object sender, EventArgs e)
        {
            string nombreUsuario = txtUsuario.Text.Trim();
            string contraseña = txtContraseña.Text;

            if (string.IsNullOrEmpty(nombreUsuario) || string.IsNullOrEmpty(contraseña))
            {
                MessageBox.Show("Por favor completá todos los campos.", "SneakRush — Campos vacíos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Usuario486LP usuario = null;
            string mensaje = string.Empty;

            BLL_Usuarios486LP bll = new BLL_Usuarios486LP();
            int resultado = bll.Login(nombreUsuario, contraseña, out usuario, out mensaje);

            switch (resultado)
            {
                case 1:
                    if (this.MdiParent is FrmMenuPrincipal486LP menu)
                    {
                        if (SessionManager486LP.ObtenerInstancia().IsLogged())
                        {
                            // Re-Login — el SessionManager ya tiene una instancia activa con usuario logueado
                            // El Singleton no permite crear otra sesión mientras haya una activa
                            var usuarioActual = SessionManager486LP.ObtenerInstancia().UsuarioActual();

                            if (usuario.IdUsuario == usuarioActual.IdUsuario)
                            {
                                // Mismo usuario el Singleton rechaza una segunda instancia de la misma sesión
                                MessageBox.Show("No se puede crear más de una instancia de la sesión: ya tenés esta misma cuenta en uso.",
                                    "SneakRush — Sesión única", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                            else
                            {
                                MessageBox.Show($"Ya hay una sesión activa como {usuarioActual.Nombre} {usuarioActual.Apellido}.\nCerrá la sesión antes de iniciar otra.",
                                    "SneakRush — Sesión activa", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                            LimpiarCampos();
                            return;
                        }

                        // Si por alguna razón no hay usuario activo (caso borde), hacemos el login normal
                        SessionManager486LP.ObtenerInstancia().LogIN(usuario);

                        if (usuario.DebeCambiarContraseña)
                        {
                            MessageBox.Show("Por seguridad, debe cambiar su contraseña antes de continuar.", "SneakRush — Cambio requerido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            FrmCambiarContraseña486LP frmCambiar = new FrmCambiarContraseña486LP();
                            frmCambiar.ShowDialog();
                        }

                        menu.ActualizarEstado();
                        this.Close();
                    }
                    else
                    {
                        // Login normal
                        SessionManager486LP.ObtenerInstancia().LogOut();
                        SessionManager486LP.ObtenerInstancia().LogIN(usuario);

                        // esto es lo que utilice para el cambio de idioma y que quede guarado cuando hago LogOut
                        string nombreIdiomaUsuario = new BLL_Idioma486LP().ObtenerIdioma(usuario.IdUsuario);
                        Program.LanguageManager.CambiarIdioma(Program.LanguageManager.MapearCodigo(nombreIdiomaUsuario));

                        if (usuario.DebeCambiarContraseña)
                        {
                            MessageBox.Show("Por seguridad, debe cambiar su contraseña antes de continuar.", "SneakRush — Cambio requerido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            FrmCambiarContraseña486LP frmCambiar = new FrmCambiarContraseña486LP();
                            frmCambiar.ShowDialog();
                        }

                        BLL_DV486LP bllDV = new BLL_DV486LP();
                        string tablaAfectada;
                        string mensajeDV;

                        if (!bllDV.VerificarIntegridad("Usuarios", out tablaAfectada, out mensajeDV))
                        {
                            if (!string.IsNullOrEmpty(mensajeDV))
                            {
                                MessageBox.Show($"Error al verificar integridad: {mensajeDV}", "SneakRush — Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }

                            if (usuario.Rol == "Administrador")
                            {
                                // Administrador → abre formulario de reparación
                                this.Hide();
                                DV486LP dv = new DV486LP(tablaAfectada, "", "");
                                FrmReparacionBD486LP frmReparacion = new FrmReparacionBD486LP(dv);
                                frmReparacion.ShowDialog();
                                this.Show();
                                return;
                            }
                            else
                            {
                                // Otro rol → abre el menú con acceso restringido y muestra aviso
                                MessageBox.Show("Se detectó un Error en el Sistema.\nContacte al Administrador para resolver el problema.",
                                    "SneakRush — Error en el Sistema Detectado", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                                FrmMenuPrincipal486LP menuPrincipal = new FrmMenuPrincipal486LP(true);
                                menuPrincipal.Show();
                                this.Hide();
                            }
                        }
                        else
                        {

                            FrmMenuPrincipal486LP menuPrincipal = new FrmMenuPrincipal486LP(false);
                            menuPrincipal.Show();
                            this.Hide();
                        }
                    }
                    break;


                case 0:
                    MessageBox.Show("Usuario no encontrado.", "SneakRush — Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    LimpiarCampos();
                    break;

                case -1:
                    MessageBox.Show("Contraseña incorrecta.", "SneakRush — Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtContraseña.Clear();
                    txtContraseña.Focus();
                    break;

                case -2:
                    MessageBox.Show("El usuario está inactivo. Contacte al Administrador.", "SneakRush — Usuario inactivo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    LimpiarCampos();
                    break;

                case -3:
                    MessageBox.Show("El usuario está bloqueado. Contacte al Administrador.", "SneakRush — Usuario bloqueado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    LimpiarCampos();
                    break;

                default:
                    MessageBox.Show($"Error inesperado: {mensaje}", "SneakRush — Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;

            }
        }

        private void txtUsuario_KeyDown(object sender, KeyEventArgs e) //
        {
            if (e.KeyCode == Keys.Enter) txtContraseña.Focus();
        }

        private void txtContraseña_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) btnIngresar_Click(sender, e);
        }

        private void FrmIniciarSesionLP486_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                if (this.MdiParent == null)
                {
                    Application.Exit();
                }
                else
                {
                    e.Cancel = true;
                    this.Hide();
                    return;   // no desregistrar: el form sigue vivo, solo oculto
                }
            }

            Program.LanguageManager.Quitar(this);
        }

        private void LimpiarCampos()
        {
            txtUsuario.Clear();
            txtContraseña.Clear();
            txtUsuario.Focus();
        }

        private void btnVerContraseña_Click(object sender, EventArgs e)
        {
            txtContraseña.PasswordChar = txtContraseña.PasswordChar == '*' ? '\0' : '*';
        }

        public void ActualizarIdioma()
        {
            var lm = Program.LanguageManager;
            string f = "FrmIniciarSesionLP486";

            this.Text = lm.ObtenerTexto(f, "Title");
            label1.Text = lm.ObtenerTexto(f, "lblUsuario");
            label2.Text = lm.ObtenerTexto(f, "lblContraseña");
            btnIngresar.Text = lm.ObtenerTexto(f, "btnIngresar");
        }


    }
}

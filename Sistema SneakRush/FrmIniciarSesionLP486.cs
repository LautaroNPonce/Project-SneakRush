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
            var lm = Program.LanguageManager;
            string f = "FrmIniciarSesionLP486";

            string nombreUsuario = txtUsuario.Text.Trim();
            string contraseña = txtContraseña.Text;

            if (string.IsNullOrEmpty(nombreUsuario) || string.IsNullOrEmpty(contraseña))
            {
                MessageBox.Show(lm.ObtenerTexto(f, "Msg.CamposVacios", "Por favor completá todos los campos."),
                    lm.ObtenerTexto(f, "Msg.CamposVacios.Titulo", "SneakRush — Campos vacíos"),MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                                MessageBox.Show(
                                    lm.ObtenerTexto(f, "Msg.SesionUnica", "No se puede crear más de una instancia de la sesión: ya tenés esta misma cuenta en uso."),
                                    lm.ObtenerTexto(f, "Msg.SesionUnica.Titulo", "SneakRush — Sesión única"),
                                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                            else
                            {
                                MessageBox.Show(
                                    string.Format(lm.ObtenerTexto(f, "Msg.SesionActiva", "Ya hay una sesión activa como {0} {1}.\nCerrá la sesión antes de iniciar otra."), usuarioActual.Nombre, usuarioActual.Apellido),
                                    lm.ObtenerTexto(f, "Msg.SesionActiva.Titulo", "SneakRush — Sesión activa"),
                                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                            LimpiarCampos();
                            return;
                        }

                        // Si por alguna razón no hay usuario activo (caso borde), hacemos el login normal
                        SessionManager486LP.ObtenerInstancia().LogIN(usuario);

                        if (usuario.DebeCambiarContraseña)
                        {
                            MessageBox.Show(
                                lm.ObtenerTexto(f, "Msg.CambioRequerido", "Por seguridad, debe cambiar su contraseña antes de continuar."),
                                lm.ObtenerTexto(f, "Msg.CambioRequerido.Titulo", "SneakRush — Cambio requerido"),
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                            MessageBox.Show(
                                lm.ObtenerTexto(f, "Msg.CambioRequerido", "Por seguridad, debe cambiar su contraseña antes de continuar."),
                                lm.ObtenerTexto(f, "Msg.CambioRequerido.Titulo", "SneakRush — Cambio requerido"),
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            FrmCambiarContraseña486LP frmCambiar = new FrmCambiarContraseña486LP();
                            frmCambiar.ShowDialog();
                        }

                        BLL_DV486LP bllDV = new BLL_DV486LP();
                        string errorTecnicoDV;
                        List<string> tablasConProblemas = bllDV.VerificarTodas(out errorTecnicoDV);

                        // Error técnico real (BD caída, etc.): no abrir menú
                        if (!string.IsNullOrEmpty(errorTecnicoDV))
                        {
                            MessageBox.Show(
                                string.Format(lm.ObtenerTexto(f, "Msg.ErrorIntegridad", "Error al verificar integridad: {0}"), errorTecnicoDV),
                                lm.ObtenerTexto(f, "Msg.Error.Titulo", "SneakRush — Error"),
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        if (tablasConProblemas.Count > 0)
                        {
                            if (usuario.Rol == "Administrador")
                            {
                                // Administrador abre reparación con TODAS las tablas afectadas
                                this.Hide();
                                FrmReparacionBD486LP frmReparacion = new FrmReparacionBD486LP(tablasConProblemas);
                                DialogResult resultadoReparacion = frmReparacion.ShowDialog();

                                if (resultadoReparacion == DialogResult.OK)
                                {
                                    if (!this.IsDisposed)
                                    {
                                        // Recalculó los DV correctamente: la sesión YA está activa, así que
                                        // entramos directo al menú sin volver a pedir login.
                                        FrmMenuPrincipal486LP menuPrincipal = new FrmMenuPrincipal486LP(false);
                                        menuPrincipal.Show();
                                        this.Hide();
                                    }
                                }
                                else
                                {
                                    if (!this.IsDisposed) this.Show();
                                }
                                return;
                            }
                            else
                            {
                                // Otro rol menú restringido con aviso
                                MessageBox.Show(
                                    lm.ObtenerTexto(f, "Msg.InconsistenciaNoAdmin", "Se detectó un Error en el Sistema.\nContacte al Administrador para resolver el problema."),
                                    lm.ObtenerTexto(f, "Msg.InconsistenciaNoAdmin.Titulo", "SneakRush — Error en el Sistema Detectado"),
                                    MessageBoxButtons.OK, MessageBoxIcon.Warning);

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
                    MessageBox.Show(
                        lm.ObtenerTexto(f, "Msg.UsuarioNoEncontrado", "Usuario no encontrado."),
                        lm.ObtenerTexto(f, "Msg.Error.Titulo", "SneakRush — Error"),
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    LimpiarCampos();
                    break;

                case -1:
                    MessageBox.Show(
                        lm.ObtenerTexto(f, "Msg.PasswordIncorrecta", "Contraseña incorrecta."),
                        lm.ObtenerTexto(f, "Msg.Error.Titulo", "SneakRush — Error"),
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtContraseña.Clear();
                    txtContraseña.Focus();
                    break;

                case -2:
                    MessageBox.Show(
                        lm.ObtenerTexto(f, "Msg.UsuarioInactivo", "El usuario está inactivo. Contacte al Administrador."),
                        lm.ObtenerTexto(f, "Msg.UsuarioInactivo.Titulo", "SneakRush — Usuario inactivo"),
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    LimpiarCampos();
                    break;

                case -3:
                    MessageBox.Show(
                        lm.ObtenerTexto(f, "Msg.UsuarioBloqueado", "El usuario está bloqueado. Contacte al Administrador."),
                        lm.ObtenerTexto(f, "Msg.UsuarioBloqueado.Titulo", "SneakRush — Usuario bloqueado"),
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    LimpiarCampos();
                    break;

                default:
                    MessageBox.Show(
                        string.Format(lm.ObtenerTexto(f, "Msg.ErrorInesperado", "Error inesperado: {0}"), mensaje),
                        lm.ObtenerTexto(f, "Msg.Error.Titulo", "SneakRush — Error"),
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
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

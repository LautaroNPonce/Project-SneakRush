using BLL;
using Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sistema_SneakRush
{
    public partial class FrmGestionRespaldo486LP : Form, IObserver486LP
    {
        private BLL_Respaldo486LP _bllRespaldo = new BLL_Respaldo486LP();
        private bool _soloRestore;
        private bool _puedeSeleccionarCarpeta;
        private bool _puedeRespaldar;
        private bool _puedeSeleccionarArchivo;
        private bool _puedeRestaurar;
        private bool _puedeSalir;

        public FrmGestionRespaldo486LP(bool soloRestore = false)
        {
            InitializeComponent();
            Program.LanguageManager.Agregar(this);
            this.FormClosing += FrmGestionRespaldo486LP_FormClosing;
            _soloRestore = soloRestore;
        }

        private void FrmGestionRespaldo486LP_Load(object sender, EventArgs e)
        {
            txtRutaBackup.Text = string.Empty;
            txtRutaRestore.Text = string.Empty;

            if (_soloRestore)
            {
                grpBackup.Visible = false;
                grpRestore.Top = grpBackup.Top;
                btnSalir.Top = grpRestore.Bottom + 20;
                this.ClientSize = new System.Drawing.Size(this.ClientSize.Width, btnSalir.Bottom + 20);
            }

            ActualizarIdioma();
            AjustarBotonesSegunPerfil();
            AplicarPermisosBotones();
        }

        private void btnSeleccionarCarpeta_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog fbd = new FolderBrowserDialog())
            {
                if (fbd.ShowDialog() == DialogResult.OK)
                { 
                    txtRutaBackup.Text = fbd.SelectedPath; 
                }
            }
        }

        private void btnRespaldar_Click(object sender, EventArgs e)
        {
            var lm = Program.LanguageManager;
            string f = "FrmGestionRespaldo486LP";

            string ruta;
            string mensaje;

            if (!_bllRespaldo.Backup(txtRutaBackup.Text, out ruta, out mensaje))
            {
                MessageBox.Show(lm.ObtenerTexto(f, mensaje, mensaje),
                    lm.ObtenerTexto(f, "Msg.Backup.ErrorTitle", "Error"),
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            MessageBox.Show(lm.ObtenerTexto(f, "Msg.Backup.Exito", "Backup generado correctamente en:") + "\n" + ruta,
                lm.ObtenerTexto(f, "Msg.Backup.ExitoTitle", "Éxito"),MessageBoxButtons.OK, MessageBoxIcon.Information);

            txtRutaBackup.Text = string.Empty;
        }

        private void btnSeleccionarArchivo_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "SQL Server Backup (*.bak)|*.bak";
                if (ofd.ShowDialog() == DialogResult.OK)
                { 
                    txtRutaRestore.Text = ofd.FileName; 
                }
            }
        }

        private void btnRestaurar_Click(object sender, EventArgs e)
        {
            var lm = Program.LanguageManager;
            string f = "FrmGestionRespaldo486LP";

            // Valido del archivo ANTES de tocar la base
            if (string.IsNullOrWhiteSpace(txtRutaRestore.Text) ||!File.Exists(txtRutaRestore.Text) ||Path.GetExtension(txtRutaRestore.Text).ToLower() != ".bak")
            {
                MessageBox.Show(
                    lm.ObtenerTexto(f, "Msg.Restore.ArchivoInvalido", "Debe seleccionar un archivo .bak válido."),
                    lm.ObtenerTexto(f, "Msg.Restore.ArchivoInvalidoTitle", "Aviso"),
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult confirma = MessageBox.Show(lm.ObtenerTexto(f, "Msg.Restore.Confirmar","La restauración reemplazará TODA la base de datos actual y cerrará la sesión.\n¿Desea continuar?"),
                lm.ObtenerTexto(f, "Msg.Restore.ConfirmarTitle", "Confirmar restauración"),MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (confirma != DialogResult.Yes)
            { 
                return; 
            }

            string mensaje;

            if (!_bllRespaldo.Restore(txtRutaRestore.Text, out mensaje))
            {
                MessageBox.Show(lm.ObtenerTexto(f, mensaje, mensaje),lm.ObtenerTexto(f, "Msg.Restore.ErrorTitle", "Error"),
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            MessageBox.Show(
                lm.ObtenerTexto(f, "Msg.Restore.Exito", "Base de datos restaurada correctamente.\nLa aplicación se reiniciará para recargar los datos."),
                lm.ObtenerTexto(f, "Msg.Restore.ExitoTitle", "Éxito"), MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.DialogResult = DialogResult.OK;
            Application.Restart();
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        public void ActualizarIdioma()
        {
            var lm = Program.LanguageManager;
            string f = "FrmGestionRespaldo486LP";

            this.Text = lm.ObtenerTexto(f, "Title", "Gestión de Respaldos");
            lblTitulo.Text = lm.ObtenerTexto(f, "lblTitulo", "Gestión de Respaldos");

            grpBackup.Text = lm.ObtenerTexto(f, "grpBackup", "Respaldo (Backup)");
            lblBackupPath.Text = lm.ObtenerTexto(f, "lblBackupPath", "Carpeta de destino:");
            btnSeleccionarCarpeta.Text = lm.ObtenerTexto(f, "btnSeleccionarCarpeta", "Seleccionar carpeta...");
            btnRespaldar.Text = lm.ObtenerTexto(f, "btnRespaldar", "Respaldar");

            grpRestore.Text = lm.ObtenerTexto(f, "grpRestore", "Restauración (Restore)");
            lblRestorePath.Text = lm.ObtenerTexto(f, "lblRestorePath", "Archivo de backup (.bak):");
            btnSeleccionarArchivo.Text = lm.ObtenerTexto(f, "btnSeleccionarArchivo", "Seleccionar archivo...");
            btnRestaurar.Text = lm.ObtenerTexto(f, "btnRestaurar", "Restaurar");

            btnSalir.Text = lm.ObtenerTexto(f, "btnSalir", "Salir");
        }

        private void FrmGestionRespaldo486LP_FormClosing(object sender, FormClosingEventArgs e)
        {
            Program.LanguageManager.Quitar(this);
        }

        private void AjustarBotonesSegunPerfil()
        {
            var usuario = SessionManager486LP.ObtenerInstancia().UsuarioActual();
            if (usuario == null) return;

            BLL_Perfil486LP bllPerfil = new BLL_Perfil486LP();
            List<string> permisos = bllPerfil.ObtenerPermisosPorRol(usuario.Rol);

            _puedeSeleccionarCarpeta = permisos.Contains("RESPALDOS_SELECCIONAR_CARPETA");
            _puedeRespaldar = permisos.Contains("RESPALDOS_BACKUP");
            _puedeSeleccionarArchivo = permisos.Contains("RESPALDOS_SELECCIONAR_ARCHIVO");
            _puedeRestaurar = permisos.Contains("RESPALDOS_RESTORE");
            _puedeSalir = permisos.Contains("RESPALDOS_SALIR");
        }

        private void AplicarPermisosBotones()
        {
            btnSeleccionarCarpeta.Enabled = _puedeSeleccionarCarpeta;
            btnRespaldar.Enabled = _puedeRespaldar;
            btnSeleccionarArchivo.Enabled = _puedeSeleccionarArchivo;
            btnRestaurar.Enabled = _puedeRestaurar;
            btnSalir.Enabled = _puedeSalir;
        }
    }
}

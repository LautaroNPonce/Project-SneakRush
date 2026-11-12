using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace Sistema_SneakRush
{

    public static class Ayuda486LP
    {
        private const string CARPETA = "Ayuda";
        private const string INICIO = "index";
        private static DateTime _ultimaApertura = DateTime.MinValue;

        public static void Mostrar(string tema = null)
        {
            if ((DateTime.Now - _ultimaApertura).TotalMilliseconds < 700)
            {
                return;
            }
            _ultimaApertura = DateTime.Now;

            try
            {
                string carpeta = Path.Combine(Application.StartupPath, CARPETA);
                string ruta = Path.Combine(carpeta, (string.IsNullOrEmpty(tema) ? INICIO : tema) + ".html");

                if (!File.Exists(ruta))
                {
                    ruta = Path.Combine(carpeta, INICIO + ".html");
                }

                if (!File.Exists(ruta))
                {
                    MessageBox.Show(
                        "No se encontró la carpeta de ayuda. Avise al administrador del sistema.",
                        "SneakRush - Ayuda", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                Process.Start(new ProcessStartInfo(ruta) { UseShellExecute = true });
            }
            catch (Exception)
            {
                MessageBox.Show(
                    "No se pudo abrir la ayuda en el navegador.",
                    "SneakRush - Ayuda", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        public static void Vincular(Form formulario, string tema)
        {
            formulario.KeyPreview = true;

            formulario.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.F1 && !e.Alt && !e.Control && !e.Shift)
                {
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                    Mostrar(tema);
                }
            };

            formulario.HelpRequested += (s, e) =>
            {
                e.Handled = true;
                Mostrar(tema);
            };
        }
    }
}

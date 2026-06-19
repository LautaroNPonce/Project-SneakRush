using Services;
using System;
using System.Windows.Forms;

namespace Sistema_SneakRush
{
    internal static class Program
    {
        /// <summary>
        /// Punto de entrada principal para la aplicación.
        /// </summary>


        public static LanguageManager486LP LanguageManager = new LanguageManager486LP();

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            LanguageManager.CambiarIdioma(LanguageManager.ObtenerUltimoIdioma());
            Application.Run(new FrmIniciarSesionLP486());
        }
    }
}

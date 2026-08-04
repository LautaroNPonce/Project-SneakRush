using BLL;
using Services;
using System;
using System.Windows.Forms;

namespace Sistema_SneakRush
{
    //internal static class Program
    //{
    //    /// <summary>
    //    /// Punto de entrada principal para la aplicación.
    //    /// </summary>


    //    public static LanguageManager486LP LanguageManager = new LanguageManager486LP();

    //    [STAThread]
    //    static void Main()
    //    {
    //        Application.EnableVisualStyles();
    //        Application.SetCompatibleTextRenderingDefault(false);
    //        LanguageManager.CambiarIdioma(LanguageManager.ObtenerUltimoIdioma());
    //        Application.Run(new FrmIniciarSesionLP486());
    //    }
    //}

    internal static class Program
    {
        /// <summary>
        /// Punto de entrada principal para la aplicacion.
        /// </summary>
        public static LanguageManager486LP LanguageManager = new LanguageManager486LP();

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Asegura la base de datos ANTES de mostrar el login, si no existe, se crea y se siembra automaticamente (instalacion "a un clic").
            var inicializador = new BLL_Inicializador486LP();
            string errorBD;
            if (!inicializador.AsegurarBaseDeDatos(out errorBD))
            {
                MessageBox.Show(
                    "No se pudo preparar la base de datos del sistema.\r\n\r\n" +
                    "Detalle: " + errorBD + "\r\n\r\n" +
                    "Verifica que esta PC tenga SQL Server instalado (LocalDB o Express).",
                    "Sistema SneakRush",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return; // sin base de datos no tiene sentido abrir la aplicacion
            }

            // Flujo normal de arranque.
            LanguageManager.CambiarIdioma(LanguageManager.ObtenerUltimoIdioma());
            Application.Run(new FrmIniciarSesionLP486());
        }
    }
}

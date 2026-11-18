using BE;
using BLL;
using Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
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
        static void Main(string[] args)
        {
            // Para que la moneda sea el peso argentino, y el formato de fecha sea dd/MM/yyyy, se fuerza la cultura a "es-AR" (Español - Argentina).
            var ar = new System.Globalization.CultureInfo("es-AR");
            System.Threading.Thread.CurrentThread.CurrentCulture = ar;
            System.Threading.Thread.CurrentThread.CurrentUICulture = ar;

            if (args.Length > 0 && args[0].StartsWith("--"))
            {
                Environment.Exit(EjecutarModoInstalacion(args));
                return;
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
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
            Mensajes486LP.Configurar(LanguageManager);
            LanguageManager.CambiarIdioma(LanguageManager.ObtenerUltimoIdioma());
            Application.Run(new FrmIniciarSesionLP486());
        }

        private static int EjecutarModoInstalacion(string[] args)
        {
            Dictionary<string, string> opciones = LeerOpciones(args);
            string archivoResultado = opciones.ContainsKey("--resultado") ? opciones["--resultado"] : null;
            ResultadoInstalacion486LP resultado;

            try
            {
                BLL_Instalacion486LP bll = new BLL_Instalacion486LP();
                string modo = args[0].ToLowerInvariant();
                string carpetaRespaldos = opciones.ContainsKey("--respaldos") ? opciones["--respaldos"] : null;

                if (modo == "--estado")
                {
                    resultado = bll.Estado();
                }
                else if (modo == "--preparar")
                {
                    resultado = bll.Preparar(LeerDatosAdmin(opciones));
                }
                else if (modo == "--reiniciar")
                {
                    resultado = bll.Reiniciar(LeerDatosAdmin(opciones), carpetaRespaldos);
                }
                else if (modo == "--borrar-base")
                {
                    resultado = bll.BorrarBase(carpetaRespaldos);
                }
                else
                {
                    resultado = new ResultadoInstalacion486LP { Codigo = BLL_Instalacion486LP.ARGUMENTOS_INVALIDOS, Mensaje = "Modo desconocido: " + args[0] };
                }
            }
            catch (Exception ex)
            {
                resultado = new ResultadoInstalacion486LP { Codigo = BLL_Instalacion486LP.ERROR_BASE, Mensaje = ex.Message.Replace("\r", " ").Replace("\n", " ") };
            }

            EscribirResultado(archivoResultado, resultado);
            return resultado.Codigo;
        }

        private static Dictionary<string, string> LeerOpciones(string[] args)
        {
            Dictionary<string, string> opciones = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            for (int i = 1; i < args.Length - 1; i++)
            {
                if (args[i].StartsWith("--"))
                {
                    opciones[args[i]] = args[i + 1];
                    i++;
                }
            }
            return opciones;
        }

        // Lee el archivo temporal con los datos del administrador y lo borra enseguida
        private static DatosAdminInstalacion486LP LeerDatosAdmin(Dictionary<string, string> opciones)
        {
            if (!opciones.ContainsKey("--datos")) return null;

            string ruta = opciones["--datos"];
            if (!File.Exists(ruta)) return null;

            DatosAdminInstalacion486LP datos = new DatosAdminInstalacion486LP();
            try
            {
                foreach (string linea in File.ReadAllLines(ruta, Encoding.UTF8))
                {
                    int igual = linea.IndexOf('=');
                    if (igual <= 0) continue;

                    string clave = linea.Substring(0, igual).Trim().ToUpperInvariant();
                    string valor = linea.Substring(igual + 1).Trim();

                    if (clave == "DNI") datos.DNI = valor;
                    else if (clave == "NOMBRE") datos.Nombre = valor;
                    else if (clave == "APELLIDO") datos.Apellido = valor;
                    else if (clave == "CORREO") datos.Correo = valor;
                }
            }
            finally
            {
                try { File.Delete(ruta); } catch { }
            }
            return datos;
        }

        // Archivo de resultado que lee el asistente: CODIGO, MENSAJE, USUARIO, CLAVE y RESPALDO (uno por linea)
        private static void EscribirResultado(string archivoResultado, ResultadoInstalacion486LP r)
        {
            if (string.IsNullOrWhiteSpace(archivoResultado)) return;

            try
            {
                string[] lineas = new string[]
                {
                    "CODIGO=" + r.Codigo,
                    "MENSAJE=" + (r.Mensaje ?? string.Empty),
                    "USUARIO=" + (r.Usuario ?? string.Empty),
                    "CLAVE=" + (r.ContraseñaInicial ?? string.Empty),
                    "RESPALDO=" + (r.RutaRespaldo ?? string.Empty)
                };
                File.WriteAllLines(archivoResultado, lineas, new UTF8Encoding(true));
            }
            catch { }
        }
    }
}

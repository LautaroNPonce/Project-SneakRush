using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    // Da acceso a los mensajes de negocio (los que devuelven las BLL) en el idioma activo.
    // Los textos viven en la seccion "Mensajes" de Idiomas/es.json, en.json y pt.json, y se leen con el mismo LanguageManager que usan los formularios.
    // Program.cs le pasa el LanguageManager al iniciar la aplicacion.
    public static class Mensajes486LP
    {
        private const string SECCION = "Mensajes";
        private static LanguageManager486LP _idioma;

        public static void Configurar(LanguageManager486LP idioma)
        {
            _idioma = idioma;
        }

        // Devuelve el mensaje de la clave en el idioma actual. Si el texto lleva {0}, {1}... se reemplazan con los argumentos.
        public static string Obtener(string clave, params object[] argumentos)
        {
            string texto = clave;

            if (_idioma != null)
                texto = _idioma.ObtenerTexto(SECCION, clave, clave);

            if (argumentos != null && argumentos.Length > 0)
            {
                try
                {
                    texto = string.Format(texto, argumentos);
                }
                catch (FormatException)
                {
                    // si el texto traducido no coincide con los argumentos, se devuelve sin formatear antes que romper el flujo
                }
            }

            return texto;
        }
    }
}

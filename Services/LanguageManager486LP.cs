using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class LanguageManager486LP : ISubject486LP
    {
        private string _idiomaActual = "es";
        private JObject _diccionario = new JObject();
        private List<IObserver486LP> _observers = new List<IObserver486LP>();

        // ISubject486LP
        public void Agregar(IObserver486LP observer)
        {
            if (!_observers.Contains(observer))
                _observers.Add(observer);
        }

        public void Quitar(IObserver486LP observer)
        {
            _observers.Remove(observer);
        }

        public void Notificar()
        {
            foreach (IObserver486LP observer in _observers)
                observer.ActualizarIdioma();
        }

        // Idioma actual
        public string IdiomaActual => _idiomaActual;

        public void CambiarIdioma(string codigoIdioma)
        {
            _idiomaActual = codigoIdioma;
            CargarJSON(codigoIdioma);
            Notificar();
        }

        public void CargarJSON(string codigoIdioma)
        {
            try
            {
                string ruta = Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory,
                    "Idiomas",
                    codigoIdioma + ".json");

                if (!File.Exists(ruta))
                {
                    _diccionario = new JObject();
                    return;
                }

                string contenido = File.ReadAllText(ruta, System.Text.Encoding.UTF8);
                _diccionario = JObject.Parse(contenido);
            }
            catch
            {
                _diccionario = new JObject();
            }
        }

        // Obtener texto traducido
        public string ObtenerTexto(string form, string clave)
        {
            try
            {
                if (_diccionario == null) return clave;

                JToken formToken = _diccionario[form];
                if (formToken == null) return clave;

                JToken texto = formToken[clave];
                return texto != null ? texto.ToString() : clave;
            }
            catch
            {
                return clave;
            }
        }

        public string MapearCodigo(string nombreIdioma)
        {
            switch (nombreIdioma)
            {
                case "Español": 
                    return "es";
                case "Inglés": 
                    return "en";
                case "Portugués": 
                    return "pt";
                default: 
                    return "es";
            }
        }
    }
}

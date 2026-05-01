using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BE
{
    public class Idioma486LP
    {
        public string NombreIdioma { get; set; }
        public string Tipo { get; set; } // Ejemplo: "Nativo", "Fluido", "Intermedio", etc.

        public Idioma486LP() { }

        // Constructor para inicializar el idioma con un nombre y un tipo
        public Idioma486LP(string nombreIdioma, string tipo)
        {
            NombreIdioma = nombreIdioma;
            Tipo = tipo;
        }

        // Sobrescribir el método ToString para mostrar el nombre del idioma
        public override string ToString()
        {
            return NombreIdioma;
        }
    }
}

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
        public string Tipo { get; set; } 

        public Idioma486LP() { }

        public Idioma486LP(string nombreIdioma, string tipo)
        {
            NombreIdioma = nombreIdioma;
            Tipo = tipo;
        }

        public override string ToString()
        {
            return NombreIdioma;
        }
    }
}

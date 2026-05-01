using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BE
{
    public class Perfil486LP
    {
        public int IdPerfil { get; set; }
        public string Nombre { get; set; }

        public Perfil486LP() { }

        // Constructor para inicializar el perfil con un ID y un nombre
        public Perfil486LP(int idPerfil, string nombre)
        {
            IdPerfil = idPerfil;
            Nombre = nombre;
        }

        // Sobrescribir el método ToString para mostrar el nombre del perfil
        public override string ToString()
        {
            return Nombre;
        }
    }
}

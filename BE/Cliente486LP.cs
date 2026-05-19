using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BE
{
    public class Cliente486LP
    {
        public string DNI { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Correo { get; set; }
        public string Telefono { get; set; }
        public Cliente486LP() { }

        public Cliente486LP(string dni, string nombre, string apellido, string correo, string telefono)
        {
            DNI = dni;
            Nombre = nombre;
            Apellido = apellido;
            Correo = correo;
            Telefono = telefono;
        }

        public override string ToString()
        {
            return $"{Nombre} {Apellido} (DNI: {DNI})";
        }
    }
}

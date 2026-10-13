using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BE
{
    public class Marca486LP
    {
        [ColumnaTabla486LP]
        public int IdMarca { get; set; }
        [ColumnaTabla486LP]
        public string Nombre { get; set; }

        public Marca486LP() { }

        public Marca486LP(int idMarca, string nombre)
        {
            IdMarca = idMarca;
            Nombre = nombre;
        }

        // Se usa tal cual en el combo de seleccion del Form (cmbMarca): que se vea el Nombre, no "BE.Marca486LP".
        public override string ToString()
        {
            return Nombre;
        }
    }
}

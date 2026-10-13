using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BE
{
    public class Categoria486LP
    {
        [ColumnaTabla486LP]
        public int IdCategoria { get; set; }
        [ColumnaTabla486LP]
        public string Nombre { get; set; }

        public Categoria486LP() { }

        public Categoria486LP(int idCategoria, string nombre)
        {
            IdCategoria = idCategoria;
            Nombre = nombre;
        }

        // Se usa tal cual en el combo de seleccion del Form (cmbCategoria): que se vea el Nombre, no "BE.Categoria486LP".
        public override string ToString()
        {
            return Nombre;
        }
    }
}

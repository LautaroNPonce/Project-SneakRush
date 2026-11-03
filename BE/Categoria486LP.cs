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

        public override string ToString()
        {
            return Nombre;
        }
    }
}

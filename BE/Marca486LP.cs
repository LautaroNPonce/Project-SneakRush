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

        public override string ToString()
        {
            return Nombre;
        }
    }
}

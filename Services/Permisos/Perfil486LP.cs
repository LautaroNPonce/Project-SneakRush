using BE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class Perfil486LP
    {
        // Perfil486LP NO hereda de Composite486LP (a diferencia de Permiso/Familia), por eso el atributo va directo sobre sus propias propiedades.
        [ColumnaTabla486LP]
        public int IdPerfil { get; set; }
        [ColumnaTabla486LP]
        public string Nombre { get; set; }

        // NO lleva el atributo: no es columna de la tabla Perfil, es la representacion en memoria del patron Composite (familias + permisos sueltos asignados), se arma aparte.
        public List<Composite486LP> Componentes { get; set; }

        public Perfil486LP()
        {
            Componentes = new List<Composite486LP>();
        }
    }
}
}

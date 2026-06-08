using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class Perfil486LP
    {
        public int IdPerfil { get; set; }
        public string Nombre { get; set; }
        public List<Composite486LP> Componentes { get; set; }

        public Perfil486LP()
        {
            Componentes = new List<Composite486LP>();
        }

        public override string ToString()
        {
            return Nombre;
        }
    }
}

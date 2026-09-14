using BE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public abstract class Composite486LP
    {
        // [ColumnaTabla486LP] puesto aca (clase base) porque tanto Permiso486LP (hoja) como Familia486LP (compuesto) heredan Id y Nombre de aca, y las
        // dos son columna real de su tabla (Permiso.IdPermiso/Nombre, Familia.IdFamilia/Nombre). El mapeo por reflexion recorre tambien las
        // propiedades heredadas, asi que esto ya deja lista la base para cuando se migre Familia.
        [ColumnaTabla486LP]
        public int Id { get; set; }
        [ColumnaTabla486LP]
        public string Nombre { get; set; }

        public abstract void operacion();
        public abstract void add(Composite486LP c);
        public abstract void remove(Composite486LP c);
        public abstract Composite486LP get(int i);
    }
}

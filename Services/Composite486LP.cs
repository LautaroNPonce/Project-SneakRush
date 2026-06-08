using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public abstract class Composite486LP
    {
        public int Id { get; set; }
        public string Nombre { get; set; }

        public abstract void operacion();
        public abstract void add(Composite486LP c);
        public abstract void remove(Composite486LP c);
        public abstract Composite486LP get(int i);

        public override string ToString()
        {
            return Nombre;
        }
    }
}

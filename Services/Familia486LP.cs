using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class Familia486LP : Composite486LP
    {
        private List<Composite486LP> _listaHijos;
 
        public Familia486LP()
        {
            _listaHijos = new List<Composite486LP>();
        }
 
        public override void operacion()
        {
        }
 
        public override void add(Composite486LP c)
        {
            _listaHijos.Add(c);
        }
 
        public override void remove(Composite486LP c)
        {
            _listaHijos.Remove(c);
        }
 
        public override Composite486LP get(int i)
        {
            return _listaHijos[i];
        }
    }
}

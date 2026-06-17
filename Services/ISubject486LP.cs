using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public interface ISubject486LP
    {
        void Agregar(IObserver486LP observer);
        void Quitar(IObserver486LP observer);
        void Notificar();
    }
}

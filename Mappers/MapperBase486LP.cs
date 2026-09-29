using DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mappers
{
    // Base de todos los Mappers. No expone la conexión (eso es de DAL.Conexion486LP)
    // cada Mapper arma el SqlCommand y se lo pasa a Conexion486LP para que lo ejecute. Queda como clase base por si más adelante hace falta algo común a todos los Mappers
    public abstract class MapperBase486LP
    {
    }
}

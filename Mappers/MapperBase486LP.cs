using DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mappers
{
    /// Base de todos los Mappers. A partir de la correccion de arquitectura, esta clase NO expone la conexion: eso ahora es responsabilidad exclusiva
    /// de DAL.Conexion486LP, que es quien conecta, desconecta, lee y escribe.
    /// Los Mappers arman el SqlCommand (nombre del SP + parametros) y se lo pasan a Conexion486LP.EjecutarConsulta/EjecutarNoConsulta/EjecutarEscalar.
    /// Se deja esta clase base para que todos los Mappers compartan un mismo tipo (util si mas adelante hace falta agregar algo comun a todos).
    public abstract class MapperBase486LP
    {
    }
}

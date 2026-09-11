using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BE
{
    /// Marca una propiedad de una entidad (BE) como columna real de tabla, para que ManejadorMapeo486LP la pueda llenar automaticamente por reflexion
    /// desde un SqlDataReader. El nombre de la propiedad tiene que coincidir con el nombre de la columna del SELECT del Stored Procedure.
    /// Propiedades que NO son columna (objetos relacionados, calculadas de sololectura, listas de detalle) NO llevan este atributo.
    /// 
    [AttributeUsage(AttributeTargets.Property)]
    public class ColumnaTabla486LP : Attribute
    {
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BE
{
    // Marca una propiedad de un BE como columna real de tabla, para que ManejadorMapeo486LP la complete sola por reflexión desde un DataTable/DataRow. 
    // El nombre de la propiedad tiene que coincidir con el de la columna del SELECT del SP. Objetos relacionados, listas de detalle y propiedades calculadas NO llevan este atributo. 
    [AttributeUsage(AttributeTargets.Property)]
    public class ColumnaTabla486LP : Attribute
    {
    }
}

using BE;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Mappers
{

    /// Mapea un DataRow (de un DataTable devuelto por Conexion486LP.EjecutarConsulta) a una entidad, de forma generica, mirando que propiedades tienen el atributo
    /// [ColumnaTabla486LP]. Reemplaza los metodos "Mapear(...)" que antes se repetian, escritos a mano, en cada Mapper de cada entidad.
    public class ManejadorMapeo486LP
    {
        // Mapea una fila (DataRow) a una entidad T.
        public static T MapearEntidad<T>(DataRow fila) where T : class, new()
        {
            T obj = new T();
            Type tipo = typeof(T);

            foreach (PropertyInfo prop in tipo.GetProperties())
            {
                bool esColumna = prop.GetCustomAttributes(typeof(ColumnaTabla486LP), false).Any();
                if (!esColumna) continue;

                object valor = fila[prop.Name];

                if (valor == null || valor == DBNull.Value)
                {
                    // Columna nullable en NULL (ej: Telefono): se deja el valor por defecto de la propiedad, no se intenta convertir.
                    continue;
                }

                Type tipoDestino = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
                prop.SetValue(obj, Convert.ChangeType(valor, tipoDestino));
            }

            return obj;
        }

        // Mapea TODAS las filas de un DataTable a una lista de entidades T.
        public static List<T> MapearLista<T>(DataTable tabla) where T : class, new()
        {
            List<T> lista = new List<T>();

            foreach (DataRow fila in tabla.Rows)
            {
                lista.Add(MapearEntidad<T>(fila));
            }

            return lista;
        }
    }
}

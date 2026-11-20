using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    // Una fila de la tabla DV: dice qué tabla está protegida, cuál es su clave primaria y cuánto vale su DVV guardado.
    public class DV486LP
    {
        public string Tabla { get; set; }

        // Nombre de la columna clave de la tabla. Si la clave es compuesta, las columnas van separadas por coma (ej: "IdFamilia, IdPermiso")
        public string ClavePrimaria { get; set; }

        // Dígito verificador vertical guardado: la suma de los DVH de todas las filas de la tabla
        public long DVV { get; set; }

        public string TablaAfectada { get; set; }

        public DV486LP()
        {
        }

        public DV486LP(string tabla, string clavePrimaria, long dvv)
        {
            Tabla = tabla;
            ClavePrimaria = clavePrimaria;
            DVV = dvv;
            TablaAfectada = tabla;
        }

        // Las columnas que forman la clave, una por elemento (la mayoría de las tablas tiene una sola)
        public string[] ColumnasClave()
        {
            return ClavePrimaria
                .Split(',')
                .Select(c => c.Trim())
                .Where(c => c.Length > 0)
                .ToArray();
        }

        public class InconsistenciaDV486LP
        {
            public string ID { get; set; }
            public string Tabla { get; set; }
            public string Inconsistencia { get; set; }
        }
    }
}

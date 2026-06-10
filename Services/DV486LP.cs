using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class DV486LP
    {
        public string Tabla { get; set; }
        public string DVH { get; set; }
        public string DVV { get; set; }
        public string TablaAfectada { get; set; }

        public DV486LP(string tabla, string dvh, string dvv)
        {
            Tabla = tabla;
            DVH = dvh;
            DVV = dvv;
            TablaAfectada = tabla;
        }

        public class InconsistenciaDV486LP
        {
            public string ID { get; set; }
            public string Tabla { get; set; }
            public string Inconsistencia { get; set; }
        }
    }
}

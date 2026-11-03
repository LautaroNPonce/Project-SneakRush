using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BE
{
    public class ResultadoImportacion486LP
    {
        public int IdImportacion { get; set; }
        public int IdProveedor { get; set; }
        public string NroRemito { get; set; }
        public string CUITProveedor { get; set; }
        public string NombreArchivo { get; set; }
        public string EstadoRemito { get; set; }
        public int Altas { get; set; }
        public int Sumas { get; set; }
        public int Rechazadas { get; set; }
        public int YaImportadas { get; set; }
        public List<LineaImportacion486LP> Lineas { get; set; } = new List<LineaImportacion486LP>();

        public ResultadoImportacion486LP() { }
    }
}

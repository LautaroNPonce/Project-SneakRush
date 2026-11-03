using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BE
{
    public class LineaImportacion486LP
    {
        public const string RESULTADO_ALTA = "Alta";
        public const string RESULTADO_SUMA_STOCK = "SumaStock";
        public const string RESULTADO_RECHAZADA = "Rechazada";
        public const string RESULTADO_YA_IMPORTADA = "YaImportada"; // Boton Ver
        public const string RESULTADO_PENDIENTE = "Pendiente";

        public int Linea { get; set; } // Los datos tal como vinieron en el XML (texto), para mostrarlos y para poder exportar la linea rechazada sin perder nada
        public string Marca { get; set; }
        public string Categoria { get; set; }
        public string Modelo { get; set; }
        public string Color { get; set; }
        public string Talle { get; set; }
        public string Cantidad { get; set; }
        public string Precio { get; set; }
        public string Resultado { get; set; } // El motivo exacto si se rechazo, o lo que se hizo si se importo
        public string Detalle { get; set; }
        public bool Exportable { get; set; }

        public LineaImportacion486LP() { }
    }
}

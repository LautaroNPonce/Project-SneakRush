using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BE
{
    // Resultado de una linea del XML dentro de una importacion (no es una tabla: es lo que la BLL le devuelve al formulario para mostrar el reporte)
    public class LineaImportacion486LP
    {
        public const string RESULTADO_ALTA = "Alta";
        public const string RESULTADO_SUMA_STOCK = "SumaStock";
        public const string RESULTADO_RECHAZADA = "Rechazada";
        public const string RESULTADO_YA_IMPORTADA = "YaImportada";
        // Solo para la vista previa (boton Ver): la linea se leyo del XML pero todavia no se importo
        public const string RESULTADO_PENDIENTE = "Pendiente";

        public int Linea { get; set; }
        // Los datos tal como vinieron en el XML (texto), para mostrarlos y para poder exportar la linea rechazada sin perder nada
        public string Marca { get; set; }
        public string Categoria { get; set; }
        public string Modelo { get; set; }
        public string Color { get; set; }
        public string Talle { get; set; }
        public string Cantidad { get; set; }
        public string Precio { get; set; }
        public string Resultado { get; set; }
        // Que paso con la linea: el motivo exacto si se rechazo, o lo que se hizo si se importo
        public string Detalle { get; set; }
        // Una linea rechazada porque su numero ya esta ocupado por otra importada no se puede reenviar corregida: no se exporta
        public bool Exportable { get; set; }

        public LineaImportacion486LP() { }
    }
}

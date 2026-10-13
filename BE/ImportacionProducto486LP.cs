using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BE
{
    // Una fila por remito de proveedor importado (clave: proveedor + numero de remito). Es el registro que impide importar dos veces el mismo remito.
    public class ImportacionProducto486LP
    {
        public const string ESTADO_COMPLETO = "Completo";
        public const string ESTADO_CON_RECHAZOS = "ConRechazos";

        [ColumnaTabla486LP]
        public int IdImportacion { get; set; }
        [ColumnaTabla486LP]
        public int IdProveedor { get; set; }
        [ColumnaTabla486LP]
        public string NroRemito { get; set; }
        // Completo: todas las lineas del remito se importaron. ConRechazos: quedan lineas rechazadas pendientes de corregir
        [ColumnaTabla486LP]
        public string Estado { get; set; }
        // Fecha de la ultima importacion de este remito
        [ColumnaTabla486LP]
        public DateTime FechaImportacion { get; set; }
        [ColumnaTabla486LP]
        public string Usuario { get; set; }
        [ColumnaTabla486LP]
        public string NombreArchivo { get; set; }
        [ColumnaTabla486LP]
        public int LineasAlta { get; set; }
        [ColumnaTabla486LP]
        public int LineasSuma { get; set; }
        [ColumnaTabla486LP]
        public int LineasRechazadas { get; set; }

        public ImportacionProducto486LP() { }

        public ImportacionProducto486LP(int idImportacion, int idProveedor, string nroRemito, string estado, DateTime fechaImportacion, string usuario, string nombreArchivo, int lineasAlta, int lineasSuma, int lineasRechazadas)
        {
            IdImportacion = idImportacion;
            IdProveedor = idProveedor;
            NroRemito = nroRemito;
            Estado = estado;
            FechaImportacion = fechaImportacion;
            Usuario = usuario;
            NombreArchivo = nombreArchivo;
            LineasAlta = lineasAlta;
            LineasSuma = lineasSuma;
            LineasRechazadas = lineasRechazadas;
        }

        public override string ToString()
        {
            return $"Remito {NroRemito} ({Estado})";
        }
    }
}

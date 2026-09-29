using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BE
{
    public class Factura486LP
    {
        [ColumnaTabla486LP]
        public int IdFactura { get; set; }
        [ColumnaTabla486LP]
        public string NroFactura { get; set; }
        [ColumnaTabla486LP]
        public DateTime Fecha { get; set; }
        [ColumnaTabla486LP]
        public int IdOrden { get; set; }
        [ColumnaTabla486LP]
        public string MedioPago { get; set; }
        [ColumnaTabla486LP]
        public decimal Total { get; set; }

        // "Pendiente de Pago" -> "Pagada" (mismo patron que Estado en las demas entidades).
        [ColumnaTabla486LP]
        public string Estado { get; set; }

        public Factura486LP() { }

        public Factura486LP(int idFactura, string nroFactura, DateTime fecha, int idOrden, string medioPago, decimal total, string estado)
        {
            IdFactura = idFactura;
            NroFactura = nroFactura;
            Fecha = fecha;
            IdOrden = idOrden;
            MedioPago = medioPago;
            Total = total;
            Estado = estado;
        }

        public override string ToString()
        {
            return $"Factura #{IdFactura} - {NroFactura}";
        }
    }
}

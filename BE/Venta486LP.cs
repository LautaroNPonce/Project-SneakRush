using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BE
{
    public class Venta486LP
    {
        [ColumnaTabla486LP]
        public int IdVenta { get; set; }
        [ColumnaTabla486LP]
        public string NroComprobante { get; set; }
        [ColumnaTabla486LP]
        public DateTime Fecha { get; set; }
        [ColumnaTabla486LP]
        public string DNICliente { get; set; }
        [ColumnaTabla486LP]
        public string MedioPago { get; set; }
        [ColumnaTabla486LP]
        public decimal Total { get; set; }

        // no lleva [ColumnaTabla486LP] xq no es una columna de la tabla Venta, es la coleccion de detalles relacionados (se llena aparte).
        public List<DetalleVenta486LP> Detalles { get; set; }

        public Venta486LP()
        {
            Detalles = new List<DetalleVenta486LP>();
        }

        public Venta486LP(int idVenta, string nroComprobante, DateTime fecha, string dniCliente, string medioPago, decimal total)
        {
            IdVenta = idVenta;
            NroComprobante = nroComprobante;
            Fecha = fecha;
            DNICliente = dniCliente;
            MedioPago = medioPago;
            Total = total;
            Detalles = new List<DetalleVenta486LP>();
        }

        public override string ToString()
        {
            return $"Venta #{IdVenta} - Comprobante {NroComprobante} - Total ${Total}";
        }
    }
}

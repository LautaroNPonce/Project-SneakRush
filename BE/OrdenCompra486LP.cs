using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BE
{
    public class OrdenCompra486LP
    {
        [ColumnaTabla486LP]
        public int IdOrden { get; set; }
        [ColumnaTabla486LP]
        public string NroOrden { get; set; }
        [ColumnaTabla486LP]
        public DateTime Fecha { get; set; }
        [ColumnaTabla486LP]
        public int IdProveedor { get; set; }
        [ColumnaTabla486LP]
        public int IdSolicitud { get; set; }
        [ColumnaTabla486LP]
        public decimal CostoTotal { get; set; }
        // Nuevo en CUN07: "Pendiente de Recepción" -> "Recibida" (mismo patron que Estado en
        // SolicitudCompra486LP y Carrito486LP).
        [ColumnaTabla486LP]
        public string Estado { get; set; }

        // NO lleva [ColumnaTabla486LP]: no es una columna de la tabla OrdenCompra, es la
        // coleccion de detalles relacionados (se llena aparte).
        public List<DetalleOrdenCompra486LP> Detalles { get; set; }

        public OrdenCompra486LP()
        {
            Detalles = new List<DetalleOrdenCompra486LP>();
        }

        public OrdenCompra486LP(int idOrden, string nroOrden, DateTime fecha, int idProveedor, int idSolicitud, decimal costoTotal, string estado)
        {
            IdOrden = idOrden;
            NroOrden = nroOrden;
            Fecha = fecha;
            IdProveedor = idProveedor;
            IdSolicitud = idSolicitud;
            CostoTotal = costoTotal;
            Estado = estado;
            Detalles = new List<DetalleOrdenCompra486LP>();
        }

        public override string ToString()
        {
            return $"Orden #{IdOrden} - {NroOrden}";
        }
    }
}

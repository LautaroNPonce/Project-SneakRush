using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BE
{
    public class DetalleRecepcion486LP
    {
        [ColumnaTabla486LP]
        public int IdDetalleRecepcion { get; set; }
        [ColumnaTabla486LP]
        public int IdRecepcion { get; set; }
        [ColumnaTabla486LP]
        public int IdProducto { get; set; }
        [ColumnaTabla486LP]
        public int CantidadRecibida { get; set; }
        [ColumnaTabla486LP]
        public int CantidadFaltante { get; set; }

        // Vienen del JOIN con Producto - mismo patron que DetalleVenta486LP, DetalleSolicitudCompra486LP
        // y DetalleOrdenCompra486LP: no son columnas reales de DetalleRecepcion, pero llevan
        // [ColumnaTabla486LP] para que el mapeo por reflexion las complete solas.
        [ColumnaTabla486LP]
        public string Marca { get; set; }
        [ColumnaTabla486LP]
        public string Modelo { get; set; }
        [ColumnaTabla486LP]
        public string Color { get; set; }
        [ColumnaTabla486LP]
        public string Talle { get; set; }

        // Dato de referencia (Cantidad Pedida de la Orden de origen) - NO es columna de
        // DetalleRecepcion, se completa en memoria al armar el detalle desde la Orden, para
        // poder mostrar la comparacion Pedida vs Recibida en pantalla.
        public int CantidadPedida { get; set; }

        public DetalleRecepcion486LP() { }

        public DetalleRecepcion486LP(int idDetalleRecepcion, int idRecepcion, int idProducto, int cantidadRecibida, int cantidadFaltante)
        {
            IdDetalleRecepcion = idDetalleRecepcion;
            IdRecepcion = idRecepcion;
            IdProducto = idProducto;
            CantidadRecibida = cantidadRecibida;
            CantidadFaltante = cantidadFaltante;
        }

        public override string ToString()
        {
            return $"Detalle #{IdDetalleRecepcion} - Producto {IdProducto}, recibido {CantidadRecibida}";
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BE
{
    public class DetalleSolicitudCompra486LP
    {
        [ColumnaTabla486LP]
        public int IdDetalleSolicitud { get; set; }
        [ColumnaTabla486LP]
        public int IdSolicitud { get; set; }
        [ColumnaTabla486LP]
        public int IdProducto { get; set; }
        [ColumnaTabla486LP]
        public int CantidadSolicitada { get; set; }

        // Vienen del JOIN con Producto en el SP DetalleSolicitudCompra_ListarPorSolicitud - NO son
        // columnas reales de la tabla DetalleSolicitudCompra (por eso no participan del mecanismo
        // de DV, que lee "SELECT * FROM DetalleSolicitudCompra"). Llevan [ColumnaTabla486LP] igual,
        // para que el mapeo por reflexion las complete solas desde el DataTable del JOIN.
        [ColumnaTabla486LP]
        public string Marca { get; set; }
        [ColumnaTabla486LP]
        public string Modelo { get; set; }
        [ColumnaTabla486LP]
        public string Color { get; set; }
        [ColumnaTabla486LP]
        public string Talle { get; set; }

        public DetalleSolicitudCompra486LP() { }

        public DetalleSolicitudCompra486LP(int idDetalleSolicitud, int idSolicitud, int idProducto, int cantidadSolicitada)
        {
            IdDetalleSolicitud = idDetalleSolicitud;
            IdSolicitud = idSolicitud;
            IdProducto = idProducto;
            CantidadSolicitada = cantidadSolicitada;
        }

        public override string ToString()
        {
            return $"Detalle #{IdDetalleSolicitud} - Producto {IdProducto} x{CantidadSolicitada}";
        }
    }
}

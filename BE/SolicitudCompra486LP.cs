using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BE
{
    public class SolicitudCompra486LP
    {
        [ColumnaTabla486LP]
        public int IdSolicitud { get; set; }
        [ColumnaTabla486LP]
        public DateTime Fecha { get; set; }
        [ColumnaTabla486LP]
        public string DNIAdministrador { get; set; }
        [ColumnaTabla486LP]
        public string Estado { get; set; }
        public List<DetalleSolicitudCompra486LP> Detalles { get; set; }

        public SolicitudCompra486LP()
        {
            Detalles = new List<DetalleSolicitudCompra486LP>();
        }

        public SolicitudCompra486LP(int idSolicitud, DateTime fecha, string dniAdministrador, string estado)
        {
            IdSolicitud = idSolicitud;
            Fecha = fecha;
            DNIAdministrador = dniAdministrador;
            Estado = estado;
            Detalles = new List<DetalleSolicitudCompra486LP>();
        }

        public override string ToString()
        {
            return $"Solicitud #{IdSolicitud} - {Estado}";
        }
    }
}

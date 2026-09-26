using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BE
{
    public class Recepcion486LP
    {
        [ColumnaTabla486LP]
        public int IdRecepcion { get; set; }
        [ColumnaTabla486LP]
        public string NroRecepcion { get; set; }
        [ColumnaTabla486LP]
        public DateTime Fecha { get; set; }
        [ColumnaTabla486LP]
        public int IdOrden { get; set; }

        // NO lleva [ColumnaTabla486LP]: no es una columna de la tabla Recepcion, es la
        // coleccion de detalles relacionados (se llena aparte).
        public List<DetalleRecepcion486LP> Detalles { get; set; }

        public Recepcion486LP()
        {
            Detalles = new List<DetalleRecepcion486LP>();
        }

        public Recepcion486LP(int idRecepcion, string nroRecepcion, DateTime fecha, int idOrden)
        {
            IdRecepcion = idRecepcion;
            NroRecepcion = nroRecepcion;
            Fecha = fecha;
            IdOrden = idOrden;
            Detalles = new List<DetalleRecepcion486LP>();
        }

        public override string ToString()
        {
            return $"Recepción #{IdRecepcion} - {NroRecepcion}";
        }
    }
}

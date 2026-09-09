using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BE
{
    public class Carrito486LP
    {
        public int IdCarrito { get; set; }
        public string DNICliente { get; set; }
        public DateTime Fecha { get; set; }
        public decimal Total { get; set; }
        public string Estado { get; set; }
        public List<DetalleCarrito486LP> Detalles { get; set; }

        public Carrito486LP()
        {
            Detalles = new List<DetalleCarrito486LP>();
        }

        public Carrito486LP(int idCarrito, string dniCliente, DateTime fecha, decimal total, string estado)
        {
            IdCarrito = idCarrito;
            DNICliente = dniCliente;
            Fecha = fecha;
            Total = total;
            Estado = estado;
            Detalles = new List<DetalleCarrito486LP>();
        }

        public override string ToString()
        {
            return $"Carrito #{IdCarrito} - DNI {DNICliente} - Total ${Total}";
        }
    }
}

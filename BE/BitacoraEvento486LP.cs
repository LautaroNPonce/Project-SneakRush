using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BE
{
    public class BitacoraEvento486LP
    {
        public int Numero { get; set; }
        public DateTime Fecha { get; set; }
        public string Modulo { get; set; }
        public string Descripcion { get; set; }
        public string Criticidad { get; set; }  // Advertencia | Error | Crítico
        public string DNI { get; set; }

        // Constructores
        public BitacoraEvento486LP() { }

        // Constructor cuando creás un evento nuevo para insertar en la BD
        public BitacoraEvento486LP(string modulo, string descripcion, string criticidad, string dni)
        {
            Fecha = DateTime.Now;
            Modulo = modulo;
            Descripcion = descripcion;
            Criticidad = criticidad;
            DNI = dni;
        }

        // Constructor cuando creás un evento nuevo para insertar en la BD
        public BitacoraEvento486LP(int numero, DateTime fecha, string modulo, string descripcion, string criticidad, string dni)
        {
            Numero = numero;
            Fecha = fecha;
            Modulo = modulo;
            Descripcion = descripcion;
            Criticidad = criticidad;
            DNI = dni;
        }
        // Método para mostrar el evento en formato legible
        public override string ToString()
        {
            return $"[{Criticidad}] {Fecha:dd/MM/yyyy HH:mm} — {Modulo}: {Descripcion}";
        }
    }
}

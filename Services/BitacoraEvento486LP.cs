using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class BitacoraEvento486LP
    {
        public int Numero { get; set; }
        public DateTime Fecha { get; set; }
        public string Modulo { get; set; }
        public string Descripcion { get; set; }
        public string Criticidad { get; set; }  // Advertencia | Error | Crítico
        public string DNI { get; set; }
        public string NombreUsuario { get; set; }

        // Constructores
        public BitacoraEvento486LP() { }

        // Constructor cuando creás un evento nuevo para insertar en la BD
        public BitacoraEvento486LP(string modulo, string descripcion, string criticidad, string dni, string nombreUsuario = "")
        {
            Fecha = DateTime.Now;
            Modulo = modulo;
            Descripcion = descripcion;
            Criticidad = criticidad;
            DNI = dni;
            NombreUsuario = nombreUsuario; 
        }

        // Constructor cuando creás un evento nuevo para insertar en la BD
        public BitacoraEvento486LP(int numero, DateTime fecha, string modulo, string descripcion, string criticidad, string dni, string nombreUsuario = "")
        {
            Numero = numero;
            Fecha = fecha;
            Modulo = modulo;
            Descripcion = descripcion;
            Criticidad = criticidad;
            DNI = dni;
            NombreUsuario = nombreUsuario;
        }
        // Método para mostrar el evento en formato legible
        public override string ToString()
        {
            return $"[{Criticidad}] {Fecha:dd/MM/yyyy HH:mm} — {Modulo}: {Descripcion}";
        }
    }
}

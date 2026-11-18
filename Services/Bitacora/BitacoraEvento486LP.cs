using BE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class BitacoraEvento486LP
    {
        [ColumnaTabla486LP]
        public int Numero { get; set; }
        [ColumnaTabla486LP]
        public DateTime Fecha { get; set; }
        [ColumnaTabla486LP]
        public string Modulo { get; set; }
        [ColumnaTabla486LP]
        public string Descripcion { get; set; }
        [ColumnaTabla486LP]
        public int Criticidad { get; set; }
        [ColumnaTabla486LP]
        public string DNI { get; set; }
        [ColumnaTabla486LP]
        public string NombreUsuario { get; set; }

        [ColumnaTabla486LP]
        public string Nombre { get; set; }
        [ColumnaTabla486LP]
        public string Apellido { get; set; }

        // Constructores
        public BitacoraEvento486LP() { }

        public BitacoraEvento486LP(string modulo, string descripcion, int criticidad, string dni, string nombreUsuario = "")
        {
            Fecha = DateTime.Now;
            Modulo = modulo;
            Descripcion = descripcion;
            Criticidad = criticidad;
            DNI = dni;
            NombreUsuario = nombreUsuario;
        }

        // Lo uso para LEER un evento desde la BD (En DAL_Bitacora — Listar y Filtrar)
        public BitacoraEvento486LP(int numero, DateTime fecha, string modulo, string descripcion, int criticidad, string dni, string nombreUsuario = "")
        {
            Numero = numero;
            Fecha = fecha;
            Modulo = modulo;
            Descripcion = descripcion;
            Criticidad = criticidad;
            DNI = dni;
            NombreUsuario = nombreUsuario;
        }

        public override string ToString()
        {
            return $"[{Criticidad486LP.ATexto(Criticidad)}] {Fecha:dd/MM/yyyy HH:mm} — {Modulo}: {Descripcion}";
        }
    }
}

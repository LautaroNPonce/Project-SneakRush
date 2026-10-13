using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BE
{
    public class Proveedor486LP
    {
        [ColumnaTabla486LP]
        public int IdProveedor { get; set; }
        [ColumnaTabla486LP]
        public string Nombre { get; set; }
        // CUIT guardado solo con los 11 digitos (sin guiones)
        [ColumnaTabla486LP]
        public string CUIT { get; set; }
        [ColumnaTabla486LP]
        public string Telefono { get; set; }
        [ColumnaTabla486LP]
        public string Correo { get; set; }

        public Proveedor486LP() { }

        public Proveedor486LP(int idProveedor, string nombre, string cuit, string telefono, string correo)
        {
            IdProveedor = idProveedor;
            Nombre = nombre;
            CUIT = cuit;
            Telefono = telefono;
            Correo = correo;
        }

        // Se usa tal cual en el combo de seleccion del Form (cmbProveedor) - que se vea el Nombre, no "BE.Proveedor486LP".
        public override string ToString()
        {
            return Nombre;
        }
    }
}

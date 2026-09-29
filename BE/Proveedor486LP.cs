using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BE
{
    // todavia no llegamos a CUN12 "Gestionar Proveedores" entonces tiene datos minimos
    public class Proveedor486LP
    {
        [ColumnaTabla486LP]
        public int IdProveedor { get; set; }
        [ColumnaTabla486LP]
        public string Nombre { get; set; }
        [ColumnaTabla486LP]
        public string Contacto { get; set; }

        public Proveedor486LP() { }

        public Proveedor486LP(int idProveedor, string nombre, string contacto)
        {
            IdProveedor = idProveedor;
            Nombre = nombre;
            Contacto = contacto;
        }

        // Se usa tal cual en el combo de seleccion del Form (cmbProveedor) - que se vea el Nombre, no "BE.Proveedor486LP".
        public override string ToString()
        {
            return Nombre;
        }
    }
}

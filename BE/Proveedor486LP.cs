using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BE
{
    // Version MINIMA a proposito - se amplia cuando se llegue a CUN12 "Gestionar Proveedores" (ver pendiente guardado: hay que actualizar tambien el Diagrama de Clases y el DER de CUN06,
    // y el script SQL con ALTER TABLE sobre esta misma tabla).
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

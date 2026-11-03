using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace BE
{
    // Forma del XML de un remito de proveedor. XmlSerializer lo usa para leer el archivo que se importa (deserializacion)
    // y para armar el archivo de lineas rechazadas que se exporta para corregir (serializacion).
    // Todos los datos son texto a proposito: asi cada linea se valida por separado y un dato mal escrito no rechaza el archivo completo.
    [XmlRoot("remito")]
    public class RemitoXml486LP
    {
        [XmlAttribute("numero")]
        public string Numero { get; set; }

        [XmlAttribute("cuitProveedor")]
        public string CuitProveedor { get; set; }

        [XmlElement("item")]
        public List<ItemRemitoXml486LP> Items { get; set; } = new List<ItemRemitoXml486LP>();

        public RemitoXml486LP() { }
    }

    // Una linea (item) del remito
    public class ItemRemitoXml486LP
    {
        // Numero de linea. Si el archivo no lo trae queda en null y se numera por posicion
        [XmlAttribute("linea")]
        public string Linea { get; set; }

        [XmlElement("marca")]
        public string Marca { get; set; }

        [XmlElement("categoria")]
        public string Categoria { get; set; }

        [XmlElement("modelo")]
        public string Modelo { get; set; }

        [XmlElement("color")]
        public string Color { get; set; }

        [XmlElement("talle")]
        public string Talle { get; set; }

        [XmlElement("cantidad")]
        public string Cantidad { get; set; }

        [XmlElement("precio")]
        public string Precio { get; set; }

        public ItemRemitoXml486LP() { }
    }
}

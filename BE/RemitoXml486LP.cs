using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace BE
{
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

    public class ItemRemitoXml486LP
    {
        // Si el archivo no lo trae queda en null y se numera por posicion
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

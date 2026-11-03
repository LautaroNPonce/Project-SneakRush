using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace BE
{
    // Forma del XML con los productos que estan a la venta (los que tienen stock). XmlSerializer lo usa para exportarlos (serializacion).
    [XmlRoot("catalogo")]
    public class CatalogoXml486LP
    {
        [XmlAttribute("fecha")]
        public string Fecha { get; set; }

        [XmlAttribute("usuario")]
        public string Usuario { get; set; }

        [XmlAttribute("cantidad")]
        public int Cantidad { get; set; }

        [XmlElement("producto")]
        public List<ProductoCatalogoXml486LP> Productos { get; set; } = new List<ProductoCatalogoXml486LP>();

        public CatalogoXml486LP() { }
    }

    // Un producto a la venta
    public class ProductoCatalogoXml486LP
    {
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

        [XmlElement("precio")]
        public decimal Precio { get; set; }

        [XmlElement("stock")]
        public int Stock { get; set; }

        public ProductoCatalogoXml486LP() { }
    }
}

using BE;
using System;
using System.IO;
using System.Linq;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace BLL
{
    // Genera el comprobante de una Venta en PDF, con formato de TICKET (angosto, como el de una
    // maquina de tarjeta o Mercado Pago) - no una factura A4 formal. Usa iTextSharp 5.5.13, que
    // ya estaba referenciado en el proyecto de antes.
    public static class GeneradorComprobantePDF486LP
    {
        private const float AnchoTicket = 226f; // ~80mm, ancho tipico de un rollo termico de POS

        // Genera el PDF y devuelve la ruta del archivo creado. "cliente" puede ser null (si no se
        // pudo resolver por algun motivo) - el comprobante igual se genera, solo sin el nombre.
        public static string Generar(Venta486LP venta, Cliente486LP cliente)
        {
            string carpeta = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Comprobantes");
            Directory.CreateDirectory(carpeta);
            string rutaArchivo = Path.Combine(carpeta, $"Comprobante_{venta.NroComprobante}.pdf");

            // Alto dinamico: mas renglones de detalle => ticket mas largo, para no dejar espacio de mas.
            float alto = 240f + (venta.Detalles.Count * 40f);
            Document doc = new Document(new Rectangle(AnchoTicket, alto), 10f, 10f, 10f, 10f);

            using (FileStream fs = new FileStream(rutaArchivo, FileMode.Create))
            {
                PdfWriter.GetInstance(doc, fs);
                doc.Open();

                Font fuenteTitulo = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 14);
                Font fuenteNormal = FontFactory.GetFont(FontFactory.HELVETICA, 9);
                Font fuenteNegrita = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 9);
                Font fuenteChica = FontFactory.GetFont(FontFactory.HELVETICA, 8);
                string separador = new string('-', 38);

                Paragraph titulo = new Paragraph("SneakRush", fuenteTitulo) { Alignment = Element.ALIGN_CENTER };
                doc.Add(titulo);

                Paragraph subtitulo = new Paragraph("Comprobante de venta", fuenteChica) { Alignment = Element.ALIGN_CENTER, SpacingAfter = 6f };
                doc.Add(subtitulo);

                doc.Add(new Paragraph(separador, fuenteChica));

                doc.Add(new Paragraph($"Comprobante N°: {venta.NroComprobante}", fuenteNormal));
                doc.Add(new Paragraph($"Fecha: {venta.Fecha:dd/MM/yyyy HH:mm}", fuenteNormal));
                doc.Add(new Paragraph($"DNI: {venta.DNICliente}", fuenteNormal));

                if (cliente != null)
                {
                    doc.Add(new Paragraph($"Cliente: {cliente.Nombre} {cliente.Apellido}", fuenteNormal));
                }

                doc.Add(new Paragraph($"Medio de pago: {venta.MedioPago}", fuenteNormal));

                doc.Add(new Paragraph(separador, fuenteChica) { SpacingBefore = 4f });

                foreach (DetalleVenta486LP det in venta.Detalles)
                {
                    doc.Add(new Paragraph($"{det.Marca} {det.Modelo} - {det.Color} - Talle {det.Talle}", fuenteNegrita));
                    doc.Add(new Paragraph($"  {det.Cantidad} x {det.PrecioUnitario:C} = {det.Subtotal:C}", fuenteNormal) { SpacingAfter = 3f });
                }

                doc.Add(new Paragraph(separador, fuenteChica) { SpacingBefore = 2f });

                Paragraph total = new Paragraph($"TOTAL: {venta.Total:C}", fuenteTitulo) { SpacingBefore = 4f };
                doc.Add(total);

                doc.Add(new Paragraph(separador, fuenteChica) { SpacingBefore = 6f });

                Paragraph gracias = new Paragraph("¡Gracias por su compra!", fuenteChica) { Alignment = Element.ALIGN_CENTER, SpacingBefore = 6f };
                doc.Add(gracias);

                doc.Close();
            }

            return rutaArchivo;
        }
    }
}
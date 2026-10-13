using BE;
using Mappers;
using Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace BLL
{
    // Importacion masiva de productos desde el XML de un remito de proveedor (extension del CUN10 Procesar productos).
    // Por cada linea: si el producto existe (marca + modelo + color + talle) le suma el stock sin tocar el precio; si no existe lo da de alta con ese stock.
    // Las lineas validas se importan y las rechazadas se informan una por una. Un mismo remito nunca suma dos veces la misma linea.
    public class BLL_ImportacionProducto486LP
    {
        private Mapper_ImportacionProducto486LP ObjetoMapper = new Mapper_ImportacionProducto486LP();
        private BLL_Producto486LP ObjProducto = new BLL_Producto486LP();
        private BLL_Marca486LP ObjMarca = new BLL_Marca486LP();
        private BLL_Categoria486LP ObjCategoria = new BLL_Categoria486LP();
        private BLL_Proveedor486LP ObjProveedor = new BLL_Proveedor486LP();
        private BLL_Bitacora486LP ObjBitacora = new BLL_Bitacora486LP();
        private BLL_DV486LP ObjDV = new BLL_DV486LP();

        private const string MODULO = "Maestros";
        private const long TAMANIO_MAXIMO_BYTES = 5 * 1024 * 1024;
        private const int LINEAS_MAXIMO = 2000;
        private const int NUMERO_LINEA_MAXIMO = 100000;
        private const int CANTIDAD_MAXIMA = 10000;

        // Lo que se pudo interpretar de una linea valida
        private class LineaValidada
        {
            public int IdMarca;
            public string Marca;
            public int IdCategoria;
            public string Categoria;
            public string Modelo;
            public string Color;
            public string Talle;
            public int Cantidad;
            public decimal Precio;
        }

        // Proveedores para el combo del formulario de importacion
        public List<Proveedor486LP> ListarProveedores()
        {
            return ObjProveedor.Listar();
        }

        // Importa el XML. Devuelve false solo si se rechaza el archivo completo (mensaje con el motivo y resultado en null).
        // Devuelve true si se procesó, aunque haya lineas rechazadas: el detalle esta en resultado y mensaje trae el resumen.
        public bool Importar(int idProveedor, string rutaArchivo, out ResultadoImportacion486LP resultado, out string mensaje)
        {
            resultado = null;
            mensaje = "";

            try
            {
                if (idProveedor <= 0)
                {
                    mensaje = Mensajes486LP.Obtener("Importacion.ProveedorObligatorio"); // Muestra: Seleccione el proveedor del remito
                    return false;
                }

                if (string.IsNullOrWhiteSpace(rutaArchivo))
                {
                    mensaje = Mensajes486LP.Obtener("Importacion.ArchivoObligatorio"); // Muestra: Seleccione el archivo XML a importar
                    return false;
                }

                if (!File.Exists(rutaArchivo))
                {
                    mensaje = Mensajes486LP.Obtener("Importacion.ArchivoNoExiste"); // Muestra: No se encontró el archivo seleccionado
                    return false;
                }

                FileInfo info = new FileInfo(rutaArchivo);
                if (!string.Equals(info.Extension, ".xml", StringComparison.OrdinalIgnoreCase) || info.Length == 0 || info.Length > TAMANIO_MAXIMO_BYTES)
                {
                    mensaje = Mensajes486LP.Obtener("Importacion.ArchivoInvalido"); // Muestra: El archivo debe ser un .xml con contenido y no superar los 5 MB
                    return false;
                }

                Proveedor486LP proveedor = ObjProveedor.Listar().FirstOrDefault(p => p.IdProveedor == idProveedor);
                if (proveedor == null)
                {
                    mensaje = Mensajes486LP.Obtener("Importacion.ProveedorNoExiste"); // Muestra: El proveedor elegido ya no existe. Actualice la lista y vuelva a intentar
                    return false;
                }

                XDocument doc;
                if (!LeerXml(rutaArchivo, out doc, out mensaje)) return false;

                XElement raiz = doc.Root;
                if (raiz == null || raiz.Name.LocalName != "remito")
                {
                    mensaje = Mensajes486LP.Obtener("Importacion.EstructuraInvalida"); // Muestra: El archivo no tiene la estructura esperada: debe ser un remito con al menos una línea item
                    return false;
                }

                string nroRemito = ((string)raiz.Attribute("numero") ?? "").Trim();
                if (!Regex.IsMatch(nroRemito, @"^[A-Za-z0-9][A-Za-z0-9\-_/.]{0,29}$"))
                {
                    mensaje = Mensajes486LP.Obtener("Importacion.RemitoInvalido"); // Muestra: El número de remito es obligatorio, admite letras, números y los signos - _ / . y hasta 30 caracteres
                    return false;
                }

                // Si el XML trae el CUIT del proveedor tiene que ser el del proveedor elegido (evita cargar el remito de uno a nombre de otro)
                string cuitArchivo = ((string)raiz.Attribute("cuitProveedor") ?? "").Trim();
                if (cuitArchivo != "" && ObjProveedor.NormalizarCUIT(cuitArchivo) != ObjProveedor.NormalizarCUIT(proveedor.CUIT))
                {
                    mensaje = Mensajes486LP.Obtener("Importacion.ProveedorNoCoincide", cuitArchivo, proveedor.CUIT); // Muestra: El CUIT del archivo ({0}) no coincide con el del proveedor elegido ({1})
                    return false;
                }

                List<XElement> items = raiz.Elements("item").ToList();
                if (items.Count == 0)
                {
                    mensaje = Mensajes486LP.Obtener("Importacion.EstructuraInvalida"); // Muestra: El archivo no tiene la estructura esperada: debe ser un remito con al menos una línea item
                    return false;
                }
                if (items.Count > LINEAS_MAXIMO)
                {
                    mensaje = Mensajes486LP.Obtener("Importacion.DemasiadasLineas", LINEAS_MAXIMO); // Muestra: El archivo supera el máximo de {0} líneas por remito
                    return false;
                }

                int[] numeros;
                if (!NumerarLineas(items, out numeros))
                {
                    mensaje = Mensajes486LP.Obtener("Importacion.NumeracionInvalida"); // Muestra: La numeración de las líneas (atributo linea) está incompleta, repetida o no es válida
                    return false;
                }

                // Un remito ya importado completo no se puede volver a subir
                ImportacionProducto486LP remito = ObjetoMapper.ObtenerPorRemito(idProveedor, nroRemito);
                if (remito != null && remito.Estado == ImportacionProducto486LP.ESTADO_COMPLETO)
                {
                    mensaje = Mensajes486LP.Obtener("Importacion.RemitoYaImportado", nroRemito, proveedor.Nombre, remito.FechaImportacion.ToString("dd/MM/yyyy HH:mm")); // Muestra: El remito {0} de {1} ya fue importado completo el {2}. No se importó nada
                    return false;
                }

                string nombreArchivo = Path.GetFileName(rutaArchivo);

                // Lineas que el remito ya tiene registradas de pasadas anteriores
                List<DetalleImportacion486LP> previas = remito == null ? new List<DetalleImportacion486LP>() : ObjetoMapper.ListarDetalle(remito.IdImportacion);
                Dictionary<int, DetalleImportacion486LP> importadas = previas
                    .Where(d => d.Accion != DetalleImportacion486LP.ACCION_RECHAZADA)
                    .ToDictionary(d => d.Linea);

                // Producto -> numero de linea que ya lo trae (en pasadas anteriores o en esta). Sirve para rechazar el producto repetido dentro del mismo remito
                Dictionary<string, int> lineaDeProducto = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
                foreach (DetalleImportacion486LP d in importadas.Values)
                    lineaDeProducto[ClaveProducto(d.Marca, d.Modelo, d.Color, d.Talle)] = d.Linea;

                Dictionary<string, Marca486LP> marcas = ObjMarca.Listar()
                    .GroupBy(m => m.Nombre.Trim(), StringComparer.OrdinalIgnoreCase)
                    .ToDictionary(g => g.Key, g => g.First(), StringComparer.OrdinalIgnoreCase);
                Dictionary<string, Categoria486LP> categorias = ObjCategoria.Listar()
                    .GroupBy(c => c.Nombre.Trim(), StringComparer.OrdinalIgnoreCase)
                    .ToDictionary(g => g.Key, g => g.First(), StringComparer.OrdinalIgnoreCase);

                int idImportacion = remito != null
                    ? remito.IdImportacion
                    : ObjetoMapper.Crear(idProveedor, nroRemito, UsuarioNombre(), nombreArchivo);

                resultado = new ResultadoImportacion486LP
                {
                    IdImportacion = idImportacion,
                    IdProveedor = idProveedor,
                    NroRemito = nroRemito,
                    CUITProveedor = proveedor.CUIT,
                    NombreArchivo = nombreArchivo
                };

                for (int i = 0; i < items.Count; i++)
                {
                    LineaImportacion486LP linea = new LineaImportacion486LP
                    {
                        Linea = numeros[i],
                        Marca = Texto(items[i], "marca"),
                        Categoria = Texto(items[i], "categoria"),
                        Modelo = Texto(items[i], "modelo"),
                        Color = Texto(items[i], "color"),
                        Talle = Texto(items[i], "talle"),
                        Cantidad = Texto(items[i], "cantidad"),
                        Precio = Texto(items[i], "precio"),
                        Exportable = true
                    };
                    resultado.Lineas.Add(linea);

                    try
                    {
                        ProcesarItem(idImportacion, linea, importadas, lineaDeProducto, marcas, categorias);
                    }
                    catch (Exception ex)
                    {
                        // Una linea que falla no corta las demas: queda rechazada con el motivo general
                        ObjBitacora.Registrar(new BitacoraEvento486LP(MODULO, $"Error en BLL_ImportacionProducto.Importar() linea {linea.Linea} del remito {nroRemito}: {ex.Message}", Criticidad486LP.MuyAlta, "Sistema", "Sistema"));
                        linea.Resultado = LineaImportacion486LP.RESULTADO_RECHAZADA;
                        linea.Detalle = Mensajes486LP.Obtener("Importacion.Linea.ErrorInesperado"); // Muestra: Ocurrió un error inesperado al procesar la línea. El detalle quedó registrado en la bitácora
                        linea.Exportable = true;
                        GuardarRechazo(idImportacion, linea);
                    }
                }

                resultado.Altas = resultado.Lineas.Count(l => l.Resultado == LineaImportacion486LP.RESULTADO_ALTA);
                resultado.Sumas = resultado.Lineas.Count(l => l.Resultado == LineaImportacion486LP.RESULTADO_SUMA_STOCK);
                resultado.Rechazadas = resultado.Lineas.Count(l => l.Resultado == LineaImportacion486LP.RESULTADO_RECHAZADA);
                resultado.YaImportadas = resultado.Lineas.Count(l => l.Resultado == LineaImportacion486LP.RESULTADO_YA_IMPORTADA);

                resultado.EstadoRemito = ObjetoMapper.Cerrar(idImportacion, UsuarioNombre(), nombreArchivo);

                if (resultado.Altas + resultado.Sumas > 0) RecalcularDV("Producto");
                RecalcularDV("ImportacionProducto");
                RecalcularDV("DetalleImportacion");

                ObjBitacora.Registrar(new BitacoraEvento486LP(MODULO,
                    $"Importacion de productos desde XML: proveedor Id {idProveedor}, remito {nroRemito}, archivo '{nombreArchivo}'. Altas: {resultado.Altas}, stock sumado: {resultado.Sumas}, rechazadas: {resultado.Rechazadas}, ya importadas: {resultado.YaImportadas}. Remito {resultado.EstadoRemito}",
                    Criticidad486LP.Media, UsuarioDNI(), UsuarioNombre()));

                mensaje = Mensajes486LP.Obtener("Importacion.Resumen", resultado.Altas, resultado.Sumas, resultado.Rechazadas, resultado.YaImportadas); // Muestra: Importación terminada. Productos nuevos: {0}. Stock sumado: {1}. Líneas rechazadas: {2}. Ya importadas antes: {3}
                return true;
            }
            catch (Exception ex)
            {
                ObjBitacora.Registrar(new BitacoraEvento486LP(MODULO, $"Error en BLL_ImportacionProducto.Importar(): {ex.Message}", Criticidad486LP.MuyAlta, "Sistema", "Sistema"));
                mensaje = Mensajes486LP.Obtener("General.ErrorInesperado"); // Muestra: Ocurrió un error inesperado. El detalle quedó registrado en la bitácora
                return false;
            }
        }

        // Genera un XML con solo las lineas rechazadas (mismo remito, mismo proveedor y mismo numero de linea) para corregirlas y volver a subirlas
        public bool ExportarRechazadas(ResultadoImportacion486LP resultado, string rutaDestino, out string mensaje)
        {
            mensaje = "";

            try
            {
                List<LineaImportacion486LP> rechazadas = resultado == null
                    ? new List<LineaImportacion486LP>()
                    : resultado.Lineas.Where(l => l.Resultado == LineaImportacion486LP.RESULTADO_RECHAZADA && l.Exportable).ToList();

                if (rechazadas.Count == 0)
                {
                    mensaje = Mensajes486LP.Obtener("Importacion.SinRechazadasParaExportar"); // Muestra: No hay líneas rechazadas para exportar
                    return false;
                }

                XElement raiz = new XElement("remito", new XAttribute("numero", resultado.NroRemito));
                if (!string.IsNullOrEmpty(resultado.CUITProveedor))
                    raiz.Add(new XAttribute("cuitProveedor", resultado.CUITProveedor));

                foreach (LineaImportacion486LP l in rechazadas)
                {
                    raiz.Add(new XElement("item",
                        new XAttribute("linea", l.Linea),
                        new XElement("marca", l.Marca),
                        new XElement("categoria", l.Categoria),
                        new XElement("modelo", l.Modelo),
                        new XElement("color", l.Color),
                        new XElement("talle", l.Talle),
                        new XElement("cantidad", l.Cantidad),
                        new XElement("precio", l.Precio)));
                }

                new XDocument(new XDeclaration("1.0", "utf-8", null), raiz).Save(rutaDestino);

                ObjBitacora.Registrar(new BitacoraEvento486LP(MODULO,
                    $"Exportacion de lineas rechazadas: remito {resultado.NroRemito}, {rechazadas.Count} lineas",
                    Criticidad486LP.Media, UsuarioDNI(), UsuarioNombre()));

                mensaje = Mensajes486LP.Obtener("Importacion.Exportado", rechazadas.Count); // Muestra: Se exportaron {0} líneas rechazadas. Corríjalas en el archivo y vuelva a importarlo
                return true;
            }
            catch (Exception ex)
            {
                ObjBitacora.Registrar(new BitacoraEvento486LP(MODULO, $"Error en BLL_ImportacionProducto.ExportarRechazadas(): {ex.Message}", Criticidad486LP.MuyAlta, "Sistema", "Sistema"));
                mensaje = Mensajes486LP.Obtener("Importacion.ErrorExportar"); // Muestra: No se pudo guardar el archivo. Verifique la ubicación y vuelva a intentar
                return false;
            }
        }

        // ----- Lectura del archivo -----

        // Lee el XML sin permitir DTD ni entidades externas
        private bool LeerXml(string ruta, out XDocument doc, out string mensaje)
        {
            doc = null;
            mensaje = "";

            try
            {
                XmlReaderSettings configuracion = new XmlReaderSettings
                {
                    DtdProcessing = DtdProcessing.Prohibit,
                    XmlResolver = null,
                    IgnoreComments = true
                };

                using (XmlReader lector = XmlReader.Create(ruta, configuracion))
                {
                    doc = XDocument.Load(lector);
                }
                return true;
            }
            catch (XmlException ex)
            {
                mensaje = Mensajes486LP.Obtener("Importacion.XmlMalFormado", ex.LineNumber, ex.LinePosition); // Muestra: El archivo no es un XML válido (error en la línea {0}, posición {1})
                return false;
            }
            catch (IOException)
            {
                mensaje = Mensajes486LP.Obtener("Importacion.ArchivoNoLegible"); // Muestra: No se pudo leer el archivo. Verifique que no esté abierto en otro programa
                return false;
            }
            catch (UnauthorizedAccessException)
            {
                mensaje = Mensajes486LP.Obtener("Importacion.ArchivoNoLegible"); // Muestra: No se pudo leer el archivo. Verifique que no esté abierto en otro programa
                return false;
            }
        }

        // Numero de cada linea: el atributo linea si todas lo traen (es lo que pasa con el XML de rechazadas), o la posicion en el archivo si ninguna lo trae.
        // Mezclar las dos formas, repetir un numero o poner uno invalido rechaza el archivo completo.
        private bool NumerarLineas(List<XElement> items, out int[] numeros)
        {
            numeros = new int[items.Count];
            int conAtributo = items.Count(i => i.Attribute("linea") != null);

            if (conAtributo == 0)
            {
                for (int i = 0; i < items.Count; i++) numeros[i] = i + 1;
                return true;
            }

            if (conAtributo != items.Count) return false;

            HashSet<int> usados = new HashSet<int>();
            for (int i = 0; i < items.Count; i++)
            {
                int numero;
                string texto = ((string)items[i].Attribute("linea")).Trim();
                if (!int.TryParse(texto, NumberStyles.None, CultureInfo.InvariantCulture, out numero) ||
                    numero < 1 || numero > NUMERO_LINEA_MAXIMO || !usados.Add(numero))
                    return false;
                numeros[i] = numero;
            }
            return true;
        }

        private string Texto(XElement item, string nombre)
        {
            return ((string)item.Element(nombre) ?? "").Trim();
        }

        // ----- Procesamiento de una linea -----

        private void ProcesarItem(int idImportacion, LineaImportacion486LP linea, Dictionary<int, DetalleImportacion486LP> importadas,
            Dictionary<string, int> lineaDeProducto, Dictionary<string, Marca486LP> marcas, Dictionary<string, Categoria486LP> categorias)
        {
            // Esta linea ya se importo en una pasada anterior: no se vuelve a tocar el stock
            DetalleImportacion486LP previa;
            if (importadas.TryGetValue(linea.Linea, out previa))
            {
                linea.Exportable = false;
                if (MismoContenido(linea, previa))
                {
                    linea.Resultado = LineaImportacion486LP.RESULTADO_YA_IMPORTADA;
                    linea.Detalle = Mensajes486LP.Obtener("Importacion.Linea.YaImportada"); // Muestra: Ya se importó antes: se omite y no se vuelve a sumar el stock
                }
                else
                {
                    linea.Resultado = LineaImportacion486LP.RESULTADO_RECHAZADA;
                    linea.Detalle = Mensajes486LP.Obtener("Importacion.Linea.ContenidoDistinto", Describir(previa)); // Muestra: Ese número de línea ya se importó con otros datos ({0}) y no se modificó
                }
                return;
            }

            LineaValidada v;
            string motivo = ValidarLinea(linea, marcas, categorias, lineaDeProducto, out v);
            if (motivo != null)
            {
                linea.Resultado = LineaImportacion486LP.RESULTADO_RECHAZADA;
                linea.Detalle = motivo;
                GuardarRechazo(idImportacion, linea);
                return;
            }

            int idProducto;
            string accion = ObjetoMapper.ProcesarLinea(idImportacion, linea.Linea, v.IdMarca, v.IdCategoria, v.Marca, v.Categoria, v.Modelo, v.Color, v.Talle, v.Cantidad, v.Precio, out idProducto);

            lineaDeProducto[ClaveProducto(v.Marca, v.Modelo, v.Color, v.Talle)] = linea.Linea;

            if (accion == DetalleImportacion486LP.ACCION_ALTA)
            {
                linea.Resultado = LineaImportacion486LP.RESULTADO_ALTA;
                linea.Detalle = Mensajes486LP.Obtener("Importacion.Linea.AltaOk", v.Cantidad); // Muestra: Producto nuevo registrado con stock {0}
            }
            else if (accion == DetalleImportacion486LP.ACCION_SUMA_STOCK)
            {
                linea.Resultado = LineaImportacion486LP.RESULTADO_SUMA_STOCK;
                linea.Detalle = Mensajes486LP.Obtener("Importacion.Linea.SumaOk", v.Cantidad); // Muestra: Se sumaron {0} unidades al stock. El precio no se modificó
            }
            else
            {
                // Otra importacion de este mismo remito la registro justo antes (mismo resultado que si ya estuviera importada)
                linea.Resultado = LineaImportacion486LP.RESULTADO_YA_IMPORTADA;
                linea.Detalle = Mensajes486LP.Obtener("Importacion.Linea.YaImportada"); // Muestra: Ya se importó antes: se omite y no se vuelve a sumar el stock
                linea.Exportable = false;
            }
        }

        // Devuelve el motivo del rechazo, o null si la linea es valida. Cada motivo dice cual es el dato y que esta mal
        private string ValidarLinea(LineaImportacion486LP linea, Dictionary<string, Marca486LP> marcas, Dictionary<string, Categoria486LP> categorias,
            Dictionary<string, int> lineaDeProducto, out LineaValidada v)
        {
            v = null;

            List<string> faltantes = new List<string>();
            if (linea.Marca == "") faltantes.Add("marca");
            if (linea.Categoria == "") faltantes.Add("categoria");
            if (linea.Modelo == "") faltantes.Add("modelo");
            if (linea.Color == "") faltantes.Add("color");
            if (linea.Talle == "") faltantes.Add("talle");
            if (linea.Cantidad == "") faltantes.Add("cantidad");
            if (linea.Precio == "") faltantes.Add("precio");
            if (faltantes.Count > 0)
                return Mensajes486LP.Obtener("Importacion.Linea.FaltanDatos", string.Join(", ", faltantes)); // Muestra: Faltan datos obligatorios: {0}

            Marca486LP marca;
            if (!marcas.TryGetValue(linea.Marca, out marca))
                return Mensajes486LP.Obtener("Importacion.Linea.MarcaInexistente", linea.Marca); // Muestra: La marca '{0}' no existe. Cárguela primero en Procesar categorías y marcas

            Categoria486LP categoria;
            if (!categorias.TryGetValue(linea.Categoria, out categoria))
                return Mensajes486LP.Obtener("Importacion.Linea.CategoriaInexistente", linea.Categoria); // Muestra: La categoría '{0}' no existe. Cárguela primero en Procesar categorías y marcas

            if (!ObjProducto.ValidarModelo(linea.Modelo))
                return Mensajes486LP.Obtener("Importacion.Linea.ModeloInvalido", linea.Modelo); // Muestra: El modelo '{0}' no es válido: solo admite letras, números, espacios y los signos . & ' - / + ( ) (máximo 50 caracteres)

            if (!ObjProducto.ValidarColor(linea.Color))
                return Mensajes486LP.Obtener("Importacion.Linea.ColorInvalido", linea.Color); // Muestra: El color '{0}' no es válido: solo admite letras, y puede combinarse con espacio, guion o barra (máximo 30 caracteres)

            if (!ObjProducto.ValidarTalle(linea.Talle))
                return Mensajes486LP.Obtener("Importacion.Linea.TalleInvalido", linea.Talle); // Muestra: El talle '{0}' no es válido: debe ser un número entre 30 y 50, con medio talle opcional (por ejemplo 42 o 42.5)

            decimal precio;
            if (!decimal.TryParse(linea.Precio.Replace(',', '.'), NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out precio) || !ObjProducto.ValidarPrecio(precio))
                return Mensajes486LP.Obtener("Importacion.Linea.PrecioInvalido", linea.Precio); // Muestra: El precio '{0}' no es válido: debe ser mayor a cero, con hasta 2 decimales, y no puede superar 99.999.999,99

            int cantidad;
            if (!int.TryParse(linea.Cantidad, NumberStyles.None, CultureInfo.InvariantCulture, out cantidad) || cantidad < 1 || cantidad > CANTIDAD_MAXIMA)
                return Mensajes486LP.Obtener("Importacion.Linea.CantidadInvalida", linea.Cantidad, CANTIDAD_MAXIMA); // Muestra: La cantidad '{0}' no es válida: debe ser un número entero entre 1 y {1}

            string talle = linea.Talle.Replace(',', '.');
            int lineaRepetida;
            if (lineaDeProducto.TryGetValue(ClaveProducto(marca.Nombre, linea.Modelo, linea.Color, talle), out lineaRepetida))
                return Mensajes486LP.Obtener("Importacion.Linea.ProductoRepetido", lineaRepetida); // Muestra: Repite el mismo producto (marca, modelo, color y talle) de la línea {0} de este remito

            v = new LineaValidada
            {
                IdMarca = marca.IdMarca,
                Marca = marca.Nombre,
                IdCategoria = categoria.IdCategoria,
                Categoria = categoria.Nombre,
                Modelo = linea.Modelo,
                Color = linea.Color,
                Talle = talle,
                Cantidad = cantidad,
                Precio = precio
            };
            return null;
        }

        // Deja registrada la linea rechazada (con su motivo) para poder corregirla y reenviarla despues
        private void GuardarRechazo(int idImportacion, LineaImportacion486LP linea)
        {
            try
            {
                ObjetoMapper.RegistrarRechazo(idImportacion, linea.Linea,
                    Recortar(linea.Marca, 100), Recortar(linea.Categoria, 100), Recortar(linea.Modelo, 100), Recortar(linea.Color, 100),
                    Recortar(linea.Talle, 30), Recortar(linea.Cantidad, 30), Recortar(linea.Precio, 30), Recortar(linea.Detalle, 500));
            }
            catch (Exception ex)
            {
                ObjBitacora.Registrar(new BitacoraEvento486LP(MODULO, $"Error al registrar la linea rechazada {linea.Linea}: {ex.Message}", Criticidad486LP.MuyAlta, "Sistema", "Sistema"));
            }
        }

        // ----- Auxiliares -----

        // Mismo producto, misma cantidad: es la misma linea que ya se importo
        private bool MismoContenido(LineaImportacion486LP linea, DetalleImportacion486LP previa)
        {
            int cantidadNueva, cantidadPrevia;
            if (!int.TryParse(linea.Cantidad, NumberStyles.None, CultureInfo.InvariantCulture, out cantidadNueva) ||
                !int.TryParse(previa.Cantidad, NumberStyles.None, CultureInfo.InvariantCulture, out cantidadPrevia))
                return false;

            return cantidadNueva == cantidadPrevia &&
                   ClaveProducto(linea.Marca, linea.Modelo, linea.Color, linea.Talle) == ClaveProducto(previa.Marca, previa.Modelo, previa.Color, previa.Talle);
        }

        private string ClaveProducto(string marca, string modelo, string color, string talle)
        {
            return string.Join("|", (marca ?? "").Trim(), (modelo ?? "").Trim(), (color ?? "").Trim(), (talle ?? "").Trim().Replace(',', '.')).ToLowerInvariant();
        }

        private string Describir(DetalleImportacion486LP d)
        {
            return $"{d.Marca} {d.Modelo} {d.Color} {d.Talle} x{d.Cantidad}";
        }

        private string Recortar(string texto, int largo)
        {
            texto = texto ?? "";
            return texto.Length > largo ? texto.Substring(0, largo) : texto;
        }

        private void RecalcularDV(string tabla)
        {
            string mensajeDV;
            ObjDV.RecalcularDV(tabla, out mensajeDV);
        }

        private string UsuarioDNI()
        {
            return SessionManager486LP.ObtenerInstancia().UsuarioActual()?.DNI ?? "Sistema";
        }

        private string UsuarioNombre()
        {
            return SessionManager486LP.ObtenerInstancia().UsuarioActual()?.NombreUsuario ?? "Sistema";
        }
    }
}

using BE;
using DAL;
using Mappers;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BLL
{
    public class BLL_Producto486LP
    {
        private Mapper_Producto486LP ObjetoMapper = new Mapper_Producto486LP();
        private BLL_Bitacora486LP ObjBitacora = new BLL_Bitacora486LP();
        private BLL_DV486LP ObjDV = new BLL_DV486LP();

        private const string MODULO_MAESTROS = "Maestros";

        public List<Producto486LP> Listar()
        {
            try
            {
                return ObjetoMapper.Listar();
            }
            catch (Exception ex)
            {
                ObjBitacora.Registrar(new BitacoraEvento486LP("Ventas", $"Error en BLL_Producto.Listar(): {ex.Message}", Criticidad486LP.MuyAlta, "Sistema", "Sistema"));
                return new List<Producto486LP>();
            }
        }

        public List<Producto486LP> Buscar(string marca, string modelo, string color, string talle, bool soloConStock)
        {
            try
            {
                return ObjetoMapper.Buscar(marca, modelo, color, talle, soloConStock);
            }
            catch (Exception ex)
            {
                ObjBitacora.Registrar(new BitacoraEvento486LP("Ventas", $"Error en BLL_Producto.Buscar(): {ex.Message}", Criticidad486LP.MuyAlta, "Sistema", "Sistema"));
                return new List<Producto486LP>();
            }
        }

        public Producto486LP ObtenerProducto(string marca, string modelo, string color, string talle)
        {
            try
            {
                return ObjetoMapper.ObtenerPorAtributos(marca, modelo, color, talle);
            }
            catch (Exception ex)
            {
                ObjBitacora.Registrar(new BitacoraEvento486LP("Ventas", $"Error en BLL_Producto.ObtenerProducto(): {ex.Message}", Criticidad486LP.MuyAlta, "Sistema", "Sistema"));
                return null;
            }
        }

        public bool VerificarStock(int idProducto, int cantidad)
        {
            try
            {
                Producto486LP p = ObjetoMapper.ObtenerPorId(idProducto);
                return p != null && p.Stock >= cantidad;
            }
            catch (Exception ex)
            {
                ObjBitacora.Registrar(new BitacoraEvento486LP("Ventas", $"Error en BLL_Producto.VerificarStock(): {ex.Message}", Criticidad486LP.MuyAlta, "Sistema", "Sistema"));
                return false;
            }
        }

        // Stock actual del producto (0 si no existe o si falla la consulta)
        public int ObtenerStockDisponible(int idProducto)
        {
            try
            {
                Producto486LP p = ObjetoMapper.ObtenerPorId(idProducto);
                return p == null ? 0 : p.Stock;
            }
            catch (Exception ex)
            {
                ObjBitacora.Registrar(new BitacoraEvento486LP("Ventas", $"Error en BLL_Producto.ObtenerStockDisponible(): {ex.Message}", Criticidad486LP.MuyAlta, "Sistema", "Sistema"));
                return 0;
            }
        }

        public bool ActualizarStock(int idProducto, int cantidad, out string mensaje)
        {
            mensaje = "";
            try
            {
                if (cantidad <= 0)
                {
                    mensaje = Mensajes486LP.Obtener("Carrito.CantidadInvalida"); // Muestra: La cantidad debe ser mayor a cero.
                    return false;
                }

                int stockActual = ObtenerStockDisponible(idProducto);
                if (stockActual < cantidad)
                {
                    mensaje = Mensajes486LP.Obtener("Carrito.StockInsuficiente", stockActual); // Muestra: Stock insuficiente. Unidades disponibles: {0}.
                    return false;
                }

                string mensajeMapper;
                bool ok = ObjetoMapper.ActualizarStock(idProducto, cantidad, out mensajeMapper);

                if (!ok)
                {
                    ObjBitacora.Registrar(new BitacoraEvento486LP("Ventas", $"Error al descontar stock del producto Id {idProducto}: {mensajeMapper}", Criticidad486LP.MuyAlta, "Sistema", "Sistema"));
                    // Muestra: Ocurrió un error inesperado. El detalle quedó registrado en la bitácora.
                    mensaje = Mensajes486LP.Obtener("General.ErrorInesperado");
                }

                if (ok)
                {
                    string mensajeDV;
                    BLL_DV486LP bllDV = new BLL_DV486LP();
                    bllDV.RegistrarDVDeFilaNueva("Producto", idProducto, out mensajeDV);

                    ObjBitacora.Registrar(new BitacoraEvento486LP("Ventas", $"Stock actualizado: Producto Id {idProducto}, se descontaron {cantidad} unidades.", Criticidad486LP.Media,
                        SessionManager486LP.ObtenerInstancia().UsuarioActual()?.DNI ?? "Sistema",
                        SessionManager486LP.ObtenerInstancia().UsuarioActual()?.NombreUsuario ?? "Sistema"));
                }

                return ok;
            }
            catch (Exception ex)
            {
                // Muestra: Ocurrió un error inesperado. El detalle quedó registrado en la bitácora.
                mensaje = Mensajes486LP.Obtener("General.ErrorInesperado");
                ObjBitacora.Registrar(new BitacoraEvento486LP("Ventas", $"Error en BLL_Producto.ActualizarStock(): {ex.Message}", Criticidad486LP.MuyAlta, "Sistema", "Sistema"));
                return false;
            }
        }

        public Producto486LP ObtenerPorId(int idProducto)
        {
            try
            {
                return ObjetoMapper.ObtenerPorId(idProducto);
            }
            catch (Exception ex)
            {
                ObjBitacora.Registrar(new BitacoraEvento486LP("Ventas", $"Error en BLL_Producto.ObtenerPorId(): {ex.Message}", Criticidad486LP.MuyAlta, "Sistema", "Sistema"));
                return null;
            }
        }

        public bool AumentarStock(int idProducto, int cantidad, out string mensaje)
        {
            mensaje = "";
            try
            {
                if (cantidad <= 0)
                {
                    // Muestra: La cantidad debe ser mayor a cero.
                    mensaje = Mensajes486LP.Obtener("Carrito.CantidadInvalida");
                    return false;
                }

                string mensajeMapper;
                bool ok = ObjetoMapper.AumentarStock(idProducto, cantidad, out mensajeMapper);

                if (!ok)
                {
                    ObjBitacora.Registrar(new BitacoraEvento486LP("Compras", $"Error al sumar stock del producto Id {idProducto}: {mensajeMapper}", Criticidad486LP.MuyAlta, "Sistema", "Sistema"));
                    // Muestra: Ocurrió un error inesperado. El detalle quedó registrado en la
                    // bitácora.
                    mensaje = Mensajes486LP.Obtener("General.ErrorInesperado");
                }

                if (ok)
                {
                    string mensajeDV;
                    BLL_DV486LP bllDV = new BLL_DV486LP();
                    bllDV.RegistrarDVDeFilaNueva("Producto", idProducto, out mensajeDV);

                    ObjBitacora.Registrar(new BitacoraEvento486LP("Compras", $"Stock actualizado: Producto Id {idProducto}, se sumaron {cantidad} unidades.", Criticidad486LP.Media,
                        SessionManager486LP.ObtenerInstancia().UsuarioActual()?.DNI ?? "Sistema",
                        SessionManager486LP.ObtenerInstancia().UsuarioActual()?.NombreUsuario ?? "Sistema"));
                }

                return ok;
            }
            catch (Exception ex)
            {
                // Muestra: Ocurrió un error inesperado. El detalle quedó registrado en la bitácora.
                mensaje = Mensajes486LP.Obtener("General.ErrorInesperado");
                ObjBitacora.Registrar(new BitacoraEvento486LP("Compras", $"Error en BLL_Producto.AumentarStock(): {ex.Message}", Criticidad486LP.MuyAlta, "Sistema", "Sistema"));
                return false;
            }
        }

        public bool ValidarModelo(string modelo)
        {
            if (string.IsNullOrWhiteSpace(modelo)) return false;
            modelo = modelo.Trim();
            return modelo.Length <= 50 && Regex.IsMatch(modelo, @"^[\p{L}\d][\p{L}\d .&'\u2019\-/+()]*$");
        }

        public bool ValidarColor(string color)
        {
            if (string.IsNullOrWhiteSpace(color)) return false;
            color = color.Trim();
            return color.Length <= 30 && Regex.IsMatch(color, @"^\p{L}+(?:[ \-/]\p{L}+)*$");
        }

        public bool ValidarTalle(string talle)
        {
            if (string.IsNullOrWhiteSpace(talle)) return false;
            talle = talle.Trim();
            if (!Regex.IsMatch(talle, @"^\d{2}(?:[.,]5)?$")) return false;
            decimal valor = decimal.Parse(talle.Replace(',', '.'), System.Globalization.CultureInfo.InvariantCulture);
            return valor >= 30 && valor <= 50;
        }

        public bool ValidarPrecio(decimal precio)
        {
            return precio > 0 && precio <= 99999999.99m && decimal.Round(precio, 2) == precio;
        }

        private string ValidarDatos(Producto486LP producto, bool esAlta)
        {
            if (producto == null ||
                producto.IdMarca <= 0 ||
                producto.IdCategoria <= 0 ||
                string.IsNullOrWhiteSpace(producto.Modelo) ||
                string.IsNullOrWhiteSpace(producto.Color) ||
                string.IsNullOrWhiteSpace(producto.Talle))
                return Mensajes486LP.Obtener("Producto.CamposObligatorios"); // Muestra: Complete los campos obligatorios (marca, categoría, modelo, color, talle y precio)

            if (!ValidarModelo(producto.Modelo)) return Mensajes486LP.Obtener("Producto.ModeloInvalido"); // Muestra: El modelo solo admite letras, números, espacios y los signos . & ' - / + ( ) (máximo 50 caracteres)
            if (!ValidarColor(producto.Color)) return Mensajes486LP.Obtener("Producto.ColorInvalido"); // Muestra: El color solo admite letras, y puede combinarse con espacio, guion o barra (máximo 30 caracteres)
            if (!ValidarTalle(producto.Talle)) return Mensajes486LP.Obtener("Producto.TalleInvalido"); // Muestra: El talle debe ser un número entre 30 y 50, con medio talle opcional (por ejemplo 42 o 42.5)
            if (!ValidarPrecio(producto.Precio)) return Mensajes486LP.Obtener("Producto.PrecioInvalido"); // Muestra: El precio debe ser mayor a cero, con hasta 2 decimales, y no puede superar 99.999.999,99
            if (producto.Stock < 0) return Mensajes486LP.Obtener("Producto.StockInvalido"); // Muestra: El stock debe ser un número entero mayor o igual a cero

            return null;
        }

        // Deja los datos listos para guardar: sin espacios de mas y el medio talle siempre con punto
        private void Normalizar(Producto486LP producto)
        {
            producto.Modelo = producto.Modelo.Trim();
            producto.Color = producto.Color.Trim();
            producto.Talle = producto.Talle.Trim().Replace(',', '.');
        }

        public bool Agregar(Producto486LP producto, out string mensaje)
        {
            mensaje = "";

            string error = ValidarDatos(producto, true);
            if (error != null)
            {
                mensaje = error;
                return false;
            }

            try
            {
                Normalizar(producto);

                if (ObjetoMapper.ExisteCombinacion(producto.IdMarca, producto.Modelo, producto.Color, producto.Talle, 0))
                {
                    mensaje = Mensajes486LP.Obtener("Producto.DuplicadoAlta"); // Muestra: Ya existe un producto con esa marca, modelo, color y talle. Para cambiar su stock, elíjalo en la grilla y use Modificar
                    return false;
                }

                int idNuevo = ObjetoMapper.Agregar(producto);

                RecalcularDVProducto();
                ObjBitacora.Registrar(new BitacoraEvento486LP(MODULO_MAESTROS,
                    $"Alta de producto Id {idNuevo}: {producto.Modelo} {producto.Color} talle {producto.Talle}", Criticidad486LP.Media, UsuarioDNI(), UsuarioNombre()));

                mensaje = Mensajes486LP.Obtener("Producto.Agregado"); // Muestra: Producto registrado correctamente
                return true;
            }
            catch (Exception ex)
            {
                ObjBitacora.Registrar(new BitacoraEvento486LP(MODULO_MAESTROS, $"Error en BLL_Producto.Agregar(): {ex.Message}", Criticidad486LP.MuyAlta, "Sistema", "Sistema"));
                mensaje = Mensajes486LP.Obtener("General.ErrorInesperado"); // Muestra: Ocurrió un error inesperado. El detalle quedó registrado en la bitácora
                return false;
            }
        }

        public bool Modificar(Producto486LP producto, out string mensaje)
        {
            mensaje = "";

            string error = ValidarDatos(producto, false);
            if (error != null)
            {
                mensaje = error;
                return false;
            }

            try
            {
                Normalizar(producto);

                if (ObjetoMapper.ExisteCombinacion(producto.IdMarca, producto.Modelo, producto.Color, producto.Talle, producto.IdProducto))
                {
                    mensaje = Mensajes486LP.Obtener("Producto.Duplicado"); // Muestra: Ya existe un producto con esa marca, modelo, color y talle
                    return false;
                }

                // Se toma el producto antes de modificar para dejar en la bitacora el cambio de stock (de X a Y)
                Producto486LP anterior = ObjetoMapper.ObtenerPorId(producto.IdProducto);
                if (anterior == null)
                {
                    mensaje = Mensajes486LP.Obtener("Producto.NoExiste"); // Muestra: El producto ya no existe. Actualice la lista y vuelva a intentar
                    return false;
                }

                if (ObjetoMapper.Modificar(producto) == 0)
                {
                    mensaje = Mensajes486LP.Obtener("Producto.NoExiste"); // Muestra: El producto ya no existe. Actualice la lista y vuelva a intentar
                    return false;
                }

                RecalcularDVProducto();

                string detalleStock = anterior.Stock != producto.Stock ? $", stock de {anterior.Stock} a {producto.Stock}" : "";
                ObjBitacora.Registrar(new BitacoraEvento486LP(MODULO_MAESTROS,
                    $"Modificacion de producto Id {producto.IdProducto}: {producto.Modelo} {producto.Color} talle {producto.Talle}{detalleStock}", Criticidad486LP.Media, UsuarioDNI(), UsuarioNombre()));

                mensaje = Mensajes486LP.Obtener("Producto.Modificado"); // Muestra: Producto modificado correctamente
                return true;
            }
            catch (Exception ex)
            {
                ObjBitacora.Registrar(new BitacoraEvento486LP(MODULO_MAESTROS, $"Error en BLL_Producto.Modificar(): {ex.Message}", Criticidad486LP.MuyAlta, "Sistema", "Sistema"));
                mensaje = Mensajes486LP.Obtener("General.ErrorInesperado"); // Muestra: Ocurrió un error inesperado. El detalle quedó registrado en la bitácora
                return false;
            }
        }

        public bool Eliminar(int idProducto, out string mensaje)
        {
            mensaje = "";

            try
            {
                // Los datos se toman antes de borrar para dejarlos en la bitacora
                Producto486LP productoBaja = ObjetoMapper.ObtenerPorId(idProducto);
                string descripcionProducto = productoBaja == null ? "" : $": {productoBaja.Marca} {productoBaja.Modelo} {productoBaja.Color} talle {productoBaja.Talle}";

                if (ObjetoMapper.TieneMovimientos(idProducto))
                {
                    mensaje = Mensajes486LP.Obtener("Producto.TieneMovimientos"); // Muestra: No se puede eliminar el producto: tiene movimientos asociados (carritos, ventas o compras)
                    return false;
                }

                if (ObjetoMapper.Eliminar(idProducto) == 0)
                {
                    mensaje = Mensajes486LP.Obtener("Producto.NoExiste"); // Muestra: El producto ya no existe. Actualice la lista y vuelva a intentar
                    return false;
                }

                RecalcularDVProducto();
                ObjBitacora.Registrar(new BitacoraEvento486LP(MODULO_MAESTROS,
                    $"Baja de producto Id {idProducto}{descripcionProducto}", Criticidad486LP.Alta, UsuarioDNI(), UsuarioNombre()));

                mensaje = Mensajes486LP.Obtener("Producto.Eliminado"); // Muestra: Producto eliminado correctamente
                return true;
            }
            catch (Exception ex)
            {
                ObjBitacora.Registrar(new BitacoraEvento486LP(MODULO_MAESTROS, $"Error en BLL_Producto.Eliminar(): {ex.Message}", Criticidad486LP.MuyAlta, "Sistema", "Sistema"));
                mensaje = Mensajes486LP.Obtener("General.ErrorInesperado"); // Muestra: Ocurrió un error inesperado. El detalle quedó registrado en la bitácora
                return false;
            }
        }


        private const int RESTAURAR_OK = 0;
        private const int RESTAURAR_NO_EXISTE = 1;
        private const int RESTAURAR_YA_VIGENTE = 2;
        private const int RESTAURAR_MARCA_INEXISTENTE = 3;
        private const int RESTAURAR_CATEGORIA_INEXISTENTE = 4;
        private const int RESTAURAR_COMBINACION_REPETIDA = 5;

        public bool ValidarFechasHistorial(DateTime desde, DateTime hasta, out string mensaje)
        {
            mensaje = "";
            if (desde.Date > hasta.Date)
            {
                mensaje = Mensajes486LP.Obtener("ProductoCambio.FechasInvalidas"); // Muestra: La fecha desde no puede ser posterior a la fecha hasta.
                return false;
            }
            return true;
        }

        public List<ProductoCambio486LP> ListarCambios(string marca, string modelo, string color, string talle, DateTime? desde, DateTime? hasta)
        {
            try
            {
                return ObjetoMapper.ListarCambios(marca, modelo, color, talle, desde, hasta);
            }
            catch (Exception ex)
            {
                ObjBitacora.Registrar(new BitacoraEvento486LP(MODULO_MAESTROS, $"Error en BLL_Producto.ListarCambios(): {ex.Message}", Criticidad486LP.MuyAlta, "Sistema", "Sistema"));
                return new List<ProductoCambio486LP>();
            }
        }

        // La pantalla lo llama antes de pedir la confirmacion, asi no se le advierte al usuario de algo que despues no se puede hacer
        public bool ValidarRestauracion(ProductoCambio486LP cambio, out string mensaje)
        {
            mensaje = "";

            if (cambio == null || cambio.IdCambio <= 0)
            {
                mensaje = Mensajes486LP.Obtener("ProductoCambio.Seleccionar"); // Muestra: Seleccione una version de la grilla para restaurar.
                return false;
            }

            // La baja no cuenta como version vigente: restaurarla vuelve a crear el producto eliminado
            if (cambio.Vigente && cambio.Operacion != ProductoCambio486LP.OPERACION_BAJA)
            {
                mensaje = Mensajes486LP.Obtener("ProductoCambio.YaVigente"); // Muestra: La version seleccionada ya es la vigente de este producto.
                return false;
            }

            return true;
        }

        public bool RestaurarCambio(ProductoCambio486LP cambio, out string mensaje)
        {
            mensaje = "";

            try
            {
                if (!ValidarRestauracion(cambio, out mensaje)) return false;

                Producto486LP antes = ObjetoMapper.ObtenerPorId(cambio.IdProducto);

                int codigo = ObjetoMapper.RestaurarCambio(cambio.IdCambio);
                if (codigo != RESTAURAR_OK)
                {
                    mensaje = MensajeDeRestauracion(codigo);
                    return false;
                }

                string mensajeDV;
                ObjDV.RegistrarDVDeFilaNueva("Producto", cambio.IdProducto, out mensajeDV);

                Producto486LP despues = ObjetoMapper.ObtenerPorId(cambio.IdProducto);
                string detalle = antes == null || despues == null
                    ? "el producto estaba eliminado y se volvio a crear"
                    : $"stock {antes.Stock} a {despues.Stock}, precio {antes.Precio:0.00} a {despues.Precio:0.00}";

                ObjBitacora.Registrar(new BitacoraEvento486LP(MODULO_MAESTROS,
                    $"Restauracion de producto Id {cambio.IdProducto}: {cambio} a la version del {cambio.FechaHora:dd/MM/yyyy HH:mm:ss} ({detalle})",
                    Criticidad486LP.Alta, UsuarioDNI(), UsuarioNombre()));

                mensaje = Mensajes486LP.Obtener("ProductoCambio.Restaurado"); // Muestra: El producto fue restaurado a la version seleccionada.
                return true;
            }
            catch (Exception ex)
            {
                ObjBitacora.Registrar(new BitacoraEvento486LP(MODULO_MAESTROS, $"Error en BLL_Producto.RestaurarCambio(): {ex.Message}", Criticidad486LP.MuyAlta, "Sistema", "Sistema"));
                mensaje = Mensajes486LP.Obtener("General.ErrorInesperado"); // Muestra: Ocurrio un error inesperado. El detalle quedo registrado en la bitacora
                return false;
            }
        }

        private string MensajeDeRestauracion(int codigo)
        {
            switch (codigo)
            {
                case RESTAURAR_NO_EXISTE: return Mensajes486LP.Obtener("ProductoCambio.NoExiste"); // Muestra: La version seleccionada ya no existe. Actualice la lista y vuelva a intentar.
                case RESTAURAR_YA_VIGENTE: return Mensajes486LP.Obtener("ProductoCambio.YaVigente"); // Muestra: La version seleccionada ya es la vigente de este producto.
                case RESTAURAR_MARCA_INEXISTENTE: return Mensajes486LP.Obtener("ProductoCambio.MarcaInexistente"); // Muestra: No se puede restaurar: la marca de esa version ya no existe.
                case RESTAURAR_CATEGORIA_INEXISTENTE: return Mensajes486LP.Obtener("ProductoCambio.CategoriaInexistente"); // Muestra: No se puede restaurar: la categoria de esa version ya no existe.
                case RESTAURAR_COMBINACION_REPETIDA: return Mensajes486LP.Obtener("ProductoCambio.CombinacionRepetida"); // Muestra: No se puede restaurar: ya existe otro producto con la misma marca, modelo, color y talle.
                default: return Mensajes486LP.Obtener("General.ErrorInesperado");
            }
        }

        private void RecalcularDVProducto()
        {
            string mensajeDV;
            ObjDV.RecalcularDV("Producto", out mensajeDV);
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

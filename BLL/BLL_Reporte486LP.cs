using BE;
using Mappers;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class BLL_Reporte486LP
    {
        private Mapper_Reporte486LP ObjetoMapper = new Mapper_Reporte486LP();
        private BLL_Bitacora486LP ObjBitacora = new BLL_Bitacora486LP();

        private const string MODULO = "Ventas";

        // Cantidad de clientes que se muestran en el grafico de barras
        public const int CANTIDAD_GRAFICO = 5;

        // La fecha "desde" no puede ser posterior a la fecha "hasta"
        public bool ValidarRangoFechas(DateTime desde, DateTime hasta)
        {
            return desde.Date <= hasta.Date;
        }

        // Minimo de compras: entero de 1 en adelante
        public bool ValidarMinimoCompras(int minCompras)
        {
            return minCompras >= 1;
        }

        // Devuelve los clientes frecuentes del periodo, o null si hubo un error (en ese caso queda registrado en la bitacora).
        public List<ReporteClienteFrecuente486LP> ClientesFrecuentes(DateTime desde, DateTime hasta, int minCompras)
        {
            try
            {
                if (!ValidarRangoFechas(desde, hasta) || !ValidarMinimoCompras(minCompras))
                    return new List<ReporteClienteFrecuente486LP>();

                return ObjetoMapper.ClientesFrecuentes(desde, hasta, minCompras);
            }
            catch (Exception ex)
            {
                ObjBitacora.Registrar(new BitacoraEvento486LP(MODULO, $"Error en BLL_Reporte.ClientesFrecuentes(): {ex.Message}", Criticidad486LP.MuyAlta, "Sistema", "Sistema"));
                return null;
            }
        }

        // Los clientes que van al grafico: los que mas gastaron (la lista ya viene ordenada, igual se reordena por las dudas)
        public List<ReporteClienteFrecuente486LP> TopPorMonto(List<ReporteClienteFrecuente486LP> clientes)
        {
            if (clientes == null) return new List<ReporteClienteFrecuente486LP>();
            return clientes.OrderByDescending(c => c.TotalGastado).Take(CANTIDAD_GRAFICO).ToList();
        }

        // Monto total gastado por todos los clientes del listado (para el resumen)
        public decimal TotalGeneral(List<ReporteClienteFrecuente486LP> clientes)
        {
            if (clientes == null) return 0m;
            return clientes.Sum(c => c.TotalGastado);
        }

        // Deja constancia en la bitacora de que se exporto el reporte a PDF
        public void RegistrarExportacionClientesFrecuentes(int registros)
        {
            try
            {
                var usuario = SessionManager486LP.ObtenerInstancia().UsuarioActual();
                ObjBitacora.Registrar(new BitacoraEvento486LP(MODULO,
                    $"Reporte 'Clientes frecuentes' exportado a PDF ({registros} clientes).",
                    Criticidad486LP.Baja,
                    usuario?.DNI ?? "Sistema",
                    usuario?.NombreUsuario ?? "Sistema"));
            }
            catch (Exception ex)
            {
                ObjBitacora.Registrar(new BitacoraEvento486LP(MODULO, $"Error en BLL_Reporte.RegistrarExportacionClientesFrecuentes(): {ex.Message}", Criticidad486LP.MuyAlta, "Sistema", "Sistema"));
            }
        }

        // ===================== Reporte "Estado de stock y reposicion" =====================

        // Hasta esta cantidad (inclusive) un producto se considera con "stock bajo" (0 = sin stock; mas que esto = normal)
        public const int UMBRAL_STOCK_BAJO = 5;

        // El estado a filtrar tiene que ser uno de los 3 definidos (null = todos)
        public bool ValidarEstado(int? estado)
        {
            return !estado.HasValue || estado.Value == ReporteStockProducto486LP.SIN_STOCK
                || estado.Value == ReporteStockProducto486LP.STOCK_BAJO || estado.Value == ReporteStockProducto486LP.NORMAL;
        }

        // Devuelve los productos segun los filtros, o null si hubo un error (en ese caso queda registrado en la bitacora).
        public List<ReporteStockProducto486LP> EstadoStock(string marca, string talle, int? estado, bool soloReponer)
        {
            try
            {
                if (!ValidarEstado(estado))
                    return new List<ReporteStockProducto486LP>();

                return ObjetoMapper.EstadoStock(marca, talle, estado, soloReponer, UMBRAL_STOCK_BAJO);
            }
            catch (Exception ex)
            {
                ObjBitacora.Registrar(new BitacoraEvento486LP("Compras", $"Error en BLL_Reporte.EstadoStock(): {ex.Message}", Criticidad486LP.MuyAlta, "Sistema", "Sistema"));
                return null;
            }
        }

        // Marcas distintas de la lista (ordenadas) para llenar el filtro
        public List<string> MarcasDe(List<ReporteStockProducto486LP> productos)
        {
            if (productos == null) return new List<string>();
            return productos.Select(p => p.Marca).Distinct().OrderBy(m => m).ToList();
        }

        // Talles distintos de la lista para llenar el filtro: primero los numericos de menor a mayor y despues los que no son numeros
        public List<string> TallesDe(List<ReporteStockProducto486LP> productos)
        {
            if (productos == null) return new List<string>();
            return productos.Select(p => p.Talle).Distinct()
                .OrderBy(t => { double n; return double.TryParse(t.Replace(',', '.'), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out n) ? n : double.MaxValue; })
                .ThenBy(t => t).ToList();
        }

        // Cantidad de productos de cada estado: [0] sin stock, [1] stock bajo, [2] normal (es lo que muestra el grafico)
        public int[] ContarPorEstado(List<ReporteStockProducto486LP> productos)
        {
            int[] cantidades = new int[3];
            if (productos == null) return cantidades;
            foreach (ReporteStockProducto486LP p in productos)
            {
                if (p.Estado >= 0 && p.Estado <= 2) cantidades[p.Estado]++;
            }
            return cantidades;
        }

        public int TotalUnidades(List<ReporteStockProducto486LP> productos)
        {
            return productos == null ? 0 : productos.Sum(p => p.Stock);
        }

        public decimal ValorTotalStock(List<ReporteStockProducto486LP> productos)
        {
            return productos == null ? 0m : productos.Sum(p => p.ValorStock);
        }

        // Deja constancia en la bitacora de que se exporto el reporte a PDF
        public void RegistrarExportacionEstadoStock(int registros)
        {
            try
            {
                var usuario = SessionManager486LP.ObtenerInstancia().UsuarioActual();
                ObjBitacora.Registrar(new BitacoraEvento486LP("Compras",
                    $"Reporte 'Estado de stock' exportado a PDF ({registros} productos).",
                    Criticidad486LP.Baja,
                    usuario?.DNI ?? "Sistema",
                    usuario?.NombreUsuario ?? "Sistema"));
            }
            catch (Exception ex)
            {
                ObjBitacora.Registrar(new BitacoraEvento486LP("Compras", $"Error en BLL_Reporte.RegistrarExportacionEstadoStock(): {ex.Message}", Criticidad486LP.MuyAlta, "Sistema", "Sistema"));
            }
        }
    }
}

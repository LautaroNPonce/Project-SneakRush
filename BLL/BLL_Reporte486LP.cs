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

        public const int CANTIDAD_GRAFICO = 5;

        public bool ValidarRangoFechas(DateTime desde, DateTime hasta)
        {
            return desde.Date <= hasta.Date;
        }

        public bool ValidarMinimoCompras(int minCompras)
        {
            return minCompras >= 1;
        }

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

        public List<ReporteClienteFrecuente486LP> TopPorMonto(List<ReporteClienteFrecuente486LP> clientes)
        {
            if (clientes == null) return new List<ReporteClienteFrecuente486LP>();
            return clientes.OrderByDescending(c => c.TotalGastado).Take(CANTIDAD_GRAFICO).ToList();
        }

        public decimal TotalGeneral(List<ReporteClienteFrecuente486LP> clientes)
        {
            if (clientes == null) return 0m;
            return clientes.Sum(c => c.TotalGastado);
        }

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

        public const int UMBRAL_STOCK_BAJO = 5;

        public bool ValidarEstado(int? estado)
        {
            return !estado.HasValue || estado.Value == ReporteStockProducto486LP.SIN_STOCK
                || estado.Value == ReporteStockProducto486LP.STOCK_BAJO || estado.Value == ReporteStockProducto486LP.NORMAL;
        }

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

        public List<string> MarcasDe(List<ReporteStockProducto486LP> productos)
        {
            if (productos == null) return new List<string>();
            return productos.Select(p => p.Marca).Distinct().OrderBy(m => m).ToList();
        }

        public List<string> TallesDe(List<ReporteStockProducto486LP> productos)
        {
            if (productos == null) return new List<string>();
            return productos.Select(p => p.Talle).Distinct()
                .OrderBy(t => { double n; return double.TryParse(t.Replace(',', '.'), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out n) ? n : double.MaxValue; })
                .ThenBy(t => t).ToList();
        }

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

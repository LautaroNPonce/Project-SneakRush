using BE;
using DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mappers
{
    // Consultas del reporte de reporte inteligente (solo lectura). Cada consulta llama a su propio SP.
    // No atrapan excepciones: las maneja la BLL (registra en bitacora y avisa al usuario), asi un error de BD no se confunde con "no hay datos".
    public class Mapper_ReporteInteligente486LP : MapperBase486LP
    {
        // Ventas agrupadas por periodo. agrupacion: 1 = por mes, 2 = por semana (lunes a domingo). Solo trae los periodos que tuvieron ventas.
        public List<ReporteVentaPeriodo486LP> VentasPorPeriodo(DateTime desde, DateTime hasta, int agrupacion)
        {
            SqlCommand cmd = new SqlCommand("Reporte_VentasPorPeriodo");
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@Desde", desde.Date));
            cmd.Parameters.Add(new SqlParameter("@Hasta", hasta.Date));
            cmd.Parameters.Add(new SqlParameter("@Agrupacion", agrupacion));

            DataTable tabla = Conexion486LP.EjecutarConsulta(cmd);
            return ManejadorMapeo486LP.MapearLista<ReporteVentaPeriodo486LP>(tabla);
        }

        // Total de ventas, monto y unidades del rango (una sola fila)
        public ReporteResumenVentas486LP Resumen(DateTime desde, DateTime hasta)
        {
            SqlCommand cmd = new SqlCommand("Reporte_ResumenVentas");
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@Desde", desde.Date));
            cmd.Parameters.Add(new SqlParameter("@Hasta", hasta.Date));

            DataTable tabla = Conexion486LP.EjecutarConsulta(cmd);
            List<ReporteResumenVentas486LP> lista = ManejadorMapeo486LP.MapearLista<ReporteResumenVentas486LP>(tabla);
            return lista.Count > 0 ? lista[0] : new ReporteResumenVentas486LP();
        }

        // Los modelos mas vendidos (por unidades) del rango
        public List<ReporteTopProducto486LP> TopProductos(DateTime desde, DateTime hasta, int cantidad)
        {
            SqlCommand cmd = new SqlCommand("Reporte_TopProductos");
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@Desde", desde.Date));
            cmd.Parameters.Add(new SqlParameter("@Hasta", hasta.Date));
            cmd.Parameters.Add(new SqlParameter("@Cantidad", cantidad));

            DataTable tabla = Conexion486LP.EjecutarConsulta(cmd);
            return ManejadorMapeo486LP.MapearLista<ReporteTopProducto486LP>(tabla);
        }

        // Ventas por medio de pago del rango
        public List<ReporteMedioPago486LP> VentasPorMedioPago(DateTime desde, DateTime hasta)
        {
            SqlCommand cmd = new SqlCommand("Reporte_VentasMedioPago");
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@Desde", desde.Date));
            cmd.Parameters.Add(new SqlParameter("@Hasta", hasta.Date));

            DataTable tabla = Conexion486LP.EjecutarConsulta(cmd);
            return ManejadorMapeo486LP.MapearLista<ReporteMedioPago486LP>(tabla);
        }
    }
}

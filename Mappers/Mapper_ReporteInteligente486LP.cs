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
    public class Mapper_ReporteInteligente486LP : MapperBase486LP
    {
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

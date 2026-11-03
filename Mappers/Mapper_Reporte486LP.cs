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
    public class Mapper_Reporte486LP : MapperBase486LP
    {
        public List<ReporteClienteFrecuente486LP> ClientesFrecuentes(DateTime desde, DateTime hasta, int minCompras)
        {
            SqlCommand cmd = new SqlCommand("Reporte_ClientesFrecuentes");
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@Desde", desde.Date));
            cmd.Parameters.Add(new SqlParameter("@Hasta", hasta.Date));
            cmd.Parameters.Add(new SqlParameter("@MinCompras", minCompras));

            DataTable tabla = Conexion486LP.EjecutarConsulta(cmd);
            return ManejadorMapeo486LP.MapearLista<ReporteClienteFrecuente486LP>(tabla);
        }

        public List<ReporteStockProducto486LP> EstadoStock(string marca, string talle, int? estado, bool soloReponer, int umbralBajo)
        {
            SqlCommand cmd = new SqlCommand("Reporte_EstadoStock");
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@Marca", (object)marca ?? DBNull.Value));
            cmd.Parameters.Add(new SqlParameter("@Talle", (object)talle ?? DBNull.Value));
            cmd.Parameters.Add(new SqlParameter("@Estado", estado.HasValue ? (object)estado.Value : DBNull.Value));
            cmd.Parameters.Add(new SqlParameter("@SoloReponer", soloReponer));
            cmd.Parameters.Add(new SqlParameter("@UmbralBajo", umbralBajo));

            DataTable tabla = Conexion486LP.EjecutarConsulta(cmd);
            return ManejadorMapeo486LP.MapearLista<ReporteStockProducto486LP>(tabla);
        }
    }
}

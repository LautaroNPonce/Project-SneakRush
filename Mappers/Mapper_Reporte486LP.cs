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
    // Consultas de los reportes (solo lectura). Cada reporte llama a su propio SP.
    public class Mapper_Reporte486LP : MapperBase486LP
    {
        // Clientes con al menos "minCompras" compras entre las dos fechas (inclusive), ordenados de mayor a menor monto gastado.
        // No atrapa excepciones: las maneja la BLL (registra en bitacora y avisa al usuario), asi un error de BD no se confunde con "no hay datos".
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

        // Productos con su stock, valor del stock (stock x precio) y estado (0 sin stock, 1 stock bajo, 2 normal). Cada filtro en null = no filtra.
        // No atrapa excepciones: las maneja la BLL (registra en bitacora y avisa al usuario).
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

using DAL;
using Mappers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    // Habla con los 5 procedimientos genéricos de dígitos verificadores (DV_ObtenerTablas, DV_ObtenerDV, DV_LeerTabla, DV_GuardarDVH y DV_GuardarDVV).
    // No sabe nada de las tablas en particular: la lista de tablas protegidas sale de la tabla DV.
    public class Mapper_DV486LP : MapperBase486LP
    {
        // Todas las tablas protegidas con su clave y su DVV guardado
        public List<DV486LP> ObtenerTablas()
        {
            try
            {
                SqlCommand cmd = new SqlCommand("DV_ObtenerTablas");
                cmd.CommandType = CommandType.StoredProcedure;

                DataTable dt = Conexion486LP.EjecutarConsulta(cmd);

                List<DV486LP> lista = new List<DV486LP>();
                foreach (DataRow fila in dt.Rows)
                {
                    lista.Add(ArmarDV(fila));
                }
                return lista;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener las tablas protegidas: {ex.Message}");
            }
        }

        // Los datos de una sola tabla protegida. Devuelve null si la tabla no figura en DV
        public DV486LP ObtenerDV(string tabla)
        {
            try
            {
                SqlCommand cmd = new SqlCommand("DV_ObtenerDV");
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@Tabla", tabla));

                DataTable dt = Conexion486LP.EjecutarConsulta(cmd);

                if (dt.Rows.Count == 0) return null;
                return ArmarDV(dt.Rows[0]);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener el DV de la tabla '{tabla}': {ex.Message}");
            }
        }

        // Todas las filas de la tabla, con la columna DVH incluida
        public DataTable LeerTabla(string tabla)
        {
            try
            {
                SqlCommand cmd = new SqlCommand("DV_LeerTabla");
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@Tabla", tabla));

                return Conexion486LP.EjecutarConsulta(cmd);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al leer la tabla '{tabla}': {ex.Message}");
            }
        }

        // Guarda el DVH de una fila. La fila se ubica por su clave: una columna y un valor, o dos si la clave es compuesta
        public void GuardarDVH(string tabla, long dvh, string columna1, string valor1, string columna2 = null, string valor2 = null)
        {
            try
            {
                SqlCommand cmd = new SqlCommand("DV_GuardarDVH");
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@Tabla", tabla));
                cmd.Parameters.Add(new SqlParameter("@Columna1", columna1));
                cmd.Parameters.Add(new SqlParameter("@Valor1", valor1));
                cmd.Parameters.Add(new SqlParameter("@DVH", dvh));
                cmd.Parameters.Add(new SqlParameter("@Columna2", (object)columna2 ?? DBNull.Value));
                cmd.Parameters.Add(new SqlParameter("@Valor2", (object)valor2 ?? DBNull.Value));

                Conexion486LP.EjecutarNoConsulta(cmd);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al guardar el DVH de la fila {valor1} en '{tabla}': {ex.Message}");
            }
        }

        // Guarda el DVV de la tabla
        public void GuardarDVV(string tabla, long dvv)
        {
            try
            {
                SqlCommand cmd = new SqlCommand("DV_GuardarDVV");
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@Tabla", tabla));
                cmd.Parameters.Add(new SqlParameter("@DVV", dvv));

                Conexion486LP.EjecutarNoConsulta(cmd);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al guardar el DVV de la tabla '{tabla}': {ex.Message}");
            }
        }

        private DV486LP ArmarDV(DataRow fila)
        {
            return new DV486LP(
                fila["Tabla"].ToString(),
                fila["ClavePrimaria"].ToString(),
                Convert.ToInt64(fila["DVV"], CultureInfo.InvariantCulture));
        }
    }
}

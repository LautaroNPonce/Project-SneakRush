using DAL;
using Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mappers
{

    /// Habla con SQL para BitacoraEvento. Reemplaza a DAL_Bitacora486LP.
    /// UBICACION TRANSITORIA en Mappers (no en Services) - mismo motivo que las 5 entidades anteriores, ver Entrada 6 del CHANGELOG. Es la PENULTIMA migracion del refactor: falta solo DV.

    public class Mapper_Bitacora486LP : MapperBase486LP
    {
        // Registra un evento nuevo en la bitacora.
        public bool Registrar(BitacoraEvento486LP registro)
        {
            try
            {
                SqlCommand cmd = new SqlCommand("BitacoraEvento_Registrar");
                cmd.CommandType = CommandType.StoredProcedure;

                // Misma validacion de rango que tenia el DAL original: datetime minimo de SQL Server es 1/1/1753, si viene por debajo se usa "ahora".
                DateTime fecha = registro.Fecha < new DateTime(1753, 1, 1) ? DateTime.Now : registro.Fecha;

                cmd.Parameters.Add(new SqlParameter("@Fecha", fecha));
                cmd.Parameters.Add(new SqlParameter("@Modulo", registro.Modulo));
                cmd.Parameters.Add(new SqlParameter("@Descripcion", registro.Descripcion));
                cmd.Parameters.Add(new SqlParameter("@Criticidad", registro.Criticidad));
                cmd.Parameters.Add(new SqlParameter("@DNI", registro.DNI));
                cmd.Parameters.Add(new SqlParameter("@NombreUsuario", registro.NombreUsuario ?? ""));

                return Conexion486LP.EjecutarNoConsulta(cmd) > 0;
            }
            catch
            {
                return false;
            }
        }

        // Lista todos los eventos de la bitacora (con Nombre/Apellido del usuario relacionado, via LEFT JOIN), mas recientes primero.
        public List<BitacoraEvento486LP> Listar()
        {
            try
            {
                SqlCommand cmd = new SqlCommand("BitacoraEvento_Listar");
                cmd.CommandType = CommandType.StoredProcedure;

                DataTable tabla = Conexion486LP.EjecutarConsulta(cmd);
                return ManejadorMapeo486LP.MapearLista<BitacoraEvento486LP>(tabla);
            }
            catch
            {
                return new List<BitacoraEvento486LP>();
            }
        }

        // Filtra eventos por combinacion de criterios (todos opcionales).
        public List<BitacoraEvento486LP> Filtrar(string dni, string nombreUsuario, string modulo, int? criticidad, string fechaInicio, string fechaFin)
        {
            try
            {
                SqlCommand cmd = new SqlCommand("BitacoraEvento_Filtrar");
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new SqlParameter("@DNI", string.IsNullOrEmpty(dni) ? (object)DBNull.Value : dni));
                cmd.Parameters.Add(new SqlParameter("@NombreUsuario", string.IsNullOrEmpty(nombreUsuario) ? (object)DBNull.Value : "%" + nombreUsuario + "%"));
                cmd.Parameters.Add(new SqlParameter("@Modulo", string.IsNullOrEmpty(modulo) ? (object)DBNull.Value : modulo));
                cmd.Parameters.Add(new SqlParameter("@Criticidad", criticidad.HasValue ? (object)criticidad.Value : DBNull.Value));
                cmd.Parameters.Add(new SqlParameter("@FechaInicio", string.IsNullOrEmpty(fechaInicio) ? (object)DBNull.Value : (object)fechaInicio));
                cmd.Parameters.Add(new SqlParameter("@FechaFin", string.IsNullOrEmpty(fechaFin) ? (object)DBNull.Value : (object)fechaFin));

                DataTable tabla = Conexion486LP.EjecutarConsulta(cmd);
                return ManejadorMapeo486LP.MapearLista<BitacoraEvento486LP>(tabla);
            }
            catch
            {
                return new List<BitacoraEvento486LP>();
            }
        }
    }
}

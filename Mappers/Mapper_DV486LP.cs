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

    /// DV opera sobre 13 tablas distintas, pero NUNCA arma el nombre de tabla como texto SQL dinamico: cada tabla tiene su propio SP fijo
    /// (DV_Leer_X, DV_ActualizarFila_X), y este Mapper solo ELIGE cual SP llamar segun el nombre de tabla que le llega - la eleccion es en C#,
    /// el SQL de cada SP es fijo y conocido de antemano.
    /// UBICACION DEFINITIVA en Services (no en Mappers) - fue la ULTIMA entidad migrada; con esta se completo el refactor y se movieron los 7 Mappers
    /// de seguridad/auditoria a Services (ver CHANGELOG_Refactor_Arquitectura.md).

    public class Mapper_DV486LP : MapperBase486LP
    {
        // Nombre del SP de lectura completa, por tabla protegida (13 tablas).
        private static readonly Dictionary<string, string> _spLeerPorTabla = new Dictionary<string, string>
        {
            { "BitacoraEvento",   "DV_Leer_BitacoraEvento" },
            { "Usuarios",         "DV_Leer_Usuarios" },
            { "Perfil",           "DV_Leer_Perfil" },
            { "Familia",          "DV_Leer_Familia" },
            { "Permiso",          "DV_Leer_Permiso" },
            { "Idioma",           "DV_Leer_Idioma" },
            { "Familia_Permiso",  "DV_Leer_Familia_Permiso" },
            { "Perfil_Familia",   "DV_Leer_Perfil_Familia" },
            { "Perfil_Permiso",  "DV_Leer_Perfil_Permiso" },
            { "Producto",         "DV_Leer_Producto" },
            { "Carrito",          "DV_Leer_Carrito" },
            { "DetalleCarrito",   "DV_Leer_DetalleCarrito" },
            { "Cliente",          "DV_Leer_Cliente" }
        };

        // Nombre del SP que actualiza el DV de UNA fila, por tabla (solo las 10 que llevan DVH por fila - las 3 puente quedan afuera).
        private static readonly Dictionary<string, string> _spActualizarFilaPorTabla = new Dictionary<string, string>
        {
            { "BitacoraEvento",  "DV_ActualizarFila_BitacoraEvento" },
            { "Usuarios",        "DV_ActualizarFila_Usuarios" },
            { "Perfil",          "DV_ActualizarFila_Perfil" },
            { "Familia",         "DV_ActualizarFila_Familia" },
            { "Permiso",         "DV_ActualizarFila_Permiso" },
            { "Idioma",          "DV_ActualizarFila_Idioma" },
            { "Producto",        "DV_ActualizarFila_Producto" },
            { "Carrito",         "DV_ActualizarFila_Carrito" },
            { "DetalleCarrito",  "DV_ActualizarFila_DetalleCarrito" },
            { "Cliente",         "DV_ActualizarFila_Cliente" }
        };

        // Lee TODAS las columnas y filas de una tabla protegida (para calcular DVH/DVV). Elige el SP fijo correspondiente - nunca arma SQL con el
        // nombre de tabla como texto.
        public DataTable LeerTabla(string nombreTabla)
        {
            if (!_spLeerPorTabla.ContainsKey(nombreTabla))
            {
                throw new Exception($"No hay un SP de lectura configurado para la tabla '{nombreTabla}'.");
            }

            try
            {
                SqlCommand cmd = new SqlCommand(_spLeerPorTabla[nombreTabla]);
                cmd.CommandType = CommandType.StoredProcedure;

                return Conexion486LP.EjecutarConsulta(cmd);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al leer tabla '{nombreTabla}': {ex.Message}");
            }
        }

        // Obtiene el DVV guardado de una tabla (opera sobre la tabla DV, fija).
        public string ObtenerDVV(string tabla)
        {
            try
            {
                SqlCommand cmd = new SqlCommand("DV_ObtenerDVV");
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@Tabla", tabla));

                object resultado = Conexion486LP.EjecutarEscalar(cmd);
                return resultado?.ToString() ?? "";
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener DVV de tabla '{tabla}': {ex.Message}");
            }
        }

        // Obtiene el DVH guardado de una tabla (opera sobre la tabla DV, fija).
        public string ObtenerDVH(string tabla)
        {
            try
            {
                SqlCommand cmd = new SqlCommand("DV_ObtenerDVH");
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@Tabla", tabla));

                object resultado = Conexion486LP.EjecutarEscalar(cmd);
                return resultado?.ToString() ?? "";
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener DVH de tabla '{tabla}': {ex.Message}");
            }
        }

        // Guarda el DVH/DVV recalculados de una tabla (en la tabla DV, fija).
        public void GuardarDV(string tabla, string dvh, string dvv)
        {
            try
            {
                SqlCommand cmd = new SqlCommand("DV_GuardarDV");
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@Tabla", tabla));
                cmd.Parameters.Add(new SqlParameter("@DVH", dvh));
                cmd.Parameters.Add(new SqlParameter("@DVV", dvv));

                Conexion486LP.EjecutarNoConsulta(cmd);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al guardar DV de tabla '{tabla}': {ex.Message}");
            }
        }

        // Actualiza el DV de UNA SOLA fila puntual (sin recorrer el resto de la tabla). Se usa cuando se acaba de insertar/modificar un unico registro mucho mas rapido que
        // RecalcularDVHPorFila para tablas grandes (ej. BitacoraEvento, que crece indefinidamente).
        public void ActualizarDVDeUnaFila(string tabla, object id, string dv)
        {
            if (!_spActualizarFilaPorTabla.ContainsKey(tabla))
            {
                throw new Exception($"No hay un SP de actualización de fila configurado para la tabla '{tabla}'.");
            }

            try
            {
                SqlCommand cmd = new SqlCommand(_spActualizarFilaPorTabla[tabla]);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@Id", id));
                cmd.Parameters.Add(new SqlParameter("@DV", dv));

                Conexion486LP.EjecutarNoConsulta(cmd);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al actualizar DV de la fila {id} en '{tabla}': {ex.Message}");
            }
        }

        // Recalcula el DVH (hash por fila) de cada registro de la tabla y lo
        // guarda, fila por fila, usando el SP de actualizacion fijo de esa tabla.
        public void RecalcularDVHPorFila(string tabla, string columnaId)
        {
            if (!_spActualizarFilaPorTabla.ContainsKey(tabla))
            {
                throw new Exception($"No hay un SP de actualización de fila configurado para la tabla '{tabla}'.");
            }

            DataTable dt = LeerTabla(tabla);

            foreach (DataRow fila in dt.Rows)
            {
                StringBuilder sb = new StringBuilder();
                foreach (DataColumn col in dt.Columns)
                {
                    if (col.ColumnName != "DV")
                        sb.Append(fila[col].ToString());
                }
                string hash = Encriptacion486LP.GenerarHash(sb.ToString());

                try
                {
                    SqlCommand cmd = new SqlCommand(_spActualizarFilaPorTabla[tabla]);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@Id", fila[columnaId]));
                    cmd.Parameters.Add(new SqlParameter("@DV", hash));

                    Conexion486LP.EjecutarNoConsulta(cmd);
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error al actualizar DV fila {fila[columnaId]}: {ex.Message}");
                }
            }
        }
    }
}

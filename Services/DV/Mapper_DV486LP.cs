using DAL;
using Mappers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class Mapper_DV486LP : MapperBase486LP
    {
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
            { "Perfil_Permiso",   "DV_Leer_Perfil_Permiso" },
            { "Producto",         "DV_Leer_Producto" },
            { "Carrito",          "DV_Leer_Carrito" },
            { "DetalleCarrito",   "DV_Leer_DetalleCarrito" },
            { "Cliente",          "DV_Leer_Cliente" },
            { "Venta",            "DV_Leer_Venta" },
            { "DetalleVenta",     "DV_Leer_DetalleVenta" },
            { "Proveedor",              "DV_Leer_Proveedor" },
            { "SolicitudCompra",        "DV_Leer_SolicitudCompra" },
            { "DetalleSolicitudCompra", "DV_Leer_DetalleSolicitudCompra" },
            { "OrdenCompra",            "DV_Leer_OrdenCompra" },
            { "DetalleOrdenCompra",     "DV_Leer_DetalleOrdenCompra" },
            { "Recepcion",              "DV_Leer_Recepcion" },
            { "DetalleRecepcion",       "DV_Leer_DetalleRecepcion" },
            { "Factura",                "DV_Leer_Factura" },
            { "Categoria",              "DV_Leer_Categoria" },
            { "Marca",                  "DV_Leer_Marca" },
            { "ImportacionProducto",    "DV_Leer_ImportacionProducto" },
            { "DetalleImportacion",     "DV_Leer_DetalleImportacion" },
            { "ProductoCambio",         "DV_Leer_ProductoCambio" }
        };

        private static readonly Dictionary<string, string> _spActualizarFilaPorTabla = new Dictionary<string, string>
        {
            { "BitacoraEvento", "DV_ActualizarFila_BitacoraEvento" },
            { "Usuarios", "DV_ActualizarFila_Usuarios" },
            { "Perfil", "DV_ActualizarFila_Perfil" },
            { "Familia", "DV_ActualizarFila_Familia" },
            { "Permiso", "DV_ActualizarFila_Permiso" },
            { "Idioma", "DV_ActualizarFila_Idioma" },
            { "Producto", "DV_ActualizarFila_Producto" },
            { "Carrito", "DV_ActualizarFila_Carrito" },
            { "DetalleCarrito", "DV_ActualizarFila_DetalleCarrito" },
            { "Cliente", "DV_ActualizarFila_Cliente" },
            { "Venta", "DV_ActualizarFila_Venta" },
            { "DetalleVenta", "DV_ActualizarFila_DetalleVenta" },
            { "Proveedor", "DV_ActualizarFila_Proveedor" },
            { "SolicitudCompra", "DV_ActualizarFila_SolicitudCompra" },
            { "DetalleSolicitudCompra", "DV_ActualizarFila_DetalleSolicitudCompra" },
            { "OrdenCompra", "DV_ActualizarFila_OrdenCompra" },
            { "DetalleOrdenCompra", "DV_ActualizarFila_DetalleOrdenCompra" },
            { "Recepcion", "DV_ActualizarFila_Recepcion" },
            { "DetalleRecepcion", "DV_ActualizarFila_DetalleRecepcion" },
            { "Factura", "DV_ActualizarFila_Factura" },
            { "Categoria", "DV_ActualizarFila_Categoria" },
            { "Marca", "DV_ActualizarFila_Marca" },
            { "ImportacionProducto", "DV_ActualizarFila_ImportacionProducto" },
            { "DetalleImportacion", "DV_ActualizarFila_DetalleImportacion" },
            { "ProductoCambio", "DV_ActualizarFila_ProductoCambio" }
        };
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

using DAL;
using Mappers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Services.DV486LP;

namespace Services
{
    public class BLL_DV486LP
    {
        private Mapper_DV486LP _mapper = new Mapper_DV486LP();
        private BLL_Bitacora486LP _bllBitacora = new BLL_Bitacora486LP();
        private static readonly List<string> _columnasIgnorar = new List<string> { "DV" };

        // Nombre de la columna PK de cada tabla, para el recálculo del DV por fila.
        private static readonly Dictionary<string, string> _columnaIdPorTabla = new Dictionary<string, string>
        {
            { "Usuarios",       "IdUsuario"    },
            { "Perfil",         "IdPerfil"     },
            { "Familia",        "IdFamilia"    },
            { "Permiso",        "IdPermiso"    },
            { "BitacoraEvento", "Numero" },
            { "Idioma",         "NombreIdioma" },
            { "Producto",       "IdProducto"   },
            { "Carrito",        "IdCarrito"    },
            { "DetalleCarrito", "IdDetalle"    },
            { "Cliente",        "DNI"          },
            { "Venta",          "IdVenta"      },
            { "DetalleVenta",   "IdDetalleVenta" },
            { "Proveedor",              "IdProveedor" },
            { "SolicitudCompra",        "IdSolicitud" },
            { "DetalleSolicitudCompra", "IdDetalleSolicitud" },
            { "OrdenCompra",            "IdOrden" },
            { "DetalleOrdenCompra",     "IdDetalleOrden" },
            { "Recepcion",              "IdRecepcion" },
            { "DetalleRecepcion",       "IdDetalleRecepcion" },
            { "Factura",                "IdFactura" },
            { "Categoria",              "IdCategoria" },
            { "Marca",                  "IdMarca" },
            { "ImportacionProducto",    "IdImportacion" },
            { "DetalleImportacion",     "IdDetalleImportacion" },
            { "ProductoCambio",         "IdCambio" }
        };

        private static readonly string[] _tablasProtegidas = {
            "BitacoraEvento", "Usuarios", "Perfil", "Familia", "Permiso", "Idioma",
            "Familia_Permiso", "Perfil_Familia", "Perfil_Permiso",
            "Producto", "Carrito", "DetalleCarrito", "Cliente", "Venta", "DetalleVenta",
            "Proveedor", "SolicitudCompra", "DetalleSolicitudCompra",
            "OrdenCompra", "DetalleOrdenCompra", "Recepcion", "DetalleRecepcion", "Factura",
            "Categoria", "Marca","ImportacionProducto", "DetalleImportacion","ProductoCambio"
        };
        private const string TABLA_PRODUCTO = "Producto";
        private const string TABLA_HISTORIAL_PRODUCTO = "ProductoCambio";
        private static readonly List<string> _tablasSoloNivelTabla = new List<string> { "Familia_Permiso", "Perfil_Familia", "Perfil_Permiso" };
        private string ObtenerColumnaId(string tabla)
        {
            return _columnaIdPorTabla.ContainsKey(tabla) ? _columnaIdPorTabla[tabla] : "Id";
        }

        private string CalcularDVH(string tabla)
        {
            DataTable dt = _mapper.LeerTabla(tabla);
            return CalcularDVHDesdeTabla(dt);
        }

        private string CalcularDVHDesdeTabla(DataTable dt)
        {
            StringBuilder sb = new StringBuilder();

            foreach (DataRow fila in dt.Rows)
            {
                foreach (DataColumn col in dt.Columns)
                {
                    if (!_columnasIgnorar.Contains(col.ColumnName))
                    {
                        sb.Append(fila[col].ToString());
                    }
                }
            }

            return Encriptacion486LP.GenerarHash(sb.ToString());
        }

        private string CalcularDVV(string tabla)
        {
            DataTable dt = _mapper.LeerTabla(tabla);
            return CalcularDVVDesdeTabla(dt);
        }

        private string CalcularDVVDesdeTabla(DataTable dt)
        {
            StringBuilder sb = new StringBuilder();

            foreach (DataColumn col in dt.Columns)
            {
                if (!_columnasIgnorar.Contains(col.ColumnName))
                {
                    foreach (DataRow fila in dt.Rows)
                    {
                        sb.Append(fila[col].ToString());
                    }
                }
            }

            return Encriptacion486LP.GenerarHash(sb.ToString());
        }

        private string CalcularHashDeFila(DataRow fila, DataColumnCollection columnas)
        {
            return CalcularHashDeFila(fila, columnas, false);
        }

        private string CalcularHashDeFila(DataRow fila, DataColumnCollection columnas, bool invertirVigente)
        {
            StringBuilder sb = new StringBuilder();
            foreach (DataColumn col in columnas)
            {
                if (col.ColumnName == "DV") continue;

                if (invertirVigente && col.ColumnName == "Vigente" && fila[col] != DBNull.Value)
                    sb.Append((!Convert.ToBoolean(fila[col])).ToString());
                else
                    sb.Append(fila[col].ToString());
            }
            return Encriptacion486LP.GenerarHash(sb.ToString());
        }

        private void SincronizarDVProductoCambio()
        {
            try
            {
                DataTable dt = _mapper.LeerTabla(TABLA_HISTORIAL_PRODUCTO);
                string columnaId = ObtenerColumnaId(TABLA_HISTORIAL_PRODUCTO);

                foreach (DataRow fila in dt.Rows)
                {
                    string guardado = fila["DV"] == DBNull.Value ? "" : fila["DV"].ToString();
                    string actual = CalcularHashDeFila(fila, dt.Columns, false);

                    if (guardado == actual) continue;

                    bool sinDV = guardado.Length == 0;
                    bool soloCambioVigente = !sinDV && guardado == CalcularHashDeFila(fila, dt.Columns, true);

                    if (sinDV || soloCambioVigente)
                    {
                        _mapper.ActualizarDVDeUnaFila(TABLA_HISTORIAL_PRODUCTO, fila[columnaId], actual);
                    }
                }

                _mapper.GuardarDV(TABLA_HISTORIAL_PRODUCTO, CalcularDVHDesdeTabla(dt), CalcularDVVDesdeTabla(dt));
            }
            catch
            {
                // Sin historial en la BD no hay nada para sincronizar. Si falla por otro motivo, la inconsistencia aparece en la verificacion de DV.
            }
        }

        public bool RegistrarDVDeFilaNueva(string tabla, object id, out string mensaje)
        {
            mensaje = "";
            try
            {
                DataTable dt = _mapper.LeerTabla(tabla); 

                if (!_tablasSoloNivelTabla.Contains(tabla))
                {
                    string columnaId = ObtenerColumnaId(tabla);
                    DataRow filaEncontrada = null;
                    foreach (DataRow fila in dt.Rows)
                    {
                        if (fila[columnaId].ToString() == id.ToString())
                        {
                            filaEncontrada = fila;
                            break;
                        }
                    }

                    if (filaEncontrada == null)
                    {
                        mensaje = $"No se encontró la fila con Id '{id}' en la tabla '{tabla}'.";
                        return false;
                    }

                    string hashFila = CalcularHashDeFila(filaEncontrada, dt.Columns);
                    _mapper.ActualizarDVDeUnaFila(tabla, id, hashFila);
                }

                string dvh = CalcularDVHDesdeTabla(dt);
                string dvv = CalcularDVVDesdeTabla(dt);
                _mapper.GuardarDV(tabla, dvh, dvv);

                if (tabla == TABLA_PRODUCTO) SincronizarDVProductoCambio();

                return true;
            }
            catch (Exception ex)
            {
                mensaje = ex.Message;
                return false;
            }
        }

        public bool RecalcularDV(string tabla, out string mensaje)
        {
            return RecalcularDV(tabla, true, out mensaje);
        }

        public bool RecalcularDV(string tabla, bool registrarBitacora, out string mensaje)
        {
            mensaje = "";
            try
            {
                string dvh = CalcularDVH(tabla);
                string dvv = CalcularDVV(tabla);
                _mapper.GuardarDV(tabla, dvh, dvv);
                if (!_tablasSoloNivelTabla.Contains(tabla))
                {
                    _mapper.RecalcularDVHPorFila(tabla, ObtenerColumnaId(tabla));
                }

                if (tabla == TABLA_PRODUCTO) SincronizarDVProductoCambio();

                if (registrarBitacora)
                {
                    string dni = SessionManager486LP.ObtenerInstancia().UsuarioActual()?.DNI ?? "Sistema";
                    string nombreUsuario = SessionManager486LP.ObtenerInstancia().UsuarioActual()?.NombreUsuario ?? "Sistema";

                    _bllBitacora.Registrar(new BitacoraEvento486LP("Dígito Verificador", $"Se recalcularon los dígitos verificadores de la tabla '{tabla}'.", Criticidad486LP.MuyAlta, dni, nombreUsuario));
                }

                return true;
            }
            catch (Exception ex)
            {
                mensaje = ex.Message;
                return false;
            }
        }

        public bool VerificarIntegridad(string tabla, out string tablaAfectada, out string mensaje)
        {
            tablaAfectada = "";
            mensaje = "";

            try
            {
                string dvhCalculado = CalcularDVH(tabla);
                string dvvCalculado = CalcularDVV(tabla);

                string dvhGuardado = _mapper.ObtenerDVH(tabla);
                string dvvGuardado = _mapper.ObtenerDVV(tabla);

                if (dvhCalculado != dvhGuardado || dvvCalculado != dvvGuardado)
                {
                    tablaAfectada = tabla;

                    string dni = SessionManager486LP.ObtenerInstancia().UsuarioActual()?.DNI ?? "Sistema";
                    string nombreUsuario = SessionManager486LP.ObtenerInstancia().UsuarioActual()?.NombreUsuario ?? "Sistema";

                    _bllBitacora.Registrar(new BitacoraEvento486LP("Dígito Verificador", $"Inconsistencia detectada en la tabla '{tabla}'.", Criticidad486LP.MuyAlta, dni, nombreUsuario));

                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                mensaje = ex.Message;
                return false;
            }
        }

        public List<InconsistenciaDV486LP> ObtenerInconsistencias(string tabla)
        {
            List<InconsistenciaDV486LP> lista = new List<InconsistenciaDV486LP>();

            try
            {
                if (_tablasSoloNivelTabla.Contains(tabla))
                {
                    string dvhCalc = CalcularDVH(tabla);
                    string dvvCalc = CalcularDVV(tabla);
                    if (dvhCalc != _mapper.ObtenerDVH(tabla) || dvvCalc != _mapper.ObtenerDVV(tabla))
                    {
                        lista.Add(new InconsistenciaDV486LP
                        {
                            ID = "-",
                            Tabla = tabla,
                            Inconsistencia = "Inc.Modificado" // Relación modificada directamente en la BD
                        });
                    }
                    return lista;
                }

                DataTable dt = _mapper.LeerTabla(tabla);
                string columnaId = ObtenerColumnaId(tabla);

                foreach (DataRow fila in dt.Rows)
                {
                    string id = fila[columnaId].ToString();

                    StringBuilder sb = new StringBuilder();
                    foreach (DataColumn col in dt.Columns)
                    {
                        if (!_columnasIgnorar.Contains(col.ColumnName))
                            sb.Append(fila[col].ToString());
                    }

                    string hashRecalculado = Encriptacion486LP.GenerarHash(sb.ToString());
                    string hashGuardado = fila["DV"]?.ToString() ?? "";

                    if (string.IsNullOrEmpty(hashGuardado))
                    {

                        lista.Add(new InconsistenciaDV486LP
                        {
                            ID = id,
                            Tabla = tabla,
                            Inconsistencia = "Inc.Insertado" // Registro insertado directamente en la BD
                        });
                    }
                    else if (hashRecalculado != hashGuardado)
                    {
                        lista.Add(new InconsistenciaDV486LP
                        {
                            ID = id,
                            Tabla = tabla,
                            Inconsistencia = "Inc.Modificado" // Registro modificado directamente en la BD
                        });
                    }
                }

                string dvvCalculado = CalcularDVV(tabla);
                string dvvGuardado = _mapper.ObtenerDVV(tabla);

                if (dvvCalculado != dvvGuardado && lista.Count == 0)
                {
                    lista.Add(new InconsistenciaDV486LP
                    {
                        ID = "-",
                        Tabla = tabla,
                        Inconsistencia = "Inc.Eliminado" // Registro eliminado directamente en la BD
                    });
                }
            }
            catch (Exception ex)
            {
                lista.Add(new InconsistenciaDV486LP
                {
                    ID = "-",
                    Tabla = tabla,
                    Inconsistencia = $"Error al analizar inconsistencias: {ex.Message}"
                });
            }

            return lista;
        }

        public List<string> VerificarTodas(out string errorTecnico)
        {
            errorTecnico = "";
            List<string> conProblemas = new List<string>();

            foreach (string tabla in _tablasProtegidas)
            {
                string tablaAfectada;
                string mensaje;
                bool ok = VerificarIntegridad(tabla, out tablaAfectada, out mensaje);

                if (!ok)
                {
                    if (!string.IsNullOrEmpty(mensaje))
                    {
                        errorTecnico = mensaje;
                        return conProblemas;
                    }
                    conProblemas.Add(tabla);
                }
            }

            return conProblemas;
        }

        // Inconsistencias de varias tablas juntas (para mostrar todas en la grilla de reparación).
        public List<InconsistenciaDV486LP> ObtenerInconsistenciasDeTablas(List<string> tablas)
        {
            List<InconsistenciaDV486LP> todas = new List<InconsistenciaDV486LP>();
            foreach (string tabla in tablas)
                todas.AddRange(ObtenerInconsistencias(tabla));
            return todas;
        }

        public bool RecalcularTablas(List<string> tablas, out string mensaje)
        {
            mensaje = "";
            foreach (string tabla in tablas)
            {
                bool registrar = tabla != "BitacoraEvento";
                if (!RecalcularDV(tabla, registrar, out mensaje))
                {
                    return false;
                }
            }
            return true;
        }
    }
}

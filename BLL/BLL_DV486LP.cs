using DAL;
using Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Services.DV486LP;

namespace BLL
{
    public class BLL_DV486LP
    {
        private DAL_DV486LP _dal = new DAL_DV486LP();
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
            { "Idioma",         "NombreIdioma" }
        };

        private static readonly string[] _tablasProtegidas = { "BitacoraEvento","Usuarios", "Perfil", "Familia", "Permiso", "Idioma", "Familia_Permiso", "Perfil_Familia", "Perfil_Permiso" };
        private static readonly List<string> _tablasSoloNivelTabla = new List<string> { "Familia_Permiso", "Perfil_Familia", "Perfil_Permiso" };
        private string ObtenerColumnaId(string tabla)
        {
            return _columnaIdPorTabla.ContainsKey(tabla) ? _columnaIdPorTabla[tabla] : "Id";
        }

        private string CalcularDVH(string tabla)
        {
            DataTable dt = _dal.LeerTabla(tabla);
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
            DataTable dt = _dal.LeerTabla(tabla);
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
                _dal.GuardarDV(tabla, dvh, dvv);
                if (!_tablasSoloNivelTabla.Contains(tabla)) 
                { 
                    _dal.RecalcularDVHPorFila(tabla, ObtenerColumnaId(tabla)); 
                }
                    
                // No registro en bitácora cuando recalculamos la propia tabla Bitacora:
                // evita un bucle infinito (el evento de recálculo dejaría el hash viejo otra vez).
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

                string dvhGuardado = _dal.ObtenerDVH(tabla);
                string dvvGuardado = _dal.ObtenerDVV(tabla);

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
                    if (dvhCalc != _dal.ObtenerDVH(tabla) || dvvCalc != _dal.ObtenerDVV(tabla))
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

                DataTable dt = _dal.LeerTabla(tabla);
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
                string dvvGuardado = _dal.ObtenerDVV(tabla);

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

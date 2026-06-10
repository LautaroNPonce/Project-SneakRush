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
            mensaje = "";
            try
            {
                string dvh = CalcularDVH(tabla);
                string dvv = CalcularDVV(tabla);
                _dal.GuardarDV(tabla, dvh, dvv);
                _dal.RecalcularDVHPorFila(tabla);

                string dni = SessionManager486LP.ObtenerInstancia().UsuarioActual()?.DNI ?? "Sistema";
                string nombreUsuario = SessionManager486LP.ObtenerInstancia().UsuarioActual()?.NombreUsuario ?? "Sistema";

                _bllBitacora.Registrar(new BitacoraEvento486LP("Dígito Verificador",$"Se recalcularon los dígitos verificadores de la tabla '{tabla}'.",Criticidad486LP.MuyAlta, dni, nombreUsuario));

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

                    _bllBitacora.Registrar(new BitacoraEvento486LP("Dígito Verificador",$"Inconsistencia detectada en la tabla '{tabla}'.",Criticidad486LP.MuyAlta, dni, nombreUsuario));

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
                DataTable dt = _dal.LeerTabla(tabla);

                foreach (DataRow fila in dt.Rows)
                {
                    string id = fila["IdUsuario"].ToString();

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
                            Inconsistencia = "Registro insertado directamente en la BD"
                        });
                    }
                    else if (hashRecalculado != hashGuardado)
                    {
                        lista.Add(new InconsistenciaDV486LP
                        {
                            ID = id,
                            Tabla = tabla,
                            Inconsistencia = "Registro modificado directamente en la BD"
                        });
                    }
                }

                // Si no hay filas modificadas ni insertadas pero el DVV no coincide → se eliminó un registro
                string dvvCalculado = CalcularDVV(tabla);
                string dvvGuardado = _dal.ObtenerDVV(tabla);

                if (dvvCalculado != dvvGuardado && lista.Count == 0)
                {
                    lista.Add(new InconsistenciaDV486LP
                    {
                        ID = "-",
                        Tabla = tabla,
                        Inconsistencia = "Registro eliminado directamente en la BD"
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
    }
}

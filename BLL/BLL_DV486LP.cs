using DAL;
using Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                        sb.Append(fila[col].ToString());
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

        public void RecalcularDV(string tabla)
        {
            string dvh = CalcularDVH(tabla);
            string dvv = CalcularDVV(tabla);
            _dal.GuardarDV(tabla, dvh, dvv);
            _dal.RecalcularDVHPorFila(tabla);

            string dni = SessionManager486LP.ObtenerInstancia().UsuarioActual()?.DNI ?? "Sistema";
            string nombreUsuario = SessionManager486LP.ObtenerInstancia().UsuarioActual()?.NombreUsuario ?? "Sistema";

            _bllBitacora.Registrar(new BitacoraEvento486LP("Dígito Verificador",$"Se recalcularon los dígitos verificadores de la tabla '{tabla}'.",Criticidad486LP.MuyAlta, dni, nombreUsuario));
        }

        public bool VerificarIntegridad(string tabla, out string tablaAfectada)
        {
            tablaAfectada = "";

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
    }
}

using BE;
using DAL;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class BLL_Bitacora486LP
    {
        private DAL_Bitacora486LP Dal = new DAL_Bitacora486LP();

        public bool Registrar(BitacoraEvento486LP registro)
        {
            return Dal.Registrar(registro);
        }

        public List<BitacoraEvento486LP> Listar()
        {
            return Dal.Listar();
        }

        public List<BitacoraEvento486LP> Filtrar(string dni, string nombreUsuario, string modulo, int? criticidad, string fechaInicio, string fechaFin)
        {
            return Dal.Filtrar(dni, nombreUsuario, modulo, criticidad, fechaInicio, fechaFin);
        }
    }
}

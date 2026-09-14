using BE;
using DAL;
using Mappers;
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
        private Mapper_Bitacora486LP Mapper = new Mapper_Bitacora486LP();

        public bool Registrar(BitacoraEvento486LP registro)
        {
            bool resultado = Mapper.Registrar(registro);

            if (resultado)
            {
                string mensajeDV;
                BLL_DV486LP bllDV = new BLL_DV486LP();
                bllDV.RecalcularDV("BitacoraEvento", false, out mensajeDV);
            }

            return resultado;
        }

        public List<BitacoraEvento486LP> Listar()
        {
            return Mapper.Listar();
        }

        public List<BitacoraEvento486LP> Filtrar(string dni, string nombreUsuario, string modulo, int? criticidad, string fechaInicio, string fechaFin)
        {
            return Mapper.Filtrar(dni, nombreUsuario, modulo, criticidad, fechaInicio, fechaFin);
        }
    }
}

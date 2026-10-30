using BE;
using DAL;
using Mappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class BLL_Bitacora486LP
    {
        private Mapper_Bitacora486LP Mapper = new Mapper_Bitacora486LP();
        public bool Registrar(BitacoraEvento486LP registro)
        {
            int numeroGenerado = Mapper.Registrar(registro);
            bool resultado = numeroGenerado > 0;

            if (resultado)
            {
                string mensajeDV;
                BLL_DV486LP bllDV = new BLL_DV486LP();
                bllDV.RegistrarDVDeFilaNueva("BitacoraEvento", numeroGenerado, out mensajeDV);
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

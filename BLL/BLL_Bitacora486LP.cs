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

        // Firma publica SIN CAMBIOS (sigue devolviendo bool) - no rompe a ninguno de los lugares que ya llaman ObjBitacora.Registrar(...) en el resto del sistema.
        public bool Registrar(BitacoraEvento486LP registro)
        {
            // Mapper.Registrar ahora devuelve el "Numero" (Id) generado, no un bool - lo necesitamos para actualizar el DV de ESA fila
            // puntual, sin recorrer toda la tabla (que hoy tiene miles de filas y hacia fallar el recalculo completo en cada evento).
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

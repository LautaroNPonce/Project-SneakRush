using DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class BLL_Inicializador486LP
    {
        /// Se asegura de que la base de datos exista (la crea y siembra si hace falta)
        //  Devuelve true si quedo lista, false y mensaje con el detalle si hubo error
        public bool AsegurarBaseDeDatos(out string mensaje)
        {
            return InicializadorBD486LP.AsegurarBaseDeDatos(out mensaje);
        }
    }
}

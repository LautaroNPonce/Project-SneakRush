using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class BLL_Patente486LP
    {
        private DAL.DAL_Patente486LP _dalPatente = new DAL.DAL_Patente486LP();

        public List<Permiso486LP> ObtenerPatentes()
        {
            return _dalPatente.Listar();
        }
    }
}

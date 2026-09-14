using Mappers;
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
        private Mapper_Permiso486LP _mapperPermiso = new Mapper_Permiso486LP();

        public List<Permiso486LP> ObtenerPatentes()
        {
            return _mapperPermiso.Listar();
        }
    }
}

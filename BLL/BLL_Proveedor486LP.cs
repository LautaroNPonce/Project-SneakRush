using BE;
using Mappers;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class BLL_Proveedor486LP
    {
        private Mapper_Proveedor486LP ObjetoMapper = new Mapper_Proveedor486LP();
        private BLL_Bitacora486LP ObjBitacora = new BLL_Bitacora486LP();

        public List<Proveedor486LP> Listar()
        {
            try
            {
                return ObjetoMapper.Listar();
            }
            catch (Exception ex)
            {
                ObjBitacora.Registrar(new BitacoraEvento486LP("Compras", $"Error en BLL_Proveedor.Listar(): {ex.Message}", Criticidad486LP.MuyAlta, "Sistema", "Sistema"));
                return new List<Proveedor486LP>();
            }
        }
    }
}

using DAL;
using Mappers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class Mapper_Permiso486LP : MapperBase486LP
    {
        // Lista todos los permisos (patentes).
        public List<Permiso486LP> Listar()
        {
            try
            {
                SqlCommand cmd = new SqlCommand("Permiso_Listar");
                cmd.CommandType = CommandType.StoredProcedure;

                DataTable tabla = Conexion486LP.EjecutarConsulta(cmd);
                return ManejadorMapeo486LP.MapearLista<Permiso486LP>(tabla);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al listar patentes: " + ex.Message);
            }
        }
    }
}

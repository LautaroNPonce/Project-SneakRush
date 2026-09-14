using DAL;
using Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mappers
{
    /// Habla con SQL para la entidad Permiso (patentes). Reemplaza a DAL_Patente486LP.
    /// UBICACION TRANSITORIA: vive en Mappers (no en Services) porque hoy DAL todavia depende de Services (por los DAL_Xxx.cs de seguridad que faltan
    /// migrar: Idioma, Familia, Perfil, Usuario), lo que impide que Services ependa de Mappers sin generar un ciclo. Cuando se termine de migrar TODA
    /// la seguridad y se pueda sacar la referencia DAL->Services, este archivo (y los Mappers de Idioma/Familia/Perfil/Usuario) se mudan juntos a
    /// Services, en un solo paso - ver CHANGELOG_Refactor_Arquitectura.md.
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

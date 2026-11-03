using BE;
using DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mappers
{
    public class Mapper_Marca486LP : MapperBase486LP
    {
        public List<Marca486LP> Listar()
        {
            SqlCommand cmd = new SqlCommand("Marca_Listar");
            cmd.CommandType = CommandType.StoredProcedure;

            DataTable tabla = Conexion486LP.EjecutarConsulta(cmd);
            return ManejadorMapeo486LP.MapearLista<Marca486LP>(tabla);
        }

        public int Agregar(Marca486LP marca)
        {
            SqlCommand cmd = new SqlCommand("Marca_Agregar");
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@Nombre", marca.Nombre));

            return Convert.ToInt32(Conexion486LP.EjecutarEscalar(cmd));
        }

        public int Modificar(Marca486LP marca)
        {
            SqlCommand cmd = new SqlCommand("Marca_Modificar");
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@IdMarca", marca.IdMarca));
            cmd.Parameters.Add(new SqlParameter("@Nombre", marca.Nombre));

            return Conexion486LP.EjecutarNoConsulta(cmd);
        }

        public int Eliminar(int idMarca)
        {
            SqlCommand cmd = new SqlCommand("Marca_Eliminar");
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@IdMarca", idMarca));

            return Conexion486LP.EjecutarNoConsulta(cmd);
        }

        public bool ExisteNombre(string nombre, int idExcluir)
        {
            SqlCommand cmd = new SqlCommand("Marca_ExisteNombre");
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@Nombre", nombre));
            cmd.Parameters.Add(new SqlParameter("@IdExcluir", idExcluir));

            return Convert.ToInt32(Conexion486LP.EjecutarEscalar(cmd)) > 0;
        }

        public bool EstaEnUso(int idMarca)
        {
            SqlCommand cmd = new SqlCommand("Marca_EstaEnUso");
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@IdMarca", idMarca));

            return Convert.ToInt32(Conexion486LP.EjecutarEscalar(cmd)) > 0;
        }
    }
}

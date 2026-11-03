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
    public class Mapper_Categoria486LP : MapperBase486LP
    {
        public List<Categoria486LP> Listar()
        {
            SqlCommand cmd = new SqlCommand("Categoria_Listar");
            cmd.CommandType = CommandType.StoredProcedure;

            DataTable tabla = Conexion486LP.EjecutarConsulta(cmd);
            return ManejadorMapeo486LP.MapearLista<Categoria486LP>(tabla);
        }

        public int Agregar(Categoria486LP categoria)
        {
            SqlCommand cmd = new SqlCommand("Categoria_Agregar");
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@Nombre", categoria.Nombre));

            return Convert.ToInt32(Conexion486LP.EjecutarEscalar(cmd));
        }

        public int Modificar(Categoria486LP categoria)
        {
            SqlCommand cmd = new SqlCommand("Categoria_Modificar");
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@IdCategoria", categoria.IdCategoria));
            cmd.Parameters.Add(new SqlParameter("@Nombre", categoria.Nombre));

            return Conexion486LP.EjecutarNoConsulta(cmd);
        }

        public int Eliminar(int idCategoria)
        {
            SqlCommand cmd = new SqlCommand("Categoria_Eliminar");
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@IdCategoria", idCategoria));

            return Conexion486LP.EjecutarNoConsulta(cmd);
        }

        public bool ExisteNombre(string nombre, int idExcluir)
        {
            SqlCommand cmd = new SqlCommand("Categoria_ExisteNombre");
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@Nombre", nombre));
            cmd.Parameters.Add(new SqlParameter("@IdExcluir", idExcluir));

            return Convert.ToInt32(Conexion486LP.EjecutarEscalar(cmd)) > 0;
        }

        public bool EstaEnUso(int idCategoria)
        {
            SqlCommand cmd = new SqlCommand("Categoria_EstaEnUso");
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@IdCategoria", idCategoria));

            return Convert.ToInt32(Conexion486LP.EjecutarEscalar(cmd)) > 0;
        }
    }
}

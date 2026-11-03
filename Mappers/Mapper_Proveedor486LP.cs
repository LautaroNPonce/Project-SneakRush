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
    public class Mapper_Proveedor486LP : MapperBase486LP
    {
        public List<Proveedor486LP> Listar()
        {
            SqlCommand cmd = new SqlCommand("Proveedor_Listar");
            cmd.CommandType = CommandType.StoredProcedure;

            DataTable tabla = Conexion486LP.EjecutarConsulta(cmd);
            return ManejadorMapeo486LP.MapearLista<Proveedor486LP>(tabla);
        }
        public int Agregar(Proveedor486LP proveedor)
        {
            SqlCommand cmd = new SqlCommand("Proveedor_Agregar");
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@Nombre", proveedor.Nombre));
            cmd.Parameters.Add(new SqlParameter("@CUIT", proveedor.CUIT));
            cmd.Parameters.Add(new SqlParameter("@Telefono", string.IsNullOrWhiteSpace(proveedor.Telefono) ? (object)DBNull.Value : proveedor.Telefono));
            cmd.Parameters.Add(new SqlParameter("@Correo", proveedor.Correo));

            return Convert.ToInt32(Conexion486LP.EjecutarEscalar(cmd));
        }

        public int Modificar(Proveedor486LP proveedor)
        {
            SqlCommand cmd = new SqlCommand("Proveedor_Modificar");
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@IdProveedor", proveedor.IdProveedor));
            cmd.Parameters.Add(new SqlParameter("@Nombre", proveedor.Nombre));
            cmd.Parameters.Add(new SqlParameter("@CUIT", proveedor.CUIT));
            cmd.Parameters.Add(new SqlParameter("@Telefono", string.IsNullOrWhiteSpace(proveedor.Telefono) ? (object)DBNull.Value : proveedor.Telefono));
            cmd.Parameters.Add(new SqlParameter("@Correo", proveedor.Correo));

            return Conexion486LP.EjecutarNoConsulta(cmd);
        }

        public int Eliminar(int idProveedor)
        {
            SqlCommand cmd = new SqlCommand("Proveedor_Eliminar");
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@IdProveedor", idProveedor));

            return Conexion486LP.EjecutarNoConsulta(cmd);
        }

        public bool ExisteNombre(string nombre, int idExcluir)
        {
            SqlCommand cmd = new SqlCommand("Proveedor_ExisteNombre");
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@Nombre", nombre));
            cmd.Parameters.Add(new SqlParameter("@IdExcluir", idExcluir));

            return Convert.ToInt32(Conexion486LP.EjecutarEscalar(cmd)) > 0;
        }

        public bool ExisteCUIT(string cuit, int idExcluir)
        {
            SqlCommand cmd = new SqlCommand("Proveedor_ExisteCUIT");
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@CUIT", cuit));
            cmd.Parameters.Add(new SqlParameter("@IdExcluir", idExcluir));

            return Convert.ToInt32(Conexion486LP.EjecutarEscalar(cmd)) > 0;
        }

        public bool TieneMovimientos(int idProveedor)
        {
            SqlCommand cmd = new SqlCommand("Proveedor_TieneMovimientos");
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@IdProveedor", idProveedor));

            return Convert.ToInt32(Conexion486LP.EjecutarEscalar(cmd)) > 0;
        }
    }
}

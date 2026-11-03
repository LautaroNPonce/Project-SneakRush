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

    public class Mapper_Cliente486LP : MapperBase486LP
    {
        public List<Cliente486LP> Listar()
        {
            SqlCommand cmd = new SqlCommand("Cliente_Listar");
            cmd.CommandType = CommandType.StoredProcedure;

            DataTable tabla = Conexion486LP.EjecutarConsulta(cmd);
            return ManejadorMapeo486LP.MapearLista<Cliente486LP>(tabla);
        }

        public Cliente486LP Obtener(string dni)
        {
            SqlCommand cmd = new SqlCommand("Cliente_Obtener");
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@DNI", dni));

            DataTable tabla = Conexion486LP.EjecutarConsulta(cmd);
            return tabla.Rows.Count > 0 ? ManejadorMapeo486LP.MapearEntidad<Cliente486LP>(tabla.Rows[0]) : null;
        }

        public bool Existe(string dni)
        {
            SqlCommand cmd = new SqlCommand("Cliente_Existe");
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@DNI", dni));

            int cantidad = Convert.ToInt32(Conexion486LP.EjecutarEscalar(cmd));
            return cantidad > 0;
        }

        public int Agregar(Cliente486LP cliente)
        {
            SqlCommand cmd = new SqlCommand("Cliente_Agregar");
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@DNI", cliente.DNI));
            cmd.Parameters.Add(new SqlParameter("@Nombre", cliente.Nombre));
            cmd.Parameters.Add(new SqlParameter("@Apellido", cliente.Apellido));
            cmd.Parameters.Add(new SqlParameter("@Correo", cliente.Correo));
            cmd.Parameters.Add(new SqlParameter("@Telefono", (object)cliente.Telefono ?? DBNull.Value));

            return Conexion486LP.EjecutarNoConsulta(cmd);
        }

        public int Modificar(Cliente486LP cliente)
        {
            SqlCommand cmd = new SqlCommand("Cliente_Modificar");
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@DNI", cliente.DNI));
            cmd.Parameters.Add(new SqlParameter("@Nombre", cliente.Nombre));
            cmd.Parameters.Add(new SqlParameter("@Apellido", cliente.Apellido));
            cmd.Parameters.Add(new SqlParameter("@Correo", cliente.Correo));
            cmd.Parameters.Add(new SqlParameter("@Telefono", (object)cliente.Telefono ?? DBNull.Value));

            return Conexion486LP.EjecutarNoConsulta(cmd);
        }

        public int Eliminar(string dni)
        {
            SqlCommand cmd = new SqlCommand("Cliente_Eliminar");
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@DNI", dni));

            return Conexion486LP.EjecutarNoConsulta(cmd);
        }

        public bool TieneVentas(string dni)
        {
            SqlCommand cmd = new SqlCommand("Cliente_TieneVentas");
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@DNI", dni));

            return Convert.ToInt32(Conexion486LP.EjecutarEscalar(cmd)) > 0;
        }
    }
}

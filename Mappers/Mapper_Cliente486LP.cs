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


    /// Habla con SQL para la entidad Cliente. Reemplaza a DAL_Cliente486LP. Arma el SqlCommand (SP + parametros) y se lo pasa a Conexion486LP para que lo ejecute 
   /// (conectar/leer/escribir/desconectar es responsabilidad e la DAL, no de este Mapper). El mapeo fila-a-entidad lo hace ManejadorMapeo486LP (reflexion + atributo).

    public class Mapper_Cliente486LP : MapperBase486LP
    {
        // Lista todos los clientes.
        public List<Cliente486LP> Listar()
        {
            try
            {
                SqlCommand cmd = new SqlCommand("Cliente_Listar");
                cmd.CommandType = CommandType.StoredProcedure;

                DataTable tabla = Conexion486LP.EjecutarConsulta(cmd);
                return ManejadorMapeo486LP.MapearLista<Cliente486LP>(tabla);
            }
            catch
            {
                return new List<Cliente486LP>();
            }
        }

        // Obtiene un cliente por su DNI (devuelve null si no existe).
        public Cliente486LP Obtener(string dni)
        {
            try
            {
                SqlCommand cmd = new SqlCommand("Cliente_Obtener");
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@DNI", dni));

                DataTable tabla = Conexion486LP.EjecutarConsulta(cmd);
                return tabla.Rows.Count > 0 ? ManejadorMapeo486LP.MapearEntidad<Cliente486LP>(tabla.Rows[0]) : null;
            }
            catch
            {
                return null;
            }
        }

        // Indica si ya existe un cliente con ese DNI.
        public bool Existe(string dni)
        {
            try
            {
                SqlCommand cmd = new SqlCommand("Cliente_Existe");
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@DNI", dni));

                int cantidad = Convert.ToInt32(Conexion486LP.EjecutarEscalar(cmd));
                return cantidad > 0;
            }
            catch
            {
                return false;
            }
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

        // Modifica los datos de un cliente (por DNI). El Correo llega YA cifrado desde la BLL.
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

        // Baja fisica de un cliente por DNI.
        public int Eliminar(string dni)
        {
            SqlCommand cmd = new SqlCommand("Cliente_Eliminar");
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@DNI", dni));

            return Conexion486LP.EjecutarNoConsulta(cmd);
        }
    }
}

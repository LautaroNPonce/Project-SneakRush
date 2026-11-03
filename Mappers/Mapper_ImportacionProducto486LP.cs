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
    public class Mapper_ImportacionProducto486LP : MapperBase486LP
    {
        public ImportacionProducto486LP ObtenerPorRemito(int idProveedor, string nroRemito)
        {
            SqlCommand cmd = new SqlCommand("ImportacionProducto_ObtenerPorRemito");
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@IdProveedor", idProveedor));
            cmd.Parameters.Add(new SqlParameter("@NroRemito", nroRemito));

            DataTable tabla = Conexion486LP.EjecutarConsulta(cmd);
            return tabla.Rows.Count > 0 ? ManejadorMapeo486LP.MapearEntidad<ImportacionProducto486LP>(tabla.Rows[0]) : null;
        }

        public ImportacionProducto486LP ObtenerPorHuella(int idProveedor, string huella)
        {
            SqlCommand cmd = new SqlCommand("ImportacionProducto_ObtenerPorHuella");
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@IdProveedor", idProveedor));
            cmd.Parameters.Add(new SqlParameter("@HashContenido", huella));

            DataTable tabla = Conexion486LP.EjecutarConsulta(cmd);
            return tabla.Rows.Count > 0 ? ManejadorMapeo486LP.MapearEntidad<ImportacionProducto486LP>(tabla.Rows[0]) : null;
        }

        public int Crear(int idProveedor, string nroRemito, string usuario, string nombreArchivo, string huella)
        {
            SqlCommand cmd = new SqlCommand("ImportacionProducto_Crear");
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@IdProveedor", idProveedor));
            cmd.Parameters.Add(new SqlParameter("@NroRemito", nroRemito));
            cmd.Parameters.Add(new SqlParameter("@Usuario", usuario));
            cmd.Parameters.Add(new SqlParameter("@NombreArchivo", nombreArchivo));
            cmd.Parameters.Add(new SqlParameter("@HashContenido", huella));

            return Convert.ToInt32(Conexion486LP.EjecutarEscalar(cmd));
        }

        public string Cerrar(int idImportacion, string usuario, string nombreArchivo)
        {
            SqlCommand cmd = new SqlCommand("ImportacionProducto_Cerrar");
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@IdImportacion", idImportacion));
            cmd.Parameters.Add(new SqlParameter("@Usuario", usuario));
            cmd.Parameters.Add(new SqlParameter("@NombreArchivo", nombreArchivo));

            return Convert.ToString(Conexion486LP.EjecutarEscalar(cmd));
        }

        public List<DetalleImportacion486LP> ListarDetalle(int idImportacion)
        {
            SqlCommand cmd = new SqlCommand("DetalleImportacion_ListarPorImportacion");
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@IdImportacion", idImportacion));

            DataTable tabla = Conexion486LP.EjecutarConsulta(cmd);
            return ManejadorMapeo486LP.MapearLista<DetalleImportacion486LP>(tabla);
        }

        public string ProcesarLinea(int idImportacion, int linea, int idMarca, int idCategoria, string marca, string categoria, string modelo, string color, string talle, int cantidad, decimal precio, out int idProducto)
        {
            SqlCommand cmd = new SqlCommand("DetalleImportacion_ProcesarLinea");
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@IdImportacion", idImportacion));
            cmd.Parameters.Add(new SqlParameter("@Linea", linea));
            cmd.Parameters.Add(new SqlParameter("@IdMarca", idMarca));
            cmd.Parameters.Add(new SqlParameter("@IdCategoria", idCategoria));
            cmd.Parameters.Add(new SqlParameter("@Marca", marca));
            cmd.Parameters.Add(new SqlParameter("@Categoria", categoria));
            cmd.Parameters.Add(new SqlParameter("@Modelo", modelo));
            cmd.Parameters.Add(new SqlParameter("@Color", color));
            cmd.Parameters.Add(new SqlParameter("@Talle", talle));
            cmd.Parameters.Add(new SqlParameter("@Cantidad", cantidad));
            cmd.Parameters.Add(new SqlParameter("@Precio", precio));

            DataTable tabla = Conexion486LP.EjecutarConsulta(cmd);
            DataRow fila = tabla.Rows[0];
            idProducto = fila["IdProducto"] == DBNull.Value ? 0 : Convert.ToInt32(fila["IdProducto"]);
            return Convert.ToString(fila["Accion"]);
        }

        public int RegistrarRechazo(int idImportacion, int linea, string marca, string categoria, string modelo, string color, string talle, string cantidad, string precio, string motivo)
        {
            SqlCommand cmd = new SqlCommand("DetalleImportacion_RegistrarRechazo");
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@IdImportacion", idImportacion));
            cmd.Parameters.Add(new SqlParameter("@Linea", linea));
            cmd.Parameters.Add(new SqlParameter("@Marca", marca));
            cmd.Parameters.Add(new SqlParameter("@Categoria", categoria));
            cmd.Parameters.Add(new SqlParameter("@Modelo", modelo));
            cmd.Parameters.Add(new SqlParameter("@Color", color));
            cmd.Parameters.Add(new SqlParameter("@Talle", talle));
            cmd.Parameters.Add(new SqlParameter("@Cantidad", cantidad));
            cmd.Parameters.Add(new SqlParameter("@Precio", precio));
            cmd.Parameters.Add(new SqlParameter("@Motivo", motivo));

            return Conexion486LP.EjecutarNoConsulta(cmd);
        }
    }
}

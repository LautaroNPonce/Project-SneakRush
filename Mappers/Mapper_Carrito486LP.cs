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

    /// Para la transaccion (cabecera + N detalles, todo o nada), el Mapper le pide a Conexion486LP una conexion ABIERTA (AbrirConexion) y arranca su propia
    /// SqlTransaction. Cada operacion individual sigue pasando por la DAL, usando las sobrecargas "EnTransaccion" para compartir la misma conexion+transaccion.

    public class Mapper_Carrito486LP : MapperBase486LP
    {
        // Guarda el carrito (cabecera) y sus detalles en una transaccion.
        // Devuelve el IdCarrito generado (o 0 si falla).
        public int Guardar(Carrito486LP carrito, out string Mensaje)
        {
            int idGenerado = 0;
            Mensaje = string.Empty;

            SqlConnection con = Conexion486LP.AbrirConexion();
            SqlTransaction tran = con.BeginTransaction();

            try
            {
                // Insertar la cabecera del carrito y recuperar su Id
                SqlCommand cmdCab = new SqlCommand("Carrito_Agregar");
                cmdCab.CommandType = CommandType.StoredProcedure;
                cmdCab.Parameters.Add(new SqlParameter("@DNICliente", carrito.DNICliente));
                cmdCab.Parameters.Add(new SqlParameter("@Fecha", carrito.Fecha));
                cmdCab.Parameters.Add(new SqlParameter("@Total", carrito.Total));
                cmdCab.Parameters.Add(new SqlParameter("@Estado", carrito.Estado));
                idGenerado = (int)Conexion486LP.EjecutarEscalarEnTransaccion(cmdCab, con, tran);

                // Insertar cada renglon del detalle
                foreach (DetalleCarrito486LP det in carrito.Detalles)
                {
                    SqlCommand cmdDet = new SqlCommand("DetalleCarrito_Agregar");
                    cmdDet.CommandType = CommandType.StoredProcedure;
                    cmdDet.Parameters.Add(new SqlParameter("@IdCarrito", idGenerado));
                    cmdDet.Parameters.Add(new SqlParameter("@IdProducto", det.IdProducto));
                    cmdDet.Parameters.Add(new SqlParameter("@Cantidad", det.Cantidad));
                    cmdDet.Parameters.Add(new SqlParameter("@Precio", det.Precio));
                    cmdDet.Parameters.Add(new SqlParameter("@Subtotal", det.Subtotal));
                    Conexion486LP.EjecutarNoConsultaEnTransaccion(cmdDet, con, tran);
                }

                tran.Commit();
            }
            catch (Exception ex)
            {
                tran.Rollback();
                idGenerado = 0;
                Mensaje = ex.Message;
            }
            finally
            {
                con.Close();
            }

            return idGenerado;
        }

        // Obtiene un carrito por su Id (cabecera + detalles).
        public Carrito486LP Obtener(int idCarrito)
        {
            try
            {
                // Cabecera
                SqlCommand cmdCab = new SqlCommand("Carrito_Obtener");
                cmdCab.CommandType = CommandType.StoredProcedure;
                cmdCab.Parameters.Add(new SqlParameter("@IdCarrito", idCarrito));

                DataTable tablaCab = Conexion486LP.EjecutarConsulta(cmdCab);
                if (tablaCab.Rows.Count == 0) return null;

                Carrito486LP carrito = ManejadorMapeo486LP.MapearEntidad<Carrito486LP>(tablaCab.Rows[0]);

                // Detalles (Producto queda null: se resuelve aparte si hace falta,
                // igual que hacia el codigo original).
                SqlCommand cmdDet = new SqlCommand("DetalleCarrito_ListarPorCarrito");
                cmdDet.CommandType = CommandType.StoredProcedure;
                cmdDet.Parameters.Add(new SqlParameter("@IdCarrito", idCarrito));

                DataTable tablaDet = Conexion486LP.EjecutarConsulta(cmdDet);
                carrito.Detalles = ManejadorMapeo486LP.MapearLista<DetalleCarrito486LP>(tablaDet);

                return carrito;
            }
            catch
            {
                return null;
            }
        }
    }
}

using BE;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class DAL_Carrito486LP
    {
        // Guarda el carrito (cabecera) y sus detalles en una transaccion.
        // Devuelve el IdCarrito generado (o 0 si falla).
        public int Guardar(Carrito486LP carrito, out string Mensaje)
        {
            int idGenerado = 0;
            Mensaje = string.Empty;

            using (SqlConnection con = new SqlConnection(Conexion486LP.BD))
            {
                con.Open();
                SqlTransaction tran = con.BeginTransaction();

                try
                {
                    // Insertar la cabecera del carrito y recuperar su Id
                    string queryCab = @"INSERT INTO Carrito (DNICliente, Fecha, Total, Estado)
                    VALUES (@DNICliente, @Fecha, @Total, @Estado);
                    SELECT CAST(SCOPE_IDENTITY() AS int);";

                    SqlCommand cmdCab = new SqlCommand(queryCab, con, tran);
                    cmdCab.Parameters.AddWithValue("@DNICliente", carrito.DNICliente);
                    cmdCab.Parameters.AddWithValue("@Fecha", carrito.Fecha);
                    cmdCab.Parameters.AddWithValue("@Total", carrito.Total);
                    cmdCab.Parameters.AddWithValue("@Estado", carrito.Estado);
                    idGenerado = (int)cmdCab.ExecuteScalar();

                    // Insertar cada renglon del detalle
                    foreach (DetalleCarrito486LP det in carrito.Detalles)
                    {
                        string queryDet = @"INSERT INTO DetalleCarrito (IdCarrito, IdProducto, Cantidad, Precio, Subtotal)
                        VALUES (@IdCarrito, @IdProducto, @Cantidad, @Precio, @Subtotal)";

                        SqlCommand cmdDet = new SqlCommand(queryDet, con, tran);
                        cmdDet.Parameters.AddWithValue("@IdCarrito", idGenerado);
                        cmdDet.Parameters.AddWithValue("@IdProducto", det.IdProducto);
                        cmdDet.Parameters.AddWithValue("@Cantidad", det.Cantidad);
                        cmdDet.Parameters.AddWithValue("@Precio", det.Precio);
                        cmdDet.Parameters.AddWithValue("@Subtotal", det.Subtotal);
                        cmdDet.ExecuteNonQuery();
                    }

                    tran.Commit();
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    idGenerado = 0;
                    Mensaje = ex.Message;
                }
            }

            return idGenerado;
        }

        // Obtiene un carrito por su Id (cabecera + detalles)
        public Carrito486LP Obtener(int idCarrito)
        {
            Carrito486LP carrito = null;

            try
            {
                using (SqlConnection con = new SqlConnection(Conexion486LP.BD))
                {
                    con.Open();

                    // Cabecera
                    string queryCab = @"SELECT IdCarrito, DNICliente, Fecha, Total, Estado
                    FROM Carrito WHERE IdCarrito = @IdCarrito";

                    SqlCommand cmdCab = new SqlCommand(queryCab, con);
                    cmdCab.Parameters.AddWithValue("@IdCarrito", idCarrito);

                    using (SqlDataReader dr = cmdCab.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            carrito = new Carrito486LP()
                            {
                                IdCarrito = Convert.ToInt32(dr["IdCarrito"]),
                                DNICliente = dr["DNICliente"].ToString(),
                                Fecha = Convert.ToDateTime(dr["Fecha"]),
                                Total = Convert.ToDecimal(dr["Total"]),
                                Estado = dr["Estado"].ToString()
                            };
                        }
                    }

                    // Detalles
                    if (carrito != null)
                    {
                        string queryDet = @"SELECT IdDetalle, IdCarrito, IdProducto, Cantidad, Precio, Subtotal
                        FROM DetalleCarrito WHERE IdCarrito = @IdCarrito";

                        SqlCommand cmdDet = new SqlCommand(queryDet, con);
                        cmdDet.Parameters.AddWithValue("@IdCarrito", idCarrito);

                        using (SqlDataReader dr = cmdDet.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                carrito.Detalles.Add(new DetalleCarrito486LP()
                                {
                                    IdDetalle = Convert.ToInt32(dr["IdDetalle"]),
                                    IdCarrito = Convert.ToInt32(dr["IdCarrito"]),
                                    IdProducto = Convert.ToInt32(dr["IdProducto"]),
                                    Cantidad = Convert.ToInt32(dr["Cantidad"]),
                                    Precio = Convert.ToDecimal(dr["Precio"]),
                                    Subtotal = Convert.ToDecimal(dr["Subtotal"])
                                });
                            }
                        }
                    }
                }
            }
            catch
            {
                carrito = null;
            }

            return carrito;
        }
    }
}

using BE;
using Mappers;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class BLL_OrdenCompra486LP
    {
        private Mapper_OrdenCompra486LP ObjetoMapper = new Mapper_OrdenCompra486LP();
        private BLL_SolicitudCompra486LP ObjSolicitud = new BLL_SolicitudCompra486LP();
        private BLL_Bitacora486LP ObjBitacora = new BLL_Bitacora486LP();

        // Registra la orden a partir de lo armado en pantalla (Solicitud elegida, Proveedor,
        // Cantidad/Costo por producto). Fecha y CostoTotal los calcula/pone la BLL. Al persistir
        // con exito, cierra la Solicitud de origen pasandola a "Procesada" (ese mismo metodo
        // ya recalcula el DV de SolicitudCompra por su cuenta, no hace falta repetirlo aca).
        public OrdenCompra486LP RegistrarOrden(OrdenCompra486LP orden, out string mensaje)
        {
            mensaje = "";

            try
            {
                if (orden == null || orden.Detalles == null || orden.Detalles.Count == 0)
                {
                    mensaje = "La orden no tiene productos.";
                    return null;
                }
                if (orden.IdProveedor <= 0)
                {
                    mensaje = "Debe seleccionar un proveedor.";
                    return null;
                }

                orden.Fecha = DateTime.Now;
                orden.CostoTotal = orden.Detalles.Sum(d => d.Subtotal);

                bool ok = ObjetoMapper.Agregar(orden, out mensaje);
                if (!ok)
                {
                    ObjBitacora.Registrar(new BitacoraEvento486LP("Compras", $"Error al registrar orden de compra: {mensaje}", Criticidad486LP.MuyAlta, "Sistema", "Sistema"));
                    return null;
                }

                // La solicitud de origen pasa a "Procesada": no vuelve a aparecer como pendiente.
                string mensajeSolicitud;
                ObjSolicitud.ActualizarEstado(orden.IdSolicitud, "Procesada", out mensajeSolicitud);

                string dni = SessionManager486LP.ObtenerInstancia().UsuarioActual()?.DNI ?? "Sistema";
                string nombreUsuario = SessionManager486LP.ObtenerInstancia().UsuarioActual()?.NombreUsuario ?? "Sistema";

                ObjBitacora.Registrar(new BitacoraEvento486LP("Compras",
                    $"Orden de compra {orden.NroOrden} generada a partir de la Solicitud #{orden.IdSolicitud}, Costo Total ${orden.CostoTotal}.",
                    Criticidad486LP.Alta, dni, nombreUsuario));

                string mensajeDV;
                BLL_DV486LP bllDV = new BLL_DV486LP();
                bllDV.RecalcularDV("OrdenCompra", out mensajeDV);
                bllDV.RecalcularDV("DetalleOrdenCompra", out mensajeDV);

                mensaje = "Orden de compra registrada correctamente.";
                return orden;
            }
            catch (Exception ex)
            {
                mensaje = ex.Message;
                ObjBitacora.Registrar(new BitacoraEvento486LP("Compras", $"Error en BLL_OrdenCompra.RegistrarOrden(): {ex.Message}", Criticidad486LP.MuyAlta, "Sistema", "Sistema"));
                return null;
            }
        }

        // Trae una orden ya registrada, con su detalle (CUN07/08 la necesitan).
        public OrdenCompra486LP ObtenerPorId(int idOrden)
        {
            try
            {
                return ObjetoMapper.ObtenerPorId(idOrden);
            }
            catch (Exception ex)
            {
                ObjBitacora.Registrar(new BitacoraEvento486LP("Compras", $"Error en BLL_OrdenCompra.ObtenerPorId(): {ex.Message}", Criticidad486LP.MuyAlta, "Sistema", "Sistema"));
                return null;
            }
        }

        // Costo mas reciente pagado por este producto - dato de referencia para CUN06.
        public decimal? ObtenerUltimoCosto(int idProducto)
        {
            try
            {
                return ObjetoMapper.ObtenerUltimoCosto(idProducto);
            }
            catch (Exception ex)
            {
                ObjBitacora.Registrar(new BitacoraEvento486LP("Compras", $"Error en BLL_OrdenCompra.ObtenerUltimoCosto(): {ex.Message}", Criticidad486LP.MuyAlta, "Sistema", "Sistema"));
                return null;
            }
        }

        // Lista las ordenes en estado "Pendiente de Recepción" (CUN07 las necesita).
        public List<OrdenCompra486LP> ListarPendientesDeRecepcion()
        {
            try
            {
                return ObjetoMapper.ListarPendientesDeRecepcion();
            }
            catch (Exception ex)
            {
                ObjBitacora.Registrar(new BitacoraEvento486LP("Compras", $"Error en BLL_OrdenCompra.ListarPendientesDeRecepcion(): {ex.Message}", Criticidad486LP.MuyAlta, "Sistema", "Sistema"));
                return new List<OrdenCompra486LP>();
            }
        }

        // Cambia el Estado de la orden (ej. a "Recibida" en CUN07, o a "Facturada" en CUN08).
        // No se compara el conteo de filas de ExecuteNonQuery para decidir exito - mismo
        // criterio ya aplicado en el resto del proyecto.
        public bool ActualizarEstado(int idOrden, string estado, out string mensaje)
        {
            mensaje = "";
            try
            {
                bool ok = ObjetoMapper.ActualizarEstado(idOrden, estado, out mensaje);

                if (ok)
                {
                    string mensajeDV;
                    BLL_DV486LP bllDV = new BLL_DV486LP();
                    bllDV.RegistrarDVDeFilaNueva("OrdenCompra", idOrden, out mensajeDV);
                }

                return ok;
            }
            catch (Exception ex)
            {
                mensaje = ex.Message;
                ObjBitacora.Registrar(new BitacoraEvento486LP("Compras", $"Error en BLL_OrdenCompra.ActualizarEstado(): {ex.Message}", Criticidad486LP.MuyAlta, "Sistema", "Sistema"));
                return false;
            }
        }

        // Lista las ordenes en estado "Recibida" (CUN08 las necesita).
        public List<OrdenCompra486LP> ListarPendientesDeFacturacion()
        {
            try
            {
                return ObjetoMapper.ListarPendientesDeFacturacion();
            }
            catch (Exception ex)
            {
                ObjBitacora.Registrar(new BitacoraEvento486LP("Compras", $"Error en BLL_OrdenCompra.ListarPendientesDeFacturacion(): {ex.Message}", Criticidad486LP.MuyAlta, "Sistema", "Sistema"));
                return new List<OrdenCompra486LP>();
            }
        }
    }
}

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

        public OrdenCompra486LP RegistrarOrden(OrdenCompra486LP orden, out string mensaje)
        {
            mensaje = "";

            try
            {
                if (orden == null || orden.Detalles == null || orden.Detalles.Count == 0)
                {
                    mensaje = Mensajes486LP.Obtener("Compra.OrdenSinProductos"); // Muestra: La orden no tiene productos
                    return null;
                }
                if (orden.IdProveedor <= 0)
                {
                    mensaje = Mensajes486LP.Obtener("Compra.SinProveedor"); // Muestra: Debe seleccionar un proveedor
                    return null;
                }
                if (orden.Detalles.Any(d => d.Cantidad < 1 || d.Cantidad > 9999))
                {
                    mensaje = Mensajes486LP.Obtener("Compra.CantidadInvalida"); // Muestra: La cantidad debe estar entre 1 y 9999
                    return null;
                }
                if (orden.Detalles.Any(d => d.CostoUnitario <= 0 || d.CostoUnitario > 10000000m))
                {
                    mensaje = Mensajes486LP.Obtener("Compra.CostoInvalido"); // Muestra: El costo unitario debe ser mayor a cero y no puede superar $10.000.000
                    return null;
                }

                orden.Fecha = DateTime.Now;
                orden.CostoTotal = orden.Detalles.Sum(d => d.Subtotal);

                string mensajeMapper;
                bool ok = ObjetoMapper.Agregar(orden, out mensajeMapper);
                if (!ok)
                {
                    // El texto del Mapper es tecnico: va a la bitacora y el usuario ve un mensaje general
                    ObjBitacora.Registrar(new BitacoraEvento486LP("Compras", $"Error al registrar orden de compra: {mensajeMapper}", Criticidad486LP.MuyAlta, "Sistema", "Sistema"));
                    mensaje = Mensajes486LP.Obtener("General.ErrorInesperado"); // Muestra: Ocurrió un error inesperado. El detalle quedó registrado en la bitácora
                    return null;
                }

                // La solicitud de origen pasa a procesada xq no vuelve a aparecer como pendiente
                string mensajeSolicitud;
                if (!ObjSolicitud.ActualizarEstado(orden.IdSolicitud, "Procesada", out mensajeSolicitud))
                {
                    ObjBitacora.Registrar(new BitacoraEvento486LP("Compras", $"No se pudo pasar la Solicitud #{orden.IdSolicitud} a Procesada: {mensajeSolicitud}", Criticidad486LP.MuyAlta, "Sistema", "Sistema"));
                }

                string dni = SessionManager486LP.ObtenerInstancia().UsuarioActual()?.DNI ?? "Sistema";
                string nombreUsuario = SessionManager486LP.ObtenerInstancia().UsuarioActual()?.NombreUsuario ?? "Sistema";

                ObjBitacora.Registrar(new BitacoraEvento486LP("Compras",
                    $"Orden de compra {orden.NroOrden} generada a partir de la Solicitud #{orden.IdSolicitud}, Costo Total ${orden.CostoTotal}.",
                    Criticidad486LP.Alta, dni, nombreUsuario));

                string mensajeDV;
                BLL_DV486LP bllDV = new BLL_DV486LP();
                bllDV.RecalcularDV("OrdenCompra", out mensajeDV);
                bllDV.RecalcularDV("DetalleOrdenCompra", out mensajeDV);

                mensaje = Mensajes486LP.Obtener("Compra.OrdenRegistrada"); // Muestra: Orden de compra registrada correctamente
                return orden;
            }
            catch (Exception ex)
            {
                mensaje = Mensajes486LP.Obtener("General.ErrorInesperado"); // Muestra: Ocurrió un error inesperado. El detalle quedó registrado en la bitácora
                ObjBitacora.Registrar(new BitacoraEvento486LP("Compras", $"Error en BLL_OrdenCompra.RegistrarOrden(): {ex.Message}", Criticidad486LP.MuyAlta, "Sistema", "Sistema"));
                return null;
            }
        }

        // Trae una orden ya registrada, con su detalle
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

        // Cambia el Estado de la orden (recibida en CUN07 y facturada en CUN08)
        // No se compara el conteo de filas afectadas para decidir éxito - mismo criterio de siempre
        public bool ActualizarEstado(int idOrden, string estado, out string mensaje)
        {
            mensaje = "";
            try
            {
                string mensajeMapper;
                bool ok = ObjetoMapper.ActualizarEstado(idOrden, estado, out mensajeMapper);

                if (!ok)
                {
                    ObjBitacora.Registrar(new BitacoraEvento486LP("Compras", $"Error al actualizar el estado de la orden #{idOrden} a '{estado}': {mensajeMapper}", Criticidad486LP.MuyAlta, "Sistema", "Sistema"));
                    mensaje = Mensajes486LP.Obtener("General.ErrorInesperado"); // Muestra: Ocurrió un error inesperado. El detalle quedó registrado en la bitácora
                }

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
                mensaje = Mensajes486LP.Obtener("General.ErrorInesperado"); // Muestra: Ocurrió un error inesperado. El detalle quedó registrado en la bitácora
                ObjBitacora.Registrar(new BitacoraEvento486LP("Compras", $"Error en BLL_OrdenCompra.ActualizarEstado(): {ex.Message}", Criticidad486LP.MuyAlta, "Sistema", "Sistema"));
                return false;
            }
        }
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

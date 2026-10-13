using BE;
using Mappers;
using Mappers.Mappers;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class BLL_Recepcion486LP
    {
        private Mapper_Recepcion486LP ObjetoMapper = new Mapper_Recepcion486LP();
        private BLL_OrdenCompra486LP ObjOrden = new BLL_OrdenCompra486LP();
        private BLL_Bitacora486LP ObjBitacora = new BLL_Bitacora486LP();

        // Registra la recepción armada en pantalla (Cantidad Recibida/Faltante ya calculadas ahí)
        // Al persistir, cierra la Orden de origen Recibida el stock no se toca acá, eso ocurre recién cuando se confirma el pago.
        public Recepcion486LP RegistrarRecepcion(Recepcion486LP recepcion, out string mensaje)
        {
            mensaje = "";

            try
            {
                if (recepcion == null || recepcion.Detalles == null || recepcion.Detalles.Count == 0)
                {
                    mensaje = Mensajes486LP.Obtener("Compra.RecepcionSinProductos"); // Muestra: La recepción no tiene productos
                    return null;
                }
                if (recepcion.Detalles.Any(d => d.CantidadRecibida < 0 || d.CantidadRecibida > d.CantidadPedida))
                {
                    mensaje = Mensajes486LP.Obtener("Compra.CantidadRecibidaInvalida"); // Muestra: La cantidad recibida debe estar entre 0 y la cantidad pedida
                    return null;
                }
                if (recepcion.Detalles.Sum(d => d.CantidadRecibida) == 0)
                {
                    mensaje = Mensajes486LP.Obtener("Compra.RecepcionSinUnidades"); // Muestra: Debe recibir al menos una unidad para registrar la recepción
                    return null;
                }

                recepcion.Fecha = DateTime.Now;

                string mensajeMapper;
                bool ok = ObjetoMapper.Agregar(recepcion, out mensajeMapper);
                if (!ok)
                {
                    // El texto del Mapper es tecnico: va a la bitacora y el usuario ve un mensaje general
                    ObjBitacora.Registrar(new BitacoraEvento486LP("Compras", $"Error al registrar recepción de mercadería: {mensajeMapper}", Criticidad486LP.MuyAlta, "Sistema", "Sistema"));
                    mensaje = Mensajes486LP.Obtener("General.ErrorInesperado"); // Muestra: Ocurrió un error inesperado. El detalle quedó registrado en la bitácora
                    return null;
                }

                // La orden de origen pasa a recibida, no vuelve a aparecer como pendiente de recepcion
                string mensajeOrden;
                if (!ObjOrden.ActualizarEstado(recepcion.IdOrden, "Recibida", out mensajeOrden))
                {
                    ObjBitacora.Registrar(new BitacoraEvento486LP("Compras", $"No se pudo pasar la Orden #{recepcion.IdOrden} a Recibida: {mensajeOrden}", Criticidad486LP.MuyAlta, "Sistema", "Sistema"));
                }

                string dni = SessionManager486LP.ObtenerInstancia().UsuarioActual()?.DNI ?? "Sistema";
                string nombreUsuario = SessionManager486LP.ObtenerInstancia().UsuarioActual()?.NombreUsuario ?? "Sistema";

                int totalFaltante = recepcion.Detalles.Sum(d => d.CantidadFaltante);
                string detalleFaltante = totalFaltante > 0
                    ? $", con {totalFaltante} unidad(es) faltante(s)."
                    : ", sin faltantes.";

                ObjBitacora.Registrar(new BitacoraEvento486LP("Compras",
                    $"Recepción {recepcion.NroRecepcion} registrada para la Orden #{recepcion.IdOrden}{detalleFaltante}",
                    Criticidad486LP.Alta, dni, nombreUsuario));

                string mensajeDV;
                BLL_DV486LP bllDV = new BLL_DV486LP();
                bllDV.RecalcularDV("Recepcion", out mensajeDV);
                bllDV.RecalcularDV("DetalleRecepcion", out mensajeDV);

                mensaje = Mensajes486LP.Obtener("Compra.RecepcionRegistrada"); // Muestra: Recepción registrada correctamente
                return recepcion;
            }
            catch (Exception ex)
            {
                mensaje = Mensajes486LP.Obtener("General.ErrorInesperado"); // Muestra: Ocurrió un error inesperado. El detalle quedó registrado en la bitácora
                ObjBitacora.Registrar(new BitacoraEvento486LP("Compras", $"Error en BLL_Recepcion.RegistrarRecepcion(): {ex.Message}", Criticidad486LP.MuyAlta, "Sistema", "Sistema"));
                return null;
            }
        }

        // Trae la Recepcion de una Orden puntual, con su detalle (lo utilizo para saber la Cantidad Recibida real de cada producto, y asi aumentar el stock).
        public Recepcion486LP ObtenerPorIdOrden(int idOrden)
        {
            try
            {
                return ObjetoMapper.ObtenerPorIdOrden(idOrden);
            }
            catch (Exception ex)
            {
                ObjBitacora.Registrar(new BitacoraEvento486LP("Compras", $"Error en BLL_Recepcion.ObtenerPorIdOrden(): {ex.Message}", Criticidad486LP.MuyAlta, "Sistema", "Sistema"));
                return null;
            }
        }
    }
}

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
    public class BLL_SolicitudCompra486LP
    {
        private Mapper_SolicitudCompra486LP ObjetoMapper = new Mapper_SolicitudCompra486LP();
        private BLL_Bitacora486LP ObjBitacora = new BLL_Bitacora486LP();

        // Registra la solicitud armada en pantalla. Fecha, DNIAdministrador y Estado los pone la BLL, no el Form  el Administrador es siempre el usuario en sesión, y el Estado arranca en "Pendiente"
        public SolicitudCompra486LP RegistrarSolicitud(SolicitudCompra486LP solicitud, out string mensaje)
        {
            mensaje = "";

            try
            {
                if (solicitud == null || solicitud.Detalles == null || solicitud.Detalles.Count == 0)
                {
                    mensaje = Mensajes486LP.Obtener("Compra.SolicitudSinProductos"); // Muestra: La solicitud no tiene productos
                    return null;
                }
                if (solicitud.Detalles.Any(d => d.CantidadSolicitada < 1 || d.CantidadSolicitada > 9999))
                {
                    mensaje = Mensajes486LP.Obtener("Compra.CantidadInvalida"); // Muestra: La cantidad debe estar entre 1 y 9999
                    return null;
                }

                string dni = SessionManager486LP.ObtenerInstancia().UsuarioActual()?.DNI ?? "Sistema";
                string nombreUsuario = SessionManager486LP.ObtenerInstancia().UsuarioActual()?.NombreUsuario ?? "Sistema";

                solicitud.Fecha = DateTime.Now;
                solicitud.DNIAdministrador = dni;
                solicitud.Estado = "Pendiente";

                string mensajeMapper;
                bool ok = ObjetoMapper.Agregar(solicitud, out mensajeMapper);
                if (!ok)
                {
                    // El texto del Mapper es tecnico: va a la bitacora y el usuario ve un mensaje general
                    ObjBitacora.Registrar(new BitacoraEvento486LP("Compras", $"Error al registrar solicitud de compra: {mensajeMapper}", Criticidad486LP.MuyAlta, "Sistema", "Sistema"));
                    mensaje = Mensajes486LP.Obtener("General.ErrorInesperado"); // Muestra: Ocurrió un error inesperado. El detalle quedó registrado en la bitácora
                    return null;
                }

                ObjBitacora.Registrar(new BitacoraEvento486LP("Compras",
                    $"Solicitud de compra #{solicitud.IdSolicitud} generada, con {solicitud.Detalles.Count} producto(s).",
                    Criticidad486LP.Alta, dni, nombreUsuario));

                string mensajeDV;
                BLL_DV486LP bllDV = new BLL_DV486LP();
                bllDV.RecalcularDV("SolicitudCompra", out mensajeDV);
                bllDV.RecalcularDV("DetalleSolicitudCompra", out mensajeDV);

                mensaje = Mensajes486LP.Obtener("Compra.SolicitudGenerada"); // Muestra: Solicitud de compra generada correctamente
                return solicitud;
            }
            catch (Exception ex)
            {
                mensaje = Mensajes486LP.Obtener("General.ErrorInesperado"); // Muestra: Ocurrió un error inesperado. El detalle quedó registrado en la bitácora
                ObjBitacora.Registrar(new BitacoraEvento486LP("Compras", $"Error en BLL_SolicitudCompra.RegistrarSolicitud(): {ex.Message}", Criticidad486LP.MuyAlta, "Sistema", "Sistema"));
                return null;
            }
        }

        // Trae una solicitud ya registrada, con su detalle
        public SolicitudCompra486LP ObtenerPorId(int idSolicitud)
        {
            try
            {
                return ObjetoMapper.ObtenerPorId(idSolicitud);
            }
            catch (Exception ex)
            {
                ObjBitacora.Registrar(new BitacoraEvento486LP("Compras", $"Error en BLL_SolicitudCompra.ObtenerPorId(): {ex.Message}", Criticidad486LP.MuyAlta, "Sistema", "Sistema"));
                return null;
            }
        }

        // Lista las solicitudes en estado "Pendiente"
        public List<SolicitudCompra486LP> ListarPendientes()
        {
            try
            {
                return ObjetoMapper.ListarPendientes();
            }
            catch (Exception ex)
            {
                ObjBitacora.Registrar(new BitacoraEvento486LP("Compras", $"Error en BLL_SolicitudCompra.ListarPendientes(): {ex.Message}", Criticidad486LP.MuyAlta, "Sistema", "Sistema"));
                return new List<SolicitudCompra486LP>();
            }
        }

        public bool ActualizarEstado(int idSolicitud, string estado, out string mensaje)
        {
            mensaje = "";
            try
            {
                string mensajeMapper;
                bool ok = ObjetoMapper.ActualizarEstado(idSolicitud, estado, out mensajeMapper);

                if (!ok)
                {
                    ObjBitacora.Registrar(new BitacoraEvento486LP("Compras", $"Error al actualizar el estado de la solicitud #{idSolicitud} a '{estado}': {mensajeMapper}", Criticidad486LP.MuyAlta, "Sistema", "Sistema"));
                    mensaje = Mensajes486LP.Obtener("General.ErrorInesperado"); // Muestra: Ocurrió un error inesperado. El detalle quedó registrado en la bitácora
                }

                if (ok)
                {
                    string mensajeDV;
                    BLL_DV486LP bllDV = new BLL_DV486LP();
                    bllDV.RegistrarDVDeFilaNueva("SolicitudCompra", idSolicitud, out mensajeDV);
                }

                return ok;
            }
            catch (Exception ex)
            {
                mensaje = Mensajes486LP.Obtener("General.ErrorInesperado"); // Muestra: Ocurrió un error inesperado. El detalle quedó registrado en la bitácora
                ObjBitacora.Registrar(new BitacoraEvento486LP("Compras", $"Error en BLL_SolicitudCompra.ActualizarEstado(): {ex.Message}", Criticidad486LP.MuyAlta, "Sistema", "Sistema"));
                return false;
            }
        }
    }
}

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
                    mensaje = "La solicitud no tiene productos.";
                    return null;
                }

                string dni = SessionManager486LP.ObtenerInstancia().UsuarioActual()?.DNI ?? "Sistema";
                string nombreUsuario = SessionManager486LP.ObtenerInstancia().UsuarioActual()?.NombreUsuario ?? "Sistema";

                solicitud.Fecha = DateTime.Now;
                solicitud.DNIAdministrador = dni;
                solicitud.Estado = "Pendiente";

                bool ok = ObjetoMapper.Agregar(solicitud, out mensaje);
                if (!ok)
                {
                    ObjBitacora.Registrar(new BitacoraEvento486LP("Compras", $"Error al registrar solicitud de compra: {mensaje}", Criticidad486LP.MuyAlta, "Sistema", "Sistema"));
                    return null;
                }

                ObjBitacora.Registrar(new BitacoraEvento486LP("Compras",
                    $"Solicitud de compra #{solicitud.IdSolicitud} generada, con {solicitud.Detalles.Count} producto(s).",
                    Criticidad486LP.Alta, dni, nombreUsuario));

                string mensajeDV;
                BLL_DV486LP bllDV = new BLL_DV486LP();
                bllDV.RecalcularDV("SolicitudCompra", out mensajeDV);
                bllDV.RecalcularDV("DetalleSolicitudCompra", out mensajeDV);

                mensaje = "Solicitud de compra generada correctamente.";
                return solicitud;
            }
            catch (Exception ex)
            {
                mensaje = ex.Message;
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
                bool ok = ObjetoMapper.ActualizarEstado(idSolicitud, estado, out mensaje);

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
                mensaje = ex.Message;
                ObjBitacora.Registrar(new BitacoraEvento486LP("Compras", $"Error en BLL_SolicitudCompra.ActualizarEstado(): {ex.Message}", Criticidad486LP.MuyAlta, "Sistema", "Sistema"));
                return false;
            }
        }
    }
}

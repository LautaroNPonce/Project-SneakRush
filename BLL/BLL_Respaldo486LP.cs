using DAL;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class BLL_Respaldo486LP
    {
        private DAL_Respaldo486LP ObjetoDAL = new DAL_Respaldo486LP();
        private BLL_Bitacora486LP ObjBitacora = new BLL_Bitacora486LP();

        public bool Backup(string carpetaDestino, out string rutaArchivo, out string Mensaje)
        {
            rutaArchivo = string.Empty;
            Mensaje = string.Empty;

            if (string.IsNullOrWhiteSpace(carpetaDestino))
            {
                Mensaje = "Msg.Respaldo.CarpetaVacia"; // Debe seleccionar una carpeta de destino.
                return false;
            }

            try
            {
                bool resultado = ObjetoDAL.Backup(carpetaDestino, out rutaArchivo, out Mensaje);

                if (resultado)
                {
                    ObjBitacora.Registrar(new BitacoraEvento486LP("Gestión Respaldos",$"Backup completo de la BD generado en: {rutaArchivo}.",Criticidad486LP.Alta,
                        SessionManager486LP.ObtenerInstancia().UsuarioActual()?.DNI ?? "Sistema",
                        SessionManager486LP.ObtenerInstancia().UsuarioActual()?.NombreUsuario ?? "Sistema"));
                }

                return resultado;
            }
            catch (Exception ex)
            {
                ObjBitacora.Registrar(new BitacoraEvento486LP("Gestión Respaldos",$"Error en BLL_Respaldo.Backup(): {ex.Message}",Criticidad486LP.MuyAlta, "Sistema", "Sistema"));
                Mensaje = "Msg.Respaldo.ErrorBackup"; // Ocurrió un error inesperado al generar el backup.
                return false;
            }
        }

        public bool Restore(string rutaArchivo, out string Mensaje)
        {
            Mensaje = string.Empty;

            if (string.IsNullOrWhiteSpace(rutaArchivo))
            {
                Mensaje = "Msg.Respaldo.ArchivoVacio"; // Debe seleccionar un archivo .bak.
                return false;
            }

            try
            {
                bool resultado = ObjetoDAL.Restore(rutaArchivo, out Mensaje);

                if (resultado)
                {
                    try
                    {
                        ObjBitacora.Registrar(new BitacoraEvento486LP("Gestión Respaldos",$"Restauración completa de la BD desde: {rutaArchivo}.",Criticidad486LP.MuyAlta,
                            SessionManager486LP.ObtenerInstancia().UsuarioActual()?.DNI ?? "Sistema",
                            SessionManager486LP.ObtenerInstancia().UsuarioActual()?.NombreUsuario ?? "Sistema"));
                    }
                    catch (Exception exLog)
                    {
                        Mensaje = "Restore OK, pero no se pudo registrar en bitácora: " + exLog.Message;
                    }
                }

                return resultado;
            }
            catch (Exception ex)
            {
                ObjBitacora.Registrar(new BitacoraEvento486LP("Gestión Respaldos",$"Error en BLL_Respaldo.Restore(): {ex.Message}",Criticidad486LP.MuyAlta, "Sistema", "Sistema"));
                Mensaje = "Msg.Respaldo.ErrorRestore"; // Ocurrió un error inesperado al restaurar el backup.
                return false;
            }
        }
    }
}

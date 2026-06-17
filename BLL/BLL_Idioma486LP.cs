using DAL;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class BLL_Idioma486LP
    {
        private DAL_Idioma486LP _dal = new DAL_Idioma486LP();
        private BLL_Bitacora486LP _bllBitacora = new BLL_Bitacora486LP();

        public string ObtenerIdioma(int idUsuario)
        {
            return _dal.ObtenerIdioma(idUsuario);
        }

        public bool GuardarIdioma(int idUsuario, string nombreIdioma, out string mensaje)
        {
            mensaje = "";

            if (string.IsNullOrWhiteSpace(nombreIdioma))
            {
                mensaje = "El código de idioma no puede estar vacío.";
                return false;
            }

            if (nombreIdioma != "Español" && nombreIdioma != "Inglés" && nombreIdioma != "Portugués")
            {
                mensaje = "Idioma no válido.";
                return false;
            }

            bool resultado = _dal.GuardarIdioma(idUsuario, nombreIdioma, out mensaje);

            if (resultado)
            {
                string dni = SessionManager486LP.ObtenerInstancia().UsuarioActual()?.DNI ?? "";
                string nombreUsuario = SessionManager486LP.ObtenerInstancia().UsuarioActual()?.NombreUsuario ?? "Sistema";
                _bllBitacora.Registrar(new BitacoraEvento486LP("Cambiar Idioma",$"Usuario cambió idioma a '{nombreIdioma}'.",
                    Criticidad486LP.Baja, dni, nombreUsuario));
            }

            return resultado;
        }
    }
}

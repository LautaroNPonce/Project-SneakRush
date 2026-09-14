using DAL;
using Mappers;
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
        private Mapper_Idioma486LP _mapper = new Mapper_Idioma486LP();
        private BLL_Bitacora486LP _bllBitacora = new BLL_Bitacora486LP();

        public string ObtenerIdioma(int idUsuario)
        {
            return _mapper.ObtenerIdioma(idUsuario);
        }

        public bool GuardarIdioma(int idUsuario, string nombreIdioma, out string mensaje)
        {
            mensaje = "";

            if (string.IsNullOrWhiteSpace(nombreIdioma))
            {
                mensaje = "Msg.IdiomaVacio"; // El código de idioma no puede estar vacío.
                return false;
            }

            if (nombreIdioma != "Español" && nombreIdioma != "Inglés" && nombreIdioma != "Portugués")
            {
                mensaje = "Msg.IdiomaInvalido"; // Idioma no válido.
                return false;
            }

            bool resultado = _mapper.GuardarIdioma(idUsuario, nombreIdioma, out mensaje);

            if (resultado)
            {
                string dni = SessionManager486LP.ObtenerInstancia().UsuarioActual()?.DNI ?? "";
                string nombreUsuario = SessionManager486LP.ObtenerInstancia().UsuarioActual()?.NombreUsuario ?? "Sistema";
                _bllBitacora.Registrar(new BitacoraEvento486LP("Cambiar Idioma", $"Usuario cambió idioma a '{nombreIdioma}'.",
                    Criticidad486LP.Baja, dni, nombreUsuario));

                string mensajeDV;
                new BLL_DV486LP().RecalcularDV("Usuarios", out mensajeDV);
            }

            return resultado;
        }
    }
}

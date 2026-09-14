using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BE;

namespace Services
{
    public class SessionManager486LP
    {
        private Usuario486LP _usuario;
        private SessionManager486LP() { }

        public static SessionManager486LP Instancia { get; private set; } = null;

        public static SessionManager486LP ObtenerInstancia()
        {
            if (Instancia == null)
            {
                Instancia = new SessionManager486LP();
            }

            return Instancia;
        }

        public Usuario486LP UsuarioActual()
        {
            return _usuario;
        }

        public void LogIN(Usuario486LP usuario)
        {
            if (_usuario != null)
            {
                throw new InvalidOperationException("Ya hay una sesión activa. Cerrá la sesión antes de iniciar otra.");
            }

            _usuario = usuario;
        }

        public void LogOut()
        {
            _usuario = null;
        }

        public bool IsLogged()
        {
            return _usuario != null;
        }
    }
}

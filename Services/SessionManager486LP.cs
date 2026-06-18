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

        // Estado de sesión 
        private Usuario486LP _usuario;
        private string _idiomaActual;

        //Esto es parte de la segunda entrega
        //public string IdiomaActual
        //{
        //    get { return _idiomaActual; }
        //    set
        //    {
        //        _idiomaActual = value;
        //    }
        //}

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

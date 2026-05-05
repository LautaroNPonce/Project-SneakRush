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
        // Patron Singleton 
        private SessionManager486LP() { }
 
        public static SessionManager486LP Instancia { get; private set; } = null;
 
        public static SessionManager486LP ObtenerInstancia()
        {
            if (Instancia == null)
                Instancia = new SessionManager486LP();
 
            return Instancia;
        }
 
        // Estado de sesión 
        private Usuario486LP _usuario;
        private string _idiomaActual;
 
        public string IdiomaActual
        {
            get { return _idiomaActual; }
            set
            {
                _idiomaActual = value; // Se activa cuando implemento el patrón Observer de idioma
            }
        }
 
        // Devuelve el usuario actualmente logueado.
        public Usuario486LP UsuarioActual()
        {
            return _usuario;
        }
 
        // Inicia sesión guardando el usuario en la instancia
        public void LogIN(Usuario486LP usuario)
        {
            _usuario = usuario;
        }
 
        // Cierra sesión destruyendo la instancia.
        public void LogOut()
        {
            _usuario  = null;
            Instancia = null;
        }
 
        // Indica si hay un usuario logueado actualmente
        public bool IsLogged()
        {
            return _usuario != null;
        }
    }
}

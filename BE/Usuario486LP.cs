using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BE
{
    public class Usuario486LP
    {
        // Identidad
        public int IdUsuario { get; set; }
        public string DNI { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Email { get; set; }
        public string NombreUsuario { get; set; }

        // Seguridad
        public string Contraseña { get; set; }
        public bool Activo { get; set; }
        public bool Bloqueado { get; set; }
        public int IntentosFallidos { get; set; }

        // Perfil y rol
        public string Rol { get; set; }
        public int? IdPerfil { get; set; }
        public string NombreIdioma { get; set; }

        //Dígito verificador
        //public string DV { get; set; }

        // Constructores
        public Usuario486LP() { }

        public Usuario486LP(int idUsuario, string dni, string nombre, string apellido, string email, string nombreUsuario, string contraseña,bool activo, bool bloqueado, int intentosFallidos,string rol, string dv, int? idPerfil, string nombreIdioma)
        {
            IdUsuario = idUsuario;
            DNI = dni;
            Nombre = nombre;
            Apellido = apellido;
            Email = email;
            NombreUsuario = nombreUsuario;
            Contraseña = contraseña;
            Activo = activo;
            Bloqueado = bloqueado;
            IntentosFallidos = intentosFallidos;
            Rol = rol;
            //DV = dv;
            IdPerfil = idPerfil;
            NombreIdioma = nombreIdioma;
        }

        // Es medio innecesario pero puede ayudar a la hora de debuggear o mostrar información del usuario en la UI
        public override string ToString()
        {
            return $"{NombreUsuario} — {Nombre} {Apellido} ({Rol})";
        }
    }
}

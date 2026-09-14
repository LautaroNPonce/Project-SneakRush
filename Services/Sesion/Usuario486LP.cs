using BE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class Usuario486LP
    {
        // Identidad
        [ColumnaTabla486LP]
        public int IdUsuario { get; set; }
        [ColumnaTabla486LP]
        public string DNI { get; set; }
        [ColumnaTabla486LP]
        public string Nombre { get; set; }
        [ColumnaTabla486LP]
        public string Apellido { get; set; }
        [ColumnaTabla486LP]
        public string Email { get; set; }
        [ColumnaTabla486LP]
        public string NombreUsuario { get; set; }

        // Seguridad
        [ColumnaTabla486LP]
        public string Contraseña { get; set; }
        [ColumnaTabla486LP]
        public bool Activo { get; set; }
        [ColumnaTabla486LP]
        public bool Bloqueado { get; set; }
        [ColumnaTabla486LP]
        public int IntentosFallidos { get; set; }
        [ColumnaTabla486LP]
        public bool DebeCambiarContraseña { get; set; }

        // Perfil y rol
        [ColumnaTabla486LP]
        public string Rol { get; set; }
        [ColumnaTabla486LP]
        public int? IdPerfil { get; set; }
        [ColumnaTabla486LP]
        public string NombreIdioma { get; set; }

        // Constructores
        public Usuario486LP() { }

        public Usuario486LP(int idUsuario, string dni, string nombre, string apellido, string email, string nombreUsuario, string contraseña, bool activo, bool bloqueado, int intentosFallidos, string rol, string dv, int? idPerfil, string nombreIdioma)
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
            IdPerfil = idPerfil;
            NombreIdioma = nombreIdioma;
        }

        public override string ToString()
        {
            return $"{NombreUsuario} — {Nombre} {Apellido} ({Rol})";
        }
    }
}

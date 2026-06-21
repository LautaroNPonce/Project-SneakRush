using BE;
using DAL;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL
{
    public class BLL_Usuarios486LP
    {
        private DAL_Usuarios486LP ObjetoDAL = new DAL_Usuarios486LP();
        private BLL_Bitacora486LP ObjBitacora = new BLL_Bitacora486LP();

        // Valida que la contraseña cumpla la política de seguridad (mínimo 8 caracteres, al menos una mayúscula y una minúscula)
        public bool ValidarContraseña(string contraseña, out string Mensaje)
        {
            Mensaje = string.Empty;

            if (string.IsNullOrEmpty(contraseña) || contraseña.Length < 8)
            {
                Mensaje = "Msg.ContraseñaMinimo"; // La contraseña debe tener al menos 8 caracteres.
                return false;
            }

            bool tieneMayuscula = false;
            bool tieneMinuscula = false;

            foreach (char c in contraseña)
            {
                if (char.IsUpper(c)) tieneMayuscula = true;
                if (char.IsLower(c)) tieneMinuscula = true;
            }

            if (!tieneMayuscula)
            {
                Mensaje = "Msg.ContraseñaMayuscula"; // La contraseña debe contener al menos una letra mayúscula.
                return false;
            }

            if (!tieneMinuscula)
            {
                Mensaje = "Msg.ContraseñaMinuscula"; // La contraseña debe contener al menos una letra minúscula.
                return false;
            }

            return true;
        }

        public int Login(string nombreUsuario, string contraseña, out Usuario486LP usuarioLogueado, out string Mensaje)
        {
            usuarioLogueado = null;
            Mensaje = string.Empty;
            try
            {
                Usuario486LP u = ObjetoDAL.ObtenerPorNombreUsuario(nombreUsuario);

                if (u == null)
                {
                    Mensaje = "Usuario no encontrado.";
                    return 0;
                }

                if (!u.Activo)
                {
                    Mensaje = "El usuario está inactivo. Contacte al administrador.";
                    return -2;
                }

                if (u.Bloqueado)
                {
                    Mensaje = "El usuario está bloqueado. Contacte al administrador.";
                    return -3;
                }

                string hashIngresado = Encriptacion486LP.GenerarHash(contraseña);

                if (hashIngresado != u.Contraseña)
                {
                    int intentos = u.IntentosFallidos + 1;
                    string msg;
                    ObjetoDAL.ActualizarIntentos(nombreUsuario, intentos, out msg);

                    if (intentos >= 3)
                    {
                        ObjetoDAL.Bloquear(nombreUsuario, out msg);
                        ObjBitacora.Registrar(new BitacoraEvento486LP("Login", $"Usuario {nombreUsuario} bloqueado por 3 intentos fallidos.", Criticidad486LP.MuyAlta, u.DNI, u.NombreUsuario));
                        Mensaje = "Usuario bloqueado por 3 intentos fallidos. Contacte al administrador.";
                        return -3;
                    }

                    ObjBitacora.Registrar(new BitacoraEvento486LP("Login", $"Intento fallido {intentos}/3 para {nombreUsuario}.", Criticidad486LP.MuyAlta, u.DNI, u.NombreUsuario));
                    Mensaje = $"Contraseña incorrecta. Intentos fallidos: {intentos}/3.";
                    return -1;
                }

                string mensaje;
                ObjetoDAL.ActualizarIntentos(nombreUsuario, 0, out mensaje);
                ObjBitacora.Registrar(new BitacoraEvento486LP("Login", $"Inicio de sesión exitoso: {nombreUsuario}.", Criticidad486LP.MuyAlta, u.DNI, u.NombreUsuario));
                usuarioLogueado = u;
                return 1;
            }
            catch (Exception ex)
            {
                ObjBitacora.Registrar(new BitacoraEvento486LP("Login", $"Error en BLL_Usuarios.Login(): {ex.Message}", Criticidad486LP.MuyAlta, "Sistema", "Sistema"));
                Mensaje = "Ocurrió un error inesperado al iniciar sesión.";
                return 0;
            }
        }

        public List<Usuario486LP> Listar()
        {
            try
            {
                return ObjetoDAL.Listar();
            }
            catch (Exception ex)
            {
                ObjBitacora.Registrar(new BitacoraEvento486LP("Gestión Usuarios", $"Error en BLL_Usuarios.Listar(): {ex.Message}", Criticidad486LP.MuyAlta, "Sistema"));
                return new List<Usuario486LP>();
            }
        }

        public List<Usuario486LP> ListarActivos()
        {
            try
            {
                return ObjetoDAL.ListarActivos();
            }
            catch (Exception ex)
            {
                ObjBitacora.Registrar(new BitacoraEvento486LP("Gestión Usuarios", $"Error en BLL_Usuarios.ListarActivos(): {ex.Message}", Criticidad486LP.MuyAlta, "Sistema"));
                return new List<Usuario486LP>();
            }
        }

        public bool Agregar(Usuario486LP obj, out string Mensaje, out string contraseñaTemporal)
        {
            Mensaje = string.Empty;
            contraseñaTemporal = string.Empty;

            try
            {
                if (string.IsNullOrEmpty(obj.DNI))
                {
                    { Mensaje = "Msg.Val.DNIObligatorio"; return false; } // El DNI es obligatorio.
                }

                if (string.IsNullOrEmpty(obj.Nombre))
                {
                    { Mensaje = "Msg.Val.NombreObligatorio"; return false; } // El nombre es obligatorio.
                }
                if (string.IsNullOrEmpty(obj.Apellido))
                {
                    { Mensaje = "Msg.Val.ApellidoObligatorio"; return false; } // El apellido es obligatorio.
                }
                if (string.IsNullOrEmpty(obj.NombreUsuario))
                {
                    { Mensaje = "Msg.Val.LoginObligatorio"; return false; } // El nombre de usuario es obligatorio.
                }
                if (string.IsNullOrEmpty(obj.Email))
                {
                    { Mensaje = "Msg.Val.EmailFormato"; return false; } // El correo es obligatorio.
                }
                if (!System.Text.RegularExpressions.Regex.IsMatch(obj.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                {
                    Mensaje = "Msg.Val.EmailFormato"; return false; // El formato del correo no es válido.
                }

                List<Usuario486LP> todos = ObjetoDAL.Listar();

                if (todos.Any(u => u.DNI == obj.DNI))
                {
                    { Mensaje = "Msg.BLL.DNIRegistrado"; return false; } // El DNI ingresado ya se encuentra registrado.
                }
                if (todos.Any(u => u.NombreUsuario.ToLower() == obj.NombreUsuario.ToLower()))
                {
                    { Mensaje = "Msg.BLL.LoginRegistrado"; return false; } // El nombre de usuario ya se encuentra registrado.
                }
                if (todos.Any(u => u.Email.ToLower() == obj.Email.ToLower()))
                {
                    Mensaje = "Msg.BLL.EmailRegistrado"; return false; // El correo electrónico ya se encuentra registrado.
                }

                contraseñaTemporal = obj.Apellido + obj.DNI;
                obj.Contraseña = Encriptacion486LP.GenerarHash(contraseñaTemporal);
                obj.Activo = true;
                obj.Bloqueado = false;
                obj.IntentosFallidos = 0;

                bool resultado = ObjetoDAL.Agregar(obj, out Mensaje);

                if (resultado)
                {
                    ObjBitacora.Registrar(new BitacoraEvento486LP("Gestión Usuarios", $"Usuario creado: {obj.NombreUsuario} (DNI: {obj.DNI}).", Criticidad486LP.Alta, SessionManager486LP.ObtenerInstancia().UsuarioActual()?.DNI ?? "Sistema",
                        SessionManager486LP.ObtenerInstancia().UsuarioActual()?.NombreUsuario ?? "Sistema"));

                    string mensajeDV;
                    BLL_DV486LP bllDV = new BLL_DV486LP();
                    bllDV.RecalcularDV("Usuarios", out mensajeDV);
                }

                return resultado;
            }
            catch (Exception ex)
            {
                ObjBitacora.Registrar(new BitacoraEvento486LP("Gestión Usuarios", $"Error en BLL_Usuarios.Agregar(): {ex.Message}", Criticidad486LP.MuyAlta, "Sistema", "Sistema"));
                Mensaje = "Msg.BLL.ErrorInesperadoAgregar"; // Ocurrió un error inesperado al agregar el usuario.
                return false;
            }
        }

        public bool Modificar(Usuario486LP obj, out string Mensaje)
        {
            Mensaje = string.Empty;

            try
            {
                if (string.IsNullOrEmpty(obj.Nombre))
                {
                    { Mensaje = "Msg.Val.NombreObligatorio"; return false; } // El nombre es obligatorio.
                }
                if (string.IsNullOrEmpty(obj.Apellido))
                {
                    { Mensaje = "Msg.Val.ApellidoObligatorio"; return false; } // El apellido es obligatorio.
                }
                if (string.IsNullOrEmpty(obj.Email))
                {
                    { Mensaje = "Msg.Val.EmailFormato"; return false; } // El correo es obligatorio.
                }
                if (!System.Text.RegularExpressions.Regex.IsMatch(obj.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                {
                    Mensaje = "Msg.Val.EmailFormato"; return false; // El formato del correo no es válido.
                }

                List<Usuario486LP> todos = ObjetoDAL.Listar();

                if (todos.Any(u => u.Email.ToLower() == obj.Email.ToLower() && u.IdUsuario != obj.IdUsuario))
                {
                    Mensaje = "Msg.BLL.EmailRegistradoOtro"; return false; // El correo electrónico ya se encuentra registrado por otro usuario.
                }

                bool resultado = ObjetoDAL.Modificar(obj, out Mensaje);

                if (resultado)
                {
                    ObjBitacora.Registrar(new BitacoraEvento486LP("Gestión Usuarios", $"Usuario modificado: {obj.NombreUsuario} (DNI: {obj.DNI}).", Criticidad486LP.Media, SessionManager486LP.ObtenerInstancia().UsuarioActual()?.DNI ?? "Sistema",
                        SessionManager486LP.ObtenerInstancia().UsuarioActual()?.NombreUsuario ?? "Sistema"));

                    string mensajeDV;
                    BLL_DV486LP bllDV = new BLL_DV486LP();
                    bllDV.RecalcularDV("Usuarios", out mensajeDV);
                }

                return resultado;
            }
            catch (Exception ex)
            {
                ObjBitacora.Registrar(new BitacoraEvento486LP("Gestión Usuarios", $"Error en BLL_Usuarios.Modificar(): {ex.Message}", Criticidad486LP.MuyAlta, "Sistema", "Sistema"));
                Mensaje = "Msg.BLL.ErrorInesperadoModificar"; // Ocurrió un error inesperado al modificar el usuario.
                return false;
            }
        }

        public bool Eliminar(int idUsuario, out string Mensaje)
        {
            try
            {
                bool resultado = ObjetoDAL.Eliminar(idUsuario, out Mensaje);

                if (resultado)
                {
                    ObjBitacora.Registrar(new BitacoraEvento486LP("Gestión Usuarios", $"Usuario eliminado. IdUsuario: {idUsuario}.", Criticidad486LP.Alta, SessionManager486LP.ObtenerInstancia().UsuarioActual()?.DNI ?? "Sistema",
                        SessionManager486LP.ObtenerInstancia().UsuarioActual()?.NombreUsuario ?? "Sistema"));

                    string mensajeDV;
                    BLL_DV486LP bllDV = new BLL_DV486LP();
                    bllDV.RecalcularDV("Usuarios", out mensajeDV);
                }

                return resultado;
            }
            catch (Exception ex)
            {
                ObjBitacora.Registrar(new BitacoraEvento486LP("Gestión Usuarios", $"Error en BLL_Usuarios.Eliminar(): {ex.Message}", Criticidad486LP.MuyAlta, "Sistema", "Sistema"));
                Mensaje = "Msg.BLL.ErrorEliminar"; // Ocurrió un error al eliminar el usuario.
                return false;
            }
        }

        public bool Desbloquear(string dni, out string Mensaje)
        {
            try
            {
                bool resultado = ObjetoDAL.Desbloquear(dni, out Mensaje);

                if (resultado)
                {
                    ObjBitacora.Registrar(new BitacoraEvento486LP("Gestión Usuarios", $"Usuario desbloqueado. DNI: {dni}.", Criticidad486LP.MuyAlta, SessionManager486LP.ObtenerInstancia().UsuarioActual()?.DNI ?? "Sistema",
                        SessionManager486LP.ObtenerInstancia().UsuarioActual()?.NombreUsuario ?? "Sistema"));

                    string mensajeDV;
                    BLL_DV486LP bllDV = new BLL_DV486LP();
                    bllDV.RecalcularDV("Usuarios", out mensajeDV);
                }

                return resultado;
            }
            catch (Exception ex)
            {
                ObjBitacora.Registrar(new BitacoraEvento486LP("Gestión Usuarios", $"Error en BLL_Usuarios.Desbloquear(): {ex.Message}", Criticidad486LP.MuyAlta, "Sistema", "Sistema"));
                Mensaje = "Msg.BLL.ErrorDesbloquear"; // Ocurrió un error al desbloquear el usuario.
                return false;
            }
        }

        public bool BloquearPorDNI(string dni, out string Mensaje)
        {
            try
            {
                bool resultado = ObjetoDAL.BloquearPorDNI(dni, out Mensaje);

                if (resultado)
                {
                    ObjBitacora.Registrar(new BitacoraEvento486LP("Gestión Usuarios", $"Usuario bloqueado manualmente. DNI: {dni}.", Criticidad486LP.MuyAlta, SessionManager486LP.ObtenerInstancia().UsuarioActual()?.DNI ?? "Sistema",
                        SessionManager486LP.ObtenerInstancia().UsuarioActual()?.NombreUsuario ?? "Sistema"));

                    string mensajeDV;
                    BLL_DV486LP bllDV = new BLL_DV486LP();
                    bllDV.RecalcularDV("Usuarios", out mensajeDV);
                }

                return resultado;
            }
            catch (Exception ex)
            {
                ObjBitacora.Registrar(new BitacoraEvento486LP("Gestión Usuarios", $"Error en BLL_Usuarios.BloquearPorDNI(): {ex.Message}", Criticidad486LP.MuyAlta, "Sistema", "Sistema"));
                Mensaje = "Msg.BLL.ErrorBloquear"; // Ocurrió un error al bloquear el usuario.
                return false;
            }
        }

        public bool InvertirActivo(string dni, out string Mensaje)
        {
            try
            {
                bool resultado = ObjetoDAL.InvertirActivo(dni, out Mensaje);

                if (resultado)
                {
                    ObjBitacora.Registrar(new BitacoraEvento486LP("Gestión Usuarios", $"Estado Activo invertido. DNI: {dni}.", Criticidad486LP.Media, SessionManager486LP.ObtenerInstancia().UsuarioActual()?.DNI ?? "Sistema",
                        SessionManager486LP.ObtenerInstancia().UsuarioActual()?.NombreUsuario ?? "Sistema"));

                    string mensajeDV;
                    BLL_DV486LP bllDV = new BLL_DV486LP();
                    bllDV.RecalcularDV("Usuarios", out mensajeDV);
                }

                return resultado;
            }
            catch (Exception ex)
            {
                ObjBitacora.Registrar(new BitacoraEvento486LP("Gestión Usuarios", $"Error en BLL_Usuarios.InvertirActivo(): {ex.Message}", Criticidad486LP.MuyAlta, "Sistema", "Sistema"));
                Mensaje = "Msg.BLL.ErrorInvertir"; // Ocurrió un error al cambiar el estado del usuario.
                return false;
            }
        }

        public bool CambiarContraseña(int idUsuario, string contraseñaActual, string contraseñaNueva, string contraseñaConfirmar, string dniUsuario, out string Mensaje)
        {
            Mensaje = string.Empty;
            try
            {
                if (contraseñaNueva != contraseñaConfirmar)
                {
                    { Mensaje = "Msg.ContraseñasNoCoinciden"; return false; } // La nueva contraseña y la confirmación no coinciden.
                }
                if (contraseñaNueva == contraseñaActual)
                {
                    { Mensaje = "Msg.ContraseñaIgualActual"; return false; } // La nueva contraseña no puede ser igual a la actual.
                }
                if (!ValidarContraseña(contraseñaNueva, out Mensaje))
                {
                    return false;
                }

                Usuario486LP u = ObjetoDAL.ObtenerPorNombreUsuario(SessionManager486LP.ObtenerInstancia().UsuarioActual()?.NombreUsuario ?? "");

                if (u == null)
                {
                    { Mensaje = "Msg.UsuarioSinSesion"; return false; } // No se pudo obtener el usuario de la sesión.
                }

                string hashActual = Encriptacion486LP.GenerarHash(contraseñaActual);

                if (hashActual != u.Contraseña)
                {
                    ObjBitacora.Registrar(new BitacoraEvento486LP("Cambiar Contraseña", "Intento fallido: contraseña actual incorrecta.", Criticidad486LP.MuyAlta, dniUsuario,
                        SessionManager486LP.ObtenerInstancia().UsuarioActual()?.NombreUsuario ?? "Sistema"));
                    Mensaje = "Msg.ContraseñaActualIncorrecta"; // La contraseña actual es incorrecta.
                    return false;
                }

                string hashNueva = Encriptacion486LP.GenerarHash(contraseñaNueva);
                bool resultado = ObjetoDAL.CambiarContraseña(idUsuario, hashNueva, out Mensaje);

                if (resultado)
                {
                    u.Contraseña = hashNueva;
                    ObjBitacora.Registrar(new BitacoraEvento486LP("Cambiar Contraseña", "Contraseña cambiada exitosamente.", Criticidad486LP.MuyAlta, dniUsuario,
                        SessionManager486LP.ObtenerInstancia().UsuarioActual()?.NombreUsuario ?? "Sistema"));

                    string mensajeDV;
                    BLL_DV486LP bllDV = new BLL_DV486LP();
                    bllDV.RecalcularDV("Usuarios", out mensajeDV);
                }

                return resultado;
            }
            catch (Exception ex)
            {
                ObjBitacora.Registrar(new BitacoraEvento486LP("Cambiar Contraseña", $"Error en BLL_Usuarios.CambiarContraseña(): {ex.Message}", Criticidad486LP.MuyAlta, dniUsuario, "Sistema"));
                Mensaje = "Msg.ErrorInesperado"; // Ocurrió un error inesperado al cambiar la contraseña.
                return false;
            }
        }

        public void Logout(string dni, out string mensaje)
        {
            mensaje = string.Empty;
            try
            {
                Usuario486LP usuarioActual = SessionManager486LP.ObtenerInstancia().UsuarioActual();

                BitacoraEvento486LP evento = new BitacoraEvento486LP()
                {
                    Fecha = DateTime.Now,
                    Modulo = "Logout",
                    Descripcion = "Cierre de sesión exitoso.",
                    Criticidad = Criticidad486LP.MuyAlta,
                    DNI = usuarioActual.DNI,
                    NombreUsuario = usuarioActual.NombreUsuario
                };

                DAL_Bitacora486LP dalBitacora = new DAL_Bitacora486LP();
                dalBitacora.Registrar(evento);

                SessionManager486LP.ObtenerInstancia().LogOut();
            }
            catch (Exception ex)
            {
                mensaje = "Error al cerrar sesión: " + ex.Message;
            }
        }
    }
}

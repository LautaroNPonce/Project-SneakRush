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

        private string CalcularDV(Usuario486LP u)
        {
            string datos = $"{u.IdUsuario}{u.DNI}{u.NombreUsuario}{u.Rol}";
            int suma = 0;
            foreach (char c in datos)
                suma += c;
            return (suma % 10).ToString();
        }

        // Genera una contraseña temporal de 8 caracteres (para crear usuarios).
        public static string GenerarContraseñaTemporal()
        {
            const string mayusculas = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string minusculas = "abcdefghijklmnopqrstuvwxyz";
            const string numeros = "0123456789";
            const string todos = mayusculas + minusculas + numeros;

            Random rng = new Random();
            char[] pass = new char[8];

            // Al menos 1 de cada tipo para cumplir la política
            pass[0] = mayusculas[rng.Next(mayusculas.Length)];
            pass[1] = minusculas[rng.Next(minusculas.Length)];
            pass[2] = numeros[rng.Next(numeros.Length)];

            for (int i = 3; i < 8; i++)
                pass[i] = todos[rng.Next(todos.Length)];

            // Mezclar posiciones para que no sea predecible
            for (int i = 7; i > 0; i--)
            {
                int j = rng.Next(i + 1);
                char tmp = pass[i];
                pass[i] = pass[j];
                pass[j] = tmp;
            }

            return new string(pass);
        }

        // Valida que la contraseña cumpla la política de seguridad (mínimo 8 caracteres, al menos una mayúscula y una minúscula).
        public bool ValidarContraseña(string contraseña, out string Mensaje)
        {
            Mensaje = string.Empty;

            if (string.IsNullOrEmpty(contraseña) || contraseña.Length < 8)
            {
                Mensaje = "La contraseña debe tener al menos 8 caracteres.";
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
                Mensaje = "La contraseña debe contener al menos una letra mayúscula.";
                return false;
            }

            if (!tieneMinuscula)
            {
                Mensaje = "La contraseña debe contener al menos una letra minúscula.";
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

                        ObjBitacora.Registrar(new BitacoraEvento486LP("Login", $"Usuario {nombreUsuario} bloqueado por 3 intentos fallidos.", "ADVERTENCIA", u.DNI));
                        Mensaje = "Usuario bloqueado por 3 intentos fallidos. Contacte al administrador.";
                        return -3;
                    }

                    ObjBitacora.Registrar(new BitacoraEvento486LP("Login", $"Intento fallido {intentos}/3 para {nombreUsuario}.", "ADVERTENCIA", u.DNI));
                    Mensaje = $"Contraseña incorrecta. Intentos fallidos: {intentos}/3.";
                    return -1;
                }

                // Login exitoso — se reinician intentos
                string mensaje;
                ObjetoDAL.ActualizarIntentos(nombreUsuario, 0, out mensaje);
                ObjBitacora.Registrar(new BitacoraEvento486LP("Login", $"Inicio de sesión exitoso: {nombreUsuario}.", "INFO", u.DNI));
                usuarioLogueado = u;
                return 1;
            }
            catch (Exception ex)
            {
                ObjBitacora.Registrar(new BitacoraEvento486LP("Login", $"Error en BLL_Usuarios.Login(): {ex.Message}", "ERROR", "Sistema"));
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
                ObjBitacora.Registrar(new BitacoraEvento486LP("Gestión Usuarios", $"Error en BLL_Usuarios.Listar(): {ex.Message}", "ERROR", "Sistema"));
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
                ObjBitacora.Registrar(new BitacoraEvento486LP("Gestión Usuarios", $"Error en BLL_Usuarios.ListarActivos(): {ex.Message}", "ERROR", "Sistema"));

                return new List<Usuario486LP>();
            }
        }

        public bool Agregar(Usuario486LP obj, out string Mensaje, out string contraseñaTemporal)
        {
            Mensaje = string.Empty;
            contraseñaTemporal = string.Empty;

            try
            {
                // Validaciones 
                if (string.IsNullOrEmpty(obj.DNI))
                {
                    Mensaje = "El DNI es obligatorio.";
                    return false;
                }
                if (string.IsNullOrEmpty(obj.Nombre))
                {
                    Mensaje = "El nombre es obligatorio.";
                    return false;
                }
                if (string.IsNullOrEmpty(obj.Apellido))
                {
                    Mensaje = "El apellido es obligatorio.";
                    return false;
                }
                if (string.IsNullOrEmpty(obj.NombreUsuario))
                {
                    Mensaje = "El nombre de usuario es obligatorio.";
                    return false;
                }
                if (string.IsNullOrEmpty(obj.Email))
                {
                    Mensaje = "El correo es obligatorio.";
                    return false;
                }
                if (!obj.Email.Contains("@") || !obj.Email.Contains("."))
                {
                    Mensaje = "El formato del correo no es válido.";
                    return false;
                }
                List<Usuario486LP> todos = ObjetoDAL.Listar();

                if (todos.Any(u => u.DNI == obj.DNI))
                {
                    Mensaje = "El DNI ingresado ya se encuentra registrado.";
                    return false;
                }

                if (todos.Any(u => u.NombreUsuario.ToLower() == obj.NombreUsuario.ToLower()))
                {
                    Mensaje = "El nombre de usuario ya se encuentra registrado.";
                    return false;
                }


                //  Generar contraseña temporal y hashearla
                contraseñaTemporal = GenerarContraseñaTemporal();
                obj.Contraseña = Encriptacion486LP.GenerarHash(contraseñaTemporal);
                obj.Activo = true;
                obj.Bloqueado = false;
                obj.IntentosFallidos = 0;
                obj.DV = "0";  // se recalcula tras el INSERT

                bool resultado = ObjetoDAL.Agregar(obj, out Mensaje);

                if (resultado)
                {
                    ObjBitacora.Registrar(new BitacoraEvento486LP("Gestión Usuarios", $"Usuario creado: {obj.NombreUsuario} (DNI: {obj.DNI}).", "INFO", SessionManager486LP.ObtenerInstancia().UsuarioActual()?.DNI ?? "Sistema"));
                }

                return resultado;
            }
            catch (Exception ex)
            {
                ObjBitacora.Registrar(new BitacoraEvento486LP("Gestión Usuarios", $"Error en BLL_Usuarios.Agregar(): {ex.Message}", "ERROR", "Sistema"));
                Mensaje = "Ocurrió un error inesperado al agregar el usuario.";
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
                    Mensaje = "El nombre es obligatorio.";
                    return false;
                }
                if (string.IsNullOrEmpty(obj.Apellido))
                {
                    Mensaje = "El apellido es obligatorio.";
                    return false;
                }
                if (string.IsNullOrEmpty(obj.Email))
                {
                    Mensaje = "El correo es obligatorio.";
                    return false;
                }
                if (!obj.Email.Contains("@") || !obj.Email.Contains("."))
                {
                    Mensaje = "El formato del correo no es válido.";
                    return false;
                }

                bool resultado = ObjetoDAL.Modificar(obj, out Mensaje);

                if (resultado)
                {

                    ObjBitacora.Registrar(new BitacoraEvento486LP("Gestión Usuarios", $"Usuario modificado: {obj.NombreUsuario} (DNI: {obj.DNI}).", "INFO", SessionManager486LP.ObtenerInstancia().UsuarioActual()?.DNI ?? "Sistema"));
                }

                return resultado;
            }
            catch (Exception ex)
            {
                ObjBitacora.Registrar(new BitacoraEvento486LP("Gestión Usuarios", $"Error en BLL_Usuarios.Modificar(): {ex.Message}", "ERROR", "Sistema"));
                Mensaje = "Ocurrió un error inesperado al modificar el usuario.";
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
                    ObjBitacora.Registrar(new BitacoraEvento486LP("Gestión Usuarios", $"Usuario eliminado. IdUsuario: {idUsuario}.", "ADVERTENCIA", SessionManager486LP.ObtenerInstancia().UsuarioActual()?.DNI ?? "Sistema"));
                }

                return resultado;
            }
            catch (Exception ex)
            {
                ObjBitacora.Registrar(new BitacoraEvento486LP("Gestión Usuarios", $"Error en BLL_Usuarios.Eliminar(): {ex.Message}", "ERROR", "Sistema"));
                Mensaje = "Ocurrió un error al eliminar el usuario.";
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
                    ObjBitacora.Registrar(new BitacoraEvento486LP("Gestión Usuarios", $"Usuario desbloqueado. DNI: {dni}.", "INFO", SessionManager486LP.ObtenerInstancia().UsuarioActual()?.DNI ?? "Sistema"));
                }

                return resultado;
            }
            catch (Exception ex)
            {
                ObjBitacora.Registrar(new BitacoraEvento486LP("Gestión Usuarios", $"Error en BLL_Usuarios.Desbloquear(): {ex.Message}", "ERROR", "Sistema"));
                Mensaje = "Ocurrió un error al desbloquear el usuario.";
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
                    ObjBitacora.Registrar(new BitacoraEvento486LP("Gestión Usuarios", $"Usuario bloqueado manualmente. DNI: {dni}.", "ADVERTENCIA", SessionManager486LP.ObtenerInstancia().UsuarioActual()?.DNI ?? "Sistema"));
                }

                return resultado;
            }
            catch (Exception ex)
            {
                ObjBitacora.Registrar(new BitacoraEvento486LP("Gestión Usuarios", $"Error en BLL_Usuarios.BloquearPorDNI(): {ex.Message}", "ERROR", "Sistema"));
                Mensaje = "Ocurrió un error al bloquear el usuario.";
                return false;
            }
        }

        //  Activar / Desactivar
        public bool InvertirActivo(string dni, out string Mensaje)
        {
            try
            {
                bool resultado = ObjetoDAL.InvertirActivo(dni, out Mensaje);

                if (resultado)
                {
                    ObjBitacora.Registrar(new BitacoraEvento486LP("Gestión Usuarios", $"Estado Activo invertido. DNI: {dni}.", "INFO", SessionManager486LP.ObtenerInstancia().UsuarioActual()?.DNI ?? "Sistema"));
                }

                return resultado;
            }
            catch (Exception ex)
            {
                ObjBitacora.Registrar(new BitacoraEvento486LP("Gestión Usuarios", $"Error en BLL_Usuarios.InvertirActivo(): {ex.Message}", "ERROR", "Sistema"));
                Mensaje = "Ocurrió un error al cambiar el estado del usuario.";
                return false;
            }
        }

        public bool CambiarContraseña(int idUsuario, string contraseñaActual, string contraseñaNueva, string contraseñaConfirmar, string dniUsuario, out string Mensaje)
        {
            Mensaje = string.Empty;

            try
            {
                // Validar coincidencia 
                if (contraseñaNueva != contraseñaConfirmar)
                {
                    Mensaje = "La nueva contraseña y la confirmación no coinciden.";
                    return false;
                }

                if (contraseñaNueva == contraseñaActual)
                {
                    Mensaje = "La nueva contraseña no puede ser igual a la actual.";
                    return false;
                }

                // Validar política 
                if (!ValidarContraseña(contraseñaNueva, out Mensaje))
                    return false;

                // Verificar contraseña actual contra BD 
                Usuario486LP u = ObjetoDAL.ObtenerPorNombreUsuario(SessionManager486LP.ObtenerInstancia().UsuarioActual()?.NombreUsuario ?? "");

                if (u == null)
                {
                    Mensaje = "No se pudo obtener el usuario de la sesión.";
                    return false;
                }

                string hashActual = Encriptacion486LP.GenerarHash(contraseñaActual);

                if (hashActual != u.Contraseña)
                {
                    ObjBitacora.Registrar(new BitacoraEvento486LP("Cambiar Contraseña", "Intento fallido: contraseña actual incorrecta.", "ADVERTENCIA", dniUsuario));
                    Mensaje = "La contraseña actual es incorrecta.";
                    return false;
                }

                // Hashear nueva y persistir 
                string hashNueva = Encriptacion486LP.GenerarHash(contraseñaNueva);
                bool resultado = ObjetoDAL.CambiarContraseña(idUsuario, hashNueva, out Mensaje);

                if (resultado)
                {
                    u.Contraseña = hashNueva;
                    ObjBitacora.Registrar(new BitacoraEvento486LP("Cambiar Contraseña", "Contraseña cambiada exitosamente.", "INFO", dniUsuario));
                }

                return resultado;
            }
            catch (Exception ex)
            {
                ObjBitacora.Registrar(new BitacoraEvento486LP("Cambiar Contraseña", $"Error en BLL_Usuarios.CambiarContraseña(): {ex.Message}", "ERROR", dniUsuario));
                Mensaje = "Ocurrió un error inesperado al cambiar la contraseña.";
                return false;
            }
        }

        public void Logout(string dni)
        {
            try
            {
                ObjBitacora.Registrar(new BitacoraEvento486LP("Logout", "Cierre de sesión exitoso.", "INFO", dni)); SessionManager486LP.ObtenerInstancia().LogOut();
            }
            catch (Exception ex)
            {
                ObjBitacora.Registrar(new BitacoraEvento486LP("Logout", $"Error en BLL_Usuarios.Logout(): {ex.Message}", "ERROR", dni));
            }
        }
    }
}

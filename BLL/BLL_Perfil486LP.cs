using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class BLL_Perfil486LP
    {
        private DAL.DAL_Perfil486LP _dalPerfil = new DAL.DAL_Perfil486LP();
        private DAL.DAL_Familia486LP _dalFamilia = new DAL.DAL_Familia486LP();
        private DAL.DAL_Patente486LP _dalPatente = new DAL.DAL_Patente486LP();
        private BLL_Bitacora486LP _bllBitacora = new BLL_Bitacora486LP();

        public List<Perfil486LP> ObtenerPerfiles()
        {
            return _dalPerfil.Listar();
        }

        public bool CrearPerfil(string nombre, out string mensaje)
        {
            mensaje = "";

            if (string.IsNullOrWhiteSpace(nombre))
            {
                mensaje = "Debe ingresar un nombre.";
                return false;
            }

            List<Perfil486LP> perfiles = _dalPerfil.Listar();
            if (perfiles.Any(p => p.Nombre.ToLower() == nombre.ToLower()))
            {
                mensaje = "Ya existe un Perfil con ese nombre.";
                return false;
            }

            Perfil486LP nuevo = new Perfil486LP();
            nuevo.Nombre= nombre;

            bool resultado = _dalPerfil.Agregar(nuevo, out mensaje);

            if (resultado)
            {
                string dni = SessionManager486LP.ObtenerInstancia().UsuarioActual()?.DNI ?? "";
                string nombreUsuario = SessionManager486LP.ObtenerInstancia().UsuarioActual()?.NombreUsuario ?? "Sistema";

                _bllBitacora.Registrar(new BitacoraEvento486LP("Gestión Perfiles",$"Se creó el perfil '{nombre}'.",
                    Criticidad486LP.Alta,dni,nombreUsuario));
            }

            return resultado;
        }

        public bool ModificarPerfil(int id, string nuevoNombre, out string mensaje)
        {
            mensaje = "";

            if (id <= 0)
            {
                mensaje = "Debe seleccionar un Perfil.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(nuevoNombre))
            {
                mensaje = "Debe ingresar un nombre válido.";
                return false;
            }

            List<Perfil486LP> perfiles = _dalPerfil.Listar();
            if (perfiles.Any(p => p.Nombre.ToLower() == nuevoNombre.ToLower() && p.IdPerfil != id))
            {
                mensaje = "Ya existe un Perfil con ese nombre.";
                return false;
            }

            Perfil486LP perfil = new Perfil486LP();
            perfil.IdPerfil = id;
            perfil.Nombre = nuevoNombre;

            bool resultado = _dalPerfil.Modificar(perfil, out mensaje);

            if (resultado)
            {
                string dni = SessionManager486LP.ObtenerInstancia().UsuarioActual()?.DNI ?? "";
                string nombreUsuario = SessionManager486LP.ObtenerInstancia().UsuarioActual()?.NombreUsuario ?? "Sistema";

                _bllBitacora.Registrar(new BitacoraEvento486LP("Gestión Perfiles",$"Se modificó el perfil con id '{id}' al nombre '{nuevoNombre}'.",
                    Criticidad486LP.Media,dni,nombreUsuario));
            }

            return resultado;
        }

        public bool EliminarPerfil(int id, out string mensaje)
        {
            mensaje = "";

            if (id <= 0)
            {
                mensaje = "Debe seleccionar un Perfil.";
                return false;
            }

            // Verificar si tiene usuarios asignados
            if (_dalPerfil.TieneUsuarios(id))
            {
                mensaje = "No se puede eliminar un Perfil con usuarios asignados.";
                return false;
            }

            bool resultado = _dalPerfil.Eliminar(id, out mensaje);

            if (resultado)
            {
                string dni = SessionManager486LP.ObtenerInstancia().UsuarioActual()?.DNI ?? "";
                string nombreUsuario = SessionManager486LP.ObtenerInstancia().UsuarioActual()?.NombreUsuario ?? "Sistema";

                _bllBitacora.Registrar(new BitacoraEvento486LP(
                    "Gestión Perfiles",
                    $"Se eliminó el perfil con id '{id}'.",Criticidad486LP.Alta,dni,nombreUsuario));
            }

            return resultado;
        }

        public List<Familia486LP> ObtenerFamilias()
        {
            return _dalFamilia.Listar();
        }

        public List<Permiso486LP> ObtenerPatentes()
        {
            return _dalPatente.Listar();
        }

        public List<Familia486LP> ObtenerFamiliasDePerfil(int idPerfil)
        {
            return _dalPerfil.ListarFamiliasDePerfil(idPerfil);
        }

        public List<Permiso486LP> ObtenerPermisosDePerfil(int idPerfil)
        {
            return _dalPerfil.ListarPermisosDePerfil(idPerfil);
        }

        public bool AsignarFamilia(int idPerfil, int idFamilia, out string mensaje)
        {
            mensaje = "";

            if (idPerfil <= 0)
            {
                mensaje = "Debe seleccionar un Perfil.";
                return false;
            }

            if (idFamilia <= 0)
            {
                mensaje = "Debe seleccionar una Familia.";
                return false;
            }

            List<Familia486LP> familiasActuales = _dalPerfil.ListarFamiliasDePerfil(idPerfil);
            if (familiasActuales.Any(f => f.Id == idFamilia))
            {
                mensaje = "El componente ya pertenece a este elemento.";
                return false;
            }

            // Composite add() en memoria
            Perfil486LP perfil = new Perfil486LP();
            Familia486LP familia = new Familia486LP();
            familia.Id = idFamilia;
            perfil.Componentes.Add(familia);

            // Persistir en BD
            bool resultado = _dalPerfil.AsignarFamilia(idPerfil, idFamilia, out mensaje);

            if (resultado)
            {
                string dni = SessionManager486LP.ObtenerInstancia().UsuarioActual()?.DNI ?? "";
                string nombreUsuario = SessionManager486LP.ObtenerInstancia().UsuarioActual()?.NombreUsuario ?? "Sistema";

                _bllBitacora.Registrar(new BitacoraEvento486LP("Gestión Perfiles",$"Se asignó la familia '{idFamilia}' al perfil '{idPerfil}'.",
                    Criticidad486LP.Media,dni,nombreUsuario));
            }

            return resultado;
        }

        public bool QuitarFamilia(int idPerfil, int idFamilia, out string mensaje)
        {
            mensaje = "";

            if (idPerfil <= 0)
            {
                mensaje = "Debe seleccionar un Perfil.";
                return false;
            }

            if (idFamilia <= 0)
            {
                mensaje = "Debe seleccionar una Familia.";
                return false;
            }

            List<Familia486LP> familiasActuales = _dalPerfil.ListarFamiliasDePerfil(idPerfil);
            if (!familiasActuales.Any(f => f.Id == idFamilia))
            {
                mensaje = "Debe seleccionar un componente para quitar.";
                return false;
            }

            // Composite remove() en memoria
            Perfil486LP perfil = new Perfil486LP();
            Familia486LP familia = familiasActuales.First(f => f.Id == idFamilia);
            perfil.Componentes.Remove(familia);

            // Persistir en BD
            bool resultado = _dalPerfil.QuitarFamilia(idPerfil, idFamilia, out mensaje);

            if (resultado)
            {
                string dni = SessionManager486LP.ObtenerInstancia().UsuarioActual()?.DNI ?? "";
                string nombreUsuario = SessionManager486LP.ObtenerInstancia().UsuarioActual()?.NombreUsuario ?? "Sistema";

                _bllBitacora.Registrar(new BitacoraEvento486LP("Gestión Perfiles",$"Se quitó la familia '{idFamilia}' del perfil '{idPerfil}'.",
                    Criticidad486LP.Media,dni,nombreUsuario));
            }

            return resultado;
        }

        public bool AsignarPermiso(int idPerfil, int idPermiso, out string mensaje)
        {
            mensaje = "";

            if (idPerfil <= 0)
            {
                mensaje = "Debe seleccionar un Perfil.";
                return false;
            }

            if (idPermiso <= 0)
            {
                mensaje = "Debe seleccionar un Permiso.";
                return false;
            }

            List<Permiso486LP> permisosActuales = _dalPerfil.ListarPermisosDePerfil(idPerfil);
            if (permisosActuales.Any(p => p.Id == idPermiso))
            {
                mensaje = "El componente ya pertenece a este elemento.";
                return false;
            }

            // Composite add() en memoria
            Perfil486LP perfil = new Perfil486LP();
            Permiso486LP permiso = new Permiso486LP();
            permiso.Id = idPermiso;
            perfil.Componentes.Add(permiso);

            // Persistir en BD
            bool resultado = _dalPerfil.AsignarPermiso(idPerfil, idPermiso, out mensaje);

            if (resultado)
            {
                string dni = SessionManager486LP.ObtenerInstancia().UsuarioActual()?.DNI ?? "";
                string nombreUsuario = SessionManager486LP.ObtenerInstancia().UsuarioActual()?.NombreUsuario ?? "Sistema";

                _bllBitacora.Registrar(new BitacoraEvento486LP("Gestión Perfiles",$"Se asignó el permiso '{idPermiso}' al perfil '{idPerfil}'.",
                    Criticidad486LP.Media,dni,nombreUsuario));
            }

            return resultado;
        }

        public bool QuitarPermiso(int idPerfil, int idPermiso, out string mensaje)
        {
            mensaje = "";

            if (idPerfil <= 0)
            {
                mensaje = "Debe seleccionar un Perfil.";
                return false;
            }

            if (idPermiso <= 0)
            {
                mensaje = "Debe seleccionar un Permiso.";
                return false;
            }

            List<Permiso486LP> permisosActuales = _dalPerfil.ListarPermisosDePerfil(idPerfil);
            if (!permisosActuales.Any(p => p.Id == idPermiso))
            {
                mensaje = "Debe seleccionar un componente para quitar.";
                return false;
            }

            // Composite remove() en memoria
            Perfil486LP perfil = new Perfil486LP();
            Permiso486LP permiso = permisosActuales.First(p => p.Id == idPermiso);
            perfil.Componentes.Remove(permiso);

            // Persistir en BD
            bool resultado = _dalPerfil.QuitarPermiso(idPerfil, idPermiso, out mensaje);

            if (resultado)
            {
                string dni = SessionManager486LP.ObtenerInstancia().UsuarioActual()?.DNI ?? "";
                string nombreUsuario = SessionManager486LP.ObtenerInstancia().UsuarioActual()?.NombreUsuario ?? "Sistema";

                _bllBitacora.Registrar(new BitacoraEvento486LP("Gestión Perfiles",$"Se quitó el permiso '{idPermiso}' del perfil '{idPerfil}'.",
                    Criticidad486LP.Media,dni,nombreUsuario));
            }

            return resultado;
        }

        public bool TieneUsuariosAsignados(int idPerfil)
        {
            return _dalPerfil.TieneUsuarios(idPerfil);
        }
        public bool TieneFamiliasAsignadas(int idPerfil)
        {
            List<Familia486LP> familias = _dalPerfil.ListarFamiliasDePerfil(idPerfil);
            return familias.Count > 0;
        }

        public bool TienePermisosAsignados(int idPerfil)
        {
            List<Permiso486LP> permisos = _dalPerfil.ListarPermisosDePerfil(idPerfil);
            return permisos.Count > 0;
        }

        public List<string> ObtenerPermisosDeUsuario(int? idPerfil)
        {
            if (!idPerfil.HasValue || idPerfil.Value <= 0)
                return new List<string>();

            return _dalPerfil.ObtenerNombresPermisosDePerfil(idPerfil.Value);
        }
        public List<string> ObtenerPermisosPorRol(string nombreRol)
        {
            if (string.IsNullOrWhiteSpace(nombreRol))
                return new List<string>();

            return _dalPerfil.ObtenerNombresPermisosPorRol(nombreRol);
        }

    }
}

using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class BLL_Familia486LP
    {
        private DAL.DAL_Familia486LP _dalFamilia = new DAL.DAL_Familia486LP();
        private DAL.DAL_Patente486LP _dalPatente = new DAL.DAL_Patente486LP();
        private BLL_Bitacora486LP _bllBitacora = new BLL_Bitacora486LP();

        public List<Familia486LP> ObtenerFamilias()
        {
            return _dalFamilia.Listar();
        }

        public bool CrearFamilia(string nombre, out string mensaje)
        {
            mensaje = "";

            if (string.IsNullOrWhiteSpace(nombre))
            {
                mensaje = "Debe ingresar un nombre.";
                return false;
            }

            List<Familia486LP> familias = _dalFamilia.Listar();
            if (familias.Any(f => f.Nombre.ToLower() == nombre.ToLower()))
            {
                mensaje = "Ya existe una Familia con ese nombre.";
                return false;
            }

            Familia486LP nueva = new Familia486LP();
            nueva.Nombre = nombre;

            bool resultado = _dalFamilia.Agregar(nueva, out mensaje);

            if (resultado)
            {
                string dni = SessionManager486LP.ObtenerInstancia().UsuarioActual()?.DNI ?? "";
                string nombreUsuario = SessionManager486LP.ObtenerInstancia().UsuarioActual()?.NombreUsuario ?? "Sistema";

                _bllBitacora.Registrar(new BitacoraEvento486LP("Gestión Familias", $"Se creó la familia '{nombre}'.", Criticidad486LP.Alta, dni, nombreUsuario));
            }

            return resultado;
        }

        public bool ModificarFamilia(int id, string nuevoNombre, out string mensaje)
        {
            mensaje = "";

            if (id <= 0)
            {
                mensaje = "Debe seleccionar una Familia.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(nuevoNombre))
            {
                mensaje = "Debe ingresar un nombre válido.";
                return false;
            }

            List<Familia486LP> familias = _dalFamilia.Listar();
            if (familias.Any(f => f.Nombre.ToLower() == nuevoNombre.ToLower() && f.Id != id))
            {
                mensaje = "Ya existe una Familia con ese nombre.";
                return false;
            }

            Familia486LP familia = new Familia486LP();
            familia.Id = id;
            familia.Nombre = nuevoNombre;

            bool resultado = _dalFamilia.Modificar(familia, out mensaje);

            if (resultado)
            {
                string dni = SessionManager486LP.ObtenerInstancia().UsuarioActual()?.DNI ?? "";
                string nombreUsuario = SessionManager486LP.ObtenerInstancia().UsuarioActual()?.NombreUsuario ?? "Sistema";

                _bllBitacora.Registrar(new BitacoraEvento486LP("Gestión Familias",$"Se modificó la familia con id '{id}' al nombre '{nuevoNombre}'.",Criticidad486LP.Media,dni,nombreUsuario));
            }

            return resultado;
        }

        public bool EliminarFamilia(int id, out string mensaje)
        {
            mensaje = "";

            if (id <= 0)
            {
                mensaje = "Debe seleccionar una Familia.";
                return false;
            }

            bool resultado = _dalFamilia.Eliminar(id, out mensaje);

            if (resultado)
            {
                string dni = SessionManager486LP.ObtenerInstancia().UsuarioActual()?.DNI ?? "";
                string nombreUsuario = SessionManager486LP.ObtenerInstancia().UsuarioActual()?.NombreUsuario ?? "Sistema";

                _bllBitacora.Registrar(new BitacoraEvento486LP("Gestión Familias",$"Se eliminó la familia con id '{id}'.",Criticidad486LP.Alta,dni,nombreUsuario));
            }

            return resultado;
        }

        public List<Permiso486LP> ObtenerPatentes()
        {
            return _dalPatente.Listar();
        }

        public List<Permiso486LP> ObtenerPermisosDeFamilia(int idFamilia)
        {
            return _dalFamilia.ListarPermisosDeFamilia(idFamilia);
        }

        public bool AsignarPermiso(int idFamilia, int idPermiso, out string mensaje)
        {
            mensaje = "";

            if (idFamilia <= 0)
            {
                mensaje = "Debe seleccionar una Familia.";
                return false;
            }

            if (idPermiso <= 0)
            {
                mensaje = "Debe seleccionar un Permiso.";
                return false;
            }

            // Verifico si ya está asignado
            List<Permiso486LP> permisosActuales = _dalFamilia.ListarPermisosDeFamilia(idFamilia);
            if (permisosActuales.Any(p => p.Id == idPermiso))
            {
                mensaje = "El componente ya pertenece a este elemento.";
                return false;
            }

            // Composite add() en memoria
            Familia486LP familia = new Familia486LP();
            Permiso486LP permiso = new Permiso486LP();
            permiso.Id = idPermiso;
            familia.add(permiso);

            // Persistir en BD
            bool resultado = _dalFamilia.AsignarPermiso(idFamilia, idPermiso, out mensaje);

            if (resultado)
            {
                string dni = SessionManager486LP.ObtenerInstancia().UsuarioActual()?.DNI ?? "";
                string nombreUsuario = SessionManager486LP.ObtenerInstancia().UsuarioActual()?.NombreUsuario ?? "Sistema";

                _bllBitacora.Registrar(new BitacoraEvento486LP("Gestión Familias",$"Se asignó el permiso '{idPermiso}' a la familia '{idFamilia}'.",Criticidad486LP.Media,dni,nombreUsuario));
            }

            return resultado;
        }

        public bool QuitarPermiso(int idFamilia, int idPermiso, out string mensaje)
        {
            mensaje = "";

            if (idFamilia <= 0)
            {
                mensaje = "Debe seleccionar una Familia.";
                return false;
            }

            if (idPermiso <= 0)
            {
                mensaje = "Debe seleccionar un Permiso.";
                return false;
            }

            // Verifico si está asignado
            List<Permiso486LP> permisosActuales = _dalFamilia.ListarPermisosDeFamilia(idFamilia);
            if (!permisosActuales.Any(p => p.Id == idPermiso))
            {
                mensaje = "Debe seleccionar un componente para quitar.";
                return false;
            }

            // Composite remove() en memoria
            Familia486LP familia = new Familia486LP();
            Permiso486LP permiso = new Permiso486LP();
            permiso.Id = idPermiso;
            familia.remove(permiso);

            // Persistir en BD
            bool resultado = _dalFamilia.QuitarPermiso(idFamilia, idPermiso, out mensaje);

            if (resultado)
            {
                string dni = SessionManager486LP.ObtenerInstancia().UsuarioActual()?.DNI ?? "";
                string nombreUsuario = SessionManager486LP.ObtenerInstancia().UsuarioActual()?.NombreUsuario ?? "Sistema";

                _bllBitacora.Registrar(new BitacoraEvento486LP("Gestión Familias",$"Se quitó el permiso '{idPermiso}' de la familia '{idFamilia}'.",Criticidad486LP.Media,dni,nombreUsuario));
            }

            return resultado;
        }

        public bool TienePermisosAsignados(int idFamilia)
        {
            List<Permiso486LP> permisos = _dalFamilia.ListarPermisosDeFamilia(idFamilia);
            return permisos.Count > 0;
        }

        public bool EstaAsignadaAPerfil(int idFamilia)
        {
            return _dalFamilia.EstaAsignadaAPerfil(idFamilia);
        }
    }
}

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
        private BLL_Patente486LP _bllPatente = new BLL_Patente486LP();
        private BLL_Bitacora486LP _bllBitacora = new BLL_Bitacora486LP();
        private readonly string[] _patentesBase = {"CAMBIAR_CONTRASENA_ACEPTAR", "CAMBIAR_CONTRASENA_SALIR","CERRAR_SESION_ACEPTAR", "CERRAR_SESION_CANCELAR"};

        public List<Familia486LP> ObtenerFamilias()
        {
            return _dalFamilia.Listar();
        }

        public bool CrearFamilia(string nombre, out string mensaje)
        {
            mensaje = "";

            if (string.IsNullOrWhiteSpace(nombre))
            {
                mensaje = "Msg.IngresarNombre"; // Debe ingresar un nombre.
                return false;
            }

            List<Familia486LP> familias = _dalFamilia.Listar();
            if (familias.Any(f => f.Nombre.ToLower() == nombre.ToLower()))
            {
                mensaje = "Msg.FamiliaExiste"; // Ya existe una Familia con ese nombre.
                return false;
            }

            Familia486LP nueva = new Familia486LP();
            nueva.Nombre = nombre;

            bool resultado = _dalFamilia.Agregar(nueva, out mensaje);

            if (resultado)
            {
                AsignarPatentesBase(nueva.Id);

                string dni = SessionManager486LP.ObtenerInstancia().UsuarioActual()?.DNI ?? "";
                string nombreUsuario = SessionManager486LP.ObtenerInstancia().UsuarioActual()?.NombreUsuario ?? "Sistema";

                _bllBitacora.Registrar(new BitacoraEvento486LP("Gestión Familias", $"Se creó la familia '{nombre}' con patentes base.", Criticidad486LP.Alta, dni, nombreUsuario));

                string mensajeDV;
                BLL_DV486LP bllDV = new BLL_DV486LP();
                bllDV.RecalcularDV("Familia", out mensajeDV);
                bllDV.RecalcularDV("Familia_Permiso", out mensajeDV);
            }

            return resultado;
        }

        public bool ModificarFamilia(int id, string nuevoNombre, out string mensaje)
        {
            mensaje = "";

            if (id <= 0)
            {
                mensaje = "Msg.SeleccionarFamilia"; // Debe seleccionar una Familia.
                return false;
            }

            if (string.IsNullOrWhiteSpace(nuevoNombre))
            {
                mensaje = "Msg.IngresarNombreValido"; // Debe ingresar un nombre válido.
                return false;
            }

            List<Familia486LP> familias = _dalFamilia.Listar();
            if (familias.Any(f => f.Nombre.ToLower() == nuevoNombre.ToLower() && f.Id != id))
            {
                mensaje = "Msg.FamiliaExiste"; // Ya existe una Familia con ese nombre.
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

                _bllBitacora.Registrar(new BitacoraEvento486LP("Gestión Familias", $"Se modificó la familia con id '{id}' al nombre '{nuevoNombre}'.", Criticidad486LP.Media, dni, nombreUsuario));

                string mensajeDV;
                BLL_DV486LP bllDV = new BLL_DV486LP();
                bllDV.RecalcularDV("Familia", out mensajeDV);
            }

            return resultado;
        }

        public bool EliminarFamilia(int id, out string mensaje)
        {
            mensaje = "";

            if (id <= 0)
            {
                mensaje = "Msg.SeleccionarFamilia"; // Debe seleccionar una Familia.
                return false;
            }

            bool resultado = _dalFamilia.Eliminar(id, out mensaje);

            if (resultado)
            {
                string dni = SessionManager486LP.ObtenerInstancia().UsuarioActual()?.DNI ?? "";
                string nombreUsuario = SessionManager486LP.ObtenerInstancia().UsuarioActual()?.NombreUsuario ?? "Sistema";

                _bllBitacora.Registrar(new BitacoraEvento486LP("Gestión Familias", $"Se eliminó la familia con id '{id}'.", Criticidad486LP.Alta, dni, nombreUsuario));

                string mensajeDV;
                BLL_DV486LP bllDV = new BLL_DV486LP();
                bllDV.RecalcularDV("Familia", out mensajeDV);
                bllDV.RecalcularDV("Familia_Permiso", out mensajeDV);
                bllDV.RecalcularDV("Perfil_Familia", out mensajeDV);
            }

            return resultado;
        }

        public List<Permiso486LP> ObtenerPatentes()
        {
            return _bllPatente.ObtenerPatentes();
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
                mensaje = "Msg.SeleccionarFamilia"; // Debe seleccionar una Familia.
                return false;
            }

            if (idPermiso <= 0)
            {
                mensaje = "Msg.SeleccionarPermiso"; // Debe seleccionar un Permiso.
                return false;
            }

            // Verifico si ya está asignado
            List<Permiso486LP> permisosActuales = _dalFamilia.ListarPermisosDeFamilia(idFamilia);
            if (permisosActuales.Any(p => p.Id == idPermiso))
            {
                mensaje = "Msg.ComponenteExiste"; // El componente ya pertenece a este elemento.
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

                _bllBitacora.Registrar(new BitacoraEvento486LP("Gestión Familias", $"Se asignó el permiso '{idPermiso}' a la familia '{idFamilia}'.", Criticidad486LP.Media, dni, nombreUsuario));

                string mensajeDV;
                BLL_DV486LP bllDV = new BLL_DV486LP();
                bllDV.RecalcularDV("Familia_Permiso", out mensajeDV);
            }

            return resultado;
        }

        private void AsignarPatentesBase(int idFamilia)
        {
            List<Permiso486LP> todas = _bllPatente.ObtenerPatentes();

            foreach (string codigo in _patentesBase)
            {
                Permiso486LP p = todas.FirstOrDefault(x => x.Nombre == codigo);
                if (p != null)
                {
                    string msgIgnorado;
                    _dalFamilia.AsignarPermiso(idFamilia, p.Id, out msgIgnorado);
                }
            }
        }

        public bool QuitarPermiso(int idFamilia, int idPermiso, out string mensaje)
        {
            mensaje = "";

            if (idFamilia <= 0)
            {
                mensaje = "Msg.SeleccionarFamilia"; // Debe seleccionar una Familia.
                return false;
            }

            if (idPermiso <= 0)
            {
                mensaje = "Msg.SeleccionarPermiso"; // Debe seleccionar un Permiso.
                return false;
            }

            // Verifico si está asignado
            List<Permiso486LP> permisosActuales = _dalFamilia.ListarPermisosDeFamilia(idFamilia);
            if (!permisosActuales.Any(p => p.Id == idPermiso))
            {
                mensaje = "Msg.SeleccionarPermisoQuitar"; // Debe seleccionar un componente para quitar.
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

                _bllBitacora.Registrar(new BitacoraEvento486LP("Gestión Familias", $"Se quitó el permiso '{idPermiso}' de la familia '{idFamilia}'.", Criticidad486LP.Media, dni, nombreUsuario));

                string mensajeDV;
                BLL_DV486LP bllDV = new BLL_DV486LP();
                bllDV.RecalcularDV("Familia_Permiso", out mensajeDV);
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

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
        private BLL_Patente486LP _bllPatente = new BLL_Patente486LP();
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
                mensaje = "Msg.IngresarNombre"; // Debe ingresar un nombre.
                return false;
            }

            List<Perfil486LP> perfiles = _dalPerfil.Listar();
            if (perfiles.Any(p => p.Nombre.ToLower() == nombre.ToLower()))
            {
                mensaje = "Msg.PerfilExiste"; // Ya existe un Perfil con ese nombre.
                return false;
            }

            Perfil486LP nuevo = new Perfil486LP();
            nuevo.Nombre = nombre;

            bool resultado = _dalPerfil.Agregar(nuevo, out mensaje);

            if (resultado)
            {
                string dni = SessionManager486LP.ObtenerInstancia().UsuarioActual()?.DNI ?? "";
                string nombreUsuario = SessionManager486LP.ObtenerInstancia().UsuarioActual()?.NombreUsuario ?? "Sistema";

                _bllBitacora.Registrar(new BitacoraEvento486LP("Gestión Perfiles", $"Se creó el perfil '{nombre}'.",
                    Criticidad486LP.Alta, dni, nombreUsuario));

                string mensajeDV;
                BLL_DV486LP bllDV = new BLL_DV486LP();
                bllDV.RecalcularDV("Perfil", out mensajeDV);
            }

            return resultado;
        }

        public bool ModificarPerfil(int id, string nuevoNombre, out string mensaje)
        {
            mensaje = "";

            if (id <= 0)
            {
                mensaje = "Msg.SeleccionarPerfil"; // Debe seleccionar un Perfil.
                return false;
            }

            if (string.IsNullOrWhiteSpace(nuevoNombre))
            {
                mensaje = "Msg.IngresarNombreValido"; // Debe ingresar un nombre válido.
                return false;
            }

            List<Perfil486LP> perfiles = _dalPerfil.Listar();
            if (perfiles.Any(p => p.Nombre.ToLower() == nuevoNombre.ToLower() && p.IdPerfil != id))
            {
                mensaje = "Msg.PerfilExiste"; // Ya existe un Perfil con ese nombre.
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

                _bllBitacora.Registrar(new BitacoraEvento486LP("Gestión Perfiles", $"Se modificó el perfil con id '{id}' al nombre '{nuevoNombre}'.",
                    Criticidad486LP.Media, dni, nombreUsuario));

                string mensajeDV;
                BLL_DV486LP bllDV = new BLL_DV486LP();
                bllDV.RecalcularDV("Perfil", out mensajeDV);
            }

            return resultado;
        }

        public bool EliminarPerfil(int id, out string mensaje)
        {
            mensaje = "";

            if (id <= 0)
            {
                mensaje = "Msg.SeleccionarPerfil"; // Debe seleccionar un Perfil.
                return false;
            }

            // Bloquear solo si es el perfil del usuario logueado actualmente
            Usuario486LP usuarioActual = SessionManager486LP.ObtenerInstancia().UsuarioActual();
            if (usuarioActual != null && usuarioActual.IdPerfil.HasValue && usuarioActual.IdPerfil.Value == id)
            {
                mensaje = "Msg.NoEliminarPerfilActivo"; // No puede eliminar el perfil que está usando actualmente.
                return false;
            }

            bool resultado = _dalPerfil.Eliminar(id, out mensaje);

            if (resultado)
            {
                string dni = usuarioActual?.DNI ?? "";
                string nombreUsuario = usuarioActual?.NombreUsuario ?? "Sistema";
                _bllBitacora.Registrar(new BitacoraEvento486LP
                    ("Gestión Perfiles", $"Se eliminó el perfil con id '{id}'.", Criticidad486LP.Alta, dni, nombreUsuario));

                string mensajeDV;
                BLL_DV486LP bllDV = new BLL_DV486LP();
                bllDV.RecalcularDV("Perfil", out mensajeDV);
            }

            return resultado;
        }

        public List<Familia486LP> ObtenerFamilias()
        {
            return _dalFamilia.Listar();
        }

        public List<Permiso486LP> ObtenerPatentes()
        {
            return _bllPatente.ObtenerPatentes();
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
                mensaje = "Msg.SeleccionarPerfil"; // Debe seleccionar un Perfil.
                return false;
            }

            if (idFamilia <= 0)
            {
                mensaje = "Msg.SeleccionarFamilia"; // Debe seleccionar una Familia.
                return false;
            }

            List<Familia486LP> familiasActuales = _dalPerfil.ListarFamiliasDePerfil(idPerfil);
            if (familiasActuales.Any(f => f.Id == idFamilia))
            {
                mensaje = "Msg.ComponenteExiste"; // El componente ya pertenece a este elemento.
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

                _bllBitacora.Registrar(new BitacoraEvento486LP("Gestión Perfiles", $"Se asignó la familia '{idFamilia}' al perfil '{idPerfil}'.",
                    Criticidad486LP.Media, dni, nombreUsuario));

                string mensajeDV;
                BLL_DV486LP bllDV = new BLL_DV486LP();
                bllDV.RecalcularDV("Perfil_Familia", out mensajeDV);
            }

            return resultado;
        }

        public bool QuitarFamilia(int idPerfil, int idFamilia, out string mensaje)
        {
            mensaje = "";

            if (idPerfil <= 0)
            {
                mensaje = "Msg.SeleccionarPerfil"; // Debe seleccionar un Perfil.
                return false;
            }

            if (idFamilia <= 0)
            {
                mensaje = "Msg.SeleccionarFamilia"; // Debe seleccionar una Familia.
                return false;
            }

            List<Familia486LP> familiasActuales = _dalPerfil.ListarFamiliasDePerfil(idPerfil);
            if (!familiasActuales.Any(f => f.Id == idFamilia))
            {
                mensaje = "Msg.SeleccionarFamiliaQuitar"; // Debe seleccionar una Familia para quitar.
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

                _bllBitacora.Registrar(new BitacoraEvento486LP("Gestión Perfiles", $"Se quitó la familia '{idFamilia}' del perfil '{idPerfil}'.",
                    Criticidad486LP.Media, dni, nombreUsuario));

                string mensajeDV;
                BLL_DV486LP bllDV = new BLL_DV486LP();
                bllDV.RecalcularDV("Perfil_Familia", out mensajeDV);
            }

            return resultado;
        }

        public bool AsignarPermiso(int idPerfil, int idPermiso, out string mensaje)
        {
            mensaje = "";

            if (idPerfil <= 0)
            {
                mensaje = "Msg.SeleccionarPerfil"; // Debe seleccionar un Perfil.
                return false;
            }

            if (idPermiso <= 0)
            {
                mensaje = "Msg.SeleccionarPermiso"; // Debe seleccionar un Permiso.
                return false;
            }

            List<Permiso486LP> permisosActuales = _dalPerfil.ListarPermisosDePerfil(idPerfil);
            if (permisosActuales.Any(p => p.Id == idPermiso))
            {
                mensaje = "Msg.ComponenteExiste"; // El componente ya pertenece a este elemento.
                return false;
            }

            // bloquear si el permiso ya viene por una familia asignada 
            string familiaQueLoCubre = FamiliaQueContienePermiso(idPerfil, idPermiso);

            if (familiaQueLoCubre != null)
            {
                mensaje = "Msg.PermisoYaEnFamilia"; // Este permiso ya está incluido en la familia '{0}', asignada a este perfil. No es necesario asignarlo por separado.
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

                _bllBitacora.Registrar(new BitacoraEvento486LP("Gestión Perfiles", $"Se asignó el permiso '{idPermiso}' al perfil '{idPerfil}'.",
                    Criticidad486LP.Media, dni, nombreUsuario));

                string mensajeDV;
                BLL_DV486LP bllDV = new BLL_DV486LP();
                bllDV.RecalcularDV("Perfil_Permiso", out mensajeDV);
            }

            return resultado;
        }

        public bool QuitarPermiso(int idPerfil, int idPermiso, out string mensaje)
        {
            mensaje = "";

            if (idPerfil <= 0)
            {
                mensaje = "Msg.SeleccionarPerfil"; // Debe seleccionar un Perfil.
                return false;
            }

            if (idPermiso <= 0)
            {
                mensaje = "Msg.SeleccionarPermiso"; // Debe seleccionar un Permiso.
                return false;
            }

            List<Permiso486LP> permisosActuales = _dalPerfil.ListarPermisosDePerfil(idPerfil);
            if (!permisosActuales.Any(p => p.Id == idPermiso))
            {
                mensaje = "Msg.SeleccionarPermisoQuitar"; // Debe seleccionar un Permiso para quitar.
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

                _bllBitacora.Registrar(new BitacoraEvento486LP("Gestión Perfiles", $"Se quitó el permiso '{idPermiso}' del perfil '{idPerfil}'.",
                    Criticidad486LP.Media, dni, nombreUsuario));

                string mensajeDV;
                BLL_DV486LP bllDV = new BLL_DV486LP();
                bllDV.RecalcularDV("Perfil_Permiso", out mensajeDV);
            }

            return resultado;
        }

        public bool FamiliaSolapaConAsignadas(int idPerfil, int idFamilia)
        {
            // Permisos que trae la familia candidata
            List<Permiso486LP> permisosNuevaFamilia = _dalFamilia.ListarPermisosDeFamilia(idFamilia);
            if (permisosNuevaFamilia.Count == 0)
            {
                return false; // si no tiene permisos, no puede solapar con nada
            }

            // IDs de permisos que ya vienen por las familias asignadas al perfil
            HashSet<int> permisosYaCubiertos = new HashSet<int>();
            List<Familia486LP> familiasAsignadas = _dalPerfil.ListarFamiliasDePerfil(idPerfil);

            foreach (Familia486LP fam in familiasAsignadas)
            {
                if (fam.Id == idFamilia)
                {
                    continue; // por las dudas ignorar la misma familia
                }

                foreach (Permiso486LP perm in _dalFamilia.ListarPermisosDeFamilia(fam.Id))
                    permisosYaCubiertos.Add(perm.Id);
            }

            // hay al menos un permiso en común?
            return permisosNuevaFamilia.Any(p => permisosYaCubiertos.Contains(p.Id));
        }

        // La asignar una familia, quita del perfil los permisos SUELTOS (directos) que esa familia ya cubre, para que un permiso no quede a la vez suelto y por familia.
        // Devuelve los nombres quitados (para informar en la GUI). Lista vacía si no quitó nada.
        public List<string> QuitarSueltosCubiertosPorFamilia(int idPerfil, int idFamilia)
        {
            List<string> quitados = new List<string>();

            List<Permiso486LP> permisosFamilia = _dalFamilia.ListarPermisosDeFamilia(idFamilia);
            if (permisosFamilia.Count == 0)
                return quitados;

            HashSet<int> idsFamilia = new HashSet<int>(permisosFamilia.Select(p => p.Id));
            List<Permiso486LP> sueltos = _dalPerfil.ListarPermisosDePerfil(idPerfil);

            foreach (Permiso486LP suelto in sueltos)
            {
                if (idsFamilia.Contains(suelto.Id))
                {
                    string m;
                    if (QuitarPermiso(idPerfil, suelto.Id, out m)) // La BLL: valida, persiste y registra en bitácora
                    { 
                        quitados.Add(suelto.Nombre); 
                    }
                }
            }

            return quitados;
        }

        // ¿El permiso suelto que se quiere asignar ya viene incluido en alguna familia asignada al perfil?
        // Devuelve el NOMBRE de esa familia, o null si ninguna lo contiene.
        public string FamiliaQueContienePermiso(int idPerfil, int idPermiso)
        {
            List<Familia486LP> familiasAsignadas = _dalPerfil.ListarFamiliasDePerfil(idPerfil);

            foreach (Familia486LP fam in familiasAsignadas)
            {
                List<Permiso486LP> permisosFamilia = _dalFamilia.ListarPermisosDeFamilia(fam.Id);
                if (permisosFamilia.Any(p => p.Id == idPermiso))
                {
                    return fam.Nombre; // la encontró
                }
            }

            return null; // ninguna familia asignada cubre ese permiso
        }

        public bool TieneUsuariosAsignados(int idPerfil)
        {
            return _dalPerfil.TieneUsuarios(idPerfil);
        }
        public bool TieneFamiliasAsignadas(int idPerfil)
        {
            List<Familia486LP> familias = _dalPerfil.ListarFamiliasDePerfil(idPerfil);
            {
                return familias.Count > 0;
            }
        }

        public bool TienePermisosAsignados(int idPerfil)
        {
            List<Permiso486LP> permisos = _dalPerfil.ListarPermisosDePerfil(idPerfil);
            {
                return permisos.Count > 0;
            }
        }

        public List<string> ObtenerPermisosPorRol(string nombreRol)
        {
            if (string.IsNullOrWhiteSpace(nombreRol))
            {
                return new List<string>();
            }
        
            return _dalPerfil.ObtenerNombresPermisosPorRol(nombreRol);
        }

    }
}

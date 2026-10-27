using Mappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class BLL_Perfil486LP
    {
        private Mapper_Perfil486LP _mapperPerfil = new Mapper_Perfil486LP();

        // CORRECCION: antes se accedia directo a DAL.DAL_Familia486LP (que ya no existe, se elimino al migrar Familia). Ahora se usa BLL_Familia486LP
        // (su propia BLL, via sus metodos publicos) en vez de tocar el Mapper de otra entidad directamente - una BLL habla con otra BLL, no con el Mapper ajeno.
        private BLL_Familia486LP _bllFamilia = new BLL_Familia486LP();
        private BLL_Patente486LP _bllPatente = new BLL_Patente486LP();
        private BLL_Bitacora486LP _bllBitacora = new BLL_Bitacora486LP();

        // Patentes mínimas que todo perfil debe conservar (igual que las familias): sin ellas el usuario
        // no podría cambiar su contraseña ni cerrar sesión.
        private readonly string[] _patentesBase = { "CAMBIAR_CONTRASENA_ACEPTAR", "CAMBIAR_CONTRASENA_SALIR", "CERRAR_SESION_ACEPTAR", "CERRAR_SESION_CANCELAR" };

        public List<Perfil486LP> ObtenerPerfiles()
        {
            return _mapperPerfil.Listar();
        }

        public bool CrearPerfil(string nombre, out string mensaje)
        {
            mensaje = "";

            if (string.IsNullOrWhiteSpace(nombre))
            {
                mensaje = "Msg.IngresarNombre"; // Debe ingresar un nombre.
                return false;
            }

            List<Perfil486LP> perfiles = _mapperPerfil.Listar();
            if (perfiles.Any(p => p.Nombre.ToLower() == nombre.ToLower()))
            {
                mensaje = "Msg.PerfilExiste"; // Ya existe un Perfil con ese nombre.
                return false;
            }

            Perfil486LP nuevo = new Perfil486LP();
            nuevo.Nombre = nombre;

            bool resultado = _mapperPerfil.Agregar(nuevo, out mensaje);

            if (resultado)
            {
                // Perfil_Agregar no devuelve el id generado: se busca el perfil recién creado por su nombre (único)
                Perfil486LP creado = _mapperPerfil.Listar().FirstOrDefault(p => p.Nombre.ToLower() == nombre.ToLower());
                if (creado != null)
                {
                    GarantizarPatentesBase(creado.IdPerfil);
                }

                string dni = SessionManager486LP.ObtenerInstancia().UsuarioActual()?.DNI ?? "";
                string nombreUsuario = SessionManager486LP.ObtenerInstancia().UsuarioActual()?.NombreUsuario ?? "Sistema";

                _bllBitacora.Registrar(new BitacoraEvento486LP("Gestión Perfiles", $"Se creó el perfil '{nombre}' con patentes base.",
                    Criticidad486LP.Alta, dni, nombreUsuario));

                string mensajeDV;
                BLL_DV486LP bllDV = new BLL_DV486LP();
                bllDV.RecalcularDV("Perfil", out mensajeDV);
                bllDV.RecalcularDV("Perfil_Permiso", out mensajeDV);
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

            List<Perfil486LP> perfiles = _mapperPerfil.Listar();
            if (perfiles.Any(p => p.Nombre.ToLower() == nuevoNombre.ToLower() && p.IdPerfil != id))
            {
                mensaje = "Msg.PerfilExiste"; // Ya existe un Perfil con ese nombre.
                return false;
            }

            string nombreAnterior = NombrePerfil(id);

            Perfil486LP perfil = new Perfil486LP();
            perfil.IdPerfil = id;
            perfil.Nombre = nuevoNombre;

            bool resultado = _mapperPerfil.Modificar(perfil, out mensaje);

            if (resultado)
            {
                string dni = SessionManager486LP.ObtenerInstancia().UsuarioActual()?.DNI ?? "";
                string nombreUsuario = SessionManager486LP.ObtenerInstancia().UsuarioActual()?.NombreUsuario ?? "Sistema";

                _bllBitacora.Registrar(new BitacoraEvento486LP("Gestión Perfiles", $"Se modificó el perfil '{nombreAnterior}' al nombre '{nuevoNombre}'.",
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

            // Un perfil con usuarios asignados no se puede eliminar: la base lo rechaza con un error de clave foránea
            // y el usuario veía el mensaje técnico. Se valida acá y se devuelve un mensaje claro.
            if (_mapperPerfil.TieneUsuarios(id))
            {
                mensaje = "Msg.PerfilConUsuarios"; // No se puede eliminar: hay usuarios asignados a este perfil.
                return false;
            }

            string nombrePerfil = NombrePerfil(id);

            // Se quitan antes las asociaciones (familias y permisos sueltos) para que el borrado no falle por las claves foráneas
            foreach (Familia486LP fam in _mapperPerfil.ListarFamiliasDePerfil(id))
            {
                string mQuitar;
                _mapperPerfil.QuitarFamilia(id, fam.Id, out mQuitar);
            }
            foreach (Permiso486LP perm in _mapperPerfil.ListarPermisosDePerfil(id))
            {
                string mQuitar;
                _mapperPerfil.QuitarPermiso(id, perm.Id, out mQuitar);
            }

            bool resultado = _mapperPerfil.Eliminar(id, out mensaje);

            if (resultado)
            {
                string dni = usuarioActual?.DNI ?? "";
                string nombreUsuario = usuarioActual?.NombreUsuario ?? "Sistema";
                _bllBitacora.Registrar(new BitacoraEvento486LP
                    ("Gestión Perfiles", $"Se eliminó el perfil '{nombrePerfil}'.", Criticidad486LP.Alta, dni, nombreUsuario));

                string mensajeDV;
                BLL_DV486LP bllDV = new BLL_DV486LP();
                bllDV.RecalcularDV("Perfil", out mensajeDV);
                bllDV.RecalcularDV("Perfil_Familia", out mensajeDV);
                bllDV.RecalcularDV("Perfil_Permiso", out mensajeDV);
            }

            return resultado;
        }

        public List<Familia486LP> ObtenerFamilias()
        {
            return _bllFamilia.ObtenerFamilias();
        }

        public List<Permiso486LP> ObtenerPatentes()
        {
            return _bllPatente.ObtenerPatentes();
        }

        public List<Familia486LP> ObtenerFamiliasDePerfil(int idPerfil)
        {
            return _mapperPerfil.ListarFamiliasDePerfil(idPerfil);
        }

        public List<Permiso486LP> ObtenerPermisosDePerfil(int idPerfil)
        {
            return _mapperPerfil.ListarPermisosDePerfil(idPerfil);
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

            List<Familia486LP> familiasActuales = _mapperPerfil.ListarFamiliasDePerfil(idPerfil);
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
            bool resultado = _mapperPerfil.AsignarFamilia(idPerfil, idFamilia, out mensaje);

            if (resultado)
            {
                string dni = SessionManager486LP.ObtenerInstancia().UsuarioActual()?.DNI ?? "";
                string nombreUsuario = SessionManager486LP.ObtenerInstancia().UsuarioActual()?.NombreUsuario ?? "Sistema";

                _bllBitacora.Registrar(new BitacoraEvento486LP("Gestión Perfiles", $"Se asignó la familia '{NombreFamilia(idFamilia)}' al perfil '{NombrePerfil(idPerfil)}'.",
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

            List<Familia486LP> familiasActuales = _mapperPerfil.ListarFamiliasDePerfil(idPerfil);
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
            bool resultado = _mapperPerfil.QuitarFamilia(idPerfil, idFamilia, out mensaje);

            if (resultado)
            {
                string dni = SessionManager486LP.ObtenerInstancia().UsuarioActual()?.DNI ?? "";
                string nombreUsuario = SessionManager486LP.ObtenerInstancia().UsuarioActual()?.NombreUsuario ?? "Sistema";

                _bllBitacora.Registrar(new BitacoraEvento486LP("Gestión Perfiles", $"Se quitó la familia '{familia.Nombre}' del perfil '{NombrePerfil(idPerfil)}'.",
                    Criticidad486LP.Media, dni, nombreUsuario));

                // Si las patentes base venían por esa familia, el perfil las perdería: se vuelven a asegurar
                GarantizarPatentesBase(idPerfil);

                string mensajeDV;
                BLL_DV486LP bllDV = new BLL_DV486LP();
                bllDV.RecalcularDV("Perfil_Familia", out mensajeDV);
                bllDV.RecalcularDV("Perfil_Permiso", out mensajeDV);
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

            List<Permiso486LP> permisosActuales = _mapperPerfil.ListarPermisosDePerfil(idPerfil);
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
            bool resultado = _mapperPerfil.AsignarPermiso(idPerfil, idPermiso, out mensaje);

            if (resultado)
            {
                string dni = SessionManager486LP.ObtenerInstancia().UsuarioActual()?.DNI ?? "";
                string nombreUsuario = SessionManager486LP.ObtenerInstancia().UsuarioActual()?.NombreUsuario ?? "Sistema";

                _bllBitacora.Registrar(new BitacoraEvento486LP("Gestión Perfiles", $"Se asignó el permiso '{NombrePatente(idPermiso)}' al perfil '{NombrePerfil(idPerfil)}'.",
                    Criticidad486LP.Media, dni, nombreUsuario));

                string mensajeDV;
                BLL_DV486LP bllDV = new BLL_DV486LP();
                bllDV.RecalcularDV("Perfil_Permiso", out mensajeDV);
            }

            return resultado;
        }

        public bool QuitarPermiso(int idPerfil, int idPermiso, out string mensaje)
        {
            return QuitarPermisoInterno(idPerfil, idPermiso, true, out mensaje);
        }

        // validarPatenteBase = false solo lo usa la propia BLL cuando la patente base queda cubierta por una familia (ver QuitarSueltosCubiertosPorFamilia).
        private bool QuitarPermisoInterno(int idPerfil, int idPermiso, bool validarPatenteBase, out string mensaje)
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

            List<Permiso486LP> permisosActuales = _mapperPerfil.ListarPermisosDePerfil(idPerfil);
            if (!permisosActuales.Any(p => p.Id == idPermiso))
            {
                mensaje = "Msg.SeleccionarPermisoQuitar"; // Debe seleccionar un Permiso para quitar.
                return false;
            }

            // Las patentes base (cambiar contraseña / cerrar sesión) no se pueden quitar de un perfil:
            // el perfil quedaría sin poder cerrar sesión ni cambiar la contraseña.
            if (validarPatenteBase && _patentesBase.Contains(NombrePatente(idPermiso)))
            {
                mensaje = "Msg.PatenteBasePerfil"; // Esta patente es obligatoria en todos los perfiles y no se puede quitar.
                return false;
            }

            // Composite remove() en memoria
            Perfil486LP perfil = new Perfil486LP();
            Permiso486LP permiso = permisosActuales.First(p => p.Id == idPermiso);
            perfil.Componentes.Remove(permiso);

            // Persistir en BD
            bool resultado = _mapperPerfil.QuitarPermiso(idPerfil, idPermiso, out mensaje);

            if (resultado)
            {
                string dni = SessionManager486LP.ObtenerInstancia().UsuarioActual()?.DNI ?? "";
                string nombreUsuario = SessionManager486LP.ObtenerInstancia().UsuarioActual()?.NombreUsuario ?? "Sistema";

                _bllBitacora.Registrar(new BitacoraEvento486LP("Gestión Perfiles", $"Se quitó el permiso '{permiso.Nombre}' del perfil '{NombrePerfil(idPerfil)}'.",
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
            List<Permiso486LP> permisosNuevaFamilia = _bllFamilia.ObtenerPermisosDeFamilia(idFamilia);
            if (permisosNuevaFamilia.Count == 0)
            {
                return false; // si no tiene permisos, no puede solapar con nada
            }

            // IDs de permisos que ya vienen por las familias asignadas al perfil
            HashSet<int> permisosYaCubiertos = new HashSet<int>();
            List<Familia486LP> familiasAsignadas = _mapperPerfil.ListarFamiliasDePerfil(idPerfil);

            foreach (Familia486LP fam in familiasAsignadas)
            {
                if (fam.Id == idFamilia)
                {
                    continue; // por las dudas ignorar la misma familia
                }

                foreach (Permiso486LP perm in _bllFamilia.ObtenerPermisosDeFamilia(fam.Id))
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

            List<Permiso486LP> permisosFamilia = _bllFamilia.ObtenerPermisosDeFamilia(idFamilia);
            if (permisosFamilia.Count == 0)
                return quitados;

            HashSet<int> idsFamilia = new HashSet<int>(permisosFamilia.Select(p => p.Id));
            List<Permiso486LP> sueltos = _mapperPerfil.ListarPermisosDePerfil(idPerfil);

            foreach (Permiso486LP suelto in sueltos)
            {
                if (idsFamilia.Contains(suelto.Id))
                {
                    string m;
                    // La patente base suelta se puede quitar acá porque la familia que se asigna la aporta (el perfil no la pierde)
                    if (QuitarPermisoInterno(idPerfil, suelto.Id, false, out m)) // La BLL: valida, persiste y registra en bitácora
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
            List<Familia486LP> familiasAsignadas = _mapperPerfil.ListarFamiliasDePerfil(idPerfil);

            foreach (Familia486LP fam in familiasAsignadas)
            {
                List<Permiso486LP> permisosFamilia = _bllFamilia.ObtenerPermisosDeFamilia(fam.Id);
                if (permisosFamilia.Any(p => p.Id == idPermiso))
                {
                    return fam.Nombre; // la encontró
                }
            }

            return null; // ninguna familia asignada cubre ese permiso
        }

        // Asegura que el perfil tenga las patentes base, ya sea sueltas o a través de alguna familia asignada.
        // Se usa al crear un perfil y cuando se le quita una familia (que podía aportarlas).
        public void GarantizarPatentesBase(int idPerfil)
        {
            if (idPerfil <= 0) return;

            HashSet<int> cubiertos = new HashSet<int>(_mapperPerfil.ListarPermisosDePerfil(idPerfil).Select(p => p.Id));
            foreach (Familia486LP fam in _mapperPerfil.ListarFamiliasDePerfil(idPerfil))
            {
                foreach (Permiso486LP perm in _bllFamilia.ObtenerPermisosDeFamilia(fam.Id))
                    cubiertos.Add(perm.Id);
            }

            List<Permiso486LP> todas = _bllPatente.ObtenerPatentes();
            foreach (string codigo in _patentesBase)
            {
                Permiso486LP patente = todas.FirstOrDefault(x => x.Nombre == codigo);
                if (patente != null && !cubiertos.Contains(patente.Id))
                {
                    string msgIgnorado;
                    _mapperPerfil.AsignarPermiso(idPerfil, patente.Id, out msgIgnorado);
                }
            }

            string mensajeDV;
            new BLL_DV486LP().RecalcularDV("Perfil_Permiso", out mensajeDV);
        }

        // Nombres para la bitácora (se registran nombres legibles en vez de ids)
        private string NombrePerfil(int idPerfil)
        {
            Perfil486LP p = _mapperPerfil.Listar().FirstOrDefault(x => x.IdPerfil == idPerfil);
            return p != null ? p.Nombre : idPerfil.ToString();
        }

        private string NombreFamilia(int idFamilia)
        {
            Familia486LP f = _bllFamilia.ObtenerFamilias().FirstOrDefault(x => x.Id == idFamilia);
            return f != null ? f.Nombre : idFamilia.ToString();
        }

        private string NombrePatente(int idPermiso)
        {
            Permiso486LP p = _bllPatente.ObtenerPatentes().FirstOrDefault(x => x.Id == idPermiso);
            return p != null ? p.Nombre : idPermiso.ToString();
        }

        public bool TieneUsuariosAsignados(int idPerfil)
        {
            return _mapperPerfil.TieneUsuarios(idPerfil);
        }
        public bool TieneFamiliasAsignadas(int idPerfil)
        {
            List<Familia486LP> familias = _mapperPerfil.ListarFamiliasDePerfil(idPerfil);
            {
                return familias.Count > 0;
            }
        }

        public bool TienePermisosAsignados(int idPerfil)
        {
            List<Permiso486LP> permisos = _mapperPerfil.ListarPermisosDePerfil(idPerfil);
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

            return _mapperPerfil.ObtenerNombresPermisosPorRol(nombreRol);
        }

    }
}

using BE;
using Mappers;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BLL
{
    public class BLL_Marca486LP
    {
        private Mapper_Marca486LP ObjetoMapper = new Mapper_Marca486LP();
        private BLL_Bitacora486LP ObjBitacora = new BLL_Bitacora486LP();
        private BLL_DV486LP ObjDV = new BLL_DV486LP();

        private const string MODULO = "Maestros";

        // Lista todas las marcas (para la grilla y para el combo del form de productos)
        public List<Marca486LP> Listar()
        {
            try
            {
                return ObjetoMapper.Listar();
            }
            catch (Exception ex)
            {
                ObjBitacora.Registrar(new BitacoraEvento486LP(MODULO, $"Error en BLL_Marca.Listar(): {ex.Message}", Criticidad486LP.MuyAlta, "Sistema", "Sistema"));
                return new List<Marca486LP>();
            }
        }

        // Nombre: letras (con tildes y ñ), numeros, espacios y los signos . & ' - (ej. "New Balance", "Dr. Martens"). Maximo 50 caracteres
        public bool ValidarNombre(string nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre)) return false;
            nombre = nombre.Trim();
            return nombre.Length <= 50 && Regex.IsMatch(nombre, @"^[\p{L}\d][\p{L}\d .&'\u2019\-]*$");
        }

        private string ValidarDatos(Marca486LP marca)
        {
            if (marca == null || string.IsNullOrWhiteSpace(marca.Nombre))
                return Mensajes486LP.Obtener("Marca.NombreObligatorio"); // Muestra: Ingrese el nombre de la marca

            if (!ValidarNombre(marca.Nombre)) return Mensajes486LP.Obtener("Marca.NombreInvalido"); // Muestra: El nombre de la marca solo admite letras, números, espacios y los signos . & ' - (máximo 50 caracteres)

            return null;
        }

        public bool Agregar(Marca486LP marca, out string mensaje)
        {
            mensaje = "";

            string error = ValidarDatos(marca);
            if (error != null)
            {
                mensaje = error;
                return false;
            }

            try
            {
                marca.Nombre = marca.Nombre.Trim();

                if (ObjetoMapper.ExisteNombre(marca.Nombre, 0))
                {
                    mensaje = Mensajes486LP.Obtener("Marca.Duplicada"); // Muestra: Ya existe una marca con ese nombre
                    return false;
                }

                ObjetoMapper.Agregar(marca);

                RecalcularDVMarca();
                ObjBitacora.Registrar(new BitacoraEvento486LP(MODULO,
                    $"Alta de marca '{marca.Nombre}'", Criticidad486LP.Media, UsuarioDNI(), UsuarioNombre()));

                mensaje = Mensajes486LP.Obtener("Marca.Agregada"); // Muestra: Marca registrada correctamente
                return true;
            }
            catch (Exception ex)
            {
                ObjBitacora.Registrar(new BitacoraEvento486LP(MODULO, $"Error en BLL_Marca.Agregar(): {ex.Message}", Criticidad486LP.MuyAlta, "Sistema", "Sistema"));
                mensaje = Mensajes486LP.Obtener("General.ErrorInesperado"); // Muestra: Ocurrió un error inesperado. El detalle quedó registrado en la bitácora
                return false;
            }
        }

        public bool Modificar(Marca486LP marca, out string mensaje)
        {
            mensaje = "";

            string error = ValidarDatos(marca);
            if (error != null)
            {
                mensaje = error;
                return false;
            }

            try
            {
                marca.Nombre = marca.Nombre.Trim();

                if (ObjetoMapper.ExisteNombre(marca.Nombre, marca.IdMarca))
                {
                    mensaje = Mensajes486LP.Obtener("Marca.Duplicada"); // Muestra: Ya existe una marca con ese nombre
                    return false;
                }

                if (ObjetoMapper.Modificar(marca) == 0)
                {
                    mensaje = Mensajes486LP.Obtener("Marca.NoExiste"); // Muestra: La marca ya no existe. Actualice la lista y vuelva a intentar
                    return false;
                }

                RecalcularDVMarca();
                ObjBitacora.Registrar(new BitacoraEvento486LP(MODULO,
                    $"Modificacion de marca Id {marca.IdMarca}: '{marca.Nombre}'", Criticidad486LP.Media, UsuarioDNI(), UsuarioNombre()));

                mensaje = Mensajes486LP.Obtener("Marca.Modificada"); // Muestra: Marca modificada correctamente
                return true;
            }
            catch (Exception ex)
            {
                ObjBitacora.Registrar(new BitacoraEvento486LP(MODULO, $"Error en BLL_Marca.Modificar(): {ex.Message}", Criticidad486LP.MuyAlta, "Sistema", "Sistema"));
                mensaje = Mensajes486LP.Obtener("General.ErrorInesperado"); // Muestra: Ocurrió un error inesperado. El detalle quedó registrado en la bitácora
                return false;
            }
        }

        public bool Eliminar(int idMarca, out string mensaje)
        {
            mensaje = "";

            try
            {
                if (ObjetoMapper.EstaEnUso(idMarca))
                {
                    mensaje = Mensajes486LP.Obtener("Marca.EnUso"); // Muestra: No se puede eliminar la marca: tiene productos asociados
                    return false;
                }

                if (ObjetoMapper.Eliminar(idMarca) == 0)
                {
                    mensaje = Mensajes486LP.Obtener("Marca.NoExiste"); // Muestra: La marca ya no existe. Actualice la lista y vuelva a intentar
                    return false;
                }

                RecalcularDVMarca();
                ObjBitacora.Registrar(new BitacoraEvento486LP(MODULO,
                    $"Baja de marca Id {idMarca}", Criticidad486LP.Alta, UsuarioDNI(), UsuarioNombre()));

                mensaje = Mensajes486LP.Obtener("Marca.Eliminada"); // Muestra: Marca eliminada correctamente
                return true;
            }
            catch (Exception ex)
            {
                ObjBitacora.Registrar(new BitacoraEvento486LP(MODULO, $"Error en BLL_Marca.Eliminar(): {ex.Message}", Criticidad486LP.MuyAlta, "Sistema", "Sistema"));
                mensaje = Mensajes486LP.Obtener("General.ErrorInesperado"); // Muestra: Ocurrió un error inesperado. El detalle quedó registrado en la bitácora
                return false;
            }
        }

        private void RecalcularDVMarca()
        {
            string mensajeDV;
            ObjDV.RecalcularDV("Marca", out mensajeDV);
        }

        private string UsuarioDNI()
        {
            return SessionManager486LP.ObtenerInstancia().UsuarioActual()?.DNI ?? "Sistema";
        }

        private string UsuarioNombre()
        {
            return SessionManager486LP.ObtenerInstancia().UsuarioActual()?.NombreUsuario ?? "Sistema";
        }
    }
}

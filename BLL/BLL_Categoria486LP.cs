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
    public class BLL_Categoria486LP
    {
        private Mapper_Categoria486LP ObjetoMapper = new Mapper_Categoria486LP();
        private BLL_Bitacora486LP ObjBitacora = new BLL_Bitacora486LP();
        private BLL_DV486LP ObjDV = new BLL_DV486LP();

        private const string MODULO = "Maestros";

        // Lista todas las categorías (para la grilla y para el combo del form de productos)
        public List<Categoria486LP> Listar()
        {
            try
            {
                return ObjetoMapper.Listar();
            }
            catch (Exception ex)
            {
                ObjBitacora.Registrar(new BitacoraEvento486LP(MODULO, $"Error en BLL_Categoria.Listar(): {ex.Message}", Criticidad486LP.MuyAlta, "Sistema", "Sistema"));
                return new List<Categoria486LP>();
            }
        }

        // Nombre: letras (con tildes y ñ), numeros, espacios y los signos . & ' - (ej. "New Balance", "Dr. Martens"). Maximo 50 caracteres
        public bool ValidarNombre(string nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre)) return false;
            nombre = nombre.Trim();
            return nombre.Length <= 50 && Regex.IsMatch(nombre, @"^[\p{L}\d][\p{L}\d .&'\u2019\-]*$");
        }

        private string ValidarDatos(Categoria486LP categoria)
        {
            if (categoria == null || string.IsNullOrWhiteSpace(categoria.Nombre))
                return Mensajes486LP.Obtener("Categoria.NombreObligatorio"); // Muestra: Ingrese el nombre de la categoría

            if (!ValidarNombre(categoria.Nombre)) return Mensajes486LP.Obtener("Categoria.NombreInvalido"); // Muestra: El nombre de la categoría solo admite letras, números, espacios y los signos . & ' - (máximo 50 caracteres)

            return null;
        }

        public bool Agregar(Categoria486LP categoria, out string mensaje)
        {
            mensaje = "";

            string error = ValidarDatos(categoria);
            if (error != null)
            {
                mensaje = error;
                return false;
            }

            try
            {
                categoria.Nombre = categoria.Nombre.Trim();

                if (ObjetoMapper.ExisteNombre(categoria.Nombre, 0))
                {
                    mensaje = Mensajes486LP.Obtener("Categoria.Duplicada"); // Muestra: Ya existe una categoría con ese nombre
                    return false;
                }

                ObjetoMapper.Agregar(categoria);

                RecalcularDVCategoria();
                ObjBitacora.Registrar(new BitacoraEvento486LP(MODULO,
                    $"Alta de categoria '{categoria.Nombre}'", Criticidad486LP.Media, UsuarioDNI(), UsuarioNombre()));

                mensaje = Mensajes486LP.Obtener("Categoria.Agregada"); // Muestra: Categoría registrada correctamente
                return true;
            }
            catch (Exception ex)
            {
                ObjBitacora.Registrar(new BitacoraEvento486LP(MODULO, $"Error en BLL_Categoria.Agregar(): {ex.Message}", Criticidad486LP.MuyAlta, "Sistema", "Sistema"));
                mensaje = Mensajes486LP.Obtener("General.ErrorInesperado"); // Muestra: Ocurrió un error inesperado. El detalle quedó registrado en la bitácora
                return false;
            }
        }

        public bool Modificar(Categoria486LP categoria, out string mensaje)
        {
            mensaje = "";

            string error = ValidarDatos(categoria);
            if (error != null)
            {
                mensaje = error;
                return false;
            }

            try
            {
                categoria.Nombre = categoria.Nombre.Trim();

                if (ObjetoMapper.ExisteNombre(categoria.Nombre, categoria.IdCategoria))
                {
                    mensaje = Mensajes486LP.Obtener("Categoria.Duplicada"); // Muestra: Ya existe una categoría con ese nombre
                    return false;
                }

                if (ObjetoMapper.Modificar(categoria) == 0)
                {
                    mensaje = Mensajes486LP.Obtener("Categoria.NoExiste"); // Muestra: La categoría ya no existe. Actualice la lista y vuelva a intentar
                    return false;
                }

                RecalcularDVCategoria();
                ObjBitacora.Registrar(new BitacoraEvento486LP(MODULO,
                    $"Modificacion de categoria Id {categoria.IdCategoria}: '{categoria.Nombre}'", Criticidad486LP.Media, UsuarioDNI(), UsuarioNombre()));

                mensaje = Mensajes486LP.Obtener("Categoria.Modificada"); // Muestra: Categoría modificada correctamente
                return true;
            }
            catch (Exception ex)
            {
                ObjBitacora.Registrar(new BitacoraEvento486LP(MODULO, $"Error en BLL_Categoria.Modificar(): {ex.Message}", Criticidad486LP.MuyAlta, "Sistema", "Sistema"));
                mensaje = Mensajes486LP.Obtener("General.ErrorInesperado"); // Muestra: Ocurrió un error inesperado. El detalle quedó registrado en la bitácora
                return false;
            }
        }

        public bool Eliminar(int idCategoria, out string mensaje)
        {
            mensaje = "";

            try
            {
                if (ObjetoMapper.EstaEnUso(idCategoria))
                {
                    mensaje = Mensajes486LP.Obtener("Categoria.EnUso"); // Muestra: No se puede eliminar la categoría: tiene productos asociados
                    return false;
                }

                if (ObjetoMapper.Eliminar(idCategoria) == 0)
                {
                    mensaje = Mensajes486LP.Obtener("Categoria.NoExiste"); // Muestra: La categoría ya no existe. Actualice la lista y vuelva a intentar
                    return false;
                }

                RecalcularDVCategoria();
                ObjBitacora.Registrar(new BitacoraEvento486LP(MODULO,
                    $"Baja de categoria Id {idCategoria}", Criticidad486LP.Alta, UsuarioDNI(), UsuarioNombre()));

                mensaje = Mensajes486LP.Obtener("Categoria.Eliminada"); // Muestra: Categoría eliminada correctamente
                return true;
            }
            catch (Exception ex)
            {
                ObjBitacora.Registrar(new BitacoraEvento486LP(MODULO, $"Error en BLL_Categoria.Eliminar(): {ex.Message}", Criticidad486LP.MuyAlta, "Sistema", "Sistema"));
                mensaje = Mensajes486LP.Obtener("General.ErrorInesperado"); // Muestra: Ocurrió un error inesperado. El detalle quedó registrado en la bitácora
                return false;
            }
        }

        private void RecalcularDVCategoria()
        {
            string mensajeDV;
            ObjDV.RecalcularDV("Categoria", out mensajeDV);
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

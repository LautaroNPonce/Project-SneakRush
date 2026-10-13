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
    public class BLL_Proveedor486LP
    {
        private Mapper_Proveedor486LP ObjetoMapper = new Mapper_Proveedor486LP();
        private BLL_Bitacora486LP ObjBitacora = new BLL_Bitacora486LP();
        private BLL_DV486LP ObjDV = new BLL_DV486LP();

        private const string MODULO = "Maestros";

        public List<Proveedor486LP> Listar()
        {
            try
            {
                return ObjetoMapper.Listar();
            }
            catch (Exception ex)
            {
                ObjBitacora.Registrar(new BitacoraEvento486LP("Compras", $"Error en BLL_Proveedor.Listar(): {ex.Message}", Criticidad486LP.MuyAlta, "Sistema", "Sistema"));
                return new List<Proveedor486LP>();
            }
        }

        // Nombre / razon social: letras, numeros, espacios y los signos . , & ' - (ej. "Distribuidora Calzado S.A."). Maximo 100 caracteres
        public bool ValidarNombre(string nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre)) return false;
            nombre = nombre.Trim();
            return nombre.Length <= 100 && Regex.IsMatch(nombre, @"^[\p{L}\d][\p{L}\d .,&'\u2019\-]*$");
        }

        // Deja el CUIT solo con digitos (acepta 30-70123456-8, 30 70123456 8 o 30701234568)
        public string NormalizarCUIT(string cuit)
        {
            if (string.IsNullOrWhiteSpace(cuit)) return "";
            return new string(cuit.Where(char.IsDigit).ToArray());
        }

        // CUIT: 11 digitos, prefijo valido (20, 23, 24, 27, 30, 33 o 34) y digito verificador correcto (modulo 11)
        public bool ValidarCUIT(string cuit)
        {
            string c = NormalizarCUIT(cuit);
            if (c.Length != 11) return false;
            if (!new[] { "20", "23", "24", "27", "30", "33", "34" }.Contains(c.Substring(0, 2))) return false;

            int[] pesos = { 5, 4, 3, 2, 7, 6, 5, 4, 3, 2 };
            int suma = 0;
            for (int i = 0; i < 10; i++)
                suma += (c[i] - '0') * pesos[i];

            int digito = 11 - (suma % 11);
            if (digito == 11) digito = 0;
            if (digito == 10) return false;

            return digito == (c[10] - '0');
        }

        public bool ValidarCorreo(string correo)
        {
            if (string.IsNullOrWhiteSpace(correo)) return false;
            correo = correo.Trim();
            return correo.Length <= 100 &&
                   Regex.IsMatch(correo, @"^[A-Za-z0-9]+(?:[._%+\-][A-Za-z0-9]+)*@(?:[A-Za-z0-9](?:[A-Za-z0-9\-]*[A-Za-z0-9])?\.)+[A-Za-z]{2,}$");
        }

        // El telefono es opcional
        public bool ValidarTelefono(string telefono)
        {
            if (string.IsNullOrWhiteSpace(telefono)) return true;
            telefono = telefono.Trim();
            if (!Regex.IsMatch(telefono, @"^\+?[\d\s\-()]+$")) return false;
            int digitos = telefono.Count(char.IsDigit);
            return digitos >= 8 && digitos <= 15;
        }

        private string ValidarDatos(Proveedor486LP proveedor)
        {
            if (proveedor == null ||
                string.IsNullOrWhiteSpace(proveedor.Nombre) ||
                string.IsNullOrWhiteSpace(proveedor.CUIT) ||
                string.IsNullOrWhiteSpace(proveedor.Correo))
                return Mensajes486LP.Obtener("Proveedor.CamposObligatorios"); // Muestra: Complete los campos obligatorios (nombre, CUIT y correo)

            if (!ValidarNombre(proveedor.Nombre)) return Mensajes486LP.Obtener("Proveedor.NombreInvalido"); // Muestra: El nombre del proveedor solo admite letras, números, espacios y los signos . , & ' - (máximo 100 caracteres)
            if (!ValidarCUIT(proveedor.CUIT)) return Mensajes486LP.Obtener("Proveedor.CUITInvalido"); // Muestra: El CUIT no es válido. Debe tener 11 dígitos y un dígito verificador correcto
            if (!ValidarCorreo(proveedor.Correo)) return Mensajes486LP.Obtener("Proveedor.CorreoInvalido"); // Muestra: El correo no tiene un formato válido. Debe ser del tipo usuario@dominio.com
            if (!ValidarTelefono(proveedor.Telefono)) return Mensajes486LP.Obtener("Proveedor.TelefonoInvalido"); // Muestra: El teléfono solo admite números, espacios, +, - y paréntesis, y debe tener entre 8 y 15 dígitos

            return null;
        }

        // Deja los datos listos para guardar: sin espacios de mas y el CUIT solo con digitos
        private void Normalizar(Proveedor486LP proveedor)
        {
            proveedor.Nombre = proveedor.Nombre.Trim();
            proveedor.CUIT = NormalizarCUIT(proveedor.CUIT);
            proveedor.Correo = proveedor.Correo.Trim();
            proveedor.Telefono = string.IsNullOrWhiteSpace(proveedor.Telefono) ? null : proveedor.Telefono.Trim();
        }

        public bool Agregar(Proveedor486LP proveedor, out string mensaje)
        {
            mensaje = "";

            string error = ValidarDatos(proveedor);
            if (error != null)
            {
                mensaje = error;
                return false;
            }

            try
            {
                Normalizar(proveedor);

                if (ObjetoMapper.ExisteNombre(proveedor.Nombre, 0))
                {
                    mensaje = Mensajes486LP.Obtener("Proveedor.NombreDuplicado"); // Muestra: Ya existe un proveedor con ese nombre
                    return false;
                }

                if (ObjetoMapper.ExisteCUIT(proveedor.CUIT, 0))
                {
                    mensaje = Mensajes486LP.Obtener("Proveedor.CUITDuplicado"); // Muestra: Ya existe un proveedor con ese CUIT
                    return false;
                }

                ObjetoMapper.Agregar(proveedor);

                RecalcularDVProveedor();
                ObjBitacora.Registrar(new BitacoraEvento486LP(MODULO,
                    $"Alta de proveedor '{proveedor.Nombre}' (CUIT {proveedor.CUIT})", Criticidad486LP.Media, UsuarioDNI(), UsuarioNombre()));

                mensaje = Mensajes486LP.Obtener("Proveedor.Agregado"); // Muestra: Proveedor registrado correctamente
                return true;
            }
            catch (Exception ex)
            {
                ObjBitacora.Registrar(new BitacoraEvento486LP(MODULO, $"Error en BLL_Proveedor.Agregar(): {ex.Message}", Criticidad486LP.MuyAlta, "Sistema", "Sistema"));
                mensaje = Mensajes486LP.Obtener("General.ErrorInesperado"); // Muestra: Ocurrió un error inesperado. El detalle quedó registrado en la bitácora
                return false;
            }
        }

        public bool Modificar(Proveedor486LP proveedor, out string mensaje)
        {
            mensaje = "";

            string error = ValidarDatos(proveedor);
            if (error != null)
            {
                mensaje = error;
                return false;
            }

            try
            {
                Normalizar(proveedor);

                if (ObjetoMapper.ExisteNombre(proveedor.Nombre, proveedor.IdProveedor))
                {
                    mensaje = Mensajes486LP.Obtener("Proveedor.NombreDuplicado"); // Muestra: Ya existe un proveedor con ese nombre
                    return false;
                }

                if (ObjetoMapper.ExisteCUIT(proveedor.CUIT, proveedor.IdProveedor))
                {
                    mensaje = Mensajes486LP.Obtener("Proveedor.CUITDuplicado"); // Muestra: Ya existe un proveedor con ese CUIT
                    return false;
                }

                if (ObjetoMapper.Modificar(proveedor) == 0)
                {
                    mensaje = Mensajes486LP.Obtener("Proveedor.NoExiste"); // Muestra: El proveedor ya no existe. Actualice la lista y vuelva a intentar
                    return false;
                }

                RecalcularDVProveedor();
                ObjBitacora.Registrar(new BitacoraEvento486LP(MODULO,
                    $"Modificacion de proveedor Id {proveedor.IdProveedor}: '{proveedor.Nombre}'", Criticidad486LP.Media, UsuarioDNI(), UsuarioNombre()));

                mensaje = Mensajes486LP.Obtener("Proveedor.Modificado"); // Muestra: Proveedor modificado correctamente
                return true;
            }
            catch (Exception ex)
            {
                ObjBitacora.Registrar(new BitacoraEvento486LP(MODULO, $"Error en BLL_Proveedor.Modificar(): {ex.Message}", Criticidad486LP.MuyAlta, "Sistema", "Sistema"));
                mensaje = Mensajes486LP.Obtener("General.ErrorInesperado"); // Muestra: Ocurrió un error inesperado. El detalle quedó registrado en la bitácora
                return false;
            }
        }

        public bool Eliminar(int idProveedor, out string mensaje)
        {
            mensaje = "";

            try
            {
                if (ObjetoMapper.TieneMovimientos(idProveedor))
                {
                    mensaje = Mensajes486LP.Obtener("Proveedor.TieneMovimientos"); // Muestra: No se puede eliminar el proveedor: tiene órdenes de compra asociadas
                    return false;
                }

                if (ObjetoMapper.Eliminar(idProveedor) == 0)
                {
                    mensaje = Mensajes486LP.Obtener("Proveedor.NoExiste"); // Muestra: El proveedor ya no existe. Actualice la lista y vuelva a intentar
                    return false;
                }

                RecalcularDVProveedor();
                ObjBitacora.Registrar(new BitacoraEvento486LP(MODULO,
                    $"Baja de proveedor Id {idProveedor}", Criticidad486LP.Alta, UsuarioDNI(), UsuarioNombre()));

                mensaje = Mensajes486LP.Obtener("Proveedor.Eliminado"); // Muestra: Proveedor eliminado correctamente
                return true;
            }
            catch (Exception ex)
            {
                ObjBitacora.Registrar(new BitacoraEvento486LP(MODULO, $"Error en BLL_Proveedor.Eliminar(): {ex.Message}", Criticidad486LP.MuyAlta, "Sistema", "Sistema"));
                mensaje = Mensajes486LP.Obtener("General.ErrorInesperado"); // Muestra: Ocurrió un error inesperado. El detalle quedó registrado en la bitácora
                return false;
            }
        }

        private void RecalcularDVProveedor()
        {
            string mensajeDV;
            ObjDV.RecalcularDV("Proveedor", out mensajeDV);
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

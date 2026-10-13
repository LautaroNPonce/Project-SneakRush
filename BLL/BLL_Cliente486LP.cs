using BE;
using DAL;
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
    public class BLL_Cliente486LP
    {
        private Mapper_Cliente486LP ObjetoMapper = new Mapper_Cliente486LP();
        private BLL_Bitacora486LP ObjBitacora = new BLL_Bitacora486LP();
        private BLL_DV486LP ObjDV = new BLL_DV486LP();

        private const string MODULO = "Ventas";

        // Lista todos los clientes, devolviendo el correo yA descifrado (para mostrar en la GUI)
        public List<Cliente486LP> Listar()
        {
            try
            {
                List<Cliente486LP> lista = ObjetoMapper.Listar();
                foreach (Cliente486LP c in lista)
                {
                    c.Correo = DescifrarCorreo(c.Correo);
                }
                return lista;
            }
            catch (Exception ex)
            {
                ObjBitacora.Registrar(new BitacoraEvento486LP(MODULO, $"Error en BLL_Cliente.Listar(): {ex.Message}", Criticidad486LP.MuyAlta, "Sistema", "Sistema"));
                return new List<Cliente486LP>();
            }
        }

        // Obtiene un cliente por DNI, con el Correo descifrado. Null si no existe
        public Cliente486LP Obtener(string dni)
        {
            try
            {
                Cliente486LP c = ObjetoMapper.Obtener(dni);
                if (c != null)
                {
                    c.Correo = DescifrarCorreo(c.Correo);
                }
                return c;
            }
            catch (Exception ex)
            {
                ObjBitacora.Registrar(new BitacoraEvento486LP(MODULO, $"Error en BLL_Cliente.Obtener(): {ex.Message}", Criticidad486LP.MuyAlta, "Sistema", "Sistema"));
                return null;
            }
        }

        public bool Existe(string dni)
        {
            return ObjetoMapper.Existe(dni);
        }

        public bool ValidarDNI(string dni)
        {
            if (string.IsNullOrWhiteSpace(dni)) return false;
            return Regex.IsMatch(dni.Trim(), @"^\d{7,8}$");
        }

        // Nombre y apellido: solo letras (con tildes y ñ), espacios, guiones y apostrofes. Maximo 50 caracteres
        public bool ValidarNombrePersona(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto)) return false;
            texto = texto.Trim();
            return texto.Length <= 50 && Regex.IsMatch(texto, @"^\p{L}+(?:[ '\u2019\-]\p{L}+)*$");
        }

        public bool ValidarCorreo(string correo)
        {
            if (string.IsNullOrWhiteSpace(correo)) return false;
            correo = correo.Trim();
            return correo.Length <= 100 &&
                   Regex.IsMatch(correo, @"^[A-Za-z0-9]+(?:[._%+\-][A-Za-z0-9]+)*@(?:[A-Za-z0-9](?:[A-Za-z0-9\-]*[A-Za-z0-9])?\.)+[A-Za-z]{2,}$");
        }

        public bool ValidarTelefono(string telefono)
        {
            if (string.IsNullOrWhiteSpace(telefono)) return true;
            telefono = telefono.Trim();
            if (!Regex.IsMatch(telefono, @"^\+?[\d\s\-()]+$")) return false;
            int digitos = telefono.Count(char.IsDigit);
            return digitos >= 8 && digitos <= 15;
        }

        private string ValidarDatos(Cliente486LP cliente)
        {
            if (cliente == null ||
                string.IsNullOrWhiteSpace(cliente.DNI) ||
                string.IsNullOrWhiteSpace(cliente.Nombre) ||
                string.IsNullOrWhiteSpace(cliente.Apellido) ||
                string.IsNullOrWhiteSpace(cliente.Correo))
                return Mensajes486LP.Obtener("Cliente.CamposObligatorios"); // Muestra: Complete los campos obligatorios (DNI, nombre, apellido y correo)

            if (!ValidarDNI(cliente.DNI)) return Mensajes486LP.Obtener("Cliente.DNIInvalido"); // Muestra: El DNI debe tener 7 u 8 dígitos numéricos
            if (!ValidarNombrePersona(cliente.Nombre)) return Mensajes486LP.Obtener("Cliente.NombreInvalido"); // Muestra: El nombre solo admite letras, espacios, guiones y apóstrofes (máximo 50 caracteres)
            if (!ValidarNombrePersona(cliente.Apellido)) return Mensajes486LP.Obtener("Cliente.ApellidoInvalido"); // Muestra: El apellido solo admite letras, espacios, guiones y apóstrofes (máximo 50 caracteres)
            if (!ValidarCorreo(cliente.Correo)) return Mensajes486LP.Obtener("Cliente.CorreoInvalido"); // Muestra: El correo no tiene un formato válido. Debe ser del tipo usuario@dominio.com
            if (!ValidarTelefono(cliente.Telefono)) return Mensajes486LP.Obtener("Cliente.TelefonoInvalido"); // Muestra: El teléfono solo admite números, espacios, +, - y paréntesis, y debe tener entre 8 y 15 dígitos

            return null;
        }

        public bool Agregar(Cliente486LP cliente, out string mensaje)
        {
            mensaje = "";

            // Validaciones de negocio
            string error = ValidarDatos(cliente);
            if (error != null)
            {
                mensaje = error;
                return false;
            }

            try
            {
                if (ObjetoMapper.Existe(cliente.DNI))
                {
                    mensaje = Mensajes486LP.Obtener("Cliente.DNIDuplicado"); // Muestra: Ya existe un cliente registrado con ese DNI
                    return false;
                }

                // Se cifra el correo antes de persistir
                cliente.Correo = Encriptacion486LP.EncriptarAES(cliente.Correo.Trim());

                ObjetoMapper.Agregar(cliente);

                RecalcularDVCliente();
                ObjBitacora.Registrar(new BitacoraEvento486LP(MODULO,
                    $"Alta de cliente DNI {cliente.DNI}", Criticidad486LP.Media, cliente.DNI, "Sistema"));

                mensaje = Mensajes486LP.Obtener("Cliente.Agregado"); // Muestra: Cliente registrado correctamente
                return true;
            }
            catch (Exception ex)
            {
                ObjBitacora.Registrar(new BitacoraEvento486LP(MODULO, $"Error en BLL_Cliente.Agregar(): {ex.Message}", Criticidad486LP.MuyAlta, "Sistema", "Sistema"));
                mensaje = Mensajes486LP.Obtener("General.ErrorInesperado"); // Muestra: Ocurrió un error inesperado. El detalle quedó registrado en la bitácora
                return false;
            }
        }

        public bool Modificar(Cliente486LP cliente, out string mensaje)
        {
            mensaje = "";

            string error = ValidarDatos(cliente);
            if (error != null)
            {
                mensaje = error;
                return false;
            }

            try
            {
                if (!ObjetoMapper.Existe(cliente.DNI))
                {
                    mensaje = Mensajes486LP.Obtener("Cliente.NoExisteModificar"); // Muestra: No existe un cliente con ese DNI para modificar
                    return false;
                }

                cliente.Correo = Encriptacion486LP.EncriptarAES(cliente.Correo.Trim());

                ObjetoMapper.Modificar(cliente);

                RecalcularDVCliente();
                ObjBitacora.Registrar(new BitacoraEvento486LP(MODULO,
                    $"Modificacion de cliente DNI {cliente.DNI}", Criticidad486LP.Media, cliente.DNI, "Sistema"));

                mensaje = Mensajes486LP.Obtener("Cliente.Modificado"); // Muestra: Cliente modificado correctamente
                return true;
            }
            catch (Exception ex)
            {
                ObjBitacora.Registrar(new BitacoraEvento486LP(MODULO, $"Error en BLL_Cliente.Modificar(): {ex.Message}", Criticidad486LP.MuyAlta, "Sistema", "Sistema"));
                mensaje = Mensajes486LP.Obtener("General.ErrorInesperado"); // Muestra: Ocurrió un error inesperado. El detalle quedó registrado en la bitácora
                return false;
            }
        }

        public bool Eliminar(string dni, out string mensaje)
        {
            mensaje = "";


            try
            {
                if (!ObjetoMapper.Existe(dni))
                {
                    mensaje = Mensajes486LP.Obtener("Cliente.NoExisteEliminar"); // Muestra: No existe un cliente con ese DNI para eliminar
                    return false;
                }

                if (ObjetoMapper.TieneVentas(dni))
                {
                    mensaje = Mensajes486LP.Obtener("Cliente.TieneVentas"); // Muestra: No se puede eliminar el cliente: tiene ventas asociadas
                    return false;
                }

                ObjetoMapper.Eliminar(dni);

                RecalcularDVCliente();
                ObjBitacora.Registrar(new BitacoraEvento486LP(MODULO,
                    $"Baja de cliente DNI {dni}", Criticidad486LP.Alta, dni, "Sistema"));

                mensaje = Mensajes486LP.Obtener("Cliente.Eliminado"); // Muestra: Cliente eliminado correctamente
                return true;
            }
            catch (Exception ex)
            {
                ObjBitacora.Registrar(new BitacoraEvento486LP(MODULO, $"Error en BLL_Cliente.Eliminar(): {ex.Message}", Criticidad486LP.MuyAlta, "Sistema", "Sistema"));
                mensaje = Mensajes486LP.Obtener("General.ErrorInesperado"); // Muestra: Ocurrió un error inesperado. El detalle quedó registrado en la bitácora
                return false;
            }
        }
        private void RecalcularDVCliente()
        {
            string mensajeDV;
            ObjDV.RecalcularDV("Cliente", out mensajeDV);
        }

        // Descifra el correo con tolerancia si el dato no esta cifrado (o falla), devuelve el valor original para no romper la grilla.
        private string DescifrarCorreo(string correo)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(correo)) return correo;
                if (!Encriptacion486LP.EsBase64(correo)) return correo;
                return Encriptacion486LP.DesencriptarAES(correo);
            }
            catch
            {
                return correo;
            }
        }
    }
}

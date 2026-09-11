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

        // Lista todos los clientes, devolviendo el Correo YA descifrado (para mostrar en la GUI).
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

        // Obtiene un cliente por DNI, con el Correo descifrado. Null si no existe.
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

        // Indica si existe un cliente con ese DNI.
        public bool Existe(string dni)
        {
            return ObjetoMapper.Existe(dni);
        }

        // Valida el formato del DNI: solo numeros, entre 7 y 10 digitos.
        public bool ValidarDNI(string dni)
        {
            if (string.IsNullOrWhiteSpace(dni)) return false;
            return Regex.IsMatch(dni.Trim(), @"^\d{7,10}$");
        }

        public bool Agregar(Cliente486LP cliente, out string mensaje)
        {
            mensaje = "";

            // Validaciones de negocio
            if (!ValidarDNI(cliente.DNI))
            {
                mensaje = "El formato del DNI es invalido (debe tener entre 7 y 10 digitos numericos).";
                return false;
            }
            if (string.IsNullOrWhiteSpace(cliente.Nombre) ||
                string.IsNullOrWhiteSpace(cliente.Apellido) ||
                string.IsNullOrWhiteSpace(cliente.Correo))
            {
                mensaje = "Complete los campos obligatorios (Nombre, Apellido y Correo).";
                return false;
            }
            if (ObjetoMapper.Existe(cliente.DNI))
            {
                mensaje = "Ya existe un cliente registrado con ese DNI.";
                return false;
            }

            try
            {
                // Se cifra el correo antes de persistir
                cliente.Correo = Encriptacion486LP.EncriptarAES(cliente.Correo.Trim());

                ObjetoMapper.Agregar(cliente);

                RecalcularDVCliente();
                ObjBitacora.Registrar(new BitacoraEvento486LP(MODULO,
                    $"Alta de cliente DNI {cliente.DNI}", Criticidad486LP.Media, cliente.DNI, "Sistema"));

                mensaje = "Cliente registrado correctamente.";
                return true;
            }
            catch (Exception ex)
            {
                ObjBitacora.Registrar(new BitacoraEvento486LP(MODULO, $"Error en BLL_Cliente.Agregar(): {ex.Message}", Criticidad486LP.MuyAlta, "Sistema", "Sistema"));
                mensaje = "Ocurrio un error al registrar el cliente.";
                return false;
            }
        }

        public bool Modificar(Cliente486LP cliente, out string mensaje)
        {
            mensaje = "";

            if (!ValidarDNI(cliente.DNI))
            {
                mensaje = "El formato del DNI es invalido (debe tener entre 7 y 10 digitos numericos).";
                return false;
            }
            if (string.IsNullOrWhiteSpace(cliente.Nombre) ||
                string.IsNullOrWhiteSpace(cliente.Apellido) ||
                string.IsNullOrWhiteSpace(cliente.Correo))
            {
                mensaje = "Complete los campos obligatorios (Nombre, Apellido y Correo).";
                return false;
            }
            if (!ObjetoMapper.Existe(cliente.DNI))
            {
                mensaje = "No existe un cliente con ese DNI para modificar.";
                return false;
            }

            try
            {
                cliente.Correo = Encriptacion486LP.EncriptarAES(cliente.Correo.Trim());

                ObjetoMapper.Modificar(cliente);

                RecalcularDVCliente();
                ObjBitacora.Registrar(new BitacoraEvento486LP(MODULO,
                    $"Modificacion de cliente DNI {cliente.DNI}", Criticidad486LP.Media, cliente.DNI, "Sistema"));

                mensaje = "Cliente modificado correctamente.";
                return true;
            }
            catch (Exception ex)
            {
                ObjBitacora.Registrar(new BitacoraEvento486LP(MODULO, $"Error en BLL_Cliente.Modificar(): {ex.Message}", Criticidad486LP.MuyAlta, "Sistema", "Sistema"));
                mensaje = "Ocurrio un error al modificar el cliente.";
                return false;
            }
        }

        public bool Eliminar(string dni, out string mensaje)
        {
            mensaje = "";

            if (!ObjetoMapper.Existe(dni))
            {
                mensaje = "No existe un cliente con ese DNI para eliminar.";
                return false;
            }

            try
            {
                ObjetoMapper.Eliminar(dni);

                RecalcularDVCliente();
                ObjBitacora.Registrar(new BitacoraEvento486LP(MODULO,
                    $"Baja de cliente DNI {dni}", Criticidad486LP.Alta, dni, "Sistema"));

                mensaje = "Cliente eliminado correctamente.";
                return true;
            }
            catch (Exception ex)
            {
                ObjBitacora.Registrar(new BitacoraEvento486LP(MODULO, $"Error en BLL_Cliente.Eliminar(): {ex.Message}", Criticidad486LP.MuyAlta, "Sistema", "Sistema"));
                mensaje = "Ocurrio un error al eliminar el cliente.";
                return false;
            }
        }
        private void RecalcularDVCliente()
        {
            string mensajeDV;
            ObjDV.RecalcularDV("Cliente", out mensajeDV);
        }

        // Descifra el correo con tolerancia: si el dato no esta cifrado (o falla), devuelve el valor original para no romper la grilla.
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

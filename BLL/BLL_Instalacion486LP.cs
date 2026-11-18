using BE;
using DAL;
using Services;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;

namespace BLL
{
    public class BLL_Instalacion486LP
    {
        public const int OK = 0;
        public const int DATOS_INVALIDOS = 1;
        public const int ERROR_BASE = 2;
        public const int BASE_CON_USUARIOS = 3;
        public const int ERROR_RESPALDO = 4;
        public const int BASE_INCOMPLETA = 5;
        public const int ARGUMENTOS_INVALIDOS = 9;
        public const int ESTADO_SIN_BASE = 10;
        public const int ESTADO_BASE_VACIA = 11;
        public const int ESTADO_BASE_CON_USUARIOS = 12;

        private BLL_Usuarios486LP _bllUsuarios = new BLL_Usuarios486LP();

        public ResultadoInstalacion486LP Estado()
        {
            try
            {
                if (!InicializadorBD486LP.ExisteLaBase())
                    return Resultado(ESTADO_SIN_BASE, "No hay base de datos.");

                try
                {
                    return InicializadorBD486LP.ContarUsuarios() > 0
                        ? Resultado(ESTADO_BASE_CON_USUARIOS, "La base ya tiene usuarios.")
                        : Resultado(ESTADO_BASE_VACIA, "La base existe y no tiene usuarios.");
                }
                catch (SqlException ex) when (ex.Number == 208)
                {
                    return Resultado(BASE_INCOMPLETA, "Se encontró una base de datos incompleta o de una versión anterior.");
                }
            }
            catch (Exception ex)
            {
                return Resultado(ERROR_BASE, MensajeSql(ex));
            }
        }

        // Instalacion nueva: crea la base si no existe y da de alta al administrador, si la base ya tiene usuarios no toca nada
        public ResultadoInstalacion486LP Preparar(DatosAdminInstalacion486LP datos)
        {
            ResultadoInstalacion486LP invalido = ValidarDatos(datos);
            if (invalido != null) return invalido;

            bool laCreeYo = false;
            try
            {
                bool existia = InicializadorBD486LP.ExisteLaBase();

                if (existia)
                {
                    try
                    {
                        if (InicializadorBD486LP.ContarUsuarios() > 0)
                            return Resultado(BASE_CON_USUARIOS, "Se encontró una base con usuarios y se conservó sin cambios. El administrador cargado en el asistente no se creó.");
                    }
                    catch (SqlException ex) when (ex.Number == 208)
                    {
                        return Resultado(BASE_INCOMPLETA, "Se encontró una base de datos incompleta o de una versión anterior. Volvé a ejecutar el instalador y elegí \"Reinstalar con base nueva\".");
                    }
                }
                else
                {
                    InicializadorBD486LP.CrearBase();
                    laCreeYo = true;
                }

                return CrearAdministrador(datos, laCreeYo);
            }
            catch (Exception ex)
            {
                if (laCreeYo) IntentarEliminarBase();
                return Resultado(ERROR_BASE, MensajeSql(ex));
            }
        }

        // Reinstalacion con base nueva: respalda la base actual, la borra, crea una limpia con el administrador y si algo falla, vuelve a dejar la base anterior
        public ResultadoInstalacion486LP Reiniciar(DatosAdminInstalacion486LP datos, string carpetaRespaldos)
        {
            ResultadoInstalacion486LP invalido = ValidarDatos(datos);
            if (invalido != null) return invalido;

            string rutaRespaldo = string.Empty;
            bool habiaBase = false;
            bool eliminacionIniciada = false;

            try
            {
                habiaBase = InicializadorBD486LP.ExisteLaBase();

                if (habiaBase)
                {
                    string errorRespaldo;
                    if (!HacerRespaldo(carpetaRespaldos, "Reinstalacion", out rutaRespaldo, out errorRespaldo))
                        return Resultado(ERROR_RESPALDO, "No se pudo guardar el respaldo de la base actual, así que no se borró nada. Detalle: " + errorRespaldo);

                    eliminacionIniciada = true;
                    InicializadorBD486LP.EliminarBase();
                }

                InicializadorBD486LP.CrearBase();
                ResultadoInstalacion486LP r = CrearAdministrador(datos, true);
                r.RutaRespaldo = rutaRespaldo;

                if (r.Codigo != OK)
                {
                    return VolverAtras(habiaBase, rutaRespaldo, r.Mensaje);
                }
                return r;
            }
            catch (Exception ex)
            {
                if (eliminacionIniciada)
                    return VolverAtras(habiaBase, rutaRespaldo, MensajeSql(ex));

                return Resultado(ERROR_BASE, MensajeSql(ex));
            }
        }

        // Desinstalacion con borrado de la base: guarda un respaldo y despues la elimina
        public ResultadoInstalacion486LP BorrarBase(string carpetaRespaldos)
        {
            try
            {
                if (!InicializadorBD486LP.ExisteLaBase())
                    return Resultado(OK, "No había base de datos para borrar.");

                string ruta;
                string error;
                if (!HacerRespaldo(carpetaRespaldos, "Desinstalacion", out ruta, out error))
                    return Resultado(ERROR_RESPALDO, "No se pudo guardar el respaldo, así que la base no se borró. Detalle: " + error);

                InicializadorBD486LP.EliminarBase();

                ResultadoInstalacion486LP r = Resultado(OK, "Base eliminada. Se guardó un respaldo en: " + ruta);
                r.RutaRespaldo = ruta;
                return r;
            }
            catch (Exception ex)
            {
                return Resultado(ERROR_BASE, MensajeSql(ex));
            }
        }

        private ResultadoInstalacion486LP CrearAdministrador(DatosAdminInstalacion486LP datos, bool limpiarSiFalla)
        {
            string usuario;
            string clave;
            string mensaje;

            if (!_bllUsuarios.CrearAdministradorInicial(datos.DNI, datos.Nombre, datos.Apellido, datos.Correo, out usuario, out clave, out mensaje))
            {
                if (limpiarSiFalla) IntentarEliminarBase();
                return Resultado(ERROR_BASE, "No se pudo crear el administrador: " + mensaje);
            }

            string mensajeDV;
            if (!new BLL_DV486LP().RecalcularTodas(out mensajeDV))
            {
                if (limpiarSiFalla) IntentarEliminarBase();
                return Resultado(ERROR_BASE, "No se pudieron calcular los dígitos verificadores: " + mensajeDV);
            }

            ResultadoInstalacion486LP ok = Resultado(OK, "Instalación preparada.");
            ok.Usuario = usuario;
            ok.ContraseñaInicial = clave;
            return ok;
        }

        private ResultadoInstalacion486LP VolverAtras(bool habiaBase, string rutaRespaldo, string motivo)
        {
            IntentarEliminarBase();

            if (habiaBase && !string.IsNullOrEmpty(rutaRespaldo))
            {
                try
                {
                    InicializadorBD486LP.RestaurarBase(rutaRespaldo);
                    ResultadoInstalacion486LP r = Resultado(ERROR_BASE, "No se pudo crear la base nueva y se restauró la anterior. Motivo: " + motivo);
                    r.RutaRespaldo = rutaRespaldo;
                    return r;
                }
                catch (Exception exRestore)
                {
                    ResultadoInstalacion486LP r = Resultado(ERROR_BASE, "No se pudo crear la base nueva y tampoco restaurar la anterior. Tu copia está en: " + rutaRespaldo + ". Motivo: " + motivo + " | Restauración: " + MensajeSql(exRestore));
                    r.RutaRespaldo = rutaRespaldo;
                    return r;
                }
            }

            return Resultado(ERROR_BASE, motivo);
        }

        // Intenta guardar el respaldo en la carpeta pedida y si el servicio de SQL Server no puede escribir ahi, en la carpeta de respaldos del servidor
        private bool HacerRespaldo(string carpetaPreferida, string prefijo, out string rutaRespaldo, out string error)
        {
            rutaRespaldo = string.Empty;
            error = string.Empty;

            string nombre = "SneakRushDB_" + prefijo + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".bak";
            List<string> carpetas = new List<string>();

            if (!string.IsNullOrWhiteSpace(carpetaPreferida)) carpetas.Add(carpetaPreferida);
            try
            {
                string delServidor = InicializadorBD486LP.CarpetaRespaldoDelServidor();
                if (!string.IsNullOrWhiteSpace(delServidor)) carpetas.Add(delServidor);
            }
            catch { }

            foreach (string carpeta in carpetas)
            {
                try
                {
                    try { Directory.CreateDirectory(carpeta); } catch { }

                    string ruta = Path.Combine(carpeta, nombre);
                    InicializadorBD486LP.RespaldarBase(ruta);
                    rutaRespaldo = ruta;
                    return true;
                }
                catch (Exception ex)
                {
                    error = MensajeSql(ex);
                }
            }
            return false;
        }

        private ResultadoInstalacion486LP ValidarDatos(DatosAdminInstalacion486LP datos)
        {
            if (datos == null)
                return Resultado(DATOS_INVALIDOS, "Faltan los datos del administrador.");

            string mensaje;
            if (!_bllUsuarios.ValidarAdministradorInicial(datos.DNI, datos.Nombre, datos.Apellido, datos.Correo, out mensaje))
                return Resultado(DATOS_INVALIDOS, mensaje);

            return null;
        }

        private void IntentarEliminarBase()
        {
            try { InicializadorBD486LP.EliminarBase(); } catch { }
        }

        private static string MensajeSql(Exception ex)
        {
            return ex.Message.Replace("\r", " ").Replace("\n", " ");
        }

        private static ResultadoInstalacion486LP Resultado(int codigo, string mensaje)
        {
            return new ResultadoInstalacion486LP { Codigo = codigo, Mensaje = mensaje };
        }
    }
}

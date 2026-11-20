using DAL;
using Mappers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Services.DV486LP;

namespace Services
{
    // Dígitos verificadores como los de AeroManager:
    //   DVH = columna DVH de cada fila: la suma de los códigos de todos los caracteres de los valores de la fila (sin contar el propio DVH)
    //   DVV = fila de la tabla DV: la suma de los DVH de todas las filas de la tabla
    // La lista de tablas protegidas y su clave salen de la tabla DV.
    public class BLL_DV486LP
    {
        private Mapper_DV486LP _mapper = new Mapper_DV486LP();
        private BLL_Bitacora486LP _bllBitacora = new BLL_Bitacora486LP();

        private const string COLUMNA_DVH = "DVH";
        private const string COLUMNA_VIGENTE = "Vigente";
        private const string TABLA_PRODUCTO = "Producto";
        private const string TABLA_HISTORIAL_PRODUCTO = "ProductoCambio";
        private const string TABLA_BITACORA = "BitacoraEvento";

        // ------------------------------------------------------------------ cálculo

        // Suma de los códigos de los caracteres del texto
        private static long SumarCaracteres(string texto)
        {
            long suma = 0;
            foreach (char c in texto)
            {
                suma += c;
            }
            return suma;
        }

        // Pasa un valor de la base a texto con reglas fijas (iguales en cualquier PC), para que al escribir y al leer dé siempre lo mismo
        private static string ValorATexto(object valor)
        {
            if (valor == null || valor == DBNull.Value) return "";
            if (valor is DateTime) return ((DateTime)valor).ToString("yyyy-MM-dd'T'HH:mm:ss.fff", CultureInfo.InvariantCulture);
            if (valor is bool) return ((bool)valor) ? "True" : "False";
            if (valor is byte[]) return Convert.ToBase64String((byte[])valor);
            return Convert.ToString(valor, CultureInfo.InvariantCulture);
        }

        // DVH de una fila: suma de los caracteres de todas sus columnas menos la columna DVH.
        // invertirVigente sirve solo para el historial de Producto (ver SincronizarProductoCambio)
        private static long CalcularDVHDeFila(DataRow fila, DataColumnCollection columnas, bool invertirVigente)
        {
            long suma = 0;
            foreach (DataColumn col in columnas)
            {
                if (col.ColumnName == COLUMNA_DVH) continue;

                object valor = fila[col];
                string texto;
                if (invertirVigente && col.ColumnName == COLUMNA_VIGENTE && valor != DBNull.Value)
                    texto = ValorATexto(!Convert.ToBoolean(valor));
                else
                    texto = ValorATexto(valor);

                suma += SumarCaracteres(texto);
            }
            return suma;
        }

        // DVH que tiene guardado la fila (0 si todavía no se calculó)
        private static long LeerDVH(DataRow fila)
        {
            object valor = fila[COLUMNA_DVH];
            return valor == DBNull.Value ? 0 : Convert.ToInt64(valor, CultureInfo.InvariantCulture);
        }

        // ------------------------------------------------------------------ apoyo

        private DV486LP ObtenerDV(string tabla)
        {
            DV486LP dv = _mapper.ObtenerDV(tabla);
            if (dv == null)
            {
                throw new Exception($"La tabla '{tabla}' no está protegida por dígitos verificadores.");
            }
            return dv;
        }

        // Identificador de la fila para mostrarlo: los valores de su clave separados por guion (ej: "3-12" en una tabla de relación)
        private static string IdDeFila(DV486LP dv, DataRow fila)
        {
            return string.Join("-", dv.ColumnasClave().Select(c => ValorATexto(fila[c])));
        }

        private static DataRow BuscarFila(DataTable dt, DV486LP dv, object id)
        {
            string buscado = ValorATexto(id);
            foreach (DataRow fila in dt.Rows)
            {
                if (IdDeFila(dv, fila) == buscado) return fila;
            }
            return null;
        }

        // Guarda el DVH de la fila ubicándola por su clave (una o dos columnas)
        private void GuardarDVHDeFila(DV486LP dv, DataRow fila, long dvh)
        {
            string[] clave = dv.ColumnasClave();

            if (clave.Length == 1)
            {
                _mapper.GuardarDVH(dv.Tabla, dvh, clave[0], ValorATexto(fila[clave[0]]));
            }
            else if (clave.Length == 2)
            {
                _mapper.GuardarDVH(dv.Tabla, dvh, clave[0], ValorATexto(fila[clave[0]]), clave[1], ValorATexto(fila[clave[1]]));
            }
            else
            {
                throw new Exception($"La clave de la tabla '{dv.Tabla}' tiene {clave.Length} columnas y solo se admiten una o dos.");
            }
        }

        // ------------------------------------------------------------------ historial de Producto

        // El trigger de Producto deja las versiones nuevas de ProductoCambio con DVH = 0 y marca como no vigente la anterior.
        // Acá se completa el DVH de las versiones nuevas y de la que solo cambió el Vigente. Cualquier otra diferencia se deja como está para que la verificación la marque.
        private void SincronizarProductoCambio()
        {
            try
            {
                DV486LP dv = ObtenerDV(TABLA_HISTORIAL_PRODUCTO);
                DataTable dt = _mapper.LeerTabla(TABLA_HISTORIAL_PRODUCTO);
                long diferencia = 0;

                foreach (DataRow fila in dt.Rows)
                {
                    long guardado = LeerDVH(fila);
                    long actual = CalcularDVHDeFila(fila, dt.Columns, false);

                    if (guardado == actual) continue;

                    bool sinDVH = guardado == 0;
                    bool soloCambioVigente = !sinDVH && guardado == CalcularDVHDeFila(fila, dt.Columns, true);

                    if (sinDVH || soloCambioVigente)
                    {
                        GuardarDVHDeFila(dv, fila, actual);
                        diferencia += actual - guardado;
                    }
                }

                if (diferencia != 0)
                {
                    _mapper.GuardarDVV(TABLA_HISTORIAL_PRODUCTO, dv.DVV + diferencia);
                }
            }
            catch
            {
                // Si falla, la inconsistencia aparece en la verificación al iniciar sesión.
            }
        }

        // ------------------------------------------------------------------ escritura

        // Calcula y guarda el DVH de una fila que se acaba de insertar o modificar y ajusta el DVV de la tabla con la diferencia.
        // No toca las demás filas: si alguien alteró otra fila por fuera del sistema, sigue apareciendo en la verificación.
        public bool RegistrarDVDeFilaNueva(string tabla, object id, out string mensaje)
        {
            mensaje = "";
            try
            {
                DV486LP dv = ObtenerDV(tabla);
                DataTable dt = _mapper.LeerTabla(tabla);

                DataRow fila = BuscarFila(dt, dv, id);
                if (fila == null)
                {
                    mensaje = $"No se encontró la fila con Id '{id}' en la tabla '{tabla}'.";
                    return false;
                }

                long dvhGuardado = LeerDVH(fila);
                long dvhNuevo = CalcularDVHDeFila(fila, dt.Columns, false);

                GuardarDVHDeFila(dv, fila, dvhNuevo);
                _mapper.GuardarDVV(tabla, dv.DVV - dvhGuardado + dvhNuevo);

                if (tabla == TABLA_PRODUCTO) SincronizarProductoCambio();

                return true;
            }
            catch (Exception ex)
            {
                mensaje = ex.Message;
                return false;
            }
        }

        public bool RecalcularDV(string tabla, out string mensaje)
        {
            return RecalcularDV(tabla, true, out mensaje);
        }

        // Recalcula el DVH de todas las filas de la tabla (guarda solo los que cambiaron) y deja el DVV como la suma de todos
        public bool RecalcularDV(string tabla, bool registrarBitacora, out string mensaje)
        {
            mensaje = "";
            try
            {
                DV486LP dv = ObtenerDV(tabla);
                DataTable dt = _mapper.LeerTabla(tabla);
                long dvv = 0;

                foreach (DataRow fila in dt.Rows)
                {
                    long calculado = CalcularDVHDeFila(fila, dt.Columns, false);
                    if (calculado != LeerDVH(fila))
                    {
                        GuardarDVHDeFila(dv, fila, calculado);
                    }
                    dvv += calculado;
                }

                _mapper.GuardarDVV(tabla, dvv);

                if (tabla == TABLA_PRODUCTO) SincronizarProductoCambio();

                if (registrarBitacora)
                {
                    string dni = SessionManager486LP.ObtenerInstancia().UsuarioActual()?.DNI ?? "Sistema";
                    string nombreUsuario = SessionManager486LP.ObtenerInstancia().UsuarioActual()?.NombreUsuario ?? "Sistema";

                    _bllBitacora.Registrar(new BitacoraEvento486LP("Dígito Verificador", $"Se recalcularon los dígitos verificadores de la tabla '{tabla}'.", Criticidad486LP.MuyAlta, dni, nombreUsuario));
                }

                return true;
            }
            catch (Exception ex)
            {
                mensaje = ex.Message;
                return false;
            }
        }

        // Recalcula los DV de todas las tablas protegidas sin escribir en la bitácora.
        // Es el único paso que usa el instalador para los dígitos verificadores.
        public bool RecalcularTodas(out string mensaje)
        {
            mensaje = "";
            try
            {
                foreach (DV486LP dv in _mapper.ObtenerTablas())
                {
                    if (!RecalcularDV(dv.Tabla, false, out mensaje))
                    {
                        return false;
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                mensaje = ex.Message;
                return false;
            }
        }

        // ------------------------------------------------------------------ verificación

        // Revisa la tabla fila por fila y después compara la suma de los DVH guardados con el DVV:
        //   DVH en 0 -> la fila no pasó por el sistema (Inc.Insertado)
        //   DVH guardado distinto del calculado -> la fila se cambió por fuera (Inc.Modificado)
        //   suma de los DVH guardados distinta del DVV y ninguna otra causa -> falta una fila (Inc.Eliminado)
        private List<InconsistenciaDV486LP> AnalizarTabla(string tabla)
        {
            List<InconsistenciaDV486LP> lista = new List<InconsistenciaDV486LP>();

            DV486LP dv = ObtenerDV(tabla);
            DataTable dt = _mapper.LeerTabla(tabla);
            long sumaGuardada = 0;

            foreach (DataRow fila in dt.Rows)
            {
                long guardado = LeerDVH(fila);
                sumaGuardada += guardado;

                if (guardado == 0)
                {
                    lista.Add(new InconsistenciaDV486LP
                    {
                        ID = IdDeFila(dv, fila),
                        Tabla = tabla,
                        Inconsistencia = "Inc.Insertado" // Registro insertado directamente en la BD
                    });
                }
                else if (CalcularDVHDeFila(fila, dt.Columns, false) != guardado)
                {
                    lista.Add(new InconsistenciaDV486LP
                    {
                        ID = IdDeFila(dv, fila),
                        Tabla = tabla,
                        Inconsistencia = "Inc.Modificado" // Registro modificado directamente en la BD
                    });
                }
            }

            if (lista.Count == 0 && sumaGuardada != dv.DVV)
            {
                lista.Add(new InconsistenciaDV486LP
                {
                    ID = "-",
                    Tabla = tabla,
                    Inconsistencia = "Inc.Eliminado" // Registro eliminado directamente en la BD
                });
            }

            return lista;
        }

        public bool VerificarIntegridad(string tabla, out string tablaAfectada, out string mensaje)
        {
            tablaAfectada = "";
            mensaje = "";

            try
            {
                if (AnalizarTabla(tabla).Count == 0)
                {
                    return true;
                }

                tablaAfectada = tabla;

                string dni = SessionManager486LP.ObtenerInstancia().UsuarioActual()?.DNI ?? "Sistema";
                string nombreUsuario = SessionManager486LP.ObtenerInstancia().UsuarioActual()?.NombreUsuario ?? "Sistema";

                _bllBitacora.Registrar(new BitacoraEvento486LP("Dígito Verificador", $"Inconsistencia detectada en la tabla '{tabla}'.", Criticidad486LP.MuyAlta, dni, nombreUsuario));

                return false;
            }
            catch (Exception ex)
            {
                mensaje = ex.Message;
                return false;
            }
        }

        public List<InconsistenciaDV486LP> ObtenerInconsistencias(string tabla)
        {
            try
            {
                return AnalizarTabla(tabla);
            }
            catch (Exception ex)
            {
                return new List<InconsistenciaDV486LP>
                {
                    new InconsistenciaDV486LP
                    {
                        ID = "-",
                        Tabla = tabla,
                        Inconsistencia = $"Error al analizar inconsistencias: {ex.Message}"
                    }
                };
            }
        }

        // Verifica todas las tablas que figuran en DV. Devuelve las que tienen problemas; si hay un error técnico lo informa en errorTecnico
        public List<string> VerificarTodas(out string errorTecnico)
        {
            errorTecnico = "";
            List<string> conProblemas = new List<string>();

            List<DV486LP> tablas;
            try
            {
                tablas = _mapper.ObtenerTablas();
            }
            catch (Exception ex)
            {
                errorTecnico = ex.Message;
                return conProblemas;
            }

            if (tablas.Count == 0)
            {
                errorTecnico = "La tabla DV no tiene ninguna tabla registrada, no se puede verificar la integridad.";
                return conProblemas;
            }

            foreach (DV486LP dv in tablas)
            {
                string tablaAfectada;
                string mensaje;
                bool ok = VerificarIntegridad(dv.Tabla, out tablaAfectada, out mensaje);

                if (!ok)
                {
                    if (!string.IsNullOrEmpty(mensaje))
                    {
                        errorTecnico = mensaje;
                        return conProblemas;
                    }
                    conProblemas.Add(dv.Tabla);
                }
            }

            return conProblemas;
        }

        // Inconsistencias de varias tablas juntas (para mostrar todas en la grilla de reparación).
        public List<InconsistenciaDV486LP> ObtenerInconsistenciasDeTablas(List<string> tablas)
        {
            List<InconsistenciaDV486LP> todas = new List<InconsistenciaDV486LP>();
            foreach (string tabla in tablas)
                todas.AddRange(ObtenerInconsistencias(tabla));
            return todas;
        }

        public bool RecalcularTablas(List<string> tablas, out string mensaje)
        {
            mensaje = "";
            foreach (string tabla in tablas)
            {
                bool registrar = tabla != TABLA_BITACORA;
                if (!RecalcularDV(tabla, registrar, out mensaje))
                {
                    return false;
                }
            }
            return true;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BLL
{
    // Patron Strategy para el pago simulado (recomendado por el profesor, junto con el uso de
    // Hilos). Cada medio de pago decide COMO procesa el cobro: Efectivo es instantaneo (no hay
    // conexion con ningun banco), mientras que Tarjeta/Transferencia simulan ~3 segundos de
    // conexion con la Entidad Bancaria usando un Hilo real, para no bloquear la UI mientras tanto.
    // El pago SIEMPRE se aprueba en este proyecto - los datos de tarjeta ya se validaron antes,
    // en el propio form (flujo alternativo 8.1), asi que no hace falta simular un rechazo del banco.
    //
    // IMPORTANTE: esta clase vive en BLL, que no debe depender de la UI (System.Windows.Forms) -
    // por eso, para volver al hilo de UI despues del Hilo de simulacion, se usa
    // SynchronizationContext (parte del framework base) en vez de recibir un Form/Control como
    // parametro.

    public interface IEstrategiaPago486LP
    {
        // callback se invoca cuando termina el procesamiento, con el resultado (true = aprobado).
        // Se invoca siempre en el mismo hilo desde el que se llamo a ProcesarPago (el de UI en la
        // practica), para que quien lo use pueda tocar controles sin problema.
        void ProcesarPago(decimal monto, Action<bool> callback);
    }

    // Efectivo: se cobra en el momento, no hay Entidad Bancaria de por medio.
    public class EstrategiaPagoEfectivo486LP : IEstrategiaPago486LP
    {
        public void ProcesarPago(decimal monto, Action<bool> callback)
        {
            callback(true);
        }
    }

    // Base comun para los medios de pago que si necesitan "conectarse" con el banco
    // (simulado con un Hilo de ~3 segundos). Tarjeta y Transferencia heredan de aca.
    public abstract class EstrategiaPagoConBancoBase486LP : IEstrategiaPago486LP
    {
        public void ProcesarPago(decimal monto, Action<bool> callback)
        {
            // Se captura el contexto de sincronizacion del hilo actual (el de UI) ANTES de lanzar
            // el hilo secundario - hay que leerlo aca, porque dentro del hilo secundario ya no es
            // el mismo contexto.
            SynchronizationContext contextoLlamador = SynchronizationContext.Current;

            Thread hilo = new Thread(() =>
            {
                Thread.Sleep(3000); // simula la conexion y confirmacion con la Entidad Bancaria

                bool aprobado = true; // siempre aprueba, por decision del proyecto

                if (contextoLlamador != null)
                {
                    // Vuelve al hilo original (de UI) para que el callback pueda tocar controles.
                    contextoLlamador.Post(_ => callback(aprobado), null);
                }
                else
                {
                    // Sin contexto de sincronizacion (ej. una prueba de consola) - se llama directo.
                    callback(aprobado);
                }
            });
            hilo.IsBackground = true;
            hilo.Start();
        }
    }

    public class EstrategiaPagoTarjeta486LP : EstrategiaPagoConBancoBase486LP { }

    public class EstrategiaPagoTransferencia486LP : EstrategiaPagoConBancoBase486LP { }

    // Elige la estrategia correcta segun el medio de pago seleccionado en el form.
    public static class FabricaEstrategiaPago486LP
    {
        public static IEstrategiaPago486LP Crear(string medioPago)
        {
            switch (medioPago)
            {
                case "Tarjeta":
                    return new EstrategiaPagoTarjeta486LP();
                case "Transferencia":
                    return new EstrategiaPagoTransferencia486LP();
                default:
                    return new EstrategiaPagoEfectivo486LP();
            }
        }
    }
}

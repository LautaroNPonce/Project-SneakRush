using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BLL
{
    public interface IEstrategiaPago486LP
    {
        // Primero invoco siempre en el mismo hilo desde el que se llamo a ProcesarPago (el de UI en la practica), para que quien lo use pueda tocar controles sin problema.
        void ProcesarPago(decimal monto, Action<bool> callback);
    }

    public class EstrategiaPagoEfectivo486LP : IEstrategiaPago486LP
    {
        public void ProcesarPago(decimal monto, Action<bool> callback)
        {
            callback(true);
        }
    }

    public abstract class EstrategiaPagoConBancoBase486LP : IEstrategiaPago486LP
    {
        public void ProcesarPago(decimal monto, Action<bool> callback)
        {
            SynchronizationContext contextoLlamador = SynchronizationContext.Current;

            Thread hilo = new Thread(() =>
            {
                Thread.Sleep(3000); // simula la conexion y confirmacion con la Entidad Bancaria

                bool aprobado = true;

                if (contextoLlamador != null)
                {
                    contextoLlamador.Post(_ => callback(aprobado), null);
                }
                else
                {
                    callback(aprobado);
                }
            });
            hilo.IsBackground = true;
            hilo.Start();
        }
    }

    public class EstrategiaPagoTarjeta486LP : EstrategiaPagoConBancoBase486LP { }

    public class EstrategiaPagoTransferencia486LP : EstrategiaPagoConBancoBase486LP { }

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

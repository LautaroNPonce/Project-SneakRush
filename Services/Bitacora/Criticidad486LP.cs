using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class Criticidad486LP
    {
        public const int MuyAlta = 1;
        public const int Alta = 2;
        public const int Media = 3;
        public const int Baja = 4;
        public const int MuyBaja = 5;

        public static string ATexto(int valor)
        {
            switch (valor)
            {
                case 1: return "Muy Alta";
                case 2: return "Alta";
                case 3: return "Media";
                case 4: return "Baja";
                case 5: return "Muy Baja";
                default: return "Desconocida";
            }
        }
    }
}

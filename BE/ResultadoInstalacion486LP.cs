using System;

namespace BE
{
    // Resultado de una operacion de instalacion que se le informa al asistente
    public class ResultadoInstalacion486LP
    {
        public int Codigo { get; set; }
        public string Mensaje { get; set; } = string.Empty;
        public string Usuario { get; set; } = string.Empty;
        public string ContraseñaInicial { get; set; } = string.Empty;
        public string RutaRespaldo { get; set; } = string.Empty;
    }
}

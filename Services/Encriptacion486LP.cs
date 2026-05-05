using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class Encriptacion486LP
    {
        // Clave AES (32 chars = 256 bits)
        private static readonly string ClaveAES = "SneakRush2026LP_ClaveSegura_AES256";
        private static readonly string VectorIV = "SneakRushIV_2026"; // 16 chars

        //  SHA-256 — para contraseñas (sin vuelta)
        public static string GenerarHash(string texto)
        {
            if (string.IsNullOrEmpty(texto))
                return texto;

            StringBuilder br = new StringBuilder();

            using (SHA256 hash = SHA256.Create())
            {
                Encoding encoding = Encoding.UTF8;
                byte[] result = hash.ComputeHash(encoding.GetBytes(texto));
                foreach (byte b in result)
                    br.Append(b.ToString("X2"));   // Hex MAYÚSCULA
            }
            return br.ToString();   // siempre 64 chars
        }


        //  AES-256 — lo uso para el correo de los clientes (con vuelta)
        public static string EncriptarAES(string textoPlano)
        {
            if (string.IsNullOrEmpty(textoPlano))
                return textoPlano;

            using (Aes aes = Aes.Create())
            {
                // PadRight + Substring = garantiza el largo exacto sin romperse
                aes.Key = Encoding.UTF8.GetBytes(ClaveAES.PadRight(32).Substring(0, 32));
                aes.IV = Encoding.UTF8.GetBytes(VectorIV.PadRight(16).Substring(0, 16));

                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
                    using (StreamWriter sw = new StreamWriter(cs))
                    {
                        sw.Write(textoPlano);
                    }
                    return Convert.ToBase64String(ms.ToArray());
                }
            }
        }

        // Descifra un texto cifrado en Base64 con AES-256.
        public static string DesencriptarAES(string textoCifrado)
        {
            if (string.IsNullOrEmpty(textoCifrado))
                return textoCifrado;

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(ClaveAES.PadRight(32).Substring(0, 32));
                aes.IV = Encoding.UTF8.GetBytes(VectorIV.PadRight(16).Substring(0, 16));

                using (MemoryStream ms = new MemoryStream(Convert.FromBase64String(textoCifrado)))
                using (CryptoStream cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Read))
                using (StreamReader sr = new StreamReader(cs))
                {
                    return sr.ReadToEnd();
                }
            }
        }
        //  Validación de Base64
        public static bool EsBase64(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto))
                return false;

            texto = texto.Trim();

            if (texto.Length % 4 != 0)
                return false;

            foreach (char c in texto)
            {
                if (!char.IsLetterOrDigit(c) && c != '+' && c != '/' && c != '=')
                    return false;
            }

            return true;
        }
    }
}

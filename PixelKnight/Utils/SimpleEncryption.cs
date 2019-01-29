using System;
using System.Security.Cryptography;
using System.Text;

namespace PixelKnight.Utils
{
    public static class SimpleEncryption
    {
        private static string key = ":{j%6j?E:t#}G10mM%9hp5S=%}2,Y26C";
        private static RijndaelManaged provider;

        private static void SetupProvider()
        {
            provider = new RijndaelManaged();
            provider.Key = Encoding.ASCII.GetBytes(key);
            provider.Mode = CipherMode.ECB;
        }

        public static string EncryptString(string sourceString)
        {
            if (provider == null)
                SetupProvider();
            ICryptoTransform encryptor = provider.CreateEncryptor();
            byte[] bytes = Encoding.UTF8.GetBytes(sourceString);
            return Convert.ToBase64String(encryptor.TransformFinalBlock(bytes, 0, bytes.Length));
        }

        public static string DecryptString(string sourceString)
        {
            if (provider == null)
                SetupProvider();
            ICryptoTransform decryptor = provider.CreateDecryptor();
            byte[] inputBuffer = Convert.FromBase64String(sourceString);
            return Encoding.UTF8.GetString(decryptor.TransformFinalBlock(inputBuffer, 0, inputBuffer.Length));
        }

        public static string EncryptFloat(float value)
        {
            return EncryptString(Convert.ToBase64String(BitConverter.GetBytes(value)));
        }

        public static string EncryptInt(int value)
        {
            return EncryptString(Convert.ToBase64String(BitConverter.GetBytes(value)));
        }

        public static float DecryptFloat(string sourceString)
        {
            return BitConverter.ToSingle(Convert.FromBase64String(DecryptString(sourceString)), 0);
        }

        public static int DecryptInt(string sourceString)
        {
            return BitConverter.ToInt32(Convert.FromBase64String(DecryptString(sourceString)), 0);
        }
    }
}

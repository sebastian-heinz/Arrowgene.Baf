using System;
using System.IO;
using System.Security.Cryptography;

namespace Arrowgene.Baf.Server.Common
{
    /**
     * https://tools.ietf.org/html/rfc2898#section-6.1.1
     */
    public static class BafPbeWithMd5AndDes
    {
        public static byte[] DeriveKey(byte[] buffer, uint iterationCount = 16)
        {
            if (buffer.Length != 16)
            {
                return null;
            }

            MD5 md5 = MD5.Create();
            for (int i = 0; i < iterationCount; i++)
            {
                buffer = md5.ComputeHash(buffer);
            }

            return buffer;
        }

        public static byte[] DeriveKey(byte[] password, byte[] salt, uint iterationCount = 16)
        {
            if (password.Length != 8)
            {
                return null;
            }

            if (salt.Length != 8)
            {
                return null;
            }

            byte[] buffer = new byte[16];
            Buffer.BlockCopy(password, 0, buffer, 0, 8);
            Buffer.BlockCopy(salt, 0, buffer, 8, 8);
            return DeriveKey(buffer, iterationCount);
        }

        public static byte[] Decrypt(byte[] input, byte[] key, byte[] iv)
        {
            DESCryptoServiceProvider cProv = new DESCryptoServiceProvider();
            cProv.Padding = PaddingMode.None;
            cProv.Mode = CipherMode.CBC;
            ICryptoTransform transform = cProv.CreateDecryptor(key, iv);
            MemoryStream inStream = new MemoryStream();
            CryptoStream cStream = new CryptoStream(inStream, transform, CryptoStreamMode.Write);
            cStream.Write(input, 0, input.Length);
            cStream.FlushFinalBlock();
            return inStream.ToArray();
        }

        public static byte[] Encrypt(byte[] input, byte[] key, byte[] iv)
        {
            DESCryptoServiceProvider cProv = new DESCryptoServiceProvider();
            cProv.Padding = PaddingMode.None;
            cProv.Mode = CipherMode.CBC;
            ICryptoTransform transform = cProv.CreateEncryptor(key, iv);
            MemoryStream inStream = new MemoryStream();
            CryptoStream cStream = new CryptoStream(inStream, transform, CryptoStreamMode.Write);
            cStream.Write(input, 0, input.Length);
            cStream.FlushFinalBlock();
            return inStream.ToArray();
        }
    }
}
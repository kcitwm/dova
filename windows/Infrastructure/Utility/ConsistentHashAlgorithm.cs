namespace Dova.Utility
{
    using System;
    using System.Security.Cryptography;
    using System.Text;

    internal class ConsistentHashAlgorithm
    {
        public static byte[] ComputeMD5(string k)
        {
            MD5 md = new MD5CryptoServiceProvider();
            byte[] buffer = md.ComputeHash(Encoding.UTF8.GetBytes(k));
            md.Clear();
            return buffer;
        }

        public static long Hash(byte[] digest, int nTime)
        {
            long num = ((((digest[3 + (nTime * 4)] & 0xff) << 0x18) | ((digest[2 + (nTime * 4)] & 0xff) << 0x10)) | ((digest[1 + (nTime * 4)] & 0xff) << 8)) | (digest[nTime * 4] & 0xffL);
            return (num & ((long) 0xffffffffL));
        }
    }
}


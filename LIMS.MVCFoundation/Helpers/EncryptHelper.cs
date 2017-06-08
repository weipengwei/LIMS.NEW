using System;
using System.Text;
using System.IO;
using System.Security.Cryptography;
using System.Web.Security;

namespace LIMS.MVCFoundation.Helpers
{
    public static class EncryptHelper
    {
        private static readonly string CIV = "kXwL7X2+fgM=";
        private static readonly string CKEY = "FwGQWRRgKCI=";

        /// <summary>
        /// 加密字符串
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string EncryptString(string value)
        {
            if (String.IsNullOrEmpty(value))
                return string.Empty;

            ICryptoTransform ct;
            MemoryStream ms;
            CryptoStream cs;
            byte[] byt;

            SymmetricAlgorithm algProvider = new DESCryptoServiceProvider();
            ct = algProvider.CreateEncryptor(Convert.FromBase64String(CKEY), Convert.FromBase64String(CIV));

            byt = Encoding.UTF8.GetBytes(value);

            ms = new MemoryStream();
            cs = new CryptoStream(ms, ct, CryptoStreamMode.Write);
            cs.Write(byt, 0, byt.Length);
            cs.FlushFinalBlock();

            cs.Close();

            return Convert.ToBase64String(ms.ToArray());
        }

        /// <summary>
        /// 解密字符串
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string DecryptString(string value)
        {
            if (String.IsNullOrEmpty(value))
                return string.Empty;

            ICryptoTransform ct;
            MemoryStream ms;
            CryptoStream cs;
            byte[] byt;

            SymmetricAlgorithm algProvider = new DESCryptoServiceProvider();
            ct = algProvider.CreateDecryptor(Convert.FromBase64String(CKEY), Convert.FromBase64String(CIV));

            byt = Convert.FromBase64String(value);

            ms = new MemoryStream();
            cs = new CryptoStream(ms, ct, CryptoStreamMode.Write);
            cs.Write(byt, 0, byt.Length);
            cs.FlushFinalBlock();

            cs.Close();
            return Encoding.UTF8.GetString(ms.ToArray());
        }

    }
}

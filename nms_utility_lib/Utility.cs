using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace nms_utility_lib
{
    public class Utility
    {
        public static string Encrypt(string text)
        {
            MD5 md5 = new MD5CryptoServiceProvider();

            byte[] data = Encoding.ASCII.GetBytes(text);
            if (null == data)
            {
                return null;
            }

            byte[] md5data = md5.ComputeHash(data);
            md5.Clear();
            if (null == md5data)
            {
                return null;
            }

            string Response = string.Empty;
            for (int i = 0; i < md5data.Length; i++)
            {
                Response += md5data[i].ToString("X").PadLeft(2, '0');
            }

            return Response;
        }
    }
}

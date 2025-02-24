using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer.Utilities
{
    public class IpToByteConverter
    {
        public byte[] ConvertIpV4ToByteArray(string ipAddress)
        {
            byte[] result = [4];
            string[] ipSplit = ipAddress.Split(".");
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = byte.Parse(ipSplit[i]);
            }
            return result;
        }
    }
}

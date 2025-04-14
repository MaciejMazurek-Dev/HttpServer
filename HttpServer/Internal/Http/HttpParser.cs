using HttpServer.Models;
using System.Text;

namespace HttpServer.Internal.Http
{
    class HttpParser
    {
        private const byte ByteSpace = (byte)' ';
        private const byte ByteCR = (byte)'\r';
        private const byte ByteLF = (byte)'\n';
        private const byte ByteColon = (byte)':';
        private const byte ByteTab = (byte)'\t';

        private const byte ByteG = (byte)'G';
        private const byte ByteP = (byte)'P';
        private const byte ByteO = (byte)'O';
        private const byte ByteD = (byte)'D';
        private const byte ByteH = (byte)'H';

        public HttpRequest ParseRequest(Span<byte> byteData)
        {
            HttpRequest httpRequest = new();

            httpRequest.Method = GetMethod(byteData);
            if(httpRequest.Method == HttpMethodEnum.UNKNOWN)
            {
               httpRequest.isValid = false;
                return httpRequest;
            }

            httpRequest.Uri = GetUri(byteData);
            if(httpRequest.Uri == null)
            {
                httpRequest.isValid = false;
                return httpRequest;
            }

            httpRequest.Version = GetVersion(byteData);

            httpRequest.Headers = GetHeaders(byteData);

            return httpRequest;
        }

        private Dictionary<string, string> GetHeaders(Span<byte> byteData)
        {
            Dictionary<string, string> result = new();
            int startIndex = byteData.IndexOf(ByteLF) + 1;

            for (int i = startIndex; i < byteData.Length; i++)
            {
                if (byteData[i] == ByteCR && byteData[++i] == ByteLF)
                {
                    int lineLength = (i - startIndex) - 1;
                    Span<byte> line = byteData.Slice(startIndex, lineLength);
                    int colonIndex = line.IndexOf(ByteColon);
                    string key = Encoding.UTF8.GetString(line.Slice(0, colonIndex));
                    string value = Encoding.UTF8.GetString(line.Slice(colonIndex + 2));
                    result.Add(key, value);
                    startIndex = ++i;
                }
            }
            return result;
        }
        

        private string GetVersion(Span<byte> byteData)
        {
            int endIndex = byteData.IndexOf(ByteCR);
            if ((byteData[++endIndex]) != ByteLF)
            {
                return string.Empty;
            }
            int secondSpace = 0;
            int startIndex = 0;
            for (int i = 0; secondSpace < 2; i++)
            {
                if (byteData[i] == ByteSpace)
                {
                    secondSpace++;
                    startIndex = i;
                }
            }
            string result = Encoding.UTF8.GetString(byteData[++startIndex..--endIndex]);
            return result;
        }

        private Uri? GetUri(Span<byte> byteData)
        {
            int startIndex = byteData.IndexOf(ByteSpace) + 1;
            Span<byte> uriSpan = byteData.Slice(startIndex);
            int endIndex = uriSpan.IndexOf(ByteSpace);
            uriSpan = uriSpan.Slice(0, endIndex);
            Uri? result;
            Uri.TryCreate(Encoding.UTF8.GetString(uriSpan), UriKind.Relative, out result);
            return result;
        }

        private HttpMethodEnum GetMethod(Span<byte> byteData)
        {
            switch(byteData[0])
            {
                case ByteG:
                    return HttpMethodEnum.GET;

                case ByteP:
                    if (byteData[1] == ByteO)
                    {
                        return HttpMethodEnum.POST;
                    }
                    else 
                    {
                        return HttpMethodEnum.PUT;
                    }
                case ByteD:
                    return HttpMethodEnum.DELETE;
                case ByteH:
                    return HttpMethodEnum.HEAD;
                default:
                    return HttpMethodEnum.UNKNOWN;
            }
        }
    }
}

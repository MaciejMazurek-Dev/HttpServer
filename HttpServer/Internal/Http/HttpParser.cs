using HttpServer.Models;

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
            return httpRequest;
        }

        private HttpMethodEnum GetMethod(Span<byte> data)
        {
            switch(data[0])
            {
                case ByteG:
                    return HttpMethodEnum.GET;

                case ByteP:
                    if (data[1] == ByteO)
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

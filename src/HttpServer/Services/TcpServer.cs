using HttpServer.Utilities;
using System.Net;
using System.Net.Sockets;

namespace HttpServer.Services
{
    public class TcpServer
    {
        private readonly TcpListener _tcpListener;
        public TcpServer()
        {
            IPEndPoint ipEnDPoint = new(IPAddress.Loopback, 80);
            _tcpListener = new(ipEnDPoint);
        }
        public TcpServer(string ipAddress, int portNumber)
        {
            IpToByteConverter converter = new();
            byte[] ipAddressInBytes = converter.ConvertIpV4ToByteArray(ipAddress);
            IPAddress ip = new(ipAddressInBytes);
            IPEndPoint ipEndPoint = new(ip, portNumber);
            _tcpListener = new(ipEndPoint);
        }
    }
}

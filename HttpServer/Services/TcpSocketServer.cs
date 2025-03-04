using System.Net;
using System.Net.Sockets;
using System.Text;

namespace HttpServer.Services
{
    public class TcpSocketServer
    {
        private readonly Socket _socket = new(
                                        AddressFamily.InterNetwork,
                                        SocketType.Stream,
                                        ProtocolType.Tcp);
        private readonly IPEndPoint _serverIpEndPoint;
        private bool isRunning = false;
        public TcpSocketServer() : this(IPAddress.Loopback.ToString(), 80)
        {
        }
        public TcpSocketServer(string ipString, int port)
        {
            IPAddress ipAddress = IPAddress.Parse(ipString);
            _serverIpEndPoint = new(ipAddress, port);
            _socket.Bind(_serverIpEndPoint);
            _socket.Listen(10);
        }

        public void StartServer()
        {
            Socket connectionHandler = _socket.Accept();
            isRunning = true;
            Console.WriteLine("Server is running...");
            while (isRunning)
            {
                byte[] buffer = new byte[1024];
                int bytesCount = connectionHandler.Receive(buffer);
                if (bytesCount > 0)
                {
                    string received = Encoding.ASCII.GetString(buffer);
                    Console.WriteLine($"Bytes received: {bytesCount}");
                    Console.WriteLine($"Message: {received}");
                }
                string responseMessage = $"Message received - {bytesCount} bytes";
                byte[] response = Encoding.ASCII.GetBytes(responseMessage);
                connectionHandler.Send(response);
            }
        }

        public void StopServer()
        {
            isRunning = false;
            _socket.Shutdown(SocketShutdown.Both);
            Console.WriteLine("Server shutdown...");
        }
    }
}

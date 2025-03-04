using System.Net;
using System.Net.Sockets;
using System.Text;

namespace HttpClient.Services
{
    public class TcpSocketClient
    {
        private readonly Socket _socket;
        private readonly IPEndPoint _serverIpEndPoint;
        public TcpSocketClient(string serverIp, int port)
        {
            IPAddress ipAddress = IPAddress.Parse(serverIp);
            _serverIpEndPoint = new(ipAddress, port);
            _socket = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        public void StartClient()
        {
            _socket.Connect(_serverIpEndPoint);
            Console.WriteLine("Connection with server established.");
            while (true)
            {
                Console.WriteLine("Write a message:");
                string message = Console.ReadLine();
                byte[] messageBytes = Encoding.ASCII.GetBytes(message);
                int bytesSend = _socket.Send(messageBytes);

                Console.WriteLine($"Message send - {bytesSend} bytes.");
                ReceiveResponse();
            }
        }

        public void ReceiveResponse()
        {
            byte[] buffer = new byte[1024];
            _socket.ReceiveBufferSize = buffer.Length;
            int bytesReceived = _socket.Receive(buffer);
            if (bytesReceived > 0)
            {
                string received = Encoding.ASCII.GetString(buffer);
                Console.WriteLine($"Server response: {received}");
            }
        }
    }
}

using System.Net;
using System.Net.Sockets;
using System.Text;
using Microsoft.Extensions.Logging;

namespace HttpServer.Services
{
    public class TcpSocketServer
    {
        private readonly Socket _serverSocket = new(
                                        AddressFamily.InterNetwork,
                                        SocketType.Stream,
                                        ProtocolType.Tcp);
        private bool isRunning = false;
        private readonly ILogger _logger;

        public TcpSocketServer() : this(IPAddress.Loopback.ToString(), 5050)
        {
        }
        public TcpSocketServer(string ipString, int port)
        {
            IPAddress ipAddress = IPAddress.Parse(ipString);
            IPEndPoint serverIpEndPoint = new(ipAddress, port);
            _serverSocket.Bind(serverIpEndPoint);
            _serverSocket.Listen();

            using ILoggerFactory loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.SetMinimumLevel(LogLevel.Debug);
                builder.AddConsole();
            });
            _logger = loggerFactory.CreateLogger("TcpSocketServer");
        }


        public void StartServer()
        {
            _logger.LogInformation("Server state: UP");
            _logger.LogInformation($"Listening for connections on address: {_serverSocket.LocalEndPoint}");
            isRunning = true;

            while (isRunning)
            {
                Socket client = _serverSocket.Accept();
                _logger.LogInformation("Client connected: " + client.RemoteEndPoint);
                HandleClient(client);
            }
        }

        public void StopServer()
        {
            isRunning = false;
            _serverSocket.Shutdown(SocketShutdown.Both);
            _logger.LogInformation("Server state: DOWN");
        }

        public void HandleClient(Socket client)
        {
            byte[] buffer = new byte[1024];
            int bytesReceived = client.Receive(buffer);
            if (bytesReceived > 0)
            {
                string received = Encoding.UTF8.GetString(buffer);

                _logger.LogDebug($"Data received from: {client.RemoteEndPoint}\n{received}");
            }
        }
    }
}

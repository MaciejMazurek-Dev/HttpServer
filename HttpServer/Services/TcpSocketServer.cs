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
            _serverSocket.Listen(10);

            using ILoggerFactory loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.SetMinimumLevel(LogLevel.Debug);
                builder.AddConsole();
            });
            _logger = loggerFactory.CreateLogger("TcpSocketServer");
        }


        public void StartServer()
        {
            _logger.LogDebug("Logger initialized.");

            Socket connectionHandler = _serverSocket.Accept();
            isRunning = true;

            _logger.LogInformation("Server state: UP");
            _logger.LogInformation($"Listening for connections on address: {connectionHandler.LocalEndPoint}");
            while (isRunning)
            {
                byte[] buffer = new byte[1024];
                int bytesCount = connectionHandler.Receive(buffer);
                if (bytesCount > 0)
                {
                    string received = Encoding.ASCII.GetString(buffer);

                    _logger.LogDebug($"Data received from: {connectionHandler.RemoteEndPoint}\n{received}");
                }


                string responseMessage = $"Message received - {bytesCount} bytes";
                byte[] response = Encoding.ASCII.GetBytes(responseMessage);
                connectionHandler.Send(response);
            }
        }

        public void StopServer()
        {
            isRunning = false;
            _serverSocket.Shutdown(SocketShutdown.Both);
            Console.WriteLine("Server shutdown...");
        }
    }
}

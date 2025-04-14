using System.Net;
using System.Net.Sockets;
using System.Text;
using HttpServer.Internal.Http;
using HttpServer.Models;
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
        private readonly HttpParser _httpParser = new();

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
            List<byte> dataReceived = new();
            byte[] buffer = new byte[1024];
            int sizeOfDataReceived = client.Receive(buffer);
            if (sizeOfDataReceived > 0)
            {
                dataReceived.AddRange(buffer);
            }
            HttpRequest httpRequest = _httpParser.ParseRequest(dataReceived.ToArray<byte>());


            StringBuilder headers = new();
            foreach(KeyValuePair<string, string> header in httpRequest.Headers)
            {
                headers.AppendLine(header.Key + ": " + header.Value);
            }
            _logger.LogDebug("Request received" + Environment.NewLine +
                            "Method: " + httpRequest.Method + Environment.NewLine +
                            "Uri: " + httpRequest.Uri + Environment.NewLine +
                            "Http Version: " + httpRequest.Version + Environment.NewLine +
                            headers.ToString()
                            );
            client.Close();
        }
    }
}

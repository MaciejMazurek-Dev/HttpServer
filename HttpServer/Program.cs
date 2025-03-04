using HttpServer.Services;

namespace HttpServer
{
    public class Program
    {
        static void Main(string[] args)
        {
            TcpSocketServer server = new();

            server.StartServer();
        }
    }
}

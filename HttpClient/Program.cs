using HttpClient.Services;

namespace HttpClient
{
    internal class Program
    {
        static void Main(string[] args)
        {
            TcpSocketClient client = new("127.0.0.1", 80);

            client.StartClient();
        }
    }
}

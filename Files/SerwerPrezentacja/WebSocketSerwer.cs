using System.Net;
using System.Net.WebSockets;
using System.Text;
using Logika;

namespace SerwerPrezentacja
{
    public class WebSocketServer
    {
        private readonly HttpListener _listener = new();
        private readonly AbstractLogicAPI _logic;

        public WebSocketServer(AbstractLogicAPI logic)
        {
            _logic = logic;
        }

        public void Start(string url = "http://localhost:8080/")
        {
            _listener.Prefixes.Add(url);
            _listener.Start();
            Console.WriteLine($"Serwer WebSocket działa na {url}");

            WaitForConnection();
        }

        private async void WaitForConnection()
        {
            var context = await _listener.GetContextAsync();
            if (context.Request.IsWebSocketRequest)
            {
                Console.WriteLine("Otrzymano żądanie WebSocket");
            }
            else
            {
                Console.WriteLine("To nie jest żądanie WebSocket");
            }
        }
    }
}
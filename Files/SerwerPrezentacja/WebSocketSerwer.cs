using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using SerwerLogika;
using SerwerDane;

namespace SerwerPrezentacja
{
    public class WebSocketServer
    {
        private readonly HttpListener _listener = new();
        private readonly AbstractLogicAPI _logic;
        private readonly DiscountNotifier _discountNotifier;

        public WebSocketServer(AbstractLogicAPI logic)
        {
            _logic = logic;
            _discountNotifier = new DiscountNotifier();
            _listener = new HttpListener();
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
            while (true)
            {
                var context = await _listener.GetContextAsync();
                if (context.Request.IsWebSocketRequest)
                {
                    var webSocketContext = await context.AcceptWebSocketAsync(null);
                    _ = HandleClient(webSocketContext.WebSocket);
                }
            }
        }

        private async Task HandleClient(WebSocket webSocket)
        {
            var buffer = new byte[1024];
            try
            {
                var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                if (result.MessageType == WebSocketMessageType.Text)
                {
                    var message = Encoding.UTF8.GetString(buffer, 0, result.Count);

                    if (message == WebSocketCommands.Purchase)
                    {
                        Console.WriteLine("Otrzymano żądanie zakupu");

                        await webSocket.SendAsync(Encoding.UTF8.GetBytes("PURCHASE_SUCCESS"),
                            WebSocketMessageType.Text, true, CancellationToken.None);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Błąd: {ex.Message}");
            }
        }
    }
}
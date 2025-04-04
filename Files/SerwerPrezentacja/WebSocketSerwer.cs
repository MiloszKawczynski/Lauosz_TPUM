using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using Logika;
using SerwerDane;

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
                    var message = JsonSerializer.Deserialize<Dictionary<string, object>>(
                        Encoding.UTF8.GetString(buffer, 0, result.Count));

                    if (message?["command"]?.ToString() == WebSocketCommands.Purchase)
                    {
                        var request = JsonSerializer.Deserialize<PurchaseRequest>(message["data"].ToString());
                        var success = _logic.PurchasePlant(request.PlantId);

                        var response = new
                        {
                            command = WebSocketCommands.PurchaseResponse,
                            data = new { Success = success, PlantId = request.PlantId }
                        };

                        await webSocket.SendAsync(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(response)),
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
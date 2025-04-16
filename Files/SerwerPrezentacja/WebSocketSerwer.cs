using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

namespace SerwerPrezentacja
{
    public class WebSocketServer
    {
        private readonly HttpListener _listener = new();
        private readonly List<WebSocket> _connectedClients = new();
        private IRequestHandler _requestHandler;
        private readonly SemaphoreSlim _clientLock = new(1, 1);
        private Timer _plantBroadcastTimer;

        public void SetRequestHandler(IRequestHandler handler)
        {
            _requestHandler = handler;
        }

        public WebSocketServer()
        {
            _listener = new HttpListener();
        }

        public void Start(string url = "http://localhost:8080/")
        {
            _listener.Prefixes.Add(url);
            _listener.Start();
            Console.WriteLine($"Serwer WebSocket działa na {url}");

            StartPlantBroadcast();
            WaitForConnection();
        }

        private void StartPlantBroadcast()
        {
            _plantBroadcastTimer = new Timer(async _ =>
            {
                var plants = _requestHandler.GetAllPlantsForBroadcast();
                var json = JsonSerializer.Serialize(plants, new JsonSerializerOptions { WriteIndented = true });
                var buffer = Encoding.UTF8.GetBytes(json);

                foreach (var client in _connectedClients.Where(c => c.State == WebSocketState.Open))
                {
                    await client.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
                }
                Console.WriteLine("Wysłano listę roślin do klientów");
            },
            null,
            TimeSpan.FromSeconds(4),
            TimeSpan.FromMinutes(1));
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
            await _clientLock.WaitAsync();
            _connectedClients.Add(webSocket);
            _clientLock.Release();
            var buffer = new byte[1024];
            try
            {
                while (webSocket.State == WebSocketState.Open)
                {
                    var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                    if (result.MessageType == WebSocketMessageType.Text)
                    {
                        var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                        Console.WriteLine($"Otrzymano: {message}");

                        var response = await _requestHandler.HandleRequest(message, webSocket);
                        await webSocket.SendAsync(
                            Encoding.UTF8.GetBytes(response),
                            WebSocketMessageType.Text,
                            true,
                            CancellationToken.None);
                    }
                }
            }
            finally
            {
                await _clientLock.WaitAsync();
                _connectedClients.Remove(webSocket);
                _clientLock.Release();
                if (webSocket.State == WebSocketState.Open)
                {
                    await webSocket.CloseAsync(
                        WebSocketCloseStatus.NormalClosure,
                        "Zamykanie połączenia",
                        CancellationToken.None);
                }
            }
        }

        public async Task BroadcastDiscount(float discountValue)
        {
            var notification = new SharedModel.DiscountNotification { DiscountValue = discountValue };
            var json = JsonSerializer.Serialize(notification);
            var buffer = Encoding.UTF8.GetBytes(json);

            foreach (var client in _connectedClients.Where(c => c.State == WebSocketState.Open))
            {
                await client.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
            }
        }
    }
}
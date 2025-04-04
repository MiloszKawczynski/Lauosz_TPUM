﻿using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using SerwerDane;

namespace SerwerPrezentacja
{
    public class WebSocketServer
    {
        private readonly HttpListener _listener = new();
        private readonly List<WebSocket> _connectedClients = new();

        public WebSocketServer()
        {
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
            _connectedClients.Add(webSocket);
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

                        if (message.StartsWith("PURCHASE:"))
                        {
                            try
                            {
                                var plantIdStr = message.Substring("PURCHASE:".Length);
                                if (int.TryParse(plantIdStr, out int plantId))
                                {
                                    await webSocket.SendAsync(
                                        Encoding.UTF8.GetBytes("PURCHASE_SUCCESS"),
                                        WebSocketMessageType.Text,
                                        true,
                                        CancellationToken.None);
                                }
                                else
                                {
                                    await webSocket.SendAsync(
                                        Encoding.UTF8.GetBytes("ERROR: Invalid plant ID"),
                                        WebSocketMessageType.Text,
                                        true,
                                        CancellationToken.None);
                                }
                            }
                            catch (Exception ex)
                            {
                                await webSocket.SendAsync(
                                    Encoding.UTF8.GetBytes($"ERROR: {ex.Message}"),
                                    WebSocketMessageType.Text,
                                    true,
                                    CancellationToken.None);
                            }
                        }
                    }
                }
            }
            finally
            {
                _connectedClients.Remove(webSocket);
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
            var message = new { DiscountValue = discountValue }; // anonimowy typ
            var json = JsonSerializer.Serialize(message);
            var buffer = Encoding.UTF8.GetBytes(json);

            foreach (var client in _connectedClients.Where(c => c.State == WebSocketState.Open))
            {
                await client.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
            }
        }
    }
}
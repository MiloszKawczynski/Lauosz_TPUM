﻿using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

namespace Dane
{
    public class WebSocketDataService : IDisposable
    {
        private ClientWebSocket _ws = new();
        private const string SERVER_URL = "ws://localhost:8080/";

        public async Task ConnectAsync()
        {
            await _ws.ConnectAsync(new Uri(SERVER_URL), CancellationToken.None);
        }

        public async Task<string> SendCommandAsync(int plantId)
        {
            var message = $"PURCHASE:{plantId}";
            var buffer = Encoding.UTF8.GetBytes(message);
            await _ws.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);

            var responseBuffer = new byte[1024];
            var result = await _ws.ReceiveAsync(new ArraySegment<byte>(responseBuffer), CancellationToken.None);
            return Encoding.UTF8.GetString(responseBuffer, 0, result.Count);
        }

        public IObservable<float> DiscountUpdates()
        {
            return new DiscountObservable(_ws);
        }

        public void Dispose()
        {
            _ws?.Dispose();
        }

        private class DiscountObservable : IObservable<float>
        {
            private readonly ClientWebSocket _ws;
            public DiscountObservable(ClientWebSocket ws) => _ws = ws;

            public IDisposable Subscribe(IObserver<float> observer)
            {
                _ = Task.Run(async () =>
                {
                    while (_ws.State == WebSocketState.Open)
                    {
                        var buffer = new byte[1024];
                        var result = await _ws.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                        if (result.MessageType == WebSocketMessageType.Text)
                        {
                            var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                            if (message.Contains("DiscountValue")) // Przykład parsowania JSON
                            {
                                var discount = JsonSerializer.Deserialize<DiscountNotification>(message);
                                observer.OnNext(discount.DiscountValue);
                            }
                        }
                    }
                });
                return new Unsubscriber(() => { });
            }

            private class Unsubscriber : IDisposable
            {
                private readonly Action _unsubscribe;
                public Unsubscriber(Action unsubscribe) => _unsubscribe = unsubscribe;
                public void Dispose() => _unsubscribe();
            }
        }
    }
}
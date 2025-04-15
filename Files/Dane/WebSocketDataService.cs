using System;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using SharedModel;

namespace Dane
{
    public abstract class AbstractWebSocketDataService : IDisposable
    {
        public abstract Task ConnectAsync(IObserver<float> discount, IObserver<List<IPlant>> plant);
        public abstract Task SendCommandAsync(int plantId);
        public abstract Task SendAsync(string message);
        public abstract Task<string> ReceiveAsync();
        public abstract IObservable<float> DiscountUpdates();
        public abstract IObservable<List<IPlant>> PlantUpdates();
        public abstract void Dispose();

        public static AbstractWebSocketDataService Create()
        {
            return new WebSocketDataService();
        }

        internal sealed class WebSocketDataService : AbstractWebSocketDataService
        {
            private ClientWebSocket _ws = new();
            private const string SERVER_URL = "ws://localhost:8080/";
            private IObserver<List<IPlant>> _plantObserver;
            private IObserver<float> _discountObserver;

            public override async Task ConnectAsync(IObserver<float> discount, IObserver<List<IPlant>> plant)
            {
                _plantObserver = plant;
                _discountObserver = discount;
                await _ws.ConnectAsync(new Uri(SERVER_URL), CancellationToken.None);
                _ = Task.Run(async () =>
                {
                    var buffer = new byte[1024];
                    while (_ws.State == WebSocketState.Open)
                    {
                        var result = await _ws.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                        if (result.MessageType == WebSocketMessageType.Text)
                        {
                            var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                            HandleIncomingMessage(message);
                        }
                    }
                });
            }

            private void HandleIncomingMessage(string message)
            {
                if (message.Contains("["))
                {
                    try
                    {
                        var plants = JsonSerializer.Deserialize<List<Plant>>(message)?.Cast<IPlant>().ToList();
                        if (plants != null)
                        {
                             _plantObserver.OnNext(plants);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Błąd deserializacji roślin: {ex.Message}");
                    }
                }
                else if (message.Contains("DiscountValue"))
                {
                    try
                    {
                        var discount = JsonSerializer.Deserialize<DiscountNotification>(message);
                        _discountObserver.OnNext(discount.DiscountValue);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Błąd deserializacji rabatu: {ex.Message}");
                    }
                }
            }

            public override async Task SendCommandAsync(int plantId)
            {
                var message = $"PURCHASE:{plantId}";
                var buffer = Encoding.UTF8.GetBytes(message);
                await _ws.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);

            }

            public override async Task SendAsync(string message)
            {
                Console.WriteLine($"Wysyłane dane: {message}"); 
                var buffer = Encoding.UTF8.GetBytes(message);
                await _ws.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
            }

            public override async Task<string> ReceiveAsync()
            {
                var buffer = new byte[1024];
                var result = await _ws.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                return message;
            }

            public override IObservable<List<IPlant>> PlantUpdates()
{
                return new PlantObservable(_ws);
            }

            public override IObservable<float> DiscountUpdates()
            {
                return new DiscountObservable(_ws);
            }

            public override void Dispose()
            {
                _ws?.Dispose();
            }

            private class DiscountObservable : IObservable<float>
            {
                private readonly ClientWebSocket _ws;
                public DiscountObservable(ClientWebSocket ws) => _ws = ws;

                public IDisposable Subscribe(IObserver<float> observer)
                {
                    return new Unsubscriber(() => { });
                }

                private class Unsubscriber : IDisposable
                {
                    private readonly Action _unsubscribe;
                    public Unsubscriber(Action unsubscribe) => _unsubscribe = unsubscribe;
                    public void Dispose() => _unsubscribe();
                }
            }

            private class PlantObservable : IObservable<List<IPlant>>
            {
                private readonly ClientWebSocket _ws;
                public PlantObservable(ClientWebSocket ws) => _ws = ws;

                public IDisposable Subscribe(IObserver<List<IPlant>> observer)
                {
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
}
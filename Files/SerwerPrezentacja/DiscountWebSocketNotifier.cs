using SerwerLogika;

namespace SerwerPrezentacja
{
    public class DiscountWebSocketNotifier : IDiscountNotifier
    {
        private readonly WebSocketServer _webSocketServer;

        public DiscountWebSocketNotifier(WebSocketServer webSocketServer)
        {
            _webSocketServer = webSocketServer;
        }

        public void NotifyDiscount(float discount)
        {
            try
            {
                _webSocketServer.BroadcastDiscount(discount).Wait();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Błąd wysyłania powiadomienia: {ex.Message}");
            }
        }
    }
}
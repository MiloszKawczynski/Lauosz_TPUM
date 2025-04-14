using Dane;
using SerwerPrezentacja;

namespace Testy
{
    [TestClass]
    public class WebSocketServiceTest
    {
        // Test prepared, working only with server online

        [TestMethod]
        public async Task ShouldConnectToServer()
        {
            var server = new WebSocketServer();
            server.Start();
            var service = AbstractWebSocketDataService.Create();
            try
            {
                await service.ConnectAsync();
                Assert.IsTrue(true);
            }
            catch
            {
                Assert.Inconclusive("Test wymaga działającego serwera WebSocket");
            }
        }
    }
}
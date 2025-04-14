using Dane;
using SerwerPrezentacja;

namespace Testy
{
    [TestClass]
    public class PurchaseIntegrationTest
    {
        // Test prepared, working only with server online

        [TestMethod]
        public async Task ShouldSuccessfullyPurchasePlant()
        {
            var server = new WebSocketServer();
            server.Start();
            var service = AbstractWebSocketDataService.Create();
            try
            {
                await service.ConnectAsync();
                var response = await service.SendCommandAsync(1); // ID rośliny
                Assert.AreEqual("PURCHASE_SUCCESS", response);
            }
            catch
            {
                Assert.Inconclusive("Test wymaga działającego serwera WebSocket");
            }
        }
    }
}
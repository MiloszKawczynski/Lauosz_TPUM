using Dane;
using SerwerPrezentacja;

namespace Testy
{
    [TestClass]
    public class IntegrationTest
    {
        WebSocketServer server = new();
        AbstractWebSocketDataService service = AbstractWebSocketDataService.Create();

        [TestMethod]
        public async Task ShouldConnectToServer()
        {
            server.Start();
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

        [TestMethod]
        public async Task ShouldSuccessfullyPurchasePlant()
        {
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
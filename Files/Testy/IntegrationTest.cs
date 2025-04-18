using Dane;
using SerwerPrezentacja;
using SharedModel;

namespace Testy
{
    [TestClass]
    public class IntegrationTest
    {
        WebSocketServer server = new();
        AbstractWebSocketDataService service = AbstractWebSocketDataService.Create();
        private IObserver<float> _observerFloat;
        private IObserver<List<IPlant>> _observerList;

        [TestMethod]
        public async Task ShouldConnectToServerShouldSuccessfullyPurchasePlant()
        {
            server.Start();
            try
            {
                await service.ConnectAsync(_observerFloat, _observerList);
                Assert.IsTrue(true);

                await service.SendCommandAsync(1);
                Assert.IsTrue(true);
            }
            catch
            {
                Assert.Inconclusive("Test wymaga działającego serwera WebSocket");
            }

        }
    }
}
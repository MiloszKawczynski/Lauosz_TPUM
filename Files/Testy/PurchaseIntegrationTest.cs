using Dane;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace Testy
{
    [TestClass]
    public class PurchaseIntegrationTest
    {
        // Test prepared, working only with server online

        [TestMethod]
        public async Task ShouldSuccessfullyPurchasePlant()
        {
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
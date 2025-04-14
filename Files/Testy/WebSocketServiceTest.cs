using Dane;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace Testy
{
    [TestClass]
    public class WebSocketServiceTest
    {
        // Test prepared, working only with server online

        [TestMethod]
        public async Task ShouldConnectToServer()
        {
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
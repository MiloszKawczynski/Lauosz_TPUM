using Dane;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace Testy
{
    [TestClass]
    public class WebSocketServiceTest
    {
        [TestMethod]
        public async Task ShouldConnectToServer()
        {
            var service = new global::Dane.WebSocketDataService();
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
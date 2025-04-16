
using SerwerDane;
using SerwerLogika;
using SharedModel;

namespace Testy
{
    [TestClass]
    public class ServerLogicAPITests
    {

        private AbstractLogicAPI _logicAPI;
        private AbstractDataAPI _dataAPI;
       

        [TestInitialize]
        public void Setup()
        {
            _dataAPI =AbstractDataAPI.CreateAPI();
            _logicAPI = AbstractLogicAPI.CreateAPI(_dataAPI);
        }

        [TestMethod]
        public void LogicAPIAddNewPlantTest()
        {
            _logicAPI.AddNewPlant("Cactus", 15.99f);
            var plants = _logicAPI.GetAllPlants();
            Assert.AreEqual(1, plants.Count);
            Assert.AreEqual("Cactus", plants[0].Name);
            Assert.AreEqual(15.99f, plants[0].Price);
        }

        [TestMethod]
        public async Task LogicAPIPurchase()
        {
            _logicAPI.AddNewPlant("Bonsai", 100.0f);
            var plantsBefore = _logicAPI.GetAllPlants();
            Assert.AreEqual(1, plantsBefore.Count);

            var success = await _logicAPI.PurchasePlantAsync(plantsBefore[0].ID);
            Assert.IsTrue(success);

            var plantsAfter = _logicAPI.GetAllPlants();
            Assert.AreEqual(0, plantsAfter.Count);
        }

        [TestMethod]
        public async Task LogicAPIPurchasePlantFailTest()
        {
            var success = await _logicAPI.PurchasePlantAsync(999);
            Assert.IsFalse(success);
        }


    }
}

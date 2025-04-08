extern alias ClientLogic;
using ClientLogic::Logika;
using Dane;

namespace Testy
{
    [TestClass]
    public class ClientLogicAPITest
    {
        private AbstractLogicAPI _logicAPI;
        private AbstractDataAPI _dataAPI;

        [TestInitialize]
        public void Setup()
        {
            _dataAPI = AbstractDataAPI.CreateAPI();
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
        public void LogicAPIPurchase()
        {
            _logicAPI.AddNewPlant("Bonsai", 100.0f);
            var plantsBefore = _logicAPI.GetAllPlants();
            Assert.AreEqual(1, plantsBefore.Count);

            bool success = _logicAPI.PurchasePlant(plantsBefore[0].ID);
            Assert.IsTrue(success);

            var plantsAfter = _logicAPI.GetAllPlants();
            Assert.AreEqual(0, plantsAfter.Count);
        }

        [TestMethod]
        public void LogicAPIPurchasePlantFailTest()
        {
            var success = _logicAPI.PurchasePlant(999);
            Assert.IsFalse(success);
        }
    }
}
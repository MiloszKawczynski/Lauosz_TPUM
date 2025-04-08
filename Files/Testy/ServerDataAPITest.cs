
using SerwerDane;

namespace Testy
{
    [TestClass]
    public class ServerDataAPITest
    {
        [TestMethod]
        public void DataAPIAddPlantTest()
        {
            AbstractDataAPI dataAPI = AbstractDataAPI.CreateAPI();
            dataAPI.AddPlant(0, "Kwiatek", 10.0f);
            var plant = dataAPI.GetAllPlants().First();

            Assert.IsTrue(plant.ID == 0);
            Assert.IsTrue(plant.Name == "Kwiatek");
            Assert.IsTrue(plant.Price == 10.0f);
            Assert.IsTrue(dataAPI.GetAllPlants().Count == 1);
        }

        [TestMethod]
        public void DataAPIGetByIDTest()
        {
            AbstractDataAPI dataAPI = AbstractDataAPI.CreateAPI();
            dataAPI.AddPlant(0, "Kwiatek", 10.0f);
            var plant = dataAPI.GetAllPlants().First();

            Assert.IsTrue(dataAPI.GetPlantById(0) == plant);
        }

        [TestMethod]
        public void DataAPIRemovePlantTest()
        {
            AbstractDataAPI dataAPI = AbstractDataAPI.CreateAPI();
            dataAPI.AddPlant(0, "Kwiatek", 10.0f);
            dataAPI.RemovePlant(0);

            Assert.IsTrue(dataAPI.GetAllPlants().Count == 0);
        }
    }
}

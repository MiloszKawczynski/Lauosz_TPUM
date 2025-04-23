
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json.Schema.Generation;
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


        [TestMethod]
        public void GenerateAndValidateFullSchema()
        {
            string ProjectDir = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName;
            string SchemaFilePath = Path.Combine(ProjectDir, "Schema.json");

            var generator = new JSchemaGenerator();

            var schema = new JSchema
            {
                Type = JSchemaType.Object,
                Properties =
                {
                    ["Plant"] = generator.Generate(typeof(PlantDTO)),
                    ["DiscountNotification"] = generator.Generate(typeof(DiscountNotificationDTO)),
                    ["PlantList"] = generator.Generate(typeof(List<PlantDTO>))
                }
            };

            File.WriteAllText(SchemaFilePath, schema.ToString());

            var loadedSchema = JSchema.Parse(File.ReadAllText(SchemaFilePath));
            var validPlant = JObject.FromObject(new Plant(1, "Monstera", 49.99f));
            var validDiscount = JObject.FromObject(new DiscountNotification { DiscountValue = 0.1f });
            var validPlantList = JArray.FromObject(new List<Plant> { new Plant(1, "Monstera", 49.99f) });

            bool isPlantValid = validPlant.IsValid(loadedSchema.Properties["Plant"]);
            bool isDiscountValid = validDiscount.IsValid(loadedSchema.Properties["DiscountNotification"]);
            bool isPlantListValid = validPlantList.IsValid(loadedSchema.Properties["PlantList"]);

            Assert.IsTrue(isPlantValid, "Plant JSON nie pasuje do schematu!");
            Assert.IsTrue(isDiscountValid, "Discount JSON nie pasuje do schematu!");
            Assert.IsTrue(isPlantListValid, "PlantList JSON nie pasuje do schematu!");
        }
    }
}

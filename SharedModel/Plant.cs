
namespace SharedModel
{
    [Serializable]
    public struct PlantDTO
    {
        public int ID;
        public string Name;
        public float Price;
    }

    public class Plant : IPlant
    {
        public override int ID { get; set; }

        public override string Name { get; set; }

        public override float Price { get; set; }

        public Plant(int id, string name, float price)
        {
            Name = name;
            Price = price;
            ID = id;
        }
    }
}

namespace Dane
{
    internal class Plant : IPlant
    {
        public override int ID { get; }
        public override string Name { get; }
        public override float Price { get; set; }

        public Plant(int id, string name, float price)
        {
            ID = id;
            Name = name;
            Price = price;
        }
    }
}

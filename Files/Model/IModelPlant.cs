using System.ComponentModel;

namespace Model
{
    public abstract class IModelPlant
    {
        public static IModelPlant CreateModelPlant(int id, string name, float price)
        {
            return new ModelPlant(id, name, price);
        }
        public abstract int ID { get; }
        public abstract string Name { get; }
        public abstract float Price { get; set; }

        public abstract event PropertyChangedEventHandler PropertyChanged;
    }
}

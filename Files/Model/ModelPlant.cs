using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Model
{
    public abstract class IModelPlant : INotifyPropertyChanged
    {
        public abstract int ID { get; }
        public abstract string Name { get; }
        public abstract float Price { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;

        public static IModelPlant CreateModelPlant(int id, string name, float price)
        {
            return new ModelPlant(id, name, price);
        }
    }

    internal class ModelPlant : IModelPlant
    {
        public override int ID { get; }
        public override string Name { get; }
        private float _price;
        public override float Price
        {
            get => _price;
            set
            {
                if (_price != value)
                {
                    _price = value;
                    OnPropertyChanged();
                }
            }
        }

        public ModelPlant(int id, string name, float price)
        {
            ID = id;
            Name = name;
            _price = price;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
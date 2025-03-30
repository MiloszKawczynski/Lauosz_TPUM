using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Model
{
    internal class ModelPlant : IModelPlant, INotifyPropertyChanged
    {
        public override int ID { get;}
        public override string Name { get; }
        public override float Price { get; set;}

        public ModelPlant(int id, string name, float price)
        {
            ID = id;
            Name = name;
            Price = price;
        }

        public override event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

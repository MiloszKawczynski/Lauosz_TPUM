using Model;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace ViewModel
{
    public class ViewModelAPI  :INotifyPropertyChanged
    {
        private AbstractModelAPI _modelAPI;

        public ObservableCollection<IModelPlant> ModelPlants => _modelAPI.GetModelPlants();

        public event PropertyChangedEventHandler PropertyChanged;
        public PurchaseCommand PurchasePlantCommand { get; }
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ViewModelAPI()
        {
            _modelAPI = AbstractModelAPI.CreateAPI();
            PurchasePlantCommand = new PurchaseCommand(PurchasePlant);
        }

        private void PurchasePlant(IModelPlant plant)
        {
            _modelAPI.PurchasePlant(plant.ID);
            OnPropertyChanged(nameof(ModelPlants));
        }

    }

}
using Dane;
using Model;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace ViewModel
{
    public class ViewModelAPI : INotifyPropertyChanged
    {
        private readonly WebSocketDataService _socketService = new();
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

        public async Task InitializeConnectionAsync()
        {
            await _socketService.ConnectAsync();
        }

        private async void PurchasePlant(IModelPlant plant)
        {
            var response = await _socketService.SendCommandAsync(plant.ID);
            System.Diagnostics.Debug.WriteLine("Otrzymana odpowiedź: " + response);
            if (response == "PURCHASE_SUCCESS")
            {
                _modelAPI.PurchasePlant(plant.ID);
                OnPropertyChanged(nameof(ModelPlants));
            }
        }

    }

}
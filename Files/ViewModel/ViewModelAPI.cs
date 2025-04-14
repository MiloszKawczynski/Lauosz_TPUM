using Model;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

namespace ViewModel
{
    public abstract class AbstractViewModelAPI : INotifyPropertyChanged
    {
        public static AbstractViewModelAPI CreateAPI(AbstractModelAPI abstractModelAPI = default)
        {
            return new ViewModelAPI(abstractModelAPI ?? AbstractModelAPI.CreateAPI());
        }
        public abstract ObservableCollection<IModelPlant> ModelPlants { get; }
        public abstract ICommand LoadPlantsCommand { get; }
        public abstract ICommand PurchasePlantCommand { get; }
        public abstract Task InitializeAsync();
        public abstract Task InitializeConnectionAsync();

        public event PropertyChangedEventHandler PropertyChanged;
        public abstract void SubscribeToDiscounts(IObserver<float> observer);

        protected void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        internal sealed class ViewModelAPI : AbstractViewModelAPI
        {
            private readonly AbstractModelAPI _modelAPI;
            private IDisposable _discountSubscription;

            public override ObservableCollection<IModelPlant> ModelPlants => _modelAPI.GetModelPlants();

            public override ICommand PurchasePlantCommand { get; }
            public override ICommand LoadPlantsCommand { get; }

            public ViewModelAPI(AbstractModelAPI abstractModelAPI)
            {
                if (abstractModelAPI == null)
                {
                    _modelAPI = AbstractModelAPI.CreateAPI();
                }
                else
                {
                    _modelAPI = abstractModelAPI;
                }
                LoadPlantsCommand = new RelayCommand(async () => await LoadPlantsAsync());
                PurchasePlantCommand = new PurchaseCommand(PurchasePlant);
            }

            public override async Task InitializeAsync()
            {
                try
                {
                    await _modelAPI.InitializeConnectionAsync();
                    await LoadPlantsAsync();
                }
                catch (Exception ex)
                {

                }
            }

            public override async Task InitializeConnectionAsync()
            {
                await _modelAPI.InitializeConnectionAsync();
            }

            public override void SubscribeToDiscounts(IObserver<float> observer)
            {
                _discountSubscription?.Dispose();
                _discountSubscription = _modelAPI.DiscountUpdates.Subscribe(observer);
            }

            private async Task LoadPlantsAsync()
            {
                try
                {
                    await _modelAPI.LoadPlantsAsync();
                    RaisePropertyChanged(nameof(ModelPlants));
                }
                catch (Exception ex)
                {

                }
            }

            private async void PurchasePlant(object parameter)
            {
                if (parameter is IModelPlant plant)
                {
                    var result = await _modelAPI.PurchasePlantAsync(plant.ID);
                    if (result)
                    {
                        RaisePropertyChanged(nameof(ModelPlants));
                    }
                }
            }
        }
    }
    

}
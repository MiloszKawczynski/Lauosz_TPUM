using Model;
using SharedModel;
using System.Collections.Generic;
using System;
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
        public abstract Task InitializeConnectionAsync(IObserver<float> discount, IObserver<List<IPlant>> plant);

        public event PropertyChangedEventHandler PropertyChanged;
        public abstract void SubscribeToDiscounts(IObserver<float> observer);
        public abstract void SubscribeToPlants(IObserver<List<IPlant>> observer);
        public abstract void UpdatePlants(List<IPlant> plants);

        protected void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        internal sealed class ViewModelAPI : AbstractViewModelAPI
        {
            private readonly AbstractModelAPI _modelAPI;
            private IDisposable _plantsSubscription;
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

            

            public override async Task InitializeConnectionAsync(IObserver<float> discount, IObserver<List<IPlant>> plant)
            {
                _ = Task.Run(async () =>
                {
                    
                    SubscribeToDiscounts(discount);
                    SubscribeToPlants(plant);
                    
                });
                await _modelAPI.InitializeConnectionAsync( discount, plant);
            }

            public override void SubscribeToDiscounts(IObserver<float> observer)
            {
                _discountSubscription?.Dispose();
                _discountSubscription = _modelAPI.DiscountUpdates.Subscribe(observer);
            }

            public override void SubscribeToPlants(IObserver<List<IPlant>> observer)
            {
                _plantsSubscription?.Dispose();
                _plantsSubscription = _modelAPI.PlantUpdates.Subscribe(observer);
            }

            public override void UpdatePlants(List<IPlant> plants)
            {
                _modelAPI.UpdatePlants(plants);
                RaisePropertyChanged(nameof(ModelPlants));
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
                    await _modelAPI.PurchasePlantAsync(plant.ID);
                    RaisePropertyChanged(nameof(ModelPlants));
                }
            }
        }
    }
    

}
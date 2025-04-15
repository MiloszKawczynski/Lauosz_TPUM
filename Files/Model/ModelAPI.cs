using Logika;
using SharedModel;
using System.Collections.Generic;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace Model
{
    public abstract class AbstractModelAPI
    {
        public static AbstractModelAPI CreateAPI(AbstractLogicAPI abstractLogicAPI = default)
        {
            return new ModelAPI(abstractLogicAPI ?? AbstractLogicAPI.CreateAPI());
        }
        public abstract ObservableCollection<IModelPlant> GetModelPlants();
        public abstract Task InitializeConnectionAsync(IObserver<float> discount, IObserver<List<IPlant>> plant);
        public abstract Task LoadPlantsAsync();
        public abstract Task PurchasePlantAsync(int plantId);
        public abstract IObservable<float> DiscountUpdates { get; }
        public abstract IObservable<List<IPlant>> PlantUpdates { get; }
        public abstract void UpdatePlants(List<IPlant> plants);


        internal sealed class ModelAPI : AbstractModelAPI
        {

            private AbstractLogicAPI _logicAPI;
            private ObservableCollection<IModelPlant> _modelPlants = new ObservableCollection<IModelPlant>();
            private readonly SemaphoreSlim _lock = new SemaphoreSlim(1, 1);

            public ModelAPI(AbstractLogicAPI abstractLogicAPI)
            {
                if (abstractLogicAPI == null)
                {
                    _logicAPI = AbstractLogicAPI.CreateAPI();
                }
                else
                {
                    _logicAPI = abstractLogicAPI;
                }

            }

            public override async Task LoadPlantsAsync()
            {
                await _lock.WaitAsync();
                try
                {
                    var plants = await _logicAPI.GetPlantsAsync();
                    _modelPlants.Clear();

                    foreach (var plant in plants)
                    {
                        _modelPlants.Add(new ModelPlant(plant.ID, plant.Name, plant.Price));
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Błąd ładowania roślin: {ex.Message}");
                }
                finally
                {
                    _lock.Release();
                }
            }

            public override void UpdatePlants(List<IPlant> plants)
            {
                _modelPlants.Clear();
                foreach (var plant in plants)
                {
                    if (plant.Price <= 50.00f)
                    {
                        _modelPlants.Add(new ModelPlant(plant.ID, plant.Name, plant.Price));
                    }
                }
            }


            public override IObservable<float> DiscountUpdates => _logicAPI.DiscountUpdates;
            public override IObservable<List<IPlant>> PlantUpdates => _logicAPI.PlantUpdates;

            public override ObservableCollection<IModelPlant> GetModelPlants()
            {
                return _modelPlants;
            }

            public override async Task InitializeConnectionAsync(IObserver<float> discount, IObserver<List<IPlant>> plant)
            {
                await _logicAPI.InitializeConnectionAsync(discount, plant);
            }


            public override async Task PurchasePlantAsync(int plantId)
            {
                await _logicAPI.SendCommandAsync(plantId);

                await LoadPlantsAsync();

            }

        }
    }
}
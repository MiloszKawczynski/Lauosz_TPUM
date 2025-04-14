using Logika;
using System.Collections.ObjectModel;

namespace Model
{
    public abstract class AbstractModelAPI
    {
        public static AbstractModelAPI CreateAPI(AbstractLogicAPI abstractLogicAPI = default)
        {
            return new ModelAPI(abstractLogicAPI ?? AbstractLogicAPI.CreateAPI());
        }
        public abstract ObservableCollection<IModelPlant> GetModelPlants();
        public abstract Task InitializeConnectionAsync();
        public abstract Task LoadPlantsAsync();
        public abstract Task<bool> PurchasePlantAsync(int plantId);
        public abstract IObservable<float> DiscountUpdates { get; }


        internal sealed class ModelAPI : AbstractModelAPI
        {
            
            private AbstractLogicAPI _logicAPI;
            private ObservableCollection<IModelPlant> _modelPlants = new ObservableCollection<IModelPlant>();

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
                InitializeAsync().ConfigureAwait(false);
            }

            private async Task InitializeAsync()
            {
                await _logicAPI.InitializeConnectionAsync();
                await LoadPlantsAsync();
            }

            public override async Task LoadPlantsAsync()
            {
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
            }

            public override IObservable<float> DiscountUpdates => _logicAPI.DiscountUpdates;

            public override ObservableCollection<IModelPlant> GetModelPlants()
            {
                return _modelPlants;
            }

            public override async Task InitializeConnectionAsync()
            {
                await _logicAPI.InitializeConnectionAsync();
            }


            public override async Task<bool> PurchasePlantAsync(int plantId)
            {
                var result = await _logicAPI.SendCommandAsync(plantId);

                if (result == "PURCHASE_SUCCESS")
                {
                    //_logicAPI.PurchasePlant(plantId);
                    await LoadPlantsAsync();
                    return true;
                }

                return false;
            }

        }
    }
}
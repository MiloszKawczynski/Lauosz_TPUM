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
        public abstract void AddPlant(string name, float price);
        public abstract void PurchasePlant(int id);
        public abstract Task InitializeConnectionAsync();
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
                _logicAPI.AddNewPlant("Cactus", 10.0f);
                _logicAPI.AddNewPlant("Fern", 15.0f);
                _logicAPI.AddNewPlant("Rose", 5.0f);
                LoadPlants();
            }

            public override IObservable<float> DiscountUpdates => _logicAPI.DiscountUpdates;

            public override ObservableCollection<IModelPlant> GetModelPlants()
            {
                return _modelPlants;
            }

            public override void AddPlant(string name, float price)
            {
                _logicAPI.AddNewPlant(name, price);
                LoadPlants();
            }

            public override void PurchasePlant(int id)
            {
                _logicAPI.PurchasePlant(id);
                LoadPlants();
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
                    PurchasePlant(plantId);
                    return true;
                }

                return false;
            }

            private void LoadPlants()
            {
                var plants = _logicAPI.GetAllPlants();
                _modelPlants.Clear();
                foreach (var plant in plants)
                {
                    _modelPlants.Add(new ModelPlant(plant.ID, plant.Name, plant.Price));
                }
            }

        }
    }
}
using Logika;
using Dane;
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
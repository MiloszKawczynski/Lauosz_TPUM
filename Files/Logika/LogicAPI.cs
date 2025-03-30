using Dane;
using System.Numerics;


namespace Logika
{
    public abstract class AbstractLogicAPI
    {
        public abstract List<IPlant> GetAllPlants();
        public abstract bool PurchasePlant(int id);
        public abstract void AddNewPlant(string name, float price);

        public static AbstractLogicAPI CreateAPI(AbstractDataAPI? dataApi = null)
        {
            return new LogicAPI(dataApi ?? AbstractDataAPI.CreateAPI());
        }

        internal sealed class LogicAPI : AbstractLogicAPI
        {
            private AbstractDataAPI _dataAPI;
            private int _nextId = 1;

            public LogicAPI(AbstractDataAPI? dataAPI)
            {
                if (dataAPI == null)
                {
                    _dataAPI = AbstractDataAPI.CreateAPI();
                }
                else
                {
                    _dataAPI = dataAPI;
                }
                _nextId = dataAPI.GetAllPlants().Count + 1;
            }

            public override List<IPlant> GetAllPlants()
            {
                return _dataAPI.GetAllPlants();
            }

            public override bool PurchasePlant(int id)
            {
                var plant = _dataAPI.GetPlantById(id);
                if (plant == null) return false;
                _dataAPI.RemovePlant(id);
                return true;
            }

            public override void AddNewPlant(string name, float price)
            {
                _dataAPI.AddPlant(_nextId++, name, price);
            }
        }
    }
}
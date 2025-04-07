
using Dane;


namespace Logika
{
    public abstract class AbstractLogicAPI
    {
        public abstract List<IPlant> GetAllPlants();
        public abstract bool PurchasePlant(int id);
        public abstract void AddNewPlant(string name, float price);
        public abstract void StartDiscountChecker();
        public abstract void StopDiscountChecker();

        public static AbstractLogicAPI CreateAPI(AbstractDataAPI? dataApi = null)
        {
            return new LogicAPI(dataApi ?? AbstractDataAPI.CreateAPI());
        }

        internal sealed class LogicAPI : AbstractLogicAPI
        {
            private AbstractDataAPI _dataAPI;
            private int _nextId = 1;
            private CancellationTokenSource? _discountTokenSource;
            private Dictionary<int, float> _originalPrices = new();

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
                StartDiscountChecker();
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

            public override void StartDiscountChecker()
            {
                _discountTokenSource = new CancellationTokenSource();
                Task.Run(async () =>
                {
                    while (_discountTokenSource.Token.IsCancellationRequested == false)
                    {
                        if (DateTime.Now.DayOfWeek == DayOfWeek.Friday)
                        {
                            ApplyDiscount(0.9f);


                            await Task.Delay(TimeSpan.FromDays(1), _discountTokenSource.Token);
                            RestoreOriginalPrices();
                        }
                        else
                        {
                            await Task.Delay(TimeSpan.FromHours(1), _discountTokenSource.Token);
                        }
                    }
                }, _discountTokenSource.Token);
            }

            public override void StopDiscountChecker()
            {
                _discountTokenSource?.Cancel();
            }
            private void ApplyDiscount(float discountFactor)
            {
                var plants = _dataAPI.GetAllPlants();
                foreach (var plant in plants)
                {
                    if (_originalPrices.ContainsKey(plant.ID) == false)
                    {
                        _originalPrices[plant.ID] = plant.Price;
                    }
                    _dataAPI.UpdatePlantPrice(plant.ID, plant.Price * discountFactor);
                }
            }

            private void RestoreOriginalPrices()
            {
                foreach (var kvp in _originalPrices)
                {
                    _dataAPI.UpdatePlantPrice(kvp.Key, kvp.Value);
                }
                _originalPrices.Clear();
            }

            ~LogicAPI()
            {
                StopDiscountChecker();
            }

        }
    }
}
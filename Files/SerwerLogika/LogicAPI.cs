using SerwerDane;

namespace SerwerLogika
{
    public abstract class AbstractLogicAPI
    {
        public abstract List<IPlant> GetAllPlants();
        public abstract bool PurchasePlant(int id);
        public abstract void AddNewPlant(string name, float price);
        public abstract void StartDiscountChecker();
        public abstract void StopDiscountChecker();

        public static AbstractLogicAPI CreateAPI(AbstractDataAPI? dataApi = null, IDiscountNotifier? discountNotifier = null)
        {
            return new LogicAPI(
                dataApi ?? AbstractDataAPI.CreateAPI(),
                discountNotifier ?? new DiscountNotifier()
            );
        }

        internal sealed class LogicAPI : AbstractLogicAPI
        {
            private readonly AbstractDataAPI _dataAPI;
            private readonly IDiscountNotifier _discountNotifier;
            private int _nextId = 1;
            private CancellationTokenSource? _discountTokenSource;
            private Dictionary<int, float> _originalPrices = new();

            public LogicAPI(AbstractDataAPI dataAPI, IDiscountNotifier discountNotifier)
            {
                _dataAPI = dataAPI;
                _discountNotifier = discountNotifier;
                _nextId = _dataAPI.GetAllPlants().Count + 1;
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
                    while (!_discountTokenSource.Token.IsCancellationRequested)
                    {
                        if (DateTime.Now.DayOfWeek == DayOfWeek.Friday)
                        {
                            ApplyDiscount(0.9f);
                            await Task.Delay(TimeSpan.FromDays(1), _discountTokenSource.Token);
                            RestoreOriginalPrices();
                        }
                        await Task.Delay(TimeSpan.FromHours(1), _discountTokenSource.Token);
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
                    if (!_originalPrices.ContainsKey(plant.ID))
                    {
                        _originalPrices[plant.ID] = plant.Price;
                    }
                    _dataAPI.UpdatePlantPrice(plant.ID, plant.Price * discountFactor);
                }
                _discountNotifier.NotifyDiscount(1f - discountFactor);
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
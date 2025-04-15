using SerwerDane;
using SharedModel;

namespace SerwerLogika
{
    public abstract class AbstractLogicAPI
    {
        public abstract List<IPlant> GetAllPlants();
        public abstract Task<bool> PurchasePlantAsync(int id);
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
            private readonly SemaphoreSlim _semaphore = new(1, 1);
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

            public override async Task<bool> PurchasePlantAsync(int id)
            {
                await _semaphore.WaitAsync();
                try
                {
                    var plant = _dataAPI.GetPlantById(id);
                    if (plant == null) return false;
                    _dataAPI.RemovePlant(id);
                    return true;
                }
                finally
                {
                    _semaphore.Release();
                }
            }

            public override void AddNewPlant(string name, float price)
            {
                _semaphore.Wait();
                try
                {
                    _dataAPI.AddPlant(_nextId++, name, price);
                }
                finally
                {
                    _semaphore.Release();
                }
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
                            await ApplyDiscountAsync(0.9f);
                            await Task.Delay(TimeSpan.FromDays(1), _discountTokenSource.Token);
                            await RestoreOriginalPricesAsync();
                        }
                        await Task.Delay(TimeSpan.FromHours(1), _discountTokenSource.Token);
                    }
                }, _discountTokenSource.Token);
            }

            public override void StopDiscountChecker()
            {
                _discountTokenSource?.Cancel();
            }

            private async Task ApplyDiscountAsync(float discountFactor)
            {
                await _semaphore.WaitAsync();
                try
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
                finally
                {
                    _semaphore.Release();
                }
            }

            private async Task RestoreOriginalPricesAsync()
            {
                await _semaphore.WaitAsync();
                Console.WriteLine("Lock");
                try
                {
                    foreach (var kvp in _originalPrices)
                    {
                        _dataAPI.UpdatePlantPrice(kvp.Key, kvp.Value);
                    }
                    _originalPrices.Clear();
                }
                finally
                {
                    _semaphore.Release();
                    Console.WriteLine("Open");
                }
            }

            ~LogicAPI()
            {
                StopDiscountChecker();
            }
        }
    }
}
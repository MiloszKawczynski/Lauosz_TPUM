using Dane;
using SharedModel;


namespace Logika
{
    public abstract class AbstractLogicAPI
    {
        //public abstract bool PurchasePlant(int id);
        public abstract void StartDiscountChecker();
        public abstract void StopDiscountChecker();
        public abstract Task InitializeConnectionAsync();
        public abstract Task<string> SendCommandAsync(int plantId);
        public abstract IObservable<float> DiscountUpdates { get; }
        public abstract Task<IEnumerable<IPlant>> GetPlantsAsync();

        public static AbstractLogicAPI CreateAPI(AbstractDataAPI? dataApi = null)
        {
            return new LogicAPI(dataApi ?? AbstractDataAPI.CreateAPI());
        }

        internal sealed class LogicAPI : AbstractLogicAPI
        {
            private AbstractDataAPI _dataAPI;
            private int _nextId = 1;
            private CancellationTokenSource? _discountTokenSource;
            private readonly IObservable<float> _discountUpdates;
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
                _discountUpdates = _dataAPI.DiscountUpdates();
                StartDiscountChecker();
            }
            public override async Task<IEnumerable<IPlant>> GetPlantsAsync()
            {
                var plants = await _dataAPI.GetAllPlantsAsync();
                return plants.OrderBy(p => p.Name);
            }

            public override IObservable<float> DiscountUpdates => _discountUpdates;
            //public override bool PurchasePlant(int id)
            //{
            //    var plant = _dataAPI.GetPlantById(id);
            //    if (plant == null) return false;
            //    _dataAPI.RemovePlant(id);
            //    return true;
            //}

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
                            //RestoreOriginalPrices();
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
                //var plants = _dataAPI.GetAllPlants();
                //foreach (var plant in plants)
                //{
                //    if (_originalPrices.ContainsKey(plant.ID) == false)
                //    {
                //        _originalPrices[plant.ID] = plant.Price;
                //    }
                    //_dataAPI.UpdatePlantPrice(plant.ID, plant.Price * discountFactor);
                //}
            }

            public override async Task InitializeConnectionAsync()
            {
                await _dataAPI.InitializeConnectionAsync();
            }

            public override async Task<string> SendCommandAsync(int plantId)
            {
                return await _dataAPI.SendCommandAsync(plantId);
            }

            ~LogicAPI()
            {
                StopDiscountChecker();
            }

        }
    }
}
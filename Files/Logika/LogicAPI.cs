using System.Collections.Generic;
using System;
using Dane;
using SharedModel;


namespace Logika
{
    public abstract class AbstractLogicAPI
    {
        public abstract Task InitializeConnectionAsync(IObserver<float> discount, IObserver<List<IPlant>> plant);
        public abstract Task SendCommandAsync(int plantId);
        public abstract IObservable<float> DiscountUpdates { get; }
        public abstract IObservable<List<IPlant>> PlantUpdates { get; }
        public abstract Task<IEnumerable<IPlant>> GetPlantsAsync();

        public static AbstractLogicAPI CreateAPI(AbstractDataAPI? dataApi = null)
        {
            return new LogicAPI(dataApi ?? AbstractDataAPI.CreateAPI());
        }

        internal sealed class LogicAPI : AbstractLogicAPI
        {
            private AbstractDataAPI _dataAPI;
            private readonly IObservable<float> _discountUpdates;
            private readonly IObservable<List<IPlant>> _plantUpdates;

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
                _plantUpdates = _dataAPI.PlantUpdates();
            }
            public override async Task<IEnumerable<IPlant>> GetPlantsAsync()
            {
                var plants = await _dataAPI.GetAllPlantsAsync();
                return plants.OrderBy(p => p.Name);
            }

            public override IObservable<float> DiscountUpdates => _discountUpdates;
            public override IObservable<List<IPlant>> PlantUpdates => _plantUpdates;

            public override async Task InitializeConnectionAsync(IObserver<float> discount, IObserver<List<IPlant>> plant)
            {
                await _dataAPI.InitializeConnectionAsync(discount, plant);
            }

            public override async Task SendCommandAsync(int plantId)
            {
                await _dataAPI.SendCommandAsync(plantId);
            }
        }
    }
}
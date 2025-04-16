using System.Text.Json;
using SharedModel;

namespace Dane
{
    public abstract class AbstractDataAPI
    {
        public abstract Task InitializeConnectionAsync(IObserver<float> discount, IObserver<List<IPlant>> plant);
        public abstract Task SendCommandAsync(int plantId);
        public abstract IObservable<float> DiscountUpdates();
        public abstract Task<IEnumerable<IPlant>> GetAllPlantsAsync();
        public abstract IObservable<List<IPlant>> PlantUpdates();

        public static AbstractDataAPI CreateAPI()
        {
            return new DataAPI();
        }

        internal sealed class DataAPI : AbstractDataAPI
        {
            
            private readonly AbstractWebSocketDataService _websocketDataService;

            public DataAPI()
            {
                
                _websocketDataService = AbstractWebSocketDataService.Create();
            }

            public override async Task InitializeConnectionAsync(IObserver<float> discount, IObserver<List<IPlant>> plant)
            {
                await _websocketDataService.ConnectAsync(discount, plant);
            }

            public override async Task SendCommandAsync(int plantId)
            {
               await _websocketDataService.SendCommandAsync(plantId);
            }

            public override IObservable<float> DiscountUpdates()
            {
                return _websocketDataService.DiscountUpdates();
            }

            public override IObservable<List<IPlant>> PlantUpdates()
            {
                return _websocketDataService.PlantUpdates();
            }

            public override async Task<IEnumerable<IPlant>> GetAllPlantsAsync()
            {
                await _websocketDataService.SendAsync("GET_PLANTS");
                var json = await _websocketDataService.ReceiveAsync();
                return JsonSerializer.Deserialize<List<Plant>>(json) ?? new List<Plant>();
            }
        }
    }
}

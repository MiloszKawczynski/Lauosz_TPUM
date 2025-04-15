using SharedModel;

namespace SerwerDane
{
    internal class PlantRepository : IPlantRepository
    {
        private readonly SemaphoreSlim _semaphore = new(1, 1);
        private readonly List<IPlant> _plants = new();

        public override List<IPlant> GetAllPlants()
        {
            _semaphore.Wait();
            try
            {
                return _plants;
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public override IPlant? GetPlantById(int id)
        {
            _semaphore.Wait();
            try
            {
                return _plants.FirstOrDefault(p => p.ID == id);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public override void AddPlant(IPlant plant)
        {
            _semaphore.Wait();
            try
            {
                _plants.Add(plant);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public override void RemovePlant(int id)
        {
            _semaphore.Wait();
            try
            {
                var plant = _plants.FirstOrDefault(p => p.ID == id);
                if (plant != null)
                {
                    _plants.Remove(plant);
                }
            }
            finally
            {
                _semaphore.Release();
            }
        }
    }
}

namespace Dane
{
    internal class PlantRepository: IPlantRepository
    {
        private readonly List<IPlant> _plants = new();

        public override List<IPlant> GetAllPlants()
        {
            return _plants;
        }

        public override IPlant? GetPlantById(int id)
        {
            return _plants.FirstOrDefault(p => p.ID == id);
        }

        public override void AddPlant(IPlant plant)
        {
            _plants.Add(plant);
        }

        public override void RemovePlant(int id)
        {
            var plant = _plants.FirstOrDefault(p => p.ID == id);
            if (plant != null) 
            { 
                _plants.Remove(plant); 
            }
        }
    }
}

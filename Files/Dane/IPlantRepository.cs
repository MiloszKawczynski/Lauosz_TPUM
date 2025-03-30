namespace Dane
{
    public abstract class IPlantRepository
    {
        public abstract List<IPlant> GetAllPlants();
        public abstract IPlant? GetPlantById(int id);
        public abstract void AddPlant(IPlant plant);
        public abstract void RemovePlant(int id);
    }
}

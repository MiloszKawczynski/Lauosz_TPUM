﻿namespace Dane
{
    public abstract class AbstractDataAPI
    {
        public abstract List<IPlant> GetAllPlants();
        public abstract void AddPlant(int id, string name, float price);
        public abstract void RemovePlant(int id);
        public abstract IPlant? GetPlantById(int id);

        public static AbstractDataAPI CreateApi()
        {
            return new DataAPI();
        }

        internal sealed class DataAPI : AbstractDataAPI
        {
            private readonly PlantRepository _plants;

            public DataAPI()
            {
                _plants = new PlantRepository();
            }

            public override List<IPlant> GetAllPlants()
            {
                return _plants.GetAllPlants();
            }
            public override void AddPlant(int id, string name, float price)
            {
                _plants.AddPlant(new Plant(id, name, price));
            }

            public override void RemovePlant(int id)
            {
                _plants.RemovePlant(id);
            }

            public override IPlant? GetPlantById(int id)
            {
                return _plants.GetPlantById(id);
            }
        }
    }
}

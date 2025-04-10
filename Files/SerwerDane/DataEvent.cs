namespace SerwerDane
{
    public abstract class DataEvent
    {
        public IPlant plant;
        public DataEvent(IPlant plant)
        {
            this.plant = plant;
        }
    }
}

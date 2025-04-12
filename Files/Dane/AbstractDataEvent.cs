namespace Dane
{
    public abstract class AbstractDataEvent
    {
        public IPlant Plant { get; }

        protected AbstractDataEvent(IPlant plant)
        {
            Plant = plant;
        }
        internal class DataEvent : AbstractDataEvent
        {
            public DataEvent(IPlant plant) : base(plant) { }
        }
    }
}

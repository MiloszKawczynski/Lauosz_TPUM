using System.Drawing;
using System.Numerics;
using System.Runtime.Serialization;

namespace Dane
{
    public abstract class IBall : ISerializable
    {
        public abstract int ID { get; }
        public abstract Vector2 Position { get; }
        public abstract Vector2 Movement { get; set; }
        public abstract bool IsRunning { get; set; }
        public abstract void TurnOff();

        public abstract event EventHandler<DataEvent> PropertyChanged;

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("ID: ", ID);
            info.AddValue("Position: ", Position);
            info.AddValue("Movement: ", Movement);
        }
    }
}

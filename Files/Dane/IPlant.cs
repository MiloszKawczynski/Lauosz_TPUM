﻿using System.Runtime.Serialization;

namespace Dane
{
    public abstract class IPlant : ISerializable
    {
        public abstract int ID { get; }
        public abstract string Name { get; }
        public abstract float Price { get; }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
           info.AddValue("ID: ", ID);
           info.AddValue("Name: ", Name);
           info.AddValue("Price: ", Price);
        }
       
    }
}

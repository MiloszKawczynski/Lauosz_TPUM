using Dane;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Logika
{
    public abstract class ILogicBall
    {
        public static ILogicBall CreateLogicBall(float x, float y)
        {
            return new LogicBall(x, y);
        }

        public abstract Vector2 Position { get; set; }
        public abstract void Update(Object o, DataEvent args);

        public abstract event EventHandler<LogicEvent> PropertyChanged;
    }
}

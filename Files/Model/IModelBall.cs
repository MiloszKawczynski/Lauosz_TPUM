using Logika;
using System.ComponentModel;
using System.Drawing;
using System.Numerics;

namespace Model
{
    public abstract class IModelBall
    {
        public static IModelBall CreateModelBall(int x, int y, double r)
        {
            return new ModelBall(x, y, r);
        }

        public abstract Point Position { get; set; }
        public abstract double R { get; }

        public abstract void Update(Object s, LogicEvent e);

        public abstract event PropertyChangedEventHandler PropertyChanged;

    }
}

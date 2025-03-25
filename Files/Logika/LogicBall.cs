using System.ComponentModel;
using System.Drawing;
using System.Numerics;
using System.Runtime.CompilerServices;
using Dane;

namespace Logika
{
    internal class LogicBall : ILogicBall
    {
        private Vector2 _position;

        public LogicBall(float x, float y)
        {
            _position = new Vector2(x, y);
        }

        public override Vector2 Position
        { get { return _position; } set { _position = value; } }

        public override void Update(Object o, DataEvent args)
        {
            IBall ball = (IBall)o;
            this.Position = ball.Position;
            LogicEvent e = new LogicEvent(this);
            PropertyChanged?.Invoke(this, e);
        }

        public override event EventHandler<LogicEvent>? PropertyChanged;
    }
}

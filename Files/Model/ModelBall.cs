using Logika;
using System.ComponentModel;
using System.Drawing;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Model
{
    internal class ModelBall : IModelBall, INotifyPropertyChanged
    {
        private Point _position;
        private double _r;

        public ModelBall(int x, int y, double r)
        {
            _position = new Point(x, y);
            _r = r;
        }

        public override Point Position
        { get { return _position; } set { _position = value; OnPropertyChanged(); } }

        public override double R
        { get => _r; }

        public override void Update(object obj, LogicEvent args)
        {
            ILogicBall ball = (ILogicBall)obj;
            Point point = new Point((int)ball.Position.X, (int)ball.Position.Y);
            this.Position = point;

        }
        public override event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }

}
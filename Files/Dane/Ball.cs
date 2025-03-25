using System.Numerics;

namespace Dane
{
    internal class Ball : IBall
    {
        public override int ID { get; }
        private Vector2 _Position;
        public override Vector2 Movement { get; set; }
        private bool _isRunning = false;
        private AbstractBallLogger _logger;

        public Ball(int id, float x, float y, AbstractBallLogger logger)
        {
            ID = id;
            _Position = new Vector2(x, y);
            this._logger = logger;
            Task.Run(StartMovement);
        }

        private void MakeMove()
        {
            Vector2 prevMovement = Movement;
            Vector2 prevPosition = Position;

            Vector2 newPosition = new Vector2(prevMovement.X + prevPosition.X, prevMovement.Y + prevPosition.Y);
            _Position = newPosition;
            DataEvent args = new DataEvent(this);
            PropertyChanged?.Invoke(this, args);
        }

        private async void StartMovement()
        {
            _isRunning = true;
            while (_isRunning)
            {
                MakeMove();
                _logger.addBallToQueue(this);
                double speed = Math.Sqrt(Math.Pow(Movement.X, 2) + Math.Pow(Movement.Y, 2));
                await Task.Delay((int)speed);
            }
        }

        public override Vector2 Position
        { get { return _Position; } }
        public override bool IsRunning
        { get { return _isRunning; } set { _isRunning = value; } }

        public override void TurnOff()
        {
            _isRunning = false;
        }


        public override event EventHandler<DataEvent> PropertyChanged;

    }
}

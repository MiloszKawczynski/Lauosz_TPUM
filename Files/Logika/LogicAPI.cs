using Dane;
using System.Numerics;


namespace Logika
{
    public abstract class AbstractLogicAPI
    {
        public static AbstractLogicAPI CreateAPI(AbstractDataAPI abstractDataAPI = default)
        {
            return new LogicAPI(abstractDataAPI ?? AbstractDataAPI.CreateApi());
        }
        public abstract void CreateField(int height, int width);
        public abstract void CreateBalls(int kulaAmount, int kulaRadius);
        public abstract List<ILogicBall> GetAllBalls();
        public abstract void TurnOff();
        public abstract void TurnOn();
        public abstract bool IsRunning();



        internal sealed class LogicAPI : AbstractLogicAPI
        {
            private AbstractDataAPI _dataAPI;
            private List<ILogicBall> _logicBalls;
            private int ballRadius;
            private object LockObject = new object();

            public LogicAPI(AbstractDataAPI abstractDataAPI)
            {
                if (abstractDataAPI == null)
                {
                    _dataAPI = AbstractDataAPI.CreateApi();
                }
                else
                {
                    _dataAPI = abstractDataAPI;
                }
                _logicBalls = new List<ILogicBall>();
            }

            public override void CreateField(int height, int width)
            {
                _dataAPI.CreateScene(height, width);
            }
            public override void CreateBalls(int ballsAmount, int ballRadius)
            {
                this.ballRadius = ballRadius;
                int x;
                int y;
                Random random = new Random();
                for (int i = 0; i < ballsAmount; i++)
                {

                    x = random.Next(ballRadius, _dataAPI.GetSceneWidth() - ballRadius);
                    y = random.Next(ballRadius, _dataAPI.GetSceneHeight() - ballRadius);
                    _dataAPI.CreateBall(i, x, y);

                    IBall ball = _dataAPI.GetAllBalls().ElementAt(i);
                    do
                    {
                        int xMovement = random.Next(-10000, 10000) % 3;
                        int yMovement = random.Next(-10000, 10000) % 3;
                        ball.Movement = new Vector2(xMovement, yMovement);
                    } while (ball.Movement.X == 0 || ball.Movement.Y == 0);


                    ILogicBall logicBall = ILogicBall.CreateLogicBall(ball.Position.X, ball.Position.Y);

                    ball.PropertyChanged += WallColission!;
                    ball.PropertyChanged += CheckCollision!;
                    ball.PropertyChanged += logicBall.Update!;
                    _logicBalls.Add(logicBall);

                }
            }

            public override void TurnOff()
            {
                _dataAPI.TurnOff();
                _logicBalls.Clear();
            }
            public override void TurnOn()
            {
                _dataAPI.TurnOn();
            }
            public override bool IsRunning()
            {
                return _dataAPI.IsRunning();
            }
            public override List<ILogicBall> GetAllBalls()
            {
                return _logicBalls;
            }


            private void WallColission(Object o, DataEvent args)
            {
                IBall ball = (IBall)o;
                float newXMoventValue = ball.Movement.X;
                float newYMoventValue = ball.Movement.Y;
                if (0 > (ball.Position.X + ball.Movement.X) || _dataAPI.GetSceneWidth() < (ball.Position.X + ball.Movement.X + ballRadius))
                {
                    newXMoventValue = -ball.Movement.X;
                }

                if (0 > (ball.Position.Y + ball.Movement.Y) || _dataAPI.GetSceneHeight() < (ball.Position.Y + ball.Movement.Y + ballRadius))
                {
                    newYMoventValue = -ball.Movement.Y;
                }
                ball.Movement = new Vector2(newXMoventValue, newYMoventValue);

            }

            private void BallCollision(IBall ball, IBall otherBall)
            {
                int weight = 1;
                if (otherBall != ball)
                {
                    float xDistance = ball.Position.X + ball.Movement.X - otherBall.Position.X - otherBall.Movement.X;
                    float yDistance = ball.Position.Y + ball.Movement.Y - otherBall.Position.Y - otherBall.Movement.Y;
                    double distance = Math.Sqrt(Math.Pow(xDistance, 2) + Math.Pow(yDistance, 2));

                    if (distance <= (ballRadius))
                    {
                        float newOtherBallMovementX = 2 * weight * ball.Movement.X / (2 * weight);
                        float newBallMovementX = 2 * weight * otherBall.Movement.X / (2 * weight);

                        float newOtherBallMovementY = 2 * weight * ball.Movement.Y / (2 * weight);
                        float newBallMovementY = 2 * weight * otherBall.Movement.Y / (2 * weight);

                        otherBall.Movement = new Vector2(newOtherBallMovementX, newOtherBallMovementY);
                        ball.Movement = new Vector2(newBallMovementX, newBallMovementY);
                    }
                }

            }

            private void CheckCollision(Object o, DataEvent args)
            {
                try
                {
                    Monitor.Enter(LockObject);
                    IBall ball = (IBall)o;
                    foreach (IBall otherBall in _dataAPI.GetAllBalls().ToArray())
                    {
                        if (otherBall != ball)
                        {
                            float xDistance = ball.Position.X - otherBall.Position.X;
                            float yDistance = ball.Position.Y - otherBall.Position.Y;
                            double distance = Math.Sqrt(Math.Pow(xDistance, 2) + Math.Pow(yDistance, 2));

                            if (distance <= (ballRadius))
                            {
                                BallCollision(ball, otherBall);
                            }
                        }
                    }
                }
                finally
                {
                    Monitor.Exit(LockObject);
                }
            }
        }
    }
}
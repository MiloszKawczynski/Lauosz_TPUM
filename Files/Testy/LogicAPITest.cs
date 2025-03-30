using Dane;
using Logika;
using Microsoft.VisualStudio.TestTools.UnitTesting.Logging;
using System.Drawing;
using System.Numerics;

namespace Testy
{
    [TestClass]
    public class LogicAPITest
    {
        
        internal class FakeDataBall : IBall
        {

            private Vector2 _Position;
            private bool _isRunning = false;
            public override int ID { get; }
            public override Vector2 Position
            { get { return _Position; } }
            public override Vector2 Movement { get; set; }
            public override bool IsRunning
            { get { return _isRunning; } set { _isRunning = value; } }

            public override event EventHandler<DataEvent> PropertyChanged;

            public FakeDataBall(int id, float x, float y)
            {
                ID = id;
                _Position = new Vector2(x, y);
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
                    double speed = Math.Sqrt(Math.Pow(Movement.X, 2) + Math.Pow(Movement.Y, 2));
                    await Task.Delay((int)speed);
                }
            }

            public override void TurnOff()
            {
                _isRunning = false;
            }
        }
        
        internal class FakeDataAPI : AbstractDataAPI
        {
            int ballRadius = 10;
            private int sceneHeight;
            private int sceneWidth;
            private bool isRunning;
            List<IBall> _ballList = new List<IBall>();
            private AbstractBallLogger logger;

            public override void CreateBall(int id, int x, int y)
            {
                Random random = new Random();
                x = random.Next(ballRadius, this.GetSceneWidth() - ballRadius);
                y = random.Next(ballRadius, this.GetSceneHeight() - ballRadius);

                _ballList.Add(new FakeDataBall(1, x, y));
                do
                {
                    _ballList.Last().Movement = new Vector2(random.Next(-10000, 10000) % 3, random.Next(-10000, 10000) % 3);
                } while (_ballList.Last().Movement.X == 0 || _ballList.Last().Movement.Y == 0);

            }

            public override void CreateScene(int height, int width)
            {
                sceneHeight = height;
                sceneWidth = width;
                logger = AbstractBallLogger.CreateBallLoger();
            }

            public override List<IBall> GetAllBalls()
            {
                return _ballList;
            }

            public override int GetSceneHeight()
            {
                return sceneHeight;
            }

            public override int GetSceneWidth()
            {
                return sceneWidth;
            }

            public override bool IsRunning()
            {
                return isRunning;
            }

            public override void TurnOff()
            {
                isRunning = false;
            }

            public override void TurnOn()
            {
                isRunning = true;
            }
        }


        [TestMethod]
        public void logicAPITurnOnTurnOffTest()
        {
            FakeDataAPI fakeDataAPI = new FakeDataAPI();
            AbstractLogicAPI logicAPI = AbstractLogicAPI.CreateAPI(fakeDataAPI);
            logicAPI.CreateField(400, 400);
            Assert.AreEqual(false, logicAPI.IsRunning());

            logicAPI.TurnOn();
            Assert.AreEqual(true, logicAPI.IsRunning());

            logicAPI.TurnOff();
            Assert.AreEqual(false, logicAPI.IsRunning());
        }

        [TestMethod]
        public void logicAPICreateBallsTest()
        {
            FakeDataAPI fakeDataAPI = new FakeDataAPI();
            AbstractLogicAPI logicAPI = AbstractLogicAPI.CreateAPI(fakeDataAPI);
            logicAPI.CreateField(400, 400);
            logicAPI.CreateBalls(10, 10);
            Assert.IsTrue(10 == logicAPI.GetAllBalls().Count);
        }

    }
}

using Dane;
using System.Numerics;

namespace Testy
{
    [TestClass]
    public class DataAPITest
    {

        [TestMethod]
        public void dataAPIBallPOsitionTest()
        {
            AbstractDataAPI dataAPI = AbstractDataAPI.CreateApi();
            dataAPI.CreateBall(1, 10, 10);
            Assert.IsTrue(dataAPI.GetAllBalls().First().Position.X == 10);
            Assert.IsTrue(dataAPI.GetAllBalls().First().Position.Y == 10);
        }
        
        [TestMethod]
        public void DataAPIBallMovementTest()
        {
            AbstractDataAPI dataAPI = AbstractDataAPI.CreateApi();
            dataAPI.CreateBall(1, 10, 10);
            dataAPI.GetAllBalls().First().Movement = new Vector2(1, 1);


            Assert.IsTrue(dataAPI.GetAllBalls().First().Movement.X == 1);
            Assert.IsTrue(dataAPI.GetAllBalls().First().Movement.Y == 1);
        }


        [TestMethod]
        public void dataAPITurnOnTurnOffTest()
        {
            AbstractDataAPI dataAPI = AbstractDataAPI.CreateApi();
            dataAPI.CreateScene(400, 400);
            Assert.AreEqual(false, dataAPI.IsRunning());

            dataAPI.TurnOn();
            Assert.AreEqual(true, dataAPI.IsRunning());

            dataAPI.TurnOff();
            Assert.AreEqual(false, dataAPI.IsRunning());

        }
    }
}

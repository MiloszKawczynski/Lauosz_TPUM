using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dane
{
    internal class BallList
    {
        private List<IBall> _ballList;

        public BallList()
        {
            _ballList = new List<IBall>();
        }

        public void AddBall(Ball ball)
        {
            _ballList.Add(ball);
        }

        public List<IBall> GetAllBalls()
        {
            return _ballList;
        }

        public void ClearBalls()
        {
            foreach (IBall ball in _ballList)
            {
                ball.TurnOff();
            }
            _ballList.Clear();
        }

    }
}

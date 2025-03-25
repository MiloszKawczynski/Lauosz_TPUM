using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Metrics;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Dane
{
    public abstract class AbstractDataAPI
    {

        public abstract void CreateScene(int height, int width);
        public abstract List<IBall> GetAllBalls();
        public abstract void CreateBall(int id, int x, int y);
        public abstract int GetSceneWidth();
        public abstract int GetSceneHeight();
        public abstract void TurnOff();
        public abstract void TurnOn();
        public abstract bool IsRunning();

        public static AbstractDataAPI CreateApi()
        {
            return new DataAPI();
        }

        internal sealed class DataAPI : AbstractDataAPI
        {
            private BallList _ballList;
            private int sceneHeight;
            private int sceneWidth;
            private bool isRunning;
            private AbstractBallLogger logger;

            public DataAPI()
            {
                logger = AbstractBallLogger.CreateBallLoger();
                _ballList = new BallList();
                isRunning = false;
            }

            public override void CreateScene(int height, int width)
            {
                sceneHeight = height;
                sceneWidth = width;
            }
            public override List<IBall> GetAllBalls()
            {
                return _ballList.GetAllBalls();
            }
            public override void CreateBall(int id, int x, int y)
            {
                Ball ball = new Ball(id, x, y, logger);
                _ballList.AddBall(ball);
            }

            public override int GetSceneWidth()
            {
                return sceneWidth;
            }
            public override int GetSceneHeight()
            {
                return sceneHeight;
            }

            public override void TurnOff()
            {
                _ballList.ClearBalls();
                isRunning = false;
                logger.Dispose();
            }
            public override void TurnOn()
            {
                logger.writeSceneSizeToLogFile(sceneHeight, sceneWidth);
                logger.TurnOn();
                isRunning = true;
            }
            public override bool IsRunning()
            {
                return isRunning;
            }
        }
    }
}

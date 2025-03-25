

namespace Dane
{
    public abstract class AbstractBallLogger : IDisposable
    {
        public abstract void addBallToQueue(IBall ball);
        public abstract void writeSceneSizeToLogFile(int sceneHeigth, int sceneWidth);
        public static AbstractBallLogger CreateBallLoger()
        {
            return new BallLogger();
        }
        public abstract void Dispose();
        public abstract void TurnOn();

    }
}

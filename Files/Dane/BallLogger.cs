using Newtonsoft.Json.Linq;
using System.Collections.Concurrent;
using Newtonsoft.Json;
using System.Xml;
using System;

namespace Dane
{
    internal class BallLogger : AbstractBallLogger
    {
        private string _filePath;
        private bool isRunning;
        private ConcurrentQueue<IBall> _ballsQueue;
        private Mutex _writeMutex = new Mutex();
        private Mutex _enterQueueMutex = new Mutex();
        private CancellationTokenSource StateChange = new CancellationTokenSource();

        public BallLogger()
        {
            string path = Path.GetTempPath();
            _filePath = Path.Combine(path, "DataBallsLog.json");
            _ballsQueue = new ConcurrentQueue<IBall>();

            using (FileStream file = File.Create(_filePath))
            {
                file.Close();
            }

            Task.Run(writeDataToLogFile);
        }

        public override void writeSceneSizeToLogFile(int sceneHeigth, int sceneWidth)
        {
            JObject sceneSizeObject = new JObject();
            sceneSizeObject["Scene Height: "] = sceneHeigth;
            sceneSizeObject["Scene Width: "] = sceneWidth;
            sceneSizeObject["Time"] = DateTime.Now.ToString("HH:mm:ss");
            _writeMutex.WaitOne();
            try
            {
                File.AppendAllText(_filePath, JsonConvert.SerializeObject(sceneSizeObject, Newtonsoft.Json.Formatting.Indented));
            }
            finally
            {
                _writeMutex.ReleaseMutex();
            }
        }

        public override void addBallToQueue(IBall ball)
        {
            if (_ballsQueue.Count < 50)
            {
                _ballsQueue.Enqueue(ball);
                StateChange.Cancel();
            }
        }

        private async void writeDataToLogFile()
        {
            while (this.isRunning)
            {
                while (_ballsQueue.TryDequeue(out IBall? ball))
                {
                    string data = JsonConvert.SerializeObject(ball, Newtonsoft.Json.Formatting.Indented);
                    _writeMutex.WaitOne();
                    try
                    {
                        File.AppendAllText(_filePath, data);
                    }
                    finally
                    {
                        _writeMutex.ReleaseMutex();
                    }
                }
                await Task.Delay(Timeout.Infinite, StateChange.Token).ContinueWith(_ => { });

                if (this.StateChange.IsCancellationRequested)
                {
                    this.StateChange = new CancellationTokenSource();
                }
            }
        }

        public override void TurnOn()
        {
            isRunning = true;
        }

        public override void Dispose()
        {
            this.isRunning = false;
            _ballsQueue.Clear();
        }

        ~BallLogger()
        {
            this.Dispose();
        }
    }
}

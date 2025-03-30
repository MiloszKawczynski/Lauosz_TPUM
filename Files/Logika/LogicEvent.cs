using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logika
{
    public class LogicEvent
    {
        public ILogicBall ball;
        public LogicEvent(ILogicBall ball)
        {
            this.ball = ball;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Dane
{
    public class DataEvent
    {
        public IBall ball;
        public DataEvent(IBall ball)
        {
            this.ball = ball;
        }
    }
}


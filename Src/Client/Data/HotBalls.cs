using System.Collections.Generic;

namespace Lottery.Data
{
    public class HotBalls
    {
        public string DrawNumber { get; set; }
        public Dictionary<int, int> Balls { get; set; }
    }
}

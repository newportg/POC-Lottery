using System.Collections.Generic;

namespace Lottery.Data
{
    public class Ticket
    {
        public string DrawNumber { get; set; }
        public int[] Balls { get; set; }
        public int ThunderBall { get; set; }
        public int DrawTotal { get; set; }

    }

    public class TotalThunder
    {
        public int Count { get; set; }
        public List<int> Thunderball { get; set; }
    }
}

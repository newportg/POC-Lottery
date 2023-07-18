using System.Collections.Generic;

namespace Domain.Models
{
    public class Ticket
    {
        public string DrawNumber { get; set; }
        public int[] Balls { get; set; }
        public int ThunderBall { get; set; }
        public int DrawTotal { get; set; }

        public int BallMatch { get; set; }
        public int BonusBallMatch { get; set; }
    }
}

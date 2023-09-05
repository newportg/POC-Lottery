using System.Collections.Generic;

namespace Domain.Models
{
    public class HotBalls
    {
        public string DrawNumber { get; set; }
        public Dictionary<int, int> Balls { get; set; }
    }
}

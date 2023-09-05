using System;
using System.Collections.Generic;

namespace Domain.Models
{
    public class LotteryDto
    {
        public string Lottery { get; set; }
        public DateTime DrawDate { get; set; }
        public string BallSet { get; set; }
        public string Machine { get; set; }
        public string DrawNumber { get; set; }
        public List<int> Balls { get; set; }
        public List<int> BonusBalls { get; set; }
    }
}

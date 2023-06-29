using System;
using System.Collections.Generic;
using System.Linq;

namespace Domain.Models
{
    public class Lottery
    {
        public string Name { get; set; }
        public DateTime DrawDate { get; set; }
        public string BallSet { get; set; }
        public string Machine { get; set; }
        public string DrawNumber { get; set; }
        public List<int> Balls { get; set; }
        public List<int> BonusBalls { get; set; }

        // Analysis
        public int BallTotal { get; set; }
        public int NumOddBalls { get; set; }
        //public int[] RenatoGianellaPattern { get; set; }
        public int[] Delta { get; set; }

        public bool HasBeenDrawn(int[] ticket)
        {
            if (ticket.Contains<int>(Balls[0]) && ticket.Contains<int>(Balls[1]) && ticket.Contains<int>(Balls[2]) && ticket.Contains<int>(Balls[3]) && ticket.Contains<int>(Balls[4]))
            {
                return true;
            }
            return false;
        }
    }
}

using System.Collections.Generic;

namespace Lottery.Data
{
    public class DrawAnalysis
    {
        List<Lottery> Draws { get; set; }
        public int[,] RGspread { get; set; }
        public int[,] RGguess { get; set; }
    }
}

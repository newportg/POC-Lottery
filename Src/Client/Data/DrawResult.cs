using System.Collections.Generic;

namespace Lottery.Data
{
    public class DrawResult
    {
        public string DrawNumber { get; set; }
        public List<int> DrawBalls { get; set; }
        public int DrawThunderBall { get; set; }
        public List<GuessResult> GuessResults { get; set; }
        public int Prize { get; set; }

    }

    public class GuessResult
    {
        public GuessBall[] GuessBall { get; set; }
        public int Prize { get; set; }

    }

    public class GuessBall
    {
        public int Ball { get; set; }
        public bool Match { get; set; } = false;
        public bool Thunderball { get; set; } = false;
    }
}

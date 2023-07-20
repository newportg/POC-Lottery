using System.Collections.Generic;

namespace Domain.Models
{
    public class DrawResult
    {
        public DrawResult()
        {
            GuessResults = new List<GuessResult>();
        }
        public string DrawNumber { get; set; }
        public List<int> DrawBalls { get; set; }
        public int DrawThunderBall { get; set; }
        public List<GuessResult> GuessResults { get; set; }
        public int Win { get; set; }

    }

    public class GuessResult
    {
        public GuessResult()
        {
            GuessBall = new GuessBall[6];
        }
        public GuessBall[] GuessBall { get; set; }
        public int Win { get; set; }

    }

    public class GuessBall
    {
        public GuessBall(int ball, bool match, bool thunderball = false)
        {
            Ball = ball;
            Match = match;
            Thunderball = thunderball;
        }

        public int Ball { get; set; }
        public bool Match { get; set; } = false;
        public bool Thunderball { get; set; } = false;
    }
}


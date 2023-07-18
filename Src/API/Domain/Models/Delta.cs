using System.Collections.Generic;

namespace Domain.Models
{
    public class Delta
    {
        public List<List<int>> Balls { get; set; }
        public List<Range> Ranges { get; set; }

        public Delta()
        {
            Balls = new List<List<int>>();
            Ranges = new List<Range>();
        }

        public bool CreateRange(List<Lottery> lottery)
        {
            foreach (var draw in lottery)
            {
                Balls.Add(draw.Balls);
                if (Ranges.Count == 0)
                {
                    Ranges.Add(new Range());
                    Ranges.Add(new Range());
                    Ranges.Add(new Range());
                    Ranges.Add(new Range());
                    Ranges.Add(new Range());
                }

                Ranges[0].Update(draw.Balls[0]);
                Ranges[1].Update(draw.Balls[1]);
                Ranges[2].Update(draw.Balls[2]);
                Ranges[3].Update(draw.Balls[3]);
                Ranges[4].Update(draw.Balls[4]);
            }

            return false;
        }
    }

    public class Range
    {
        public int Min { get; set; } = 39;
        public int Max { get; set; } = 0;

        public bool Update(int num)
        {
            if (num < Min) Min = num;
            if (num > Max) Max = num;

            return true;
        }

    }
}

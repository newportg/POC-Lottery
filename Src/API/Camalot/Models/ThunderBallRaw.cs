using CsvHelper.Configuration.Attributes;

namespace Camalot.Models
{
    public class ThunderBallRaw
    {
        [Name("DrawNumber")]
        public string DrawNumber { get; set; }

        [Name("DrawDate")]
        public string DrawDate { get; set; }

        [Name("Ball 1")]
        public string Ball1 { get; set; }

        [Name("Ball 2")]
        public string Ball2 { get; set; }

        [Name("Ball 3")]
        public string Ball3 { get; set; }

        [Name("Ball 4")]
        public string Ball4 { get; set; }

        [Name("Ball 5")]
        public string Ball5 { get; set; }

        [Name("Thunderball")]
        public string Thunderball { get; set; }

        [Name("Ball Set")]
        public string BallSet { get; set; }

        [Name("Machine")]
        public string Machine { get; set; }
    }
}

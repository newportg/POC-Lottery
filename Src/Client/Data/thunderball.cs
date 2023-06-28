using Microsoft.WindowsAzure.Storage.Table;

namespace Lottery.Data
{
    public class ThunderBall : TableEntity
    {
        public string DrawDate { get; set; }

        public string Ball1 { get; set; }

        public string Ball2 { get; set; }

        public string Ball3 { get; set; }

        public string Ball4 { get; set; }

        public string Ball5 { get; set; }

        public string Thunderball { get; set; }

        public string BallSet { get; set; }

        public string Machine { get; set; }

        public string DrawNumber { get; set; }
    }
}
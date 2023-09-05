using Newtonsoft.Json;
using System.Collections.Generic;

namespace Domain.Models
{
    public class NeuLotteryDto
    {
        [JsonProperty(PropertyName = "Name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "Draw")]
        public DrawEntity Draw { get; set; }
    }

    public class DrawEntity
    {
        [JsonProperty(PropertyName = "DrawDate")]
        public string DrawDate { get; set; }

        [JsonProperty(PropertyName = "DrawNumber")]
        public string DrawNumber { get; set; }

        [JsonProperty(PropertyName = "BallSet")]
        public string BallSet { get; set; }

        [JsonProperty(PropertyName = "Machine")]
        public string Machine { get; set; }

        [JsonProperty(PropertyName = "Balls")]
        public BallEntity Balls { get; set; }

        public Analysis Analysis { get; set; }

        [JsonProperty(PropertyName = "Guesses")]
        public List<Guess> Guesses { get; set; }
    }

    public class BallEntity
    {
        [JsonProperty(PropertyName = "MainBalls")]
        public List<Ball> MainBalls { get; set; }

        [JsonProperty(PropertyName = "BonusBalls")]
        public List<Ball> BonusBalls { get; set; }
    }
}

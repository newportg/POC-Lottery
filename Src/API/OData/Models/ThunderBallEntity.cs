using Newtonsoft.Json;

namespace OData.Models
{
    public class ThunderBallEntity : TableEntity
    {
        public ThunderBallEntity()
        {
            PartitionKey = "Thunderball";
        }

        [JsonProperty(PropertyName = "Ball1")]
        public string Ball1 { get; set; }

        [JsonProperty(PropertyName = "Ball2")]
        public string Ball2 { get; set; }

        [JsonProperty(PropertyName = "Ball3")]
        public string Ball3 { get; set; }

        [JsonProperty(PropertyName = "Ball4")]
        public string Ball4 { get; set; }

        [JsonProperty(PropertyName = "Ball5")]
        public string Ball5 { get; set; }

        [JsonProperty(PropertyName = "BallSet")]
        public string BallSet { get; set; }

        [JsonProperty(PropertyName = "DrawDate")]
        public string DrawDate { get; set; }

        [JsonProperty(PropertyName = "DrawNumber")]
        public string DrawNumber { get; set; }

        [JsonProperty(PropertyName = "Machine")]
        public string Machine { get; set; }

        [JsonProperty(PropertyName = "Thunderball")]
        public string Thunderball { get; set; }
    }
}

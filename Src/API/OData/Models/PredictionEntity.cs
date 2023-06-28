using Newtonsoft.Json;

namespace OData.Models
{
    public class PredictionEntity : TableEntity
    {
        public PredictionEntity()
        {
            PartitionKey = "Predictions";
        }

        [JsonProperty(PropertyName = "DrawNumber")]
        public string DrawNumber { get; set; }

        [JsonProperty(PropertyName = "DrawDate")]
        public string DrawDate { get; set; }

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
    }
}

using Library.Azure.Odata.Models;
using Newtonsoft.Json;

namespace Domain.Models
{
    public class HotBallsEntity : TableEntity
    {
        public HotBallsEntity()
        {
            PartitionKey = "hotballs";
        }

        [JsonProperty(PropertyName = "DrawNumber")]
        public string DrawNumber { get; set; }

        [JsonProperty(PropertyName = "Balls")]
        public string Balls { get; set; }

    }
}

using Library.Azure.Odata.Models;
using Newtonsoft.Json;

namespace Domain.Models
{
    public class NeuLotteryEntity : TableEntity
    {
        public NeuLotteryEntity()
        {
            PartitionKey = "Lottery";
        }

        [JsonProperty(PropertyName = "Name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "Draw")]
        public string Draw { get; set; }
    }
}

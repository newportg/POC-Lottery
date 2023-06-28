using Newtonsoft.Json;

namespace OData.Models
{
    public class TableEntity
    {
        [JsonProperty(PropertyName = "odata.etag", DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string ETag { get; set; }

        [JsonProperty(PropertyName = "PartitionKey")]
        public string PartitionKey { get; set; }

        [JsonProperty(PropertyName = "RowKey")]
        public string RowKey { get; set; }

        [JsonProperty(PropertyName = "Timestamp", DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Timestamp { get; set; }
    }
}

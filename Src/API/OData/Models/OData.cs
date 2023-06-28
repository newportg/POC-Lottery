using Newtonsoft.Json;
using System.Collections.Generic;

namespace OData.Models
{
    public class OData
    {
        [JsonProperty(PropertyName = "odata.metadata")]
        public string odata { get; set; }
        [JsonProperty(PropertyName = "value")]
        public dynamic value { get; set; }
    }
}

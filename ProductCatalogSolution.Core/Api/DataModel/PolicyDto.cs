using Newtonsoft.Json;

namespace ProductCatalogSolution.Core.Api.DataModel
{
    public class PolicyDto
    {
        [JsonProperty("min")]
        public int Min { get; set; }

        [JsonProperty("discount")]
        public double Discount { get; set; }
    }
}

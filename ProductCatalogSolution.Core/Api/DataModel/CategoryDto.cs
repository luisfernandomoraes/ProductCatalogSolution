using Newtonsoft.Json;

namespace ProductCatalogSolution.Core.Api.DataModel
{
    public class CategoryDto
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }
}

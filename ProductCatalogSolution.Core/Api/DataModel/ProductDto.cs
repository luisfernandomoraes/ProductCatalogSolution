using Newtonsoft.Json;

namespace ProductCatalogSolution.Core.Api.DataModel
{
    public class ProductDto
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("photo")]
        public string Photo { get; set; }

        [JsonProperty("price")]
        public double Price { get; set; }

        [JsonProperty("category_id")]
        public int? CategoryId { get; set; }
    }
}

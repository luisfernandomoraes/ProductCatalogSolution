using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProductCatalogSolution.Core.Api.DataModel
{
    public class PromotionDto
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("category_id")]
        public int CategoryId { get; set; }

        [JsonProperty("policies")]
        public IList<PolicyDto> Policies { get; set; }
    }
}

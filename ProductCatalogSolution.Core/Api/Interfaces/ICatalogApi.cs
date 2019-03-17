using System.Collections.Generic;
using System.Threading.Tasks;
using ProductCatalogSolution.Core.Api.DataModel;
using Refit;

namespace ProductCatalogSolution.Core.Api.Interfaces
{
    public interface ICatalogApi
    {
        [Get("/YNR2rsWe")]
        Task<IList<CategoryDto>> GetCategoriesAsync();

        [Get("/R9cJFBtG")]
        Task<IList<PromotionDto>> GetPromotionsAsync();

        [Get("/eVqp7pfX")]
        Task<IList<ProductDto>> GetProductsAsync();
    }
}

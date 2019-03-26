using ProductCatalogSolution.Core.Api.Interfaces;
using System.Collections.Generic;
using Refit;
using System.Net.Http;
using System.Threading.Tasks;
using ProductCatalogSolution.Core.Api.DataModel;

namespace ProductCatalogSolution.Core.Api.Services
{

    public class CatalogApiService : ICatalogApi
    {
        private readonly ICatalogApi _api;

        public CatalogApiService()
        {
            _api = RestService.For<ICatalogApi>("http://pastebin.com/raw");
        }

        public CatalogApiService(HttpClient client)
        {
            _api = RestService.For<ICatalogApi>(client);
        }

        public async Task<IList<CategoryDto>> GetCategoriesAsync()
        {
            return await _api.GetCategoriesAsync();
        }

        public async Task<IList<PromotionDto>> GetPromotionsAsync()
        {
            return await _api.GetPromotionsAsync();
        }

        public async Task<IList<ProductDto>> GetProductsAsync()
        {
            return await _api.GetProductsAsync();
        }
    }
}

using ProductCatalogSolution.Core.Api.DataModel;
using ProductCatalogSolution.Core.Api.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductCatalogSolution.Core.Models
{
    /// <summary>
    /// TODO: Refactor.
    /// </summary>
    public class Catalog
    {
        private readonly ICatalogApi _catalogApi;
        private IList<ProductDto> _apiProductsList;
        private IList<PromotionDto> _apiPromotionsList;
        private IList<CategoryDto> _apiCategoriesList;

        public IList<Category> Categories { get; }
        public IList<ProductCollection> ProductCollections { get; }

        public Catalog(ICatalogApi catalogApi)
        {
            _catalogApi = catalogApi;
            Categories = new List<Category>();
            ProductCollections = new List<ProductCollection>();
        }

        public async Task<IList<ProductCollection>> LoadCatalogDataAsync()
        {
            await LoadApiDataAsync();
            PopulateCategories();
            PopulateProductCollections();
            return GetProducts();
        }

        private async Task LoadApiDataAsync()
        {
            _apiProductsList = await GetApiProductsAsync();
            _apiPromotionsList = await GetApiPromotionsAsync();
            _apiCategoriesList = await GetApiCategoriesAsync();
        }

        private async Task<IList<ProductDto>> GetApiProductsAsync()
        {
            return await _catalogApi.GetProductsAsync();
        }

        private async Task<IList<PromotionDto>> GetApiPromotionsAsync()
        {
            return await _catalogApi.GetPromotionsAsync();
        }

        private async Task<IList<CategoryDto>> GetApiCategoriesAsync()
        {
            return await _catalogApi.GetCategoriesAsync();
        }

        private void PopulateCategories()
        {
            ClearCurrentCategories();
            AddApiCategoriesToMobileCategories();
        }

        private void ClearCurrentCategories()
        {
            Categories.Clear();
        }

        private void AddApiCategoriesToMobileCategories()
        {
            foreach (var apiCategory in _apiCategoriesList)
            {
                Categories.Add(new Category(apiCategory));
            }
        }

        private void PopulateProductCollections()
        {
            ClearCurrentGroups();
            AddPromotionsToAGroupOfProducts();
            AddProductsWithoutPromotionsToAGroupOfProducts();
        }

        private void ClearCurrentGroups()
        {
            ProductCollections.Clear();
        }

        private void AddPromotionsToAGroupOfProducts()
        {
            foreach (var apiPromotion in _apiPromotionsList)
            {
                var apiCategory = GetApiCategoryById(apiPromotion.CategoryId);
                var productsOfGroup = GetMobileProductsOfGroupByPromotionAndCategory(apiPromotion, apiCategory);
                var groupName = apiPromotion.Name;

                var productGroup = new ProductCollection(groupName, productsOfGroup);
                ProductCollections.Add(productGroup);
            }
        }

        private CategoryDto GetApiCategoryById(int id)
        {
            return _apiCategoriesList.Single(e => e.Id == id);
        }

        private Category GetMobileCategoryByApiCategory(CategoryDto apiCategory)
        {
            return new Category(apiCategory);
        }

        private IList<Product> GetMobileProductsOfGroupByPromotionAndCategory(
            PromotionDto apiPromotion,
            CategoryDto apiCategory
        )
        {
            var productsOfParameterCategory = _apiProductsList
                .Where(e => e.CategoryId == apiPromotion.CategoryId)
                .ToList();

            var products = new List<Product>();

            foreach (var apiProduct in productsOfParameterCategory)
            {
                var mobileProduct = GetMobileProductByParameters(apiProduct, apiPromotion, apiCategory);
                products.Add(mobileProduct);
            }

            return products;
        }

        private Product GetMobileProductByParameters(
            ProductDto apiProduct,
            PromotionDto apiPromotion,
            CategoryDto apiCategory
        )
        {
            return new Product(apiProduct, apiPromotion, apiCategory);
        }

        private void AddProductsWithoutPromotionsToAGroupOfProducts()
        {
            var productsWithoutPromotion = GetApiProductsWithoutPromotion();
            if (CheckIfApiProductsAreEmpty(productsWithoutPromotion)) return;

            var productsOfGroup = GetProductsOfGroupByProductsWithoutPromotion(productsWithoutPromotion);
            // TODO: Refactor
            var productGroup = new ProductCollection("Confira também", productsOfGroup);
            ProductCollections.Add(productGroup);
        }

        private IList<ProductDto> GetApiProductsWithoutPromotion()
        {
            var categoriesId = GetApiCategoriesIdThatContainsPromotion();
            var productsWithPromotion = GetApiProductsThatContainsPromotionByCategoriesId(categoriesId);
            return GetApiProductsThatNotContainsPromotionDisregardingParametrizedProducts(productsWithPromotion);
        }

        private bool CheckIfApiProductsAreEmpty(IList<ProductDto> products)
        {
            return products.Count == 0;
        }

        private IList<int> GetApiCategoriesIdThatContainsPromotion()
        {
            return _apiPromotionsList.Select(e => e.CategoryId).ToList();
        }

        private IList<ProductDto> GetApiProductsThatContainsPromotionByCategoriesId(IList<int> categoriesId)
        {
            return _apiProductsList.Where(
                e => e.CategoryId.HasValue && categoriesId.Contains(e.CategoryId.Value)
            ).ToList();
        }

        private IList<ProductDto> GetApiProductsThatNotContainsPromotionDisregardingParametrizedProducts(IList<ProductDto> products)
        {
            return _apiProductsList.Except(products).ToList();
        }

        private IList<Product> GetProductsOfGroupByProductsWithoutPromotion(IList<ProductDto> productsWithoutPromotion)
        {
            var products = new List<Product>();

            foreach (var apiProduct in productsWithoutPromotion)
            {
                var product = GetMobileProductByApiProduct(apiProduct);
                products.Add(product);
            }

            return products;
        }

        private Product GetMobileProductByApiProduct(ProductDto apiProduct)
        {
            if (IsApiProductContainsCategory(apiProduct))
            {
                var apiCategory = GetApiCategoryById(apiProduct.CategoryId.Value);
                return new Product(apiProduct, apiCategory);
            }

            return new Product(apiProduct);
        }

        private bool IsApiProductContainsCategory(ProductDto product)
        {
            return product.CategoryId.HasValue;
        }

        public IList<ProductCollection> GetProducts()
        {
            return ProductCollections;
        }

        public IList<ProductCollection> GetProductsByCategoryId(int categoryId)
        {
            var filteredResults = new List<ProductCollection>();

            foreach (var group in ProductCollections)
            {
                var products = new List<Product>();

                foreach (var product in group)
                {
                    if (product.Category?.Id != categoryId) continue;

                    products.Add(product);
                }

                if (products.Count == 0) continue;

                filteredResults.Add(new ProductCollection(group.Name, products));
            }

            return filteredResults;
        }

        public Product GetProductById(int id)
        {
            foreach (var group in ProductCollections)
            {
                foreach (var product in group)
                {
                    if (product.Id.Equals(id)) return product;
                }
            }

            throw new Exception($"O produto do código {id} não foi encontrado.");
        }
    }
}

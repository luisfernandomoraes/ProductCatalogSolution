using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ProductCatalogSolution.Core.Models
{
    public class Store
    {
        private Catalog _catalog { get; set; }
        private Cart _cart { get; set; }

        public Store(Catalog catalog, Cart cart)
        {
            _catalog = catalog;
            _cart = cart;
        }

        public async Task<IList<ProductCollection>> LoadCatalogDataAsync()
        {
            return await _catalog.LoadCatalogDataAsync();
        }

        public IList<ProductCollection> GetProducts()
        {
            return _catalog.GetProducts();
        }

        public IList<ProductCollection> GetProductsByCategoryId(int categoryId)
        {
            return _catalog.GetProductsByCategoryId(categoryId);
        }

        public IList<Category> GetProductsCategory()
        {
            return _catalog.Categories;
        }

        public void IncreaseProductQuantity(Product product)
        {
            product.IncreaseQuantity();
            _cart.ManageProduct(product);
        }

        public void DecreaseProductQuantity(Product product)
        {
            product.DecreaseQuantity();
            _cart.ManageProduct(product);
        }

        public double GetTotalPriceCart()
        {
            return _cart.GetTotalPrice();
        }

        public void ToggleFavoriteProduct(Product product)
        {
            product.ToggleFavorite();
        }

        public bool HasProductsInCart()
        {
            return _cart.HasProducts();
        }

        public Product GetProductById(int id)
        {
            return _catalog.GetProductById(id);
        }
    }
}

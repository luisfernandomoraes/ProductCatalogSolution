using System;
using System.Reactive.Linq;
using Akavache;
using System.Threading.Tasks;
using ProductCatalogSolution.Core.Models;
using ProductCatalogSolution.Core.Interfaces;

namespace ProductCatalogSolution.Core.Services
{
    public class AcavacheCacheService : ICacheService
    {
        private const string FAVORITE_PRODUCT_KEY = "favorite_product_id_{0}";

        public AcavacheCacheService()
        {
            BlobCache.ApplicationName = "XTProductCatalog";
        }

        public async Task SaveFavoriteProductAsync(Product product)
        {
            if (product.IsFavorite)
            {
                await SaveFavoriteProductByIdAsync(product.Id);
                return;
            }

            RemoveFavoriteProductById(product.Id);
        }

        private async Task SaveFavoriteProductByIdAsync(int id)
        {
            try
            {
                await BlobCache.LocalMachine.InsertObject(
                    string.Format(FAVORITE_PRODUCT_KEY, id),
                    true
                );
            }
            catch (Exception exception)
            {
                System.Diagnostics.Debug.WriteLine(exception.Message);
            }
        }

        private void RemoveFavoriteProductById(int id)
        {
            BlobCache.LocalMachine.InvalidateObject<bool>(string.Format(FAVORITE_PRODUCT_KEY, id));
        }

        public void LoadFromCacheIfIsFavoriteProduct(Product product)
        {
            BlobCache.LocalMachine
                     .GetObject<bool>(string.Format(FAVORITE_PRODUCT_KEY, product.Id))
                     .Subscribe(
                         cache => product.IsFavorite = cache,
                         exception => product.IsFavorite = false);
        }

        public async Task ClearCacheAsync()
        {
            await BlobCache.LocalMachine.InvalidateAll();
        }
    }
}

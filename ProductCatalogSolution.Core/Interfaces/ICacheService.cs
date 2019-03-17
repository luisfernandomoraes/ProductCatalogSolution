using ProductCatalogSolution.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProductCatalogSolution.Core.Interfaces
{
    public interface ICacheService
    {
        System.Threading.Tasks.Task SaveFavoriteProductAsync(Product product);
        void LoadFromCacheIfIsFavoriteProduct(Product product);
        System.Threading.Tasks.Task ClearCacheAsync();
    }
}

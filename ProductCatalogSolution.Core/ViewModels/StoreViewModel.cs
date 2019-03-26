using ProductCatalogSolution.Core.Helpers;
using ProductCatalogSolution.Core.Interfaces;
using ProductCatalogSolution.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;


namespace ProductCatalogSolution.Core.ViewModels
{
    public class StoreViewModel
    {
        private readonly INavigationService _navigationService;
        private readonly ICacheService _cacheService;
        private readonly Store _store;

        public delegate void CatalogDataLoadFailDelegate(Exception exception);
        public delegate void CatalogDataLoadDelegate(IList<ProductCollection> products);
        public delegate void ProductCategoryLoadDelegate(IList<Category> categories);
        public delegate void ProductUpdateDelegate(Product product);
        public delegate void ProductsCartUpdateDelegate(bool hasProductsInCart);
        public delegate void TotalPriceUpdateDelegate(double totalPrice);

        public event CatalogDataLoadFailDelegate OnCatalogDataLoadFail;
        public event CatalogDataLoadDelegate OnCatalogDataLoad;
        public event ProductCategoryLoadDelegate OnProductCategoryLoad;
        public event ProductUpdateDelegate OnProductUpdate;
        public event ProductsCartUpdateDelegate OnProductsCartUpdate;
        public event TotalPriceUpdateDelegate OnTotalPriceUpdate;

        public ICommand LoadCatalogDataCommand { get; }
        public ICommand DecreaseProductQuantityCommand { get; }
        public ICommand IncreaseProductQuantityCommand { get; }
        public ICommand ToggleFavoriteCommand { get; }
        public ICommand GetProductsByCategoryIdCommand { get; }
        public ICommand GetProductsCommand { get; }
        public ICommand NavigateToCartCommand { get; }
        public ICommand NavigateToDetailByProductIdCommand { get; }
        public ICommand UpdateCartDataCommand { get; }

        public StoreViewModel(INavigationService navigationService,
                              ICacheService cacheService,
                              Store store)
        {
            _navigationService = navigationService;
            _cacheService = cacheService;
            _store = store;

            LoadCatalogDataCommand = new RelayCommand(
                async () => await LoadCatalogDataAsync()
            );
            DecreaseProductQuantityCommand = new RelayCommand<Product>(
                DecreaseProductQuantity,
                IsProductQuantityGreaterThanZero
            );
            IncreaseProductQuantityCommand = new RelayCommand<Product>(
                IncreaseProductQuantity
            );
            ToggleFavoriteCommand = new RelayCommand<Product>(
                async (product) => await ToggleFavoriteAsync(product)
            );
            GetProductsByCategoryIdCommand = new RelayCommand<int>(
                GetProductsByCategoryId
            );
            GetProductsCommand = new RelayCommand(
                GetProducts
            );
            NavigateToCartCommand = new RelayCommand(
                NavigateToCart
            );
            NavigateToDetailByProductIdCommand = new RelayCommand<int>(
                NavigateToDetailByProductId
            );
            UpdateCartDataCommand = new RelayCommand(
                UpdateCartData
            );
        }

        private async Task LoadCatalogDataAsync()
        {
            try
            {
                var products = await _store.LoadCatalogDataAsync();
                OnCatalogDataLoad?.Invoke(products);

                var categories = _store.GetProductsCategory();
                OnProductCategoryLoad?.Invoke(categories);

                CheckIfProductsIsFavorites(products);
            }
            catch (Exception exception)
            {
                OnCatalogDataLoadFail?.Invoke(exception);
            }
        }

        private void CheckIfProductsIsFavorites(IList<ProductCollection> productsGroup)
        {
            foreach (var group in productsGroup)
            {
                foreach (var product in group.ToList())
                {
                    _cacheService.LoadFromCacheIfIsFavoriteProduct(product);
                }
            }
        }

        private void DecreaseProductQuantity(Product product)
        {
            _store.DecreaseProductQuantity(product);
            OnProductUpdate?.Invoke(product);
            UpdateCartData();
        }

        private bool IsProductQuantityGreaterThanZero(Product product)
        {
            return product.Quantity > 0;
        }

        private void IncreaseProductQuantity(Product product)
        {
            _store.IncreaseProductQuantity(product);
            OnProductUpdate?.Invoke(product);
            UpdateCartData();
        }

        private async Task ToggleFavoriteAsync(Product product)
        {
            _store.ToggleFavoriteProduct(product);
            OnProductUpdate?.Invoke(product);
            await _cacheService.SaveFavoriteProductAsync(product);
        }

        private void GetProductsByCategoryId(int categoryId)
        {
            var products = _store.GetProductsByCategoryId(categoryId);
            OnCatalogDataLoad?.Invoke(products);
        }

        private void GetProducts()
        {
            var products = _store.GetProducts();
            OnCatalogDataLoad?.Invoke(products);
        }

        private void NavigateToCart()
        {
            _navigationService.NavigateToCart();
        }

        private void NavigateToDetailByProductId(int id)
        {
            _navigationService.NavigateToDetailByProductId(id);
        }

        private void UpdateCartData()
        {
            var hasProductsInCart = _store.HasProductsInCart();
            var totalPriceCart = _store.GetTotalPriceCart();

            OnProductsCartUpdate?.Invoke(hasProductsInCart);
            OnTotalPriceUpdate?.Invoke(totalPriceCart);
        }
    }
}

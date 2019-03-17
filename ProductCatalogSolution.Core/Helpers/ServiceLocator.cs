using ProductCatalogSolution.Core.Api.Services;
using ProductCatalogSolution.Core.Interfaces;
using ProductCatalogSolution.Core.Models;
using ProductCatalogSolution.Core.ViewModels;
using System;

//TODO: MvvmCross?
//TODO: Refactor
namespace ProductCatalogSolution.Core.Helpers
{
    /// <summary>
    /// TODO: To refactor, this shit. 
    /// </summary>
    public class ServiceLocator
    {
        public static ServiceLocator _instance;
        public static ServiceLocator Instance => _instance ?? (_instance = new ServiceLocator());
        public static void Reset() => _instance = null;

        private ServiceLocator() { }

        private StoreViewModel _storeViewModel;
        public StoreViewModel ResolveStoreViewModel()
        {
            if (_storeViewModel == null)
            {
                var store = ResolveStore();
                var navigationService = ResolveNavigationService();
                var cacheService = ResolveCacheService();

                _storeViewModel = new StoreViewModel(navigationService, cacheService, store);
            }

            return _storeViewModel;
        }

        private ProductDetailViewModel _productDetailViewModel;
        public ProductDetailViewModel ResolveProductDetailViewModel()
        {
            if (_productDetailViewModel == null)
            {
                var cacheService = ResolveCacheService();
                var store = ResolveStore();

                _productDetailViewModel = new ProductDetailViewModel(cacheService, store);
            }

            return _productDetailViewModel;
        }

        private Cart _cart;
        private Cart ResolveCart()
        {
            if (_cart == null)
            {
                _cart = new Cart();
            }

            return _cart;
        }
        private Store _store;
        private Store ResolveStore()
        {
            if (_store == null)
            {
                var catalogApi = new CatalogApiService();
                var catalog = new Catalog(catalogApi);
                var cart = ResolveCart();
                _store = new Store(catalog, cart);
            }

            return _store;
        }

        private CartViewModel _cartViewModel;
        public CartViewModel ResolveCartViewModel()
        {
            if (_cartViewModel == null)
            {
                var cart = ResolveCart();
                _cartViewModel = new CartViewModel(cart);
            }

            return _cartViewModel;
        }

        private INavigationService _navigationService;
        public INavigationService ResolveNavigationService()
        {
            if (_navigationService == null)
            {
                throw new ArgumentNullException(nameof(_navigationService), "The dependency has not been registered.");
            }

            return _navigationService;
        }

        public void RegisterNavigationService(INavigationService navigationService)
        {
            _navigationService = navigationService;
        }

        private ICacheService _cacheService;
        public ICacheService ResolveCacheService()
        {
            if (_cacheService == null)
            {
                throw new ArgumentNullException(nameof(_cacheService), "The dependency has not been registered.");
            }

            return _cacheService;
        }

        public void RegisterCacheService(ICacheService cacheService)
        {
            _cacheService = cacheService;
        }
    }
}

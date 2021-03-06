﻿using ProductCatalogSolution.Core.Helpers;
using ProductCatalogSolution.Core.Interfaces;
using ProductCatalogSolution.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using MvvmCross.Core.ViewModels;

namespace ProductCatalogSolution.Core.ViewModels
{
    public class ProductDetailViewModel:MvxViewModel<Product>
    {
        private readonly ICacheService _cacheService;
        private readonly Store _store;

        public delegate void ProductLoadDelegate(Product product);

        public event ProductLoadDelegate OnProductLoad;

        public ICommand GetProductByIdCommand { get; }
        public ICommand ToggleFavoriteCommand { get; }
        public ICommand DecreaseProductQuantityCommand { get; }
        public ICommand IncreaseProductQuantityCommand { get; }

        public ProductDetailViewModel(ICacheService cacheService,
                                      Store store)
        {
            _cacheService = cacheService;
            _store = store;

            GetProductByIdCommand = new RelayCommand<int>(
                GetProductById
            );
            ToggleFavoriteCommand = new RelayCommand<Product>(
                async (product) => await ToggleFavoriteAsync(product)
            );
            DecreaseProductQuantityCommand = new RelayCommand<Product>(
                DecreaseProductQuantity,
                IsProductQuantityGreaterThanZero
            );
            IncreaseProductQuantityCommand = new RelayCommand<Product>(
                IncreaseProductQuantity
            );
        }

        public override void Prepare(Product parameter)
        {
            GetProductById(parameter.Id);
        }

        private void GetProductById(int id)
        {
            try
            {
                var product = _store.GetProductById(id);
                OnProductLoad?.Invoke(product);
            }
            catch (Exception exception)
            {
                System.Diagnostics.Debug.WriteLine(exception.Message);
            }
        }

        private async Task ToggleFavoriteAsync(Product product)
        {
            _store.ToggleFavoriteProduct(product);
            OnProductLoad?.Invoke(product);
            await _cacheService.SaveFavoriteProductAsync(product);
        }

        private void DecreaseProductQuantity(Product product)
        {
            _store.DecreaseProductQuantity(product);
            OnProductLoad?.Invoke(product);
        }

        private bool IsProductQuantityGreaterThanZero(Product product)
        {
            return product.Quantity > 0;
        }

        private void IncreaseProductQuantity(Product product)
        {
            _store.IncreaseProductQuantity(product);
            OnProductLoad?.Invoke(product);
        }

        
    }
}

using ProductCatalogSolution.Core.Helpers;
using ProductCatalogSolution.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace ProductCatalogSolution.Core.ViewModels
{
  
    public class CartViewModel
    {
        private readonly Cart _cart;

        public delegate void ProductsCartLoadDelegate(IList<Product> products);
        public delegate void TotalOfUnitsLoadDelegate(int totalOfUnits);
        public delegate void TotalPriceLoadDelegate(double totalPrice);

        public event ProductsCartLoadDelegate OnProductsCartLoad;
        public event TotalOfUnitsLoadDelegate OnTotalOfUnitsLoad;
        public event TotalPriceLoadDelegate OnTotalPriceLoad;

        public ICommand GetProductsCommand { get; }
        public ICommand GetTotalOfUnitsCommand { get; }
        public ICommand GetTotalPriceCommand { get; }

        public CartViewModel(Cart cart)
        {
            _cart = cart;

            GetProductsCommand = new RelayCommand(
                GetProducts
            );
            GetTotalOfUnitsCommand = new RelayCommand(
                GetTotalOfUnits
            );
            GetTotalPriceCommand = new RelayCommand(
                GetTotalPrice
            );
        }

        private void GetProducts()
        {
            var products = _cart.GetProducts();
            OnProductsCartLoad?.Invoke(products);
        }

        private void GetTotalOfUnits()
        {
            var totalOfUnits = _cart.GetTotalOfUnits();
            OnTotalOfUnitsLoad?.Invoke(totalOfUnits);
        }

        private void GetTotalPrice()
        {
            var totalPrice = _cart.GetTotalPrice();
            OnTotalPriceLoad?.Invoke(totalPrice);
        }
    }
}

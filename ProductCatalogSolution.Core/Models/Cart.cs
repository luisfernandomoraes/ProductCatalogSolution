using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace ProductCatalogSolution.Core.Models
{
    
    public class Cart
    {
        private List<Product> _products { get; }

        public Cart()
        {
            _products = new List<Product>();
        }

        public IList<Product> GetProducts()
        {
            return _products;
        }

        public void ManageProduct(Product product)
        {
            if (IsQuantityZero(product))
            {
                RemoveProduct(product);
                return;
            }

            if (IsAlreadyInCart(product))
            {
                UpdateProduct(product);
                return;
            }

            AddProduct(product);
        }

        private bool IsQuantityZero(Product product)
        {
            return product.Quantity.Equals(0);
        }

        private void RemoveProduct(Product product)
        {
            _products.Remove(product);
        }

        private bool IsAlreadyInCart(Product product)
        {
            return GetProductById(product.Id) != null;
        }

        private Product GetProductById(int id)
        {
            return _products.SingleOrDefault(e => e.Id == id);
        }

        private void AddProduct(Product product)
        {
            _products.Add(product);
        }

        private void UpdateProduct(Product product)
        {
            var productAlreadyAdded = GetProductById(product.Id);
            var index = _products.IndexOf(productAlreadyAdded);
            _products[index] = product;
        }

        public double GetTotalPrice()
        {
            return _products.Sum(product => product.GetTotalPrice());
        }

        public int GetTotalOfUnits()
        {
            return _products.Sum(product => product.Quantity);
        }

        public bool HasProducts()
        {
            return _products.Count > 0;
        }
    }
}

using ProductCatalogSolution.Core.Api.DataModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProductCatalogSolution.Core.Models
{


    public class Product
    {
        public int Id { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public string Photo { get; private set; }
        public double CurrentPrice { get; private set; }
        public double OriginalPrice { get; private set; }
        public int Quantity { get; private set; }
        public bool IsFavorite { get; set; }
        public double Discount { get; private set; }
        public Promotion Promotion { get; private set; }
        public Category Category { get; private set; }

        public Product(ProductDto product)
        {
            InitializeProduct(product);
        }

        public Product(ProductDto product,
                       CategoryDto category)
        {
            InitializeProduct(product);
            InitializeCategory(category);
        }

        public Product(ProductDto product,
                       PromotionDto promotion,
                       CategoryDto category)
        {
            InitializeProduct(product);
            InitializePromotion(promotion);
            InitializeCategory(category);
        }

        private void InitializeProduct(ProductDto product)
        {
            Id = product.Id;
            Name = product.Name;
            Description = product.Description;
            Photo = product.Photo;
            CurrentPrice = OriginalPrice = product.Price;
            Quantity = 0;
            IsFavorite = false;
            Discount = 0;
        }

        private void InitializePromotion(PromotionDto promotion)
        {
            Promotion = new Promotion(promotion);
        }

        private void InitializeCategory(CategoryDto category)
        {
            Category = new Category(category);
        }

        public void IncreaseQuantity()
        {
            Quantity++;
            UpdateDiscountPercentageAndCurrentPriceIfHasPromotion();
        }

        public void DecreaseQuantity()
        {
            Quantity--;
            UpdateDiscountPercentageAndCurrentPriceIfHasPromotion();
        }

        private void UpdateDiscountPercentageAndCurrentPriceIfHasPromotion()
        {
            if (DontHavePromotion())
            {
                return;
            }

            var discount = GetDiscountCanBeApplied();
            UpdateCurrentPriceByDiscount(discount);
            UpdateDiscountPercentageByDiscount(discount);
        }

        private bool DontHavePromotion()
        {
            return Promotion == null;
        }

        private double GetDiscountCanBeApplied()
        {
            return Promotion.GetMaxDiscountThatCanBeAppliedByQuantity(Quantity);
        }

        private void UpdateCurrentPriceByDiscount(double discount)
        {
            CurrentPrice = OriginalPrice - (OriginalPrice * discount / 100);
        }

        private void UpdateDiscountPercentageByDiscount(double discount)
        {
            Discount = discount;
        }

        public void ToggleFavorite()
        {
            IsFavorite = !IsFavorite;
        }

        public double GetTotalPrice()
        {
            return CurrentPrice * Quantity;
        }

        public bool HasDiscount()
        {
            if (IsDiscountEqualsZero()) return false;

            return true;
        }

        private bool IsDiscountEqualsZero()
        {
            return Discount.Equals(0);
        }
    }
}

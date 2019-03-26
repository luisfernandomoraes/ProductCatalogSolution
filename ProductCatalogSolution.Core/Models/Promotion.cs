using ProductCatalogSolution.Core.Api.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProductCatalogSolution.Core.Models
{
    public class Promotion
    {
        public string Name { get; }
        public int CategoryId { get; }
        public IList<Policy> Policies { get; }

        public Promotion(PromotionDto promotion)
        {
            Name = promotion.Name;
            CategoryId = promotion.CategoryId;
            Policies = new List<Policy>();

            foreach (var apiPolicy in promotion.Policies)
            {
                Policies.Add(new Policy(apiPolicy));
            }
        }

        public double GetMaxDiscountThatCanBeAppliedByQuantity(int quantity)
        {
            foreach (var policy in GetOrderedPoliciesByGreaterMinimumQuantity())
            {
                if (policy.IsQuantityGreaterThanOrEqualAsMiniumQuantity(quantity))
                {
                    return policy.Discount;
                }
            }

            return 0;
        }

        private IList<Policy> GetOrderedPoliciesByGreaterMinimumQuantity()
        {
            return Policies.OrderByDescending(e => e.MinimumQuantity)
                           .ToList();
        }
    }
}

using ProductCatalogSolution.Core.Api.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//TODO: Refactor
namespace ProductCatalogSolution.Core.Models
{
    public class Promotion
    {
        public string Name { get; private set; }
        public int CategoryId { get; private set; }
        public IList<Policy> Policies { get; private set; }

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

using ProductCatalogSolution.Core.Api.DataModel;

namespace ProductCatalogSolution.Core.Models
{
    public class Policy
    {
        public int MinimumQuantity { get; }
        public double Discount { get; }

        public Policy(PolicyDto policy)
        {
            MinimumQuantity = policy.Min;
            Discount = policy.Discount;
        }

        public bool IsQuantityGreaterThanOrEqualAsMiniumQuantity(int quantity)
        {
            var comparison = quantity.CompareTo(MinimumQuantity);
            return IsComparisonGreaterThanOrEquals(comparison);
        }

        private bool IsComparisonGreaterThanOrEquals(int comparison)
        {
            return comparison >= 0;
        }
    }
}

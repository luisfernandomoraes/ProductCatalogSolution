using System;
using System.Collections.Generic;
using System.Text;

namespace ProductCatalogSolution.Core.Interfaces
{
    public interface INavigationService
    {
        void NavigateToCart();
        void NavigateToDetailByProductId(int productId);
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace ProductCatalogSolution.Core.Models
{
    public class ProductCollection : List<Product>
    {
        public string Name { get; }

        public ProductCollection(string name, IList<Product> products)
        {
            Name = name;
            if (products?.Count < 0)
                throw new ArgumentException("Collection must be not null.");
            AddRange(products);
        }
    }
}

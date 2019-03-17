using ProductCatalogSolution.Core.Api.DataModel;


namespace ProductCatalogSolution.Core.Models
{
    public class Category
    {
        public int Id { get; private set; }
        public string Name { get; private set; }

        public Category(CategoryDto category)
        {
            Id = category.Id;
            Name = category.Name;
        }
    }
}

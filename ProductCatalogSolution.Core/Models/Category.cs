using ProductCatalogSolution.Core.Api.DataModel;


namespace ProductCatalogSolution.Core.Models
{
    public class Category
    {
        public int Id { get; }
        public string Name { get; }

        public Category(CategoryDto category)
        {
            Id = category.Id;
            Name = category.Name;
        }
    }
}

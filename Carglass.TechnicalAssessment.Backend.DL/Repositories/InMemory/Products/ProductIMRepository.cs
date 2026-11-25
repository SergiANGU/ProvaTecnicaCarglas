using Carglass.TechnicalAssessment.Backend.Entities;

namespace Carglass.TechnicalAssessment.Backend.DL.Repositories;

public class ProductIMRepository : ICrudRepository<Product>
{
    private ICollection<Product> _products;

    public ProductIMRepository()
    {
        _products = new HashSet<Product>()
        {
            new Product()
            {
                Id = 1111111,
                ProductName = "Cristal ventanilla delantera",
                ProductType = 25,
                NumTerminal = 933933933,
                SoldAt = new DateTime(2019, 1, 9, 14, 26, 17)
            }
        };
    }

    public IEnumerable<Product> GetAll()
    {
        return _products.ToList();
    }

    public Product? GetById(params object[] keyValues)
    {
        return _products.SingleOrDefault(x => x.Id.Equals(keyValues[0]));
    }

    public void Create(Product item)
    {
        _products.Add(item);
    }

    public void Update(Product item)
    {
        var existing = _products.Single(x => x.Id.Equals(item.Id));
        existing.ProductName = item.ProductName;
        existing.ProductType = item.ProductType;
        existing.NumTerminal = item.NumTerminal;
        existing.SoldAt = item.SoldAt;
    }

    public void Delete(Product item)
    {
        var toDeleteItem = _products.Single(x => x.Id.Equals(item.Id));

        _products.Remove(toDeleteItem);
    }
}

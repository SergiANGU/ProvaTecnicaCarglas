using Carglass.TechnicalAssessment.Backend.DL.Repositories;
using Carglass.TechnicalAssessment.Backend.Entities;

namespace Carglass.TechnicalAssessment.Backend.Tests.Products;

public class ProductIMRepositoryTests
{
    private const int SeededProductId = 1111111;

    private readonly ProductIMRepository _sut = new();

    [Fact]
    public void GetAll_ReturnsSeededProduct()
    {
        // Act
        var result = _sut.GetAll().ToList();

        // Assert
        var product = Assert.Single(result);
        Assert.Equal(SeededProductId, product.Id);
    }

    [Fact]
    public void Create_AddsProduct()
    {
        // Arrange
        var product = new Product { Id = 2, ProductName = "Luna trasera" };

        // Act
        _sut.Create(product);

        // Assert
        Assert.NotNull(_sut.GetById(2));
        Assert.Equal(2, _sut.GetAll().Count());
    }

    [Fact]
    public void Update_ChangesStoredValues()
    {
        // Arrange
        var updated = new Product
        {
            Id = SeededProductId,
            ProductName = "Cristal ventanilla trasera",
            ProductType = 30,
            NumTerminal = 911911911,
            SoldAt = new DateTime(2024, 5, 1)
        };

        // Act
        _sut.Update(updated);

        // Assert
        var stored = _sut.GetById(SeededProductId)!;
        Assert.Equal("Cristal ventanilla trasera", stored.ProductName);
        Assert.Equal(30, stored.ProductType);
        Assert.Equal(new DateTime(2024, 5, 1), stored.SoldAt);
    }

    [Fact]
    public void Delete_RemovesProduct()
    {
        // Act
        _sut.Delete(new Product { Id = SeededProductId });

        // Assert
        Assert.Null(_sut.GetById(SeededProductId));
        Assert.Empty(_sut.GetAll());
    }
}

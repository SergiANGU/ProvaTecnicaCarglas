using AutoMapper;
using Carglass.TechnicalAssessment.Backend.BL;
using Carglass.TechnicalAssessment.Backend.BL.Products.Converter;
using Carglass.TechnicalAssessment.Backend.DL.Repositories;
using Carglass.TechnicalAssessment.Backend.Dtos;
using Carglass.TechnicalAssessment.Backend.Entities;
using Moq;

namespace Carglass.TechnicalAssessment.Backend.Tests.Products;

public class ProductAppServiceTests
{
    private readonly Mock<ICrudRepository<Product>> _productRepositoryMock;
    private readonly ProductAppService _sut;

    public ProductAppServiceTests()
    {
        _productRepositoryMock = new Mock<ICrudRepository<Product>>();

        var mapper = new MapperConfiguration(cfg => cfg.AddProfile<ProductProfileConverter>())
            .CreateMapper();

        _sut = new ProductAppService(
            _productRepositoryMock.Object,
            mapper,
            new ProductDtoValidator());
    }

    private static ProductDto CreateValidDto(int id = 2) => new()
    {
        Id = id,
        ProductName = "Luna trasera",
        ProductType = 10,
        NumTerminal = 911911911,
        SoldAt = new DateTime(2024, 5, 1, 10, 0, 0)
    };

    private void SetupExistingProduct(int id)
    {
        _productRepositoryMock
            .Setup(r => r.GetById(It.Is<object[]>(keys => keys[0].Equals(id))))
            .Returns(new Product { Id = id });
    }

    [Fact]
    public void GetAll_ReturnsMappedProducts()
    {
        // Arrange
        var products = new List<Product>
        {
            new() { Id = 1, ProductName = "Cristal ventanilla delantera", SoldAt = new DateTime(2019, 1, 9, 14, 26, 17) },
            new() { Id = 2, ProductName = "Luna trasera" }
        };
        _productRepositoryMock.Setup(r => r.GetAll()).Returns(products);

        // Act
        var result = _sut.GetAll().ToList();

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Equal("Cristal ventanilla delantera", result[0].ProductName);
        Assert.Equal(new DateTime(2019, 1, 9, 14, 26, 17), result[0].SoldAt);
    }

    [Fact]
    public void GetById_WhenProductExists_ReturnsMappedProduct()
    {
        // Arrange
        _productRepositoryMock
            .Setup(r => r.GetById(It.IsAny<object[]>()))
            .Returns(new Product { Id = 1, ProductName = "Cristal ventanilla delantera" });

        // Act
        var result = _sut.GetById(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Cristal ventanilla delantera", result.ProductName);
    }

    [Fact]
    public void Create_WhenProductIsValid_CallsRepositoryCreateOnce()
    {
        // Arrange
        var dto = CreateValidDto();

        // Act
        _sut.Create(dto);

        // Assert
        _productRepositoryMock.Verify(
            r => r.Create(It.Is<Product>(p => p.Id == dto.Id && p.ProductName == dto.ProductName)),
            Times.Once);
    }

    [Fact]
    public void Create_WhenIdAlreadyExists_ThrowsAndDoesNotCreate()
    {
        // Arrange
        var dto = CreateValidDto(id: 1);
        SetupExistingProduct(1);

        // Act & Assert
        Assert.Throws<Exception>(() => _sut.Create(dto));
        _productRepositoryMock.Verify(r => r.Create(It.IsAny<Product>()), Times.Never);
    }

    [Fact]
    public void Create_WhenProductNameIsEmpty_ThrowsAndDoesNotCreate()
    {
        // Arrange
        var dto = CreateValidDto();
        dto.ProductName = "";

        // Act & Assert
        var exception = Assert.Throws<Exception>(() => _sut.Create(dto));
        Assert.Contains("validación", exception.Message);
        _productRepositoryMock.Verify(r => r.Create(It.IsAny<Product>()), Times.Never);
    }

    [Fact]
    public void Update_WhenProductDoesNotExist_ThrowsAndDoesNotUpdate()
    {
        // Arrange
        var dto = CreateValidDto();

        // Act & Assert
        Assert.Throws<Exception>(() => _sut.Update(dto));
        _productRepositoryMock.Verify(r => r.Update(It.IsAny<Product>()), Times.Never);
    }

    [Fact]
    public void Update_WhenProductExistsAndIsValid_CallsRepositoryUpdateOnce()
    {
        // Arrange
        var dto = CreateValidDto();
        SetupExistingProduct(dto.Id);

        // Act
        _sut.Update(dto);

        // Assert
        _productRepositoryMock.Verify(
            r => r.Update(It.Is<Product>(p => p.Id == dto.Id && p.ProductName == dto.ProductName)),
            Times.Once);
    }

    [Fact]
    public void Delete_WhenProductDoesNotExist_ThrowsAndDoesNotDelete()
    {
        // Arrange
        var dto = CreateValidDto();

        // Act & Assert
        Assert.Throws<Exception>(() => _sut.Delete(dto));
        _productRepositoryMock.Verify(r => r.Delete(It.IsAny<Product>()), Times.Never);
    }

    [Fact]
    public void Delete_WhenProductExists_CallsRepositoryDeleteOnce()
    {
        // Arrange
        var dto = new ProductDto { Id = 2 };
        SetupExistingProduct(dto.Id);

        // Act
        _sut.Delete(dto);

        // Assert
        _productRepositoryMock.Verify(r => r.Delete(It.Is<Product>(p => p.Id == dto.Id)), Times.Once);
    }
}

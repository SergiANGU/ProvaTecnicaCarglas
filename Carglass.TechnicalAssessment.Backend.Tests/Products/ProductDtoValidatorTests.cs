using Carglass.TechnicalAssessment.Backend.Dtos;
using FluentValidation.TestHelper;

namespace Carglass.TechnicalAssessment.Backend.Tests.Products;

public class ProductDtoValidatorTests
{
    private readonly ProductDtoValidator _sut = new();

    [Fact]
    public void Validate_WhenProductIsValid_HasNoErrors()
    {
        // Arrange
        var dto = new ProductDto { Id = 1, ProductName = "Cristal ventanilla delantera" };

        // Act
        var result = _sut.TestValidate(dto);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WhenIdIsDefault_HasIdError()
    {
        // Arrange
        var dto = new ProductDto { Id = 0, ProductName = "Cristal" };

        // Act
        var result = _sut.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Validate_WhenProductNameIsEmpty_HasProductNameError(string? productName)
    {
        // Arrange
        var dto = new ProductDto { Id = 1, ProductName = productName! };

        // Act
        var result = _sut.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ProductName);
    }

    [Fact]
    public void Validate_WhenProductNameIsTooLong_HasProductNameError()
    {
        // Arrange
        var dto = new ProductDto { Id = 1, ProductName = new string('a', 101) };

        // Act
        var result = _sut.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ProductName);
    }
}

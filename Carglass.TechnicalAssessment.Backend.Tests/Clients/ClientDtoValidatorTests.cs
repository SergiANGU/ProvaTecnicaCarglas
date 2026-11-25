using Carglass.TechnicalAssessment.Backend.Dtos;
using FluentValidation.TestHelper;

namespace Carglass.TechnicalAssessment.Backend.Tests.Clients;

public class ClientDtoValidatorTests
{
    private const string NifErrorMessage = "El NIF debe tener 8 dígitos seguidos de 1 letra.";

    private readonly ClientDtoValidator _sut = new();

    private static ClientDto CreateDto(string docType, string docNum) => new()
    {
        Id = 1,
        DocType = docType,
        DocNum = docNum,
        Email = "test@sample.com"
    };

    [Theory]
    [InlineData("nif", "12345678Z")]
    [InlineData("nif", "12345678z")]
    [InlineData("NIF", "12345678Z")]
    public void Validate_WhenNifFormatIsValid_HasNoDocNumErrors(string docType, string docNum)
    {
        // Act
        var result = _sut.TestValidate(CreateDto(docType, docNum));

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.DocNum);
    }

    [Theory]
    [InlineData("1234567Z")]
    [InlineData("123456789")]
    [InlineData("X12345678Z")]
    [InlineData("12345678ZZ")]
    [InlineData("ABCDEFGHZ")]
    public void Validate_WhenNifFormatIsInvalid_HasNifError(string docNum)
    {
        // Act
        var result = _sut.TestValidate(CreateDto("nif", docNum));

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.DocNum)
            .WithErrorMessage(NifErrorMessage);
    }

    [Theory]
    [InlineData("nie", "X1234567L")]
    [InlineData("cif", "B12345678")]
    public void Validate_WhenDocTypeIsNotNif_DoesNotCheckNifFormat(string docType, string docNum)
    {
        // Act
        var result = _sut.TestValidate(CreateDto(docType, docNum));

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.DocNum);
    }

    [Fact]
    public void Validate_WhenIdIsDefault_HasIdError()
    {
        // Arrange
        var dto = CreateDto("nif", "12345678Z");
        dto.Id = 0;

        // Act
        var result = _sut.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void Validate_WhenEmailIsInvalid_HasEmailError()
    {
        // Arrange
        var dto = CreateDto("nif", "12345678Z");
        dto.Email = "no-es-un-email";

        // Act
        var result = _sut.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }
}

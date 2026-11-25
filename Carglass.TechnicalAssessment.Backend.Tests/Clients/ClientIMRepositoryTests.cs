using Carglass.TechnicalAssessment.Backend.DL.Repositories;
using Carglass.TechnicalAssessment.Backend.Entities;

namespace Carglass.TechnicalAssessment.Backend.Tests.Clients;

public class ClientIMRepositoryTests
{
    private readonly ClientIMRepository _sut = new();

    private static Client CreateClient(int id = 2) => new()
    {
        Id = id,
        DocType = "nif",
        DocNum = "12345678Z",
        Email = "test@sample.com",
        GivenName = "Pepe",
        FamilyName1 = "García",
        Phone = "600600600"
    };

    [Fact]
    public void GetAll_ReturnsSeededClient()
    {
        // Act
        var result = _sut.GetAll().ToList();

        // Assert
        var client = Assert.Single(result);
        Assert.Equal(1, client.Id);
    }

    [Fact]
    public void GetById_WhenClientDoesNotExist_ReturnsNull()
    {
        // Act
        var result = _sut.GetById(99);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void Create_AddsClient()
    {
        // Arrange
        var client = CreateClient();

        // Act
        _sut.Create(client);

        // Assert
        Assert.NotNull(_sut.GetById(client.Id));
        Assert.Equal(2, _sut.GetAll().Count());
    }

    [Fact]
    public void Update_ChangesStoredValues()
    {
        // Arrange
        var updated = CreateClient(id: 1);
        updated.Email = "nuevo@sample.com";

        // Act
        _sut.Update(updated);

        // Assert
        Assert.Equal("nuevo@sample.com", _sut.GetById(1)!.Email);
    }

    [Fact]
    public void Delete_RemovesClient()
    {
        // Act
        _sut.Delete(new Client { Id = 1 });

        // Assert
        Assert.Null(_sut.GetById(1));
        Assert.Empty(_sut.GetAll());
    }

    [Theory]
    [InlineData("11223344E")]
    [InlineData("11223344e")]
    public void GetByDocNum_IgnoresCase(string docNum)
    {
        // Act
        var result = _sut.GetByDocNum(docNum);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
    }

    [Fact]
    public void GetByDocNum_WhenDocNumDoesNotExist_ReturnsNull()
    {
        // Act
        var result = _sut.GetByDocNum("99999999R");

        // Assert
        Assert.Null(result);
    }
}

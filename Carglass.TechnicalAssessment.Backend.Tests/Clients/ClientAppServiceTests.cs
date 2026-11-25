using AutoMapper;
using Carglass.TechnicalAssessment.Backend.BL;
using Carglass.TechnicalAssessment.Backend.BL.Clients.Converter;
using Carglass.TechnicalAssessment.Backend.DL.Repositories;
using Carglass.TechnicalAssessment.Backend.Dtos;
using Carglass.TechnicalAssessment.Backend.Entities;
using Moq;

namespace Carglass.TechnicalAssessment.Backend.Tests.Clients;

public class ClientAppServiceTests
{
    private readonly Mock<IClientRepository> _clientRepositoryMock;
    private readonly ClientAppService _sut;

    public ClientAppServiceTests()
    {
        _clientRepositoryMock = new Mock<IClientRepository>();

        var mapper = new MapperConfiguration(cfg => cfg.AddProfile<ClientProfileConverter>())
            .CreateMapper();

        _sut = new ClientAppService(
            _clientRepositoryMock.Object,
            mapper,
            new ClientDtoValidator());
    }

    private static ClientDto CreateValidDto(int id = 2) => new()
    {
        Id = id,
        DocType = "nif",
        DocNum = "12345678Z",
        Email = "test@sample.com",
        GivenName = "Pepe",
        FamilyName1 = "García",
        Phone = "600600600"
    };

    private void SetupExistingClient(int id)
    {
        _clientRepositoryMock
            .Setup(r => r.GetById(It.Is<object[]>(keys => keys[0].Equals(id))))
            .Returns(new Client { Id = id });
    }

    [Fact]
    public void GetAll_ReturnsMappedClients()
    {
        // Arrange
        var clients = new List<Client>
        {
            new() { Id = 1, DocNum = "11223344E", Email = "a@sample.com" },
            new() { Id = 2, DocNum = "12345678Z", Email = "b@sample.com" }
        };
        _clientRepositoryMock.Setup(r => r.GetAll()).Returns(clients);

        // Act
        var result = _sut.GetAll().ToList();

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Equal("11223344E", result[0].DocNum);
        Assert.Equal("b@sample.com", result[1].Email);
    }

    [Fact]
    public void GetById_WhenClientExists_ReturnsMappedClient()
    {
        // Arrange
        _clientRepositoryMock
            .Setup(r => r.GetById(It.IsAny<object[]>()))
            .Returns(new Client { Id = 1, GivenName = "Enriqueta" });

        // Act
        var result = _sut.GetById(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Enriqueta", result.GivenName);
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
    public void Create_WhenClientIsValid_CallsRepositoryCreateOnce()
    {
        // Arrange
        var dto = CreateValidDto();

        // Act
        _sut.Create(dto);

        // Assert
        _clientRepositoryMock.Verify(
            r => r.Create(It.Is<Client>(c => c.Id == dto.Id && c.DocNum == dto.DocNum)),
            Times.Once);
    }

    [Fact]
    public void Create_WhenIdAlreadyExists_ThrowsAndDoesNotCreate()
    {
        // Arrange
        var dto = CreateValidDto(id: 1);
        SetupExistingClient(1);

        // Act & Assert
        var exception = Assert.Throws<Exception>(() => _sut.Create(dto));
        Assert.Contains("Id", exception.Message);
        _clientRepositoryMock.Verify(r => r.Create(It.IsAny<Client>()), Times.Never);
    }

    [Fact]
    public void Create_WhenDocNumAlreadyExists_ThrowsAndDoesNotCreate()
    {
        // Arrange
        var dto = CreateValidDto();
        _clientRepositoryMock
            .Setup(r => r.GetByDocNum(dto.DocNum))
            .Returns(new Client { Id = 1, DocNum = dto.DocNum });

        // Act & Assert
        var exception = Assert.Throws<Exception>(() => _sut.Create(dto));
        Assert.Contains("número de documento", exception.Message);
        _clientRepositoryMock.Verify(r => r.Create(It.IsAny<Client>()), Times.Never);
    }

    [Fact]
    public void Create_WhenNifFormatIsInvalid_ThrowsAndDoesNotCreate()
    {
        // Arrange
        var dto = CreateValidDto();
        dto.DocNum = "1234567Z";

        // Act & Assert
        var exception = Assert.Throws<Exception>(() => _sut.Create(dto));
        Assert.Contains("validación", exception.Message);
        _clientRepositoryMock.Verify(r => r.Create(It.IsAny<Client>()), Times.Never);
    }

    [Fact]
    public void Update_WhenClientDoesNotExist_ThrowsAndDoesNotUpdate()
    {
        // Arrange
        var dto = CreateValidDto();

        // Act & Assert
        Assert.Throws<Exception>(() => _sut.Update(dto));
        _clientRepositoryMock.Verify(r => r.Update(It.IsAny<Client>()), Times.Never);
    }

    [Fact]
    public void Update_WhenClientIsInvalid_ThrowsAndDoesNotUpdate()
    {
        // Arrange
        var dto = CreateValidDto();
        dto.Email = "no-es-un-email";
        SetupExistingClient(dto.Id);

        // Act & Assert
        Assert.Throws<Exception>(() => _sut.Update(dto));
        _clientRepositoryMock.Verify(r => r.Update(It.IsAny<Client>()), Times.Never);
    }

    [Fact]
    public void Update_WhenClientExistsAndIsValid_CallsRepositoryUpdateOnce()
    {
        // Arrange
        var dto = CreateValidDto();
        SetupExistingClient(dto.Id);

        // Act
        _sut.Update(dto);

        // Assert
        _clientRepositoryMock.Verify(
            r => r.Update(It.Is<Client>(c => c.Id == dto.Id && c.Email == dto.Email)),
            Times.Once);
    }

    [Fact]
    public void Delete_WhenClientDoesNotExist_ThrowsAndDoesNotDelete()
    {
        // Arrange
        var dto = CreateValidDto();

        // Act & Assert
        Assert.Throws<Exception>(() => _sut.Delete(dto));
        _clientRepositoryMock.Verify(r => r.Delete(It.IsAny<Client>()), Times.Never);
    }

    [Fact]
    public void Delete_WhenClientExists_CallsRepositoryDeleteOnce()
    {
        // Arrange
        var dto = new ClientDto { Id = 2 };
        SetupExistingClient(dto.Id);

        // Act
        _sut.Delete(dto);

        // Assert
        _clientRepositoryMock.Verify(r => r.Delete(It.Is<Client>(c => c.Id == dto.Id)), Times.Once);
    }
}

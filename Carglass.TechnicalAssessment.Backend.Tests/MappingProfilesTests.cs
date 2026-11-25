using AutoMapper;
using Carglass.TechnicalAssessment.Backend.BL;

namespace Carglass.TechnicalAssessment.Backend.Tests;

public class MappingProfilesTests
{
    [Fact]
    public void AllProfiles_AreValid()
    {
        // Arrange
        var configuration = new MapperConfiguration(cfg =>
            cfg.AddMaps(typeof(Module).Assembly));

        // Act & Assert
        configuration.AssertConfigurationIsValid();
    }
}

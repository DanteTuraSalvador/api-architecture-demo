using DemoApi.Application.Services;
using DemoApi.Domain.Entities;
using DemoApi.Domain.Enums;
using FluentAssertions;

namespace DemoApi.Application.Tests.Services;

public class DriverServiceTests
{
    private readonly DriverService _sut;

    public DriverServiceTests()
    {
        _sut = new DriverService();
    }

    #region GetAllAsync Tests

    [Fact]
    public async Task GetAllAsync_WhenNoDrivers_ShouldReturnEmptyList()
    {
        // Act
        var result = await _sut.GetAllAsync();

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAllAsync_WhenDriversExist_ShouldReturnAllDrivers()
    {
        // Arrange
        await _sut.CreateAsync(new Driver { FirstName = "John", LastName = "Doe" });
        await _sut.CreateAsync(new Driver { FirstName = "Jane", LastName = "Smith" });

        // Act
        var result = await _sut.GetAllAsync();

        // Assert
        result.Should().HaveCount(2);
    }

    #endregion

    #region GetByIdAsync Tests

    [Fact]
    public async Task GetByIdAsync_WhenDriverExists_ShouldReturnDriver()
    {
        // Arrange
        var created = await _sut.CreateAsync(new Driver { FirstName = "John", LastName = "Doe" });

        // Act
        var result = await _sut.GetByIdAsync(created.Id);

        // Assert
        result.Should().NotBeNull();
        result!.FirstName.Should().Be("John");
        result.LastName.Should().Be("Doe");
    }

    [Fact]
    public async Task GetByIdAsync_WhenDriverDoesNotExist_ShouldReturnNull()
    {
        // Act
        var result = await _sut.GetByIdAsync(Guid.NewGuid());

        // Assert
        result.Should().BeNull();
    }

    #endregion

    #region CreateAsync Tests

    [Fact]
    public async Task CreateAsync_ShouldGenerateNewId()
    {
        // Arrange
        var driver = new Driver { FirstName = "John", LastName = "Doe" };

        // Act
        var result = await _sut.CreateAsync(driver);

        // Assert
        result.Id.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public async Task CreateAsync_ShouldSetCreatedAtToUtcNow()
    {
        // Arrange
        var before = DateTime.UtcNow;
        var driver = new Driver { FirstName = "John", LastName = "Doe" };

        // Act
        var result = await _sut.CreateAsync(driver);
        var after = DateTime.UtcNow;

        // Assert
        result.CreatedAt.Should().BeOnOrAfter(before);
        result.CreatedAt.Should().BeOnOrBefore(after);
    }

    [Fact]
    public async Task CreateAsync_ShouldPreserveDriverProperties()
    {
        // Arrange
        var driver = new Driver
        {
            FirstName = "John",
            LastName = "Doe",
            LicenseNumber = "DL-123456",
            PhoneNumber = "+1-555-123-4567",
            Email = "john.doe@example.com",
            Status = DriverStatus.Available
        };

        // Act
        var result = await _sut.CreateAsync(driver);

        // Assert
        result.FirstName.Should().Be("John");
        result.LastName.Should().Be("Doe");
        result.LicenseNumber.Should().Be("DL-123456");
        result.PhoneNumber.Should().Be("+1-555-123-4567");
        result.Email.Should().Be("john.doe@example.com");
        result.Status.Should().Be(DriverStatus.Available);
    }

    #endregion

    #region UpdateAsync Tests

    [Fact]
    public async Task UpdateAsync_WhenDriverExists_ShouldUpdateProperties()
    {
        // Arrange
        var created = await _sut.CreateAsync(new Driver
        {
            FirstName = "John",
            LastName = "Doe",
            Status = DriverStatus.Available
        });

        var updateData = new Driver
        {
            FirstName = "Johnny",
            LastName = "Doe-Updated",
            LicenseNumber = "DL-999999",
            PhoneNumber = "+1-555-987-6543",
            Email = "johnny.updated@example.com",
            Status = DriverStatus.OnDuty
        };

        // Act
        var result = await _sut.UpdateAsync(created.Id, updateData);

        // Assert
        result.Should().NotBeNull();
        result!.FirstName.Should().Be("Johnny");
        result.LastName.Should().Be("Doe-Updated");
        result.LicenseNumber.Should().Be("DL-999999");
        result.Status.Should().Be(DriverStatus.OnDuty);
    }

    [Fact]
    public async Task UpdateAsync_WhenDriverExists_ShouldSetUpdatedAt()
    {
        // Arrange
        var created = await _sut.CreateAsync(new Driver { FirstName = "John", LastName = "Doe" });
        var before = DateTime.UtcNow;

        // Act
        var result = await _sut.UpdateAsync(created.Id, new Driver { FirstName = "Johnny" });
        var after = DateTime.UtcNow;

        // Assert
        result!.UpdatedAt.Should().NotBeNull();
        result.UpdatedAt.Should().BeOnOrAfter(before);
        result.UpdatedAt.Should().BeOnOrBefore(after);
    }

    [Fact]
    public async Task UpdateAsync_WhenDriverDoesNotExist_ShouldReturnNull()
    {
        // Act
        var result = await _sut.UpdateAsync(Guid.NewGuid(), new Driver { FirstName = "John" });

        // Assert
        result.Should().BeNull();
    }

    #endregion

    #region DeleteAsync Tests

    [Fact]
    public async Task DeleteAsync_WhenDriverExists_ShouldReturnTrue()
    {
        // Arrange
        var created = await _sut.CreateAsync(new Driver { FirstName = "John", LastName = "Doe" });

        // Act
        var result = await _sut.DeleteAsync(created.Id);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task DeleteAsync_WhenDriverExists_ShouldRemoveFromCollection()
    {
        // Arrange
        var created = await _sut.CreateAsync(new Driver { FirstName = "John", LastName = "Doe" });

        // Act
        await _sut.DeleteAsync(created.Id);
        var allDrivers = await _sut.GetAllAsync();

        // Assert
        allDrivers.Should().BeEmpty();
    }

    [Fact]
    public async Task DeleteAsync_WhenDriverDoesNotExist_ShouldReturnFalse()
    {
        // Act
        var result = await _sut.DeleteAsync(Guid.NewGuid());

        // Assert
        result.Should().BeFalse();
    }

    #endregion

    #region GetByStatusAsync Tests

    [Theory]
    [InlineData("Available", DriverStatus.Available)]
    [InlineData("OnDuty", DriverStatus.OnDuty)]
    [InlineData("OffDuty", DriverStatus.OffDuty)]
    [InlineData("OnBreak", DriverStatus.OnBreak)]
    public async Task GetByStatusAsync_ShouldReturnDriversWithMatchingStatus(string statusString, DriverStatus status)
    {
        // Arrange
        await _sut.CreateAsync(new Driver { FirstName = "John", Status = status });
        await _sut.CreateAsync(new Driver { FirstName = "Jane", Status = DriverStatus.Available });

        // Act
        var result = await _sut.GetByStatusAsync(statusString);

        // Assert
        result.All(d => d.Status == status).Should().BeTrue();
    }

    [Theory]
    [InlineData("available")]
    [InlineData("AVAILABLE")]
    [InlineData("AvAiLaBlE")]
    public async Task GetByStatusAsync_ShouldBeCaseInsensitive(string statusString)
    {
        // Arrange
        await _sut.CreateAsync(new Driver { FirstName = "John", Status = DriverStatus.Available });

        // Act
        var result = await _sut.GetByStatusAsync(statusString);

        // Assert
        result.Should().ContainSingle();
    }

    [Fact]
    public async Task GetByStatusAsync_WithInvalidStatus_ShouldReturnEmptyList()
    {
        // Arrange
        await _sut.CreateAsync(new Driver { FirstName = "John", Status = DriverStatus.Available });

        // Act
        var result = await _sut.GetByStatusAsync("InvalidStatus");

        // Assert
        result.Should().BeEmpty();
    }

    #endregion

    #region GetAvailableDriversAsync Tests

    [Fact]
    public async Task GetAvailableDriversAsync_ShouldReturnOnlyAvailableDrivers()
    {
        // Arrange
        await _sut.CreateAsync(new Driver { FirstName = "Available1", Status = DriverStatus.Available });
        await _sut.CreateAsync(new Driver { FirstName = "Available2", Status = DriverStatus.Available });
        await _sut.CreateAsync(new Driver { FirstName = "OnDuty", Status = DriverStatus.OnDuty });
        await _sut.CreateAsync(new Driver { FirstName = "OffDuty", Status = DriverStatus.OffDuty });

        // Act
        var result = await _sut.GetAvailableDriversAsync();

        // Assert
        result.Should().HaveCount(2);
        result.All(d => d.Status == DriverStatus.Available).Should().BeTrue();
    }

    [Fact]
    public async Task GetAvailableDriversAsync_WhenNoAvailableDrivers_ShouldReturnEmptyList()
    {
        // Arrange
        await _sut.CreateAsync(new Driver { FirstName = "OnDuty", Status = DriverStatus.OnDuty });
        await _sut.CreateAsync(new Driver { FirstName = "OffDuty", Status = DriverStatus.OffDuty });

        // Act
        var result = await _sut.GetAvailableDriversAsync();

        // Assert
        result.Should().BeEmpty();
    }

    #endregion
}

using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CleanArchitecture.Sample.Tests.Integration
{

    public class IServiceCollectionExtensionsTests
    {

        #region - - - - - - AddCleanArchitectureServices Tests - - - - - -

        [Fact]
        public void AddCleanArchitectureServices_NoValidationIssues_DoesNotThrowValidationException()
        {
            // Arrange
            var _ServiceCollection = new ServiceCollection();

            // Act
            var _Exception = Record.Exception(() => _ = _ServiceCollection.AddCleanArchitectureServices());

            // Assert
            _ = _Exception.Should().BeNull();
        }

        #endregion AddCleanArchitectureServices Tests

    }

}
